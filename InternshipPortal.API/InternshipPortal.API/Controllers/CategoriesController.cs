using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Models;
using InternshipPortal.API.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService service, ILogger<CategoriesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try { return Ok(_service.GetAll()); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška GetAll categories.");
                return StatusCode(500, "Dogodila se greška.");
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try { return Ok(_service.GetById(id)); }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Get category id={Id}", id);
                return StatusCode(500, "Dogodila se greška.");
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Category category)
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
        public IActionResult Update(int id, [FromBody] Category category)
        {
            try { return Ok(_service.Update(id, category)); }
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
                _service.Delete(id);
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
