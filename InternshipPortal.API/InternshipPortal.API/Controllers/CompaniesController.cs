using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Companies;
using InternshipPortal.API.Data.EF;
using Microsoft.AspNetCore.Mvc;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _service;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(ICompanyService service, ILogger<CompaniesController> logger)
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
                _logger.LogError(ex, "Greška GetAll companies.");
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
                _logger.LogError(ex, "Greška Get company id={Id}", id);
                return StatusCode(500, "Dogodila se greška.");
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Company company)
        {
            try
            {
                var created = _service.Create(company);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Create company.");
                return StatusCode(500, "Dogodila se greška.");
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Company company)
        {
            try { return Ok(_service.Update(id, company)); }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Update company id={Id}", id);
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
                _logger.LogError(ex, "Greška Delete company id={Id}", id);
                return StatusCode(500, "Dogodila se greška.");
            }
        }
    }
}
