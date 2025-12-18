using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Repositories.Categories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        Category? GetById(int id);
        Category Create(Category entity);
        Category? Update(int id, Category entity);
        bool Delete(int id);
    }
}
