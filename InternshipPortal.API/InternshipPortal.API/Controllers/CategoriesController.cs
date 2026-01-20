using InternshipPortal.API.Common;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Categories;
using Microsoft.AspNetCore.Mvc;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly CategorySortingStrategyResolver _sortResolver;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICategoryService service,
            CategorySortingStrategyResolver sortResolver,
            ILogger<CategoriesController> logger)
        {
            _service = service;
            _sortResolver = sortResolver;
            _logger = logger;
        }

        // GET api/categories
        // GET api/categories?sort=mostUsed
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? sort = null)
        {
            try
            {
                var categories = _service.GetAll();

                if (!string.IsNullOrWhiteSpace(sort))
                {
                    var strategy = _sortResolver.Resolve(sort);
                    categories = strategy.Sort(categories);
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška GetAll categories.");
                return StatusCode(500, ErrorMessages.InternalServerError);
            }
        }

        // GET api/categories/{id}
        [HttpGet("{id:int}")]
        public ActionResult<Category> GetById(int id)
        {
            try
            {
                return Ok(_service.GetById(id));
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Get category by id.");
                return StatusCode(500, ErrorMessages.InternalServerError);
            }
        }
    }
}
