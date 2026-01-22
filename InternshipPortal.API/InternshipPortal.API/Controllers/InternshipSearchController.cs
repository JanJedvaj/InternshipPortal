using InternshipPortal.API.Services.Internships.Search;
using Microsoft.AspNetCore.Mvc;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/internships")]
    public class InternshipSearchController : ControllerBase
    {
        private readonly IInternshipSearchFacade _searchFacade;
        private readonly ILogger<InternshipSearchController> _logger;

        public InternshipSearchController(
            IInternshipSearchFacade searchFacade,
            ILogger<InternshipSearchController> logger)
        {
            _searchFacade = searchFacade;
            _logger = logger;
        }

        // GET api/internships/search
        [HttpGet("search")]
        [ProducesResponseType(typeof(InternshipSearchResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Search([FromQuery] InternshipSearchCriteria criteria)
        {
            try
            {
                var result = _searchFacade.Search(criteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom pretrage oglasa (internship search).");
                return StatusCode(500, "Dogodila se greška.");
            }
        }
    }
}
