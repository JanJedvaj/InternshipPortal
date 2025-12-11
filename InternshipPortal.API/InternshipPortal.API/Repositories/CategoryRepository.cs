using InternshipApi.Data;
using InternshipApi.Models;

namespace InternshipApi.Repositories
{
    public class CategoryRepository : IRepository<Category>
    {
        public IEnumerable<Category> GetAll() =>
            FakeDatabase.Categories;

        public Category GetById(int id) =>
            FakeDatabase.Categories.FirstOrDefault(x => x.Id == id);

        public Category Create(Category entity)
        {
            entity.Id = FakeDatabase.Categories.Max(x => x.Id) + 1;
            FakeDatabase.Categories.Add(entity);
            return entity;
        }

        public Category Update(int id, Category updated)
        {
            var item = GetById(id);
            if (item == null) return null;

            item.Name = updated.Name;
            return item;
        }

        public bool Delete(int id)
        {
            var item = GetById(id);
            if (item == null) return false;
            FakeDatabase.Categories.Remove(item);
            return true;
        }
    }
}
