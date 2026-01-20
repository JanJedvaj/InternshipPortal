using InternshipPortal.API.Common;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Categories;
using Microsoft.AspNetCore.Mvc;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryDeleteController : ControllerBase
    {
        private readonly ICategoryFacade _facade;
        private readonly ILogger<CategoryDeleteController> _logger;

        public CategoryDeleteController(ICategoryFacade facade, ILogger<CategoryDeleteController> logger)
        {
            _facade = facade;
            _logger = logger;
        }

        // DELETE api/categories/{id}
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
                return StatusCode(500, ErrorMessages.InternalServerError);
            }
        }
    }
}
