using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Internships;
using InternshipPortal.BL.DTOi.Internships;
using Microsoft.AspNetCore.Mvc;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/internships")]
    public class InternshipCommandsController : ControllerBase
    {
        private readonly IInternshipService _service;
        private readonly ILogger<InternshipCommandsController> _logger;

        public InternshipCommandsController(IInternshipService service, ILogger<InternshipCommandsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Internship), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] InternshipRequestDTO request)
        {
            try
            {
                var internship = new Internship
                {
                    Title = request.Title,
                    ShortDescription = request.ShortDescription,
                    FullDescription = request.FullDescription,
                    IsFeatured = request.IsFeatured,
                    Remote = request.Remote,
                    Location = request.Location,
                    Deadline = request.Deadline,
                    CompanyId = request.CompanyId,
                    CategoryId = request.CategoryId,
                    PostedAt = DateTime.UtcNow
                };

                var created = _service.Create(internship);

                // Link to GET by id in InternshipsController
                return CreatedAtAction(nameof(InternshipsController.GetById), "Internships",
                    new { id = created.Id }, created);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Create internship.");
                return StatusCode(500, "Dogodila se greška.");
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Internship), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Update(int id, [FromBody] InternshipRequestDTO request)
        {
            try
            {
                var internship = new Internship
                {
                    Id = id,
                    Title = request.Title,
                    ShortDescription = request.ShortDescription,
                    FullDescription = request.FullDescription,
                    IsFeatured = request.IsFeatured,
                    Remote = request.Remote,
                    Location = request.Location,
                    Deadline = request.Deadline,
                    CompanyId = request.CompanyId,
                    CategoryId = request.CategoryId
                };

                return Ok(_service.Update(id, internship));
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Update internship id={Id}", id);
                return StatusCode(500, "Dogodila se greška.");
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                _logger.LogError(ex, "Greška Delete internship id={Id}", id);
                return StatusCode(500, "Dogodila se greška.");
            }
        }
    }
}
