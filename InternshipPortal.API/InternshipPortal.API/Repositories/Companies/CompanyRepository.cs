using InternshipPortal.API.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace InternshipPortal.API.Repositories.Companies
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly InternshipPortalContext _context;

        public CompanyRepository(InternshipPortalContext context)
        {
            _context = context;
        }

        public IEnumerable<Company> GetAll()
            => _context.Companies.AsNoTracking().ToList();

        public Company? GetById(int id)
            => _context.Companies.AsNoTracking().FirstOrDefault(x => x.Id == id);

        public Company Create(Company entity)
        {
            _context.Companies.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public Company? Update(int id, Company entity)
        {
            var existing = _context.Companies.FirstOrDefault(x => x.Id == id);
            if (existing == null) return null;

            existing.Name = entity.Name;
            existing.Website = entity.Website;
            existing.Location = entity.Location;

            _context.SaveChanges();
            return existing;
        }

        public bool Delete(int id)
        {
            var existing = _context.Companies.FirstOrDefault(x => x.Id == id);
            if (existing == null) return false;

            _context.Companies.Remove(existing);
            _context.SaveChanges();
            return true;
        }
    }
}
