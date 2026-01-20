using InternshipPortal.API.Common;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Categories;
using Microsoft.AspNetCore.Mvc;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryCreateController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly ILogger<CategoryCreateController> _logger;

        public CategoryCreateController(ICategoryService service, ILogger<CategoryCreateController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // POST api/categories
        [HttpPost]
        public ActionResult<Category> Create([FromBody] Category category)
        {
            try
            {
                var created = _service.Create(category);
                return CreatedAtAction(nameof(CategoriesController.GetById), "Categories",
                    new { id = created.Id }, created);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Create category.");
                return StatusCode(500, ErrorMessages.InternalServerError);
            }
        }
    }
}
