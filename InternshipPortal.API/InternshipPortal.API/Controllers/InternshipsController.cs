using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Internships;
using InternshipPortal.BL.DTOi.Internships;
using Microsoft.AspNetCore.Mvc;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/internships")]
    public class InternshipsController : ControllerBase
    {
        private readonly IInternshipService _service;
        private readonly ILogger<InternshipsController> _logger;

        public InternshipsController(IInternshipService service, ILogger<InternshipsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InternshipResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            try
            {
                var items = _service.GetAll()
                    .Select(i => new InternshipResponseDTO
                    {
                        Id = i.Id,
                        Title = i.Title,
                        ShortDescription = i.ShortDescription,
                        FullDescription = i.FullDescription,
                        IsFeatured = i.IsFeatured,
                        Remote = i.Remote,
                        Location = i.Location,
                        PostedAt = i.PostedAt,
                        Deadline = i.Deadline,
                        CompanyId = i.CompanyId,
                        CategoryId = i.CategoryId,
                        CompanyName = (i.Company != null ? i.Company.Name : string.Empty),
                        CategoryName = (i.Category != null ? i.Category.Name : string.Empty)
                    })
                    .ToList();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška GetAll internships.");
                return StatusCode(500, "Dogodila se greška.");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(InternshipResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(int id)
        {
            try
            {
                var i = _service.GetById(id);

                var dto = new InternshipResponseDTO
                {
                    Id = i.Id,
                    Title = i.Title,
                    ShortDescription = i.ShortDescription,
                    FullDescription = i.FullDescription,
                    IsFeatured = i.IsFeatured,
                    Remote = i.Remote,
                    Location = i.Location,
                    PostedAt = i.PostedAt,
                    Deadline = i.Deadline,
                    CompanyId = i.CompanyId,
                    CategoryId = i.CategoryId,
                    CompanyName = (i.Company != null ? i.Company.Name : string.Empty),
                    CategoryName = (i.Category != null ? i.Category.Name : string.Empty)
                };

                return Ok(dto);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Get internship id={Id}", id);
                return StatusCode(500, "Dogodila se greška.");
            }
        }
    }
}
