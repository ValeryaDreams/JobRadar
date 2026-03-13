namespace Api.Services
{
        public interface IElasticsearchIndexService
        {
                Task CreateIndexAsync();
        }
}