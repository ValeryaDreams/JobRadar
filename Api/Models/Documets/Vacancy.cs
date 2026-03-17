namespace Api.Models.Documets
{
        public class Vacancy
        {
                public int Id { get; set; }
                public string Title { get; set; } = default!;
                public string Company { get; set; } = default!;
                public string Description { get; set; } = default!;                
                public string[] Skills { get; set; } = Array.Empty<string>();
                public int SalaryFrom { get; set; }
                public int SalaryTo { get; set; }
                public string Currency { get; set; } = default!;
                public string EmploymentType { get; set; } = default!;
                public string ExperienceLevel { get; set; } = default!;
                public string City { get; set; } = default!;
                public bool Remote { get; set; }
                public DateTime PostedAt { get; set; }
                public DateTime UpdatedAt { get; set; }

                public string[] TitleSuggestions { get; set; } = Array.Empty<string>();
                public string[] CompanySuggest { get; set; } = Array.Empty <string> ();
        }
}