using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Services.Categories
{
    public interface ICategoryFactory
    {
        Category CreateNew(Category category);
        Category ApplyUpdates(Category existing, Category updates);
    }
}
