using InternshipPortal.API.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace InternshipPortal.API.Repositories.Internships
{
    public class InternshipRepository : IInternshipRepository
    {
        private readonly InternshipPortalContext _context;

        public InternshipRepository(InternshipPortalContext context)
        {
            _context = context;
        }

        public IEnumerable<Internship> GetAll()
            => _context.Internships.AsNoTracking().ToList();

        public Internship? GetById(int id)
            => _context.Internships.AsNoTracking().FirstOrDefault(x => x.Id == id);

        public Internship Create(Internship entity)
        {
            _context.Internships.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public Internship? Update(int id, Internship entity)
        {
            var existing = _context.Internships.FirstOrDefault(x => x.Id == id);
            if (existing == null) return null;

            existing.Title = entity.Title;
            existing.ShortDescription = entity.ShortDescription;
            existing.FullDescription = entity.FullDescription;
            existing.IsFeatured = entity.IsFeatured;
            existing.Remote = entity.Remote;
            existing.Location = entity.Location;
            existing.Deadline = entity.Deadline;

            existing.CompanyId = entity.CompanyId;
            existing.CategoryId = entity.CategoryId;

            _context.SaveChanges();
            return existing;
        }

        public bool Delete(int id)
        {
            var existing = _context.Internships.FirstOrDefault(x => x.Id == id);
            if (existing == null) return false;

            _context.Internships.Remove(existing);
            _context.SaveChanges();
            return true;
        }
    }
}
