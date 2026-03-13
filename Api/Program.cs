using Api.Contracts;
using Api.Data;
using Api.Models.DTO;
using Api.Services;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api
{
        public class Program
        {
                public static async Task Main(string[] args)
                {
                        var builder = WebApplication.CreateBuilder(args);

                        builder.Services.AddControllers();
                        builder.Services.AddEndpointsApiExplorer();
                        builder.Services.AddSwaggerGen();

                        builder.Services.AddDbContext<JobRadarDbContext>(opt =>
                        opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

                        builder.Services.Configure<ElasticsearchOptions>(
                                builder.Configuration.GetSection("Elasticsearch"));

                        builder.Services.AddSingleton<ElasticsearchClient>(sp =>
                        {
                                var opt = sp.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;

                                var settings = new ElasticsearchClientSettings(new Uri(opt.Url)).DefaultIndex(opt.IndexName);

                                return new ElasticsearchClient(settings);
                        });

                        builder.Services.AddScoped<IElasticsearchIndexService, ElasticsearchIndexService>();
                        builder.Services.AddScoped<IVacancyReindexService, VacancyReindexService>();
                        builder.Services.AddScoped<IVacancySearchService, VacancySearchService>();

                        var app = builder.Build();

                        //using (var scope = app.Services.CreateScope())
                        //{
                        //        var dbContext = scope.ServiceProvider.GetRequiredService<JobRadarDbContext>();

                        //        await dbContext.Database.MigrateAsync();
                        //        await DbSeeder.SeedAsync(dbContext);
                        //}

                        if (app.Environment.IsDevelopment())
                        {
                                app.UseSwagger();
                                app.UseSwaggerUI();
                        }

                        app.UseHttpsRedirection();
                        app.MapControllers();

                        app.Run();
                }
        }
}