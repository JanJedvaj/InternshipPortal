using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Repositories.Companies;

namespace InternshipPortal.API.Services.Companies
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repo;

        public CompanyService(ICompanyRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Company> GetAll()
            => _repo.GetAll() ?? Enumerable.Empty<Company>();

        public Company GetById(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            return _repo.GetById(id) ?? throw new NotFoundException($"Kompanija s Id={id} nije pronađena.");
        }

        public Company Create(Company company)
        {
            if (company == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
            if (string.IsNullOrWhiteSpace(company.Name)) throw new ValidationException("Name je obavezan.");
            return _repo.Create(company);
        }

        public Company Update(int id, Company company)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (company == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
            if (company.Id != 0 && company.Id != id) throw new ValidationException("Id u ruti i Id u tijelu zahtjeva moraju biti isti.");

            return _repo.Update(id, company) ?? throw new NotFoundException($"Kompanija s Id={id} nije pronađena.");
        }

        public void Delete(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (!_repo.Delete(id)) throw new NotFoundException($"Kompanija s Id={id} nije pronađena.");
        }
    }
}
