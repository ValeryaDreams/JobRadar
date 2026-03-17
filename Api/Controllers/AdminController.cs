using Api.Models.Documets;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
        [ApiController]
        [Route("admin")]
        public class AdminController: ControllerBase
        {
                private readonly IVacancyReindexService _reindexService;
                private readonly IVacancySyncService _vacancySyncService;

                public AdminController(IVacancyReindexService reindexService, IVacancySyncService vacancySyncService)
                {
                        _reindexService = reindexService;
                        _vacancySyncService = vacancySyncService;
                }

                [HttpPost("reindex")]
                public async Task<IActionResult> Reindex()
                {
                        await _reindexService.ReindexAsync();

                        return Ok("Reindex complited");
                }

                [HttpPost("sync")]
                public async Task<IActionResult> SyncVacancy([FromBody] Vacancy vacancy)
                {
                        await _vacancySyncService.UpsertVacancyAsync(vacancy);

                        return Ok();
                }

                [HttpPost("sync/id")]
                public async Task<IActionResult> DeleteVacancy(int id)
                {
                        await _vacancySyncService.DeleteVacancyAsync(id);

                        return Ok();
                }
        }
}
