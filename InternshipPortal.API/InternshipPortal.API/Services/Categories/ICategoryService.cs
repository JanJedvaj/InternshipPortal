using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Services.Categories
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetAll();
        Category GetById(int id);
        Category Create(Category category);
        Category Update(int id, Category category);
        void Delete(int id);
    }
}
