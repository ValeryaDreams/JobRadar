using Api.Models.Documets;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
        public class JobRadarDbContext : DbContext
        {
                public JobRadarDbContext(DbContextOptions<JobRadarDbContext> options)
                        : base(options) { }

                public DbSet<Vacancy> Vacancies => Set<Vacancy>();
        }
}