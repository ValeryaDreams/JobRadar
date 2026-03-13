using Api.Contracts;

namespace Api.Models.DTO
{
        public class VacancySearchResponse
        {
                public long Total {  get; set; }
                public int Page {  get; set; }
                public int PageSize { get; set; }

                public List<VacancySearchItemDto> Items { get; set; } = new();
                public VacancyFacetsDto Facets { get; set; } = new();
        }
}