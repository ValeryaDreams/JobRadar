using Api.Models.DTO;

namespace Api.Contracts
{
        public interface IVacancySearchService
        {
                Task<VacancySearchResponse> SearchAsync(VacancySearchRequest request);
        }
}
