using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Categories;
using Microsoft.AspNetCore.Mvc;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly ICategoryFacade _facade;
        private readonly CategorySortingStrategyResolver _sortResolver;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICategoryService service,
            ICategoryFacade facade,
            CategorySortingStrategyResolver sortResolver,
            ILogger<CategoriesController> logger)
        {
            _service = service;
            _facade = facade;
            _sortResolver = sortResolver;
            _logger = logger;
        }

        // GET: /api/Categories
        // GET: /api/Categories?sort=mostUsed
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll([FromQuery] string? sort = null)
        {
            try
            {
                var categories = _service.GetAll();

                // Ako je tražen sort - koristi Strategy
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
                return StatusCode(500, "Dogodila se greška.");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get(int id)
        {
            try
            {
                return Ok(_service.GetById(id));
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Get category id={Id}", id);
                return StatusCode(500, "Dogodila se greška.");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(InternshipPortal.API.Data.EF.Category), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] InternshipPortal.API.Data.EF.Category category)
        {
            try
            {
                var created = _service.Create(category);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Create category.");
                return StatusCode(500, "Dogodila se greška.");
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(InternshipPortal.API.Data.EF.Category), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Update(int id, [FromBody] InternshipPortal.API.Data.EF.Category category)
        {
            try
            {
                return Ok(_service.Update(id, category));
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Update category id={Id}", id);
                return StatusCode(500, "Dogodila se greška.");
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _facade.DeleteCategorySafely(id);
                return NoContent();
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Delete category id={Id}", id);
                return StatusCode(500, "Dogodila se greška.");
            }
        }
    }
}
