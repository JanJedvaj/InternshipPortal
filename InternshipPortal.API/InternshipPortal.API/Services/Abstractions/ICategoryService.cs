using InternshipPortal.API.Models;

namespace InternshipPortal.API.Services.Abstractions
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
