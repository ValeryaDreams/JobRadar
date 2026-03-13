namespace Api.Models.DTO
{
        public class VacancyFacetsDto
        {
                public List<FacetBucketDto> Cities { get; set; } = new();
                public List<FacetBucketDto> ExperienceLevels { get; set; } = new();
                public List<FacetBucketDto> TopSkills { get; set; } = new();

                public SalaryStatsDto SalaryStats { get; set; } = new();
        }
}
