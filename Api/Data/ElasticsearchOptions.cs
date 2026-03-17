namespace Api.Data
{
        public class ElasticsearchOptions
        {
                public string Url { get; set; }
                public string IndexName { get; set; } = "vacancies_current";
        }
}