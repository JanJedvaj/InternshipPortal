using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Repositories.Categories;

namespace InternshipPortal.API.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Category> GetAll()
            => _repo.GetAll() ?? Enumerable.Empty<Category>();

        public Category GetById(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            return _repo.GetById(id) ?? throw new NotFoundException($"Kategorija s Id={id} nije pronađena.");
        }

        public Category Create(Category category)
        {
            if (category == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
            if (string.IsNullOrWhiteSpace(category.Name)) throw new ValidationException("Name je obavezan.");
            return _repo.Create(category);
        }

        public Category Update(int id, Category category)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (category == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
            if (category.Id != 0 && category.Id != id) throw new ValidationException("Id u ruti i Id u tijelu zahtjeva moraju biti isti.");

            return _repo.Update(id, category) ?? throw new NotFoundException($"Kategorija s Id={id} nije pronađena.");
        }

        public void Delete(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (!_repo.Delete(id)) throw new NotFoundException($"Kategorija s Id={id} nije pronađena.");
        }
    }
}
