
using Api.Data;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;

namespace Api.Services
{
        public class ElasticsearchIndexService : IElasticsearchIndexService
        {
                private readonly ElasticsearchClient _client;
                private readonly ElasticsearchOptions _opt;

                public ElasticsearchIndexService(ElasticsearchClient client, IOptions<ElasticsearchOptions> opt)
                {
                        _client = client;
                        _opt = opt.Value;
                }

                public async Task CreateIndexAsync()
                {
                        var exsist = await _client.Indices.ExistsAsync(_opt.IndexName);

                        if (exsist.Exists)
                        {
                                return;
                        }

                        await _client.Indices.CreateAsync(_opt.IndexName);
                }
        }
}
