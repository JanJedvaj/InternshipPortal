using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Models;
using InternshipPortal.API.Repositories.Abstractions;
using InternshipPortal.API.Services.Abstractions;

namespace InternshipPortal.API.Services
{
    public class InternshipService : IInternshipService
    {
        private readonly IReadRepository<Internship> _read;
        private readonly IWriteRepository<Internship> _write;

        public InternshipService(IReadRepository<Internship> read, IWriteRepository<Internship> write)
        {
            _read = read;
            _write = write;
        }

        public IEnumerable<Internship> GetAll()
            => _read.GetAll() ?? Enumerable.Empty<Internship>();

        public Internship GetById(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            return _read.GetById(id) ?? throw new NotFoundException($"Praksa s Id={id} nije pronađena.");
        }

        public Internship Create(Internship internship)
        {
            if (internship == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
            if (string.IsNullOrWhiteSpace(internship.Title)) throw new ValidationException("Title je obavezan.");

            internship.PostedAt = internship.PostedAt == default ? DateTime.UtcNow : internship.PostedAt;

            return _write.Create(internship);
        }

        public Internship Update(int id, Internship internship)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (internship == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
            if (internship.Id != 0 && internship.Id != id) throw new ValidationException("Id u ruti i Id u tijelu zahtjeva moraju biti isti.");

            return _write.Update(id, internship) ?? throw new NotFoundException($"Praksa s Id={id} nije pronađena.");
        }

        public void Delete(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (!_write.Delete(id)) throw new NotFoundException($"Praksa s Id={id} nije pronađena.");
        }
    }
}
