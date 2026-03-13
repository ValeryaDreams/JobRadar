namespace Api.Models.DTO
{
        public class SalaryStatsDto
        {
                public double? Min { get; set; }
                public double? Max { get; set; }
                public double? Avg { get; set; }
                public double? Sum { get; set; }

                public long Count { get; set; }
        }
}
