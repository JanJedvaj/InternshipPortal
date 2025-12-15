using InternshipPortal.API.Data;
using InternshipPortal.API.Models;
using InternshipPortal.API.Repositories.Abstractions;

namespace InternshipPortal.API.Repositories
{
    public class CompanyRepository : IReadRepository<Company>, IWriteRepository<Company>
    {
        public IEnumerable<Company> GetAll() => FakeDatabase.Companies;

        public Company? GetById(int id)
            => FakeDatabase.Companies.FirstOrDefault(x => x.Id == id);

        public Company Create(Company entity)
        {
            var newId = FakeDatabase.Companies.Any() ? FakeDatabase.Companies.Max(x => x.Id) + 1 : 1;
            entity.Id = newId;
            FakeDatabase.Companies.Add(entity);
            return entity;
        }

        public Company? Update(int id, Company entity)
        {
            var existing = GetById(id);
            if (existing == null) return null;

            existing.Name = entity.Name;
            existing.Website = entity.Website;
            existing.Location = entity.Location;

            return existing;
        }

        public bool Delete(int id)
        {
            var existing = GetById(id);
            if (existing == null) return false;
            return FakeDatabase.Companies.Remove(existing);
        }
    }
}
