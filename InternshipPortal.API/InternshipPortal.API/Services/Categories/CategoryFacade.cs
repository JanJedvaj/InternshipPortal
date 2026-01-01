using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Repositories.Internships;

namespace InternshipPortal.API.Services.Categories
{
    public class CategoryFacade : ICategoryFacade
    {
        private readonly ICategoryService _categoryService;
        private readonly IInternshipRepository _internshipRepository;

        public CategoryFacade(
            ICategoryService categoryService,
            IInternshipRepository internshipRepository)
        {
            _categoryService = categoryService;
            _internshipRepository = internshipRepository;
        }

        public void DeleteCategorySafely(int categoryId)
        {
            if (categoryId <= 0)
                throw new ValidationException("Id mora biti veći od nule.");

            // 1) Provjeri da kategorija postoji (service već baca NotFound)
            _categoryService.GetById(categoryId);

            // 2) Provjeri ima li oglasa u toj kategoriji
            var hasInternships = _internshipRepository
                .GetAll()
                .Any(i => i.CategoryId == categoryId);

            if (hasInternships)
                throw new ValidationException("Ne možeš obrisati kategoriju koja ima oglase/prakse.");

            // 3) Ako je sve OK, obriši
            _categoryService.Delete(categoryId);
        }
    }
}
