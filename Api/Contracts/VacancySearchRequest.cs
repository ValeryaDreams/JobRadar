namespace Api.Contracts
{
        public class VacancySearchRequest
        {
                public string? Q {  get; set; } 
                public string? City { get; set; }
                public bool? Remote { get; set; }
                public string[]? Skills { get; set; }
                public int? SalaryFrom { get; set; }
                public string? ExperienceLevel { get; set; }

                public int Page { get; set; } = 1;
                public int PageSize { get; set; } = 10;

                public bool Debug { get; set; } = false;
        }
}
