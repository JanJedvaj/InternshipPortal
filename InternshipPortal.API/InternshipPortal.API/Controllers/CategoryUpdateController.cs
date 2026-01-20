using InternshipPortal.API.Common;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Categories;
using Microsoft.AspNetCore.Mvc;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryUpdateController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly ILogger<CategoryUpdateController> _logger;

        public CategoryUpdateController(ICategoryService service, ILogger<CategoryUpdateController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // PUT api/categories/{id}
        [HttpPut("{id:int}")]
        public ActionResult<Category> Update(int id, [FromBody] Category category)
        {
            try
            {
                var updated = _service.Update(id, category);
                return Ok(updated);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Update category id={Id}", id);
                return StatusCode(500, ErrorMessages.InternalServerError);
            }
        }
    }
}
