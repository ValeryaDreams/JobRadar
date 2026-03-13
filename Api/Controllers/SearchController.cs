using Api.Contracts;
using Api.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
        [ApiController]
        [Route("search")]
        public class SearchController: ControllerBase
        {
                private readonly IVacancySearchService _searchService;

                public SearchController(IVacancySearchService searchService)
                {
                        _searchService = searchService;
                }

                [HttpGet]
                public async Task<ActionResult<VacancySearchResponse>> Search([FromQuery] VacancySearchRequest request)
                {
                        var result = await _searchService.SearchAsync(request);
                        return Ok(result);
                }
        }
}
