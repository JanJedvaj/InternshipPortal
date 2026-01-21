using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Internships;
using InternshipPortal.API.Data.EF;
using Microsoft.AspNetCore.Mvc;
using InternshipPortal.BL.DTOi.Internships;
using InternshipPortal.API.Services.Internships.Search;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InternshipsController : ControllerBase
    {
        private readonly IInternshipService _service;
        private readonly ILogger<InternshipsController> _logger;

        private readonly IInternshipSearchFacade _searchFacade;

        //Updejtan konstruktor za fasadu (injectamo fasadu)
        public InternshipsController(
       IInternshipService service,
       IInternshipSearchFacade searchFacade,
       ILogger<InternshipsController> logger)
        {
            _service = service;
            _searchFacade = searchFacade;
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InternshipPortal.API.Data.EF.Company>), StatusCodes.Status200OK)]
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
        [ProducesResponseType(typeof(IEnumerable<InternshipPortal.API.Data.EF.Company>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get(int id)
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


        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<InternshipPortal.API.Data.EF.Company>), StatusCodes.Status200OK)]
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
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška Create internship.");
                return StatusCode(500, "Dogodila se greška.");
            }
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(IEnumerable<InternshipPortal.API.Data.EF.Company>), StatusCodes.Status200OK)]
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

        [HttpGet("search")]
        [ProducesResponseType(typeof(InternshipSearchResult), StatusCodes.Status200OK)]
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
