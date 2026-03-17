using Api.Data;
using Api.Models.Documets;
using Api.Models.DTO;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;

namespace Api.Services;

public class SuggestService : ISuggestService
{
        private readonly ElasticsearchClient _client;
        private readonly ElasticsearchOptions _opt;

        public SuggestService(
            ElasticsearchClient client,
            IOptions<ElasticsearchOptions> opt)
        {
                _client = client;
                _opt = opt.Value;
        }

        public async Task<SuggestResponseDto> SuggestAsync(string query)
        {
                if (string.IsNullOrWhiteSpace(query))
                {
                        return new SuggestResponseDto();
                }

                var response = await _client.SearchAsync<Vacancy>(s => s
                    .Index(_opt.IndexName)
                    .Size(0)
                    .Suggest(su => su
                        .Suggesters(ss => ss
                            .Add("title-suggest", sugg => sugg
                                .Prefix(query)
                                .Completion(c => c
                                    .Field("titleSuggest")
                                    .Size(10)
                                )
                            )
                            .Add("company-suggest", sugg => sugg
                                .Prefix(query)
                                .Completion(c => c
                                    .Field("companySuggest")
                                    .Size(10)
                                )
                            )
                        )
                    )
                );

                if (!response.IsValidResponse)
                {
                        throw new Exception($"Elasticsearch suggest failed: {response.DebugInformation}");
                }

                var results = new List<string>();

                if (response.Suggest != null)
                {
                        foreach (var suggest in response.Suggest)
                        {
                                foreach (var entry in suggest.Value)
                                {
                                        // В 8.x Options могут быть внутри Completion
                                        var options = entry as dynamic;

                                        if (options?.Options == null)
                                                continue;

                                        foreach (var option in options.Options)
                                        {
                                                string text = option.Text;

                                                if (!string.IsNullOrWhiteSpace(text))
                                                {
                                                        results.Add(text);
                                                }
                                        }
                                }
                        }
                }

                var items = results
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(10)
                    .Select(x => new SuggestItemDto
                    {
                            Text = x
                    })
                    .ToList();

                return new SuggestResponseDto
                {
                        Items = items
                };
        }
}