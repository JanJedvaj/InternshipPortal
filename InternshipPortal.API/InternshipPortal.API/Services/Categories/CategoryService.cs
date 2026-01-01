using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Repositories.Categories;

namespace InternshipPortal.API.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly ICategoryFactory _factory;

        public CategoryService(ICategoryRepository repo, ICategoryFactory factory)
        {
            _repo = repo;
            _factory = factory;
        }

        public IEnumerable<Category> GetAll() => _repo.GetAll() ?? Enumerable.Empty<Category>();

        public Category GetById(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            return _repo.GetById(id) ?? throw new NotFoundException($"Kategorija s Id={id} nije pronađena.");
        }

        public Category Create(Category category)
        {
            var entity = _factory.CreateNew(category);
            return _repo.Create(entity);
        }

        public Category Update(int id, Category category)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");

            var existing = _repo.GetById(id)
                ?? throw new NotFoundException($"Kategorija s Id={id} nije pronađena.");

            var updated = _factory.ApplyUpdates(existing, category);

            return _repo.Update(id, updated)
                ?? throw new NotFoundException($"Kategorija s Id={id} nije pronađena.");
        }

        public void Delete(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (!_repo.Delete(id)) throw new NotFoundException($"Kategorija s Id={id} nije pronađena.");
        }
    }
}
