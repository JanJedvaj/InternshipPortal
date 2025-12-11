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

        public InternshipsController(IRepository<Internship> repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll() =>
            Ok(_repo.GetAll());

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var item = _repo.GetById(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public IActionResult Create(Internship internship)
        {
            var created = _repo.Create(internship);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Internship internship)
        {
            var updated = _repo.Update(id, internship);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var ok = _repo.Delete(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
