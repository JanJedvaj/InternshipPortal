using InternshipApi.Models;
using InternshipApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternshipApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IRepository<Category> _repo;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IRepository<Category> repo, ILogger<CategoriesController> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetAll()
        {
            try
            {
                var items = _repo.GetAll()?.ToList() ?? new List<Category>();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom dohvaćanja svih kategorija.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom dohvaćanja kategorija.");
            }
        }

        [HttpGet("{id:int}")]
        public ActionResult<Category> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Id mora biti veći od nule.");

            try
            {
                var item = _repo.GetById(id);
                if (item == null)
                    return NotFound($"Kategorija s Id={id} nije pronađena.");

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom dohvaćanja kategorije Id={Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom dohvaćanja kategorije.");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult<Category> Create([FromBody] Category category)
        {
            if (category == null)
                return BadRequest("Tijelo zahtjeva je prazno.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var created = _repo.Create(category);
                if (created == null)
                {
                    _logger.LogError("Repozitorij je vratio null prilikom kreiranja kategorije.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Dogodila se greška prilikom kreiranja kategorije.");
                }

                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neočekivana greška prilikom kreiranja kategorije.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom kreiranja kategorije.");
            }
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public ActionResult<Category> Update(int id, [FromBody] Category category)
        {
            if (id <= 0)
                return BadRequest("Id mora biti veći od nule.");

            if (category == null)
                return BadRequest("Tijelo zahtjeva je prazno.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (category.Id != 0 && category.Id != id)
                return BadRequest("Id u ruti i Id u tijelu zahtjeva moraju biti isti.");

            try
            {
                var updated = _repo.Update(id, category);
                if (updated == null)
                    return NotFound($"Kategorija s Id={id} nije pronađena za ažuriranje.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neočekivana greška prilikom ažuriranja kategorije Id={Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom ažuriranja kategorije.");
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
                    return NotFound($"Kategorija s Id={id} nije pronađena za brisanje.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neočekivana greška prilikom brisanja kategorije Id={Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom brisanja kategorije.");
            }
        }
    }
}
