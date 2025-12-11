using InternshipApi.Models;
using InternshipApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InternshipApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InternshipsController : ControllerBase
    {
        private readonly IRepository<Internship> _repo;
        private readonly ILogger<InternshipsController> _logger;

        public InternshipsController(
            IRepository<Internship> repo,
            ILogger<InternshipsController> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Internship>> GetAll()
        {
            try
            {
                var items = _repo.GetAll()?.ToList() ?? new List<Internship>();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom dohvaćanja svih praksi.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom dohvaćanja praksi.");
            }
        }

        [HttpGet("{id:int}")]
        public ActionResult<Internship> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Id mora biti veći od nule.");

            try
            {
                var item = _repo.GetById(id);

                if (item == null)
                    return NotFound($"Praksa s Id={id} nije pronađena.");

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom dohvaćanja prakse s Id={Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom dohvaćanja prakse.");
            }
        }

        [HttpPost]
        public ActionResult<Internship> Create([FromBody] Internship internship)
        {
            if (internship == null)
                return BadRequest("Tijelo zahtjeva je prazno.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var created = _repo.Create(internship);

                if (created == null)
                {
                    _logger.LogError("Repozitorij je vratio null prilikom kreiranja prakse.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Dogodila se greška prilikom kreiranja prakse.");
                }

                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                
                _logger.LogError(ex, "Greška domene prilikom kreiranja prakse.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se interna greška prilikom generiranja ID-a prakse.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neočekivana greška prilikom kreiranja prakse.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom kreiranja prakse.");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult<Internship> Update(int id, [FromBody] Internship internship)
        {
            if (id <= 0)
                return BadRequest("Id mora biti veći od nule.");

            if (internship == null)
                return BadRequest("Tijelo zahtjeva je prazno.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (internship.Id != 0 && internship.Id != id)
                return BadRequest("Id u ruti i Id u tijelu zahtjeva moraju biti isti.");

            try
            {
                var updated = _repo.Update(id, internship);

                if (updated == null)
                    return NotFound($"Praksa s Id={id} nije pronađena za ažuriranje.");

                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Pokušaj ažuriranja nepostojeće prakse Id={Id}.", id);
                return NotFound($"Praksa s Id={id} nije pronađena.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neočekivana greška prilikom ažuriranja prakse Id={Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom ažuriranja prakse.");
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Id mora biti veći od nule.");

            try
            {
                var ok = _repo.Delete(id);

                if (!ok)
                    return NotFound($"Praksa s Id={id} nije pronađena za brisanje.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neočekivana greška prilikom brisanja prakse Id={Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom brisanja prakse.");
            }
        }
    }
}
