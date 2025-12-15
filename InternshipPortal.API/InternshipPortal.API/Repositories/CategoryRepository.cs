using InternshipPortal.API.Data;
using InternshipPortal.API.Models;
using InternshipPortal.API.Repositories.Abstractions;

namespace InternshipPortal.API.Repositories
{
    public class CategoryRepository : IReadRepository<Category>, IWriteRepository<Category>
    {
        public IEnumerable<Category> GetAll() => FakeDatabase.Categories;

        public Category? GetById(int id)
            => FakeDatabase.Categories.FirstOrDefault(x => x.Id == id);

        public Category Create(Category entity)
        {
            var newId = FakeDatabase.Categories.Any() ? FakeDatabase.Categories.Max(x => x.Id) + 1 : 1;
            entity.Id = newId;
            FakeDatabase.Categories.Add(entity);
            return entity;
        }

        public Category? Update(int id, Category entity)
        {
            var existing = GetById(id);
            if (existing == null) return null;

            existing.Name = entity.Name;
            return existing;
        }

        public bool Delete(int id)
        {
            var existing = GetById(id);
            if (existing == null) return false;
            return FakeDatabase.Categories.Remove(existing);
        }
    }
}
