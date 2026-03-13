using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
        [ApiController]
        [Route("admin")]
        public class AdminController: ControllerBase
        {
                private readonly IVacancyReindexService _service;

                public AdminController(IVacancyReindexService service)
                {
                        _service = service;
                }

                [HttpPost("reindex")]
                public async Task<IActionResult> Reindex()
                {
                        await _service.ReindexAsync();

                        return Ok("Reindex complited");
                }
        }
}
