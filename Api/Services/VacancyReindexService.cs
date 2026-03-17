using Api.Data;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
        public class VacancyReindexService : IVacancyReindexService
        {
                private readonly JobRadarDbContext _context;
                private readonly ElasticsearchClient _client;

                public VacancyReindexService(JobRadarDbContext context, ElasticsearchClient client)
                {
                        _context = context;
                        _client = client;
                }

                public async Task ReindexAsync()
                {
                        const int batchSize = 500;

                        var total = await _context.Vacancies.CountAsync();

                        for (int i = 0; i < total; i += batchSize)
                        {
                                var batch = await _context.Vacancies
                                                          .Skip(i)
                                                          .Take(batchSize)
                                                          .ToListAsync();

                                foreach (var vacancy in batch)
                                {
                                        vacancy.TitleSuggestions = string.IsNullOrWhiteSpace(vacancy.Title) ? Array.Empty<string>() : new[] { vacancy.Title };
                                        vacancy.CompanySuggest = string.IsNullOrWhiteSpace(vacancy.Company) ? Array.Empty<string>() : new[] { vacancy.Currency };
                                }

                                var response = await _client.BulkAsync(b => b.IndexMany(batch));

                                if (response.Errors)
                                {
                                        throw new Exception("Bulk indexing failed");
                                }
                        }
                }
        }
}