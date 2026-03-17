using Api.Data;
using Api.Models.Documets;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;

namespace Api.Services;

public class VacancySyncService : IVacancySyncService
{
        private readonly ElasticsearchClient _client;
        private readonly ElasticsearchOptions _opt;

        public VacancySyncService(
            ElasticsearchClient client,
            IOptions<ElasticsearchOptions> opt)
        {
                _client = client;
                _opt = opt.Value;
        }

        public async Task UpsertVacancyAsync(Vacancy vacancy)
        {
                var request = new IndexRequest<Vacancy>(vacancy, _opt.IndexName, vacancy.Id.ToString());

                var response = await _client.IndexAsync(request);

                if (!response.IsValidResponse)
                {
                        throw new Exception($"Elasticsearch upsert failed: {response.DebugInformation}");
                }
        }

        public async Task DeleteVacancyAsync(int id)
        {
                var request = new DeleteRequest(_opt.IndexName, id.ToString());

                var response = await _client.DeleteAsync(request);

                if (!response.IsValidResponse)
                {
                        throw new Exception($"Elasticsearch delete failed: {response.DebugInformation}");
                }
        }
}