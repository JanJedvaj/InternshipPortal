using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Categories;

namespace InternshipPortal.API.Services.Categories
{
    public class DefaultCategoryFactory : ICategoryFactory
    {
        private readonly ICategoryNameStrategy _nameStrategy;

        public DefaultCategoryFactory(ICategoryNameStrategy nameStrategy)
        {
            _nameStrategy = nameStrategy;
        }

        public Category CreateNew(Category category)
        {
            if (category == null)
                throw new ValidationException("Tijelo zahtjeva je prazno.");

            _nameStrategy.Validate(category.Name);

            return new Category
            {
                // Id=0 -> EF dodjeljuje
                Name = _nameStrategy.Normalize(category.Name)
            };
        }

        public Category ApplyUpdates(Category existing, Category updates)
        {
            if (existing == null)
                throw new ValidationException("Postojeća kategorija ne smije biti null.");

            if (updates == null)
                throw new ValidationException("Tijelo zahtjeva je prazno.");

            _nameStrategy.Validate(updates.Name);

            existing.Name = _nameStrategy.Normalize(updates.Name);
            return existing;
        }
    }
}
