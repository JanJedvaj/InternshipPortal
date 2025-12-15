using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Models;
using InternshipPortal.API.Repositories.Abstractions;
using InternshipPortal.API.Services.Abstractions;

namespace InternshipPortal.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IReadRepository<Category> _read;
        private readonly IWriteRepository<Category> _write;

        public CategoryService(IReadRepository<Category> read, IWriteRepository<Category> write)
        {
            _read = read;
            _write = write;
        }

        public IEnumerable<Category> GetAll()
            => _read.GetAll() ?? Enumerable.Empty<Category>();

        public Category GetById(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            return _read.GetById(id) ?? throw new NotFoundException($"Kategorija s Id={id} nije pronađena.");
        }

        public Category Create(Category category)
        {
            if (category == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
            if (string.IsNullOrWhiteSpace(category.Name)) throw new ValidationException("Name je obavezan.");
            return _write.Create(category);
        }

        public Category Update(int id, Category category)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (category == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
            if (category.Id != 0 && category.Id != id) throw new ValidationException("Id u ruti i Id u tijelu zahtjeva moraju biti isti.");

            return _write.Update(id, category) ?? throw new NotFoundException($"Kategorija s Id={id} nije pronađena.");
        }

        public void Delete(int id)
        {
            if (id <= 0) throw new ValidationException("Id mora biti veći od nule.");
            if (!_write.Delete(id)) throw new NotFoundException($"Kategorija s Id={id} nije pronađena.");
        }
    }
}
