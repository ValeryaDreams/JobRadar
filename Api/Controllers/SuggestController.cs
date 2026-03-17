using Api.Models.DTO;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
        [ApiController]
        [Route("suggest")]
        public class SuggestController:ControllerBase
        {
                private readonly ISuggestService _service;

                public SuggestController(ISuggestService service)
                {
                        _service = service;
                }

                [HttpGet]
                public async Task<ActionResult<SuggestResponseDto>> Get([FromQuery] string q)
                {
                        var result = await _service.SuggestAsync(q);

                        return Ok(result);
                }
        }
}
