using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Repositories.Internships;

namespace InternshipPortal.API.Services.Internships
{
    public class InternshipService : IInternshipService
    {
        private readonly IInternshipRepository _repo;

        public InternshipService(IInternshipRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Internship> GetAll()
            => _repo.GetAll() ?? Enumerable.Empty<Internship>();

        public Internship GetById(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            return _repo.GetById(id) ?? throw new NotFoundException($"Praksa s Id={id} nije pronađena.");
        }

        public Internship Create(Internship internship)
        {
            if (internship == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
            if (string.IsNullOrWhiteSpace(internship.Title)) throw new ValidationException("Title je obavezan.");

            internship.PostedAt = internship.PostedAt == default ? DateTime.UtcNow : internship.PostedAt;

            return _repo.Create(internship);
        }

        public Internship Update(int id, Internship internship)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (internship == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
            if (internship.Id != 0 && internship.Id != id) throw new ValidationException("Id u ruti i Id u tijelu zahtjeva moraju biti isti.");

            return _repo.Update(id, internship) ?? throw new NotFoundException($"Praksa s Id={id} nije pronađena.");
        }

        public void Delete(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (!_repo.Delete(id)) throw new NotFoundException($"Praksa s Id={id} nije pronađena.");
        }
    }
}
