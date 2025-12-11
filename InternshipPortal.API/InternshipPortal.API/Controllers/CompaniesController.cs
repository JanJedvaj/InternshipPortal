using InternshipApi.Models;
using InternshipApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternshipApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepository<Company> _repo;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(IRepository<Company> repo, ILogger<CompaniesController> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Company>> GetAll()
        {
            try
            {
                var items = _repo.GetAll()?.ToList() ?? new List<Company>();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom dohvaćanja svih kompanija.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom dohvaćanja kompanija.");
            }
        }

        [HttpGet("{id:int}")]
        public ActionResult<Company> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Id mora biti veći od nule.");

            try
            {
                var item = _repo.GetById(id);
                if (item == null)
                    return NotFound($"Kompanija s Id={id} nije pronađena.");

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom dohvaćanja kompanije Id={Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom dohvaćanja kompanije.");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult<Company> Create([FromBody] Company company)
        {
            if (company == null)
                return BadRequest("Tijelo zahtjeva je prazno.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var created = _repo.Create(company);
                if (created == null)
                {
                    _logger.LogError("Repozitorij je vratio null prilikom kreiranja kompanije.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Dogodila se greška prilikom kreiranja kompanije.");
                }

                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neočekivana greška prilikom kreiranja kompanije.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom kreiranja kompanije.");
            }
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public ActionResult<Company> Update(int id, [FromBody] Company company)
        {
            if (id <= 0)
                return BadRequest("Id mora biti veći od nule.");

            if (company == null)
                return BadRequest("Tijelo zahtjeva je prazno.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (company.Id != 0 && company.Id != id)
                return BadRequest("Id u ruti i Id u tijelu zahtjeva moraju biti isti.");

            try
            {
                var updated = _repo.Update(id, company);
                if (updated == null)
                    return NotFound($"Kompanija s Id={id} nije pronađena za ažuriranje.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neočekivana greška prilikom ažuriranja kompanije Id={Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom ažuriranja kompanije.");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Id mora biti veći od nule.");

            try
            {
                var ok = _repo.Delete(id);
                if (!ok)
                    return NotFound($"Kompanija s Id={id} nije pronađena za brisanje.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neočekivana greška prilikom brisanja kompanije Id={Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom brisanja kompanije.");
            }
        }
    }
}
