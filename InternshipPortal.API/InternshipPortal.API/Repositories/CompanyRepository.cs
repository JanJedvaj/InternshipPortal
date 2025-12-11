using InternshipApi.Data;
using InternshipApi.Models;

namespace InternshipApi.Repositories
{
    public class CompanyRepository : IRepository<Company>
    {
        public IEnumerable<Company> GetAll() =>
            FakeDatabase.Companies;

        public Company GetById(int id) =>
            FakeDatabase.Companies.FirstOrDefault(x => x.Id == id);

        public Company Create(Company entity)
        {
            entity.Id = FakeDatabase.Companies.Max(x => x.Id) + 1;
            FakeDatabase.Companies.Add(entity);
            return entity;
        }

        public Company Update(int id, Company updated)
        {
            var item = GetById(id);
            if (item == null) return null;

            item.Name = updated.Name;
            item.Website = updated.Website;
            item.Location = updated.Location;

            return item;
        }

        public bool Delete(int id)
        {
            var item = GetById(id);
            if (item == null) return false;
            FakeDatabase.Companies.Remove(item);
            return true;
        }
    }
}
