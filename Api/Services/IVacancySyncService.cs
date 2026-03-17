using Api.Models.Documets;

namespace Api.Services
{
        public interface IVacancySyncService
        {
                Task UpsertVacancyAsync(Vacancy vacancy);
                Task DeleteVacancyAsync(int id);
        }
}
