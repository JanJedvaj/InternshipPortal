using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Models;
using InternshipPortal.API.Repositories.Abstractions;
using InternshipPortal.API.Services.Abstractions;

namespace InternshipPortal.API.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IReadRepository<Company> _read;
        private readonly IWriteRepository<Company> _write;

        public CompanyService(IReadRepository<Company> read, IWriteRepository<Company> write)
        {
            _read = read;
            _write = write;
        }

        public IEnumerable<Company> GetAll()
            => _read.GetAll() ?? Enumerable.Empty<Company>();

        public Company GetById(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            return _read.GetById(id) ?? throw new NotFoundException($"Kompanija s Id={id} nije pronađena.");
        }

        public Company Create(Company company)
        {
            if (company == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
            if (string.IsNullOrWhiteSpace(company.Name)) throw new ValidationException("Name je obavezan.");
            return _write.Create(company);
        }

        public Company Update(int id, Company company)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (company == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
            if (company.Id != 0 && company.Id != id) throw new ValidationException("Id u ruti i Id u tijelu zahtjeva moraju biti isti.");

            return _write.Update(id, company) ?? throw new NotFoundException($"Kompanija s Id={id} nije pronađena.");
        }

        public void Delete(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (!_write.Delete(id)) throw new NotFoundException($"Kompanija s Id={id} nije pronađena.");
        }
    }
}
