using Api.Contracts;
using Api.Data;
using Api.Models.Documets;
using Api.Models.DTO;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Options;

namespace Api.Services;

public class VacancySearchService : IVacancySearchService
{
        private readonly ElasticsearchClient _client;
        private readonly ElasticsearchOptions _opt;

        public VacancySearchService(
            ElasticsearchClient client,
            IOptions<ElasticsearchOptions> opt)
        {
                _client = client;
                _opt = opt.Value;
        }

        public async Task<VacancySearchResponse> SearchAsync(VacancySearchRequest request)
        {
                var mustQueries = new List<Query>();
                var filterQueries = new List<Query>();

                // 1. Текстовый поиск.
                if (!string.IsNullOrWhiteSpace(request.Q))
                {
                        mustQueries.Add(new MultiMatchQuery
                        {
                                Query = request.Q,
                                Fields = new[] { "title", "description" }
                        });
                }

                // 2. Фильтр по городу.
                if (!string.IsNullOrWhiteSpace(request.City))
                {
                        filterQueries.Add(new TermQuery("city")
                        {
                                Value = request.City
                        });
                }

                // 3. Фильтр по remote.
                if (request.Remote.HasValue)
                {
                        filterQueries.Add(new TermQuery("remote")
                        {
                                Value = request.Remote.Value
                        });
                }

                // 4. Фильтр по уровню опыта.
                if (!string.IsNullOrWhiteSpace(request.ExperienceLevel))
                {
                        filterQueries.Add(new TermQuery("experienceLevel")
                        {
                                Value = request.ExperienceLevel
                        });
                }

                // 5. Фильтр по минимальной зарплате.
                if (request.SalaryFrom.HasValue)
                {
                        filterQueries.Add(new NumberRangeQuery("salaryFrom")
                        {
                                Gte = request.SalaryFrom.Value
                        });
                }

                // 6. Фильтр по навыкам.
                if (request.Skills is { Length: > 0 })
                {
                        foreach (var skill in request.Skills)
                        {
                                if (string.IsNullOrWhiteSpace(skill))
                                        continue;

                                filterQueries.Add(new TermQuery("skills")
                                {
                                        Value = skill
                                });
                        }
                }

                // 7. Пагинация.
                var page = request.Page < 1 ? 1 : request.Page;
                var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

                if (pageSize > 100)
                        pageSize = 100;

                var from = (page - 1) * pageSize;

                // 8. Финальный query.
                Query finalQuery;

                if (mustQueries.Count == 0 && filterQueries.Count == 0)
                {
                        finalQuery = new MatchAllQuery();
                }
                else
                {
                        finalQuery = new BoolQuery
                        {
                                Must = mustQueries.Count > 0 ? mustQueries : null,
                                Filter = filterQueries.Count > 0 ? filterQueries : null
                        };
                }

                // 9. Формируем search request.
                var search = new SearchRequestDescriptor<Vacancy>()
                    .Index(_opt.IndexName)
                    .From(from)
                    .Size(pageSize)
                    .Query(finalQuery)
                    .Sort(so => so
                        .Score(sc => sc.Order(SortOrder.Desc))
                        .Field("postedAt", fs => fs.Order(SortOrder.Desc)))
                    .Aggregations(a => a
                        .Add("cities", agg => agg
                            .Terms(t => t
                                .Field("city")
                                .Size(20)))
                        .Add("experienceLevels", agg => agg
                            .Terms(t => t
                                .Field("experienceLevel")
                                .Size(10)))
                        .Add("topSkills", agg => agg
                            .Terms(t => t
                                .Field("skills")
                                .Size(20)))
                        .Add("salaryStats", agg => agg
                            .Stats(s => s
                                .Field("salaryFrom")))
                    );

                // 10. Debug-режим.
                if (request.Debug)
                {
                        search = search.Explain(true);
                        search = search.TrackScores(true);
                }

                // 11. Выполняем поиск.
                var response = await _client.SearchAsync<Vacancy>(search);

                if (!response.IsValidResponse)
                {
                        throw new Exception($"Elasticsearch search failed: {response.DebugInformation}");
                }

                // 12. Маппим документы.
                var items = response.Hits
                    .Where(h => h.Source is not null)
                    .Select(h => new VacancySearchItemDto
                    {
                            Id = h.Source!.Id,
                            Title = h.Source.Title,
                            Company = h.Source.Company,
                            Description = h.Source.Description,
                            Skills = h.Source.Skills,
                            SalaryFrom = h.Source.SalaryFrom,
                            SalaryTo = h.Source.SalaryTo,
                            Currency = h.Source.Currency,
                            EmploymentType = h.Source.EmploymentType,
                            ExperienceLevel = h.Source.ExperienceLevel,
                            City = h.Source.City,
                            Remote = h.Source.Remote,
                            PostedAt = h.Source.PostedAt,
                            UpdatedAt = h.Source.UpdatedAt,

                            Score = h.Score,

                            Highlights = new HighlightDto(),

                            MatchedFields = !string.IsNullOrWhiteSpace(request.Q)
                            ? new List<string> { "title", "description" }
                            : new List<string>(),

                            Explanation = request.Debug && h.Explanation is not null
                            ? h.Explanation.ToString()
                            : null
                    })
                    .ToList();

                // 13. Читаем aggregations: cities.
                var cityBuckets = new List<FacetBucketDto>();
                var citiesAgg = response.Aggregations?.GetStringTerms("cities");

                if (citiesAgg?.Buckets != null)
                {
                        cityBuckets = citiesAgg.Buckets
                            .Select(b => new FacetBucketDto
                            {
                                    Key = b.Key.ToString(),
                                    Count = b.DocCount
                            })
                            .ToList();
                }

                // 14. Читаем aggregations: experienceLevels.
                var experienceBuckets = new List<FacetBucketDto>();
                var experienceAgg = response.Aggregations?.GetStringTerms("experienceLevels");

                if (experienceAgg?.Buckets is not null)
                {
                        experienceBuckets = experienceAgg.Buckets
                            .Where(b => b.Key !=null)
                            .Select(b => new FacetBucketDto
                            {
                                    Key = b.Key.ToString(),
                                    Count = b.DocCount
                            })
                            .ToList();
                }

                // 15. Читаем aggregations: topSkills.
                var skillsBuckets = new List<FacetBucketDto>();
                var skillsAgg = response.Aggregations?.GetStringTerms("topSkills");

                if (skillsAgg?.Buckets is not null)
                {
                        skillsBuckets = skillsAgg.Buckets
                            .Where(b => b.Key != null)
                            .Select(b => new FacetBucketDto
                            {
                                    Key = b.Key.ToString(),
                                    Count = b.DocCount
                            })
                            .ToList();
                }

                // 16. Читаем aggregations: salaryStats.
                var salaryStatsDto = new SalaryStatsDto();
                var salaryStatsAgg = response.Aggregations?.GetStats("salaryStats");

                if (salaryStatsAgg is not null)
                {
                        salaryStatsDto = new SalaryStatsDto
                        {
                                Min = salaryStatsAgg.Min,
                                Max = salaryStatsAgg.Max,
                                Avg = salaryStatsAgg.Avg,
                                Sum = salaryStatsAgg.Sum,
                                Count = salaryStatsAgg.Count
                        };
                }

                // 17. Собираем facets.
                var facets = new VacancyFacetsDto
                {
                        Cities = cityBuckets,
                        ExperienceLevels = experienceBuckets,
                        TopSkills = skillsBuckets,
                        SalaryStats = salaryStatsDto
                };

                // 18. Возвращаем итоговый ответ.
                return new VacancySearchResponse
                {
                        Total = response.Total,
                        Page = page,
                        PageSize = pageSize,
                        Items = items,
                        Facets = facets
                };
        }
}