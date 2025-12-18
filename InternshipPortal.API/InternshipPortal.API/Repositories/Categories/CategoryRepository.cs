using InternshipPortal.API.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace InternshipPortal.API.Repositories.Categories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly InternshipPortalContext _context;

        public CategoryRepository(InternshipPortalContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetAll()
            => _context.Categories.AsNoTracking().ToList();

        public Category? GetById(int id)
            => _context.Categories.AsNoTracking().FirstOrDefault(x => x.Id == id);

        public Category Create(Category entity)
        {
            _context.Categories.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public Category? Update(int id, Category entity)
        {
            var existing = _context.Categories.FirstOrDefault(x => x.Id == id);
            if (existing == null) return null;

            existing.Name = entity.Name;
            _context.SaveChanges();
            return existing;
        }

        public bool Delete(int id)
        {
            var existing = _context.Categories.FirstOrDefault(x => x.Id == id);
            if (existing == null) return false;

            _context.Categories.Remove(existing);
            _context.SaveChanges();
            return true;
        }
    }
}
