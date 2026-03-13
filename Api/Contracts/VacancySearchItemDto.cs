namespace Api.Contracts
{
        public class VacancySearchItemDto
        {
                public int Id { get; set; }
                public string Title { get; set; } = null!;
                public string Company { get; set; } = null!;
                public string Description { get; set; } = null!;
                public string[] Skills { get; set; } = Array.Empty<string>();
                public int SalaryFrom { get; set; }
                public int SalaryTo { get; set; }
                public string Currency { get; set; } = null!;
                public string EmploymentType { get; set; } = null!;
                public string ExperienceLevel { get; set; } = null!;
                public string City { get; set; } = null!;
                public bool Remote { get; set; }
                public DateTime PostedAt { get; set; }
                public DateTime UpdatedAt { get; set; }

                public double? Score { get; set; } = new();
                public HighlightDto Highlights { get; set; } = new();
                public List<string> MatchedFields { get; set; } = new();
                public string? Explanation { get; set; }

        }
}
