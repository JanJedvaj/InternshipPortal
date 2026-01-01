namespace InternshipPortal.API.Services.Categories
{
    /// Facade: pojednostavljuje kompleksnije operacije nad kategorijama
    /// (npr. safe delete).
    public interface ICategoryFacade
    {
        void DeleteCategorySafely(int categoryId);
    }
}
