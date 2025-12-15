using InternshipPortal.API.Data;
using InternshipPortal.API.Models;
using InternshipPortal.API.Repositories.Abstractions;

namespace InternshipPortal.API.Repositories
{
    public class InternshipRepository : IReadRepository<Internship>, IWriteRepository<Internship>
    {
        public IEnumerable<Internship> GetAll() => FakeDatabase.Internships;

        public Internship? GetById(int id)
            => FakeDatabase.Internships.FirstOrDefault(x => x.Id == id);

        public Internship Create(Internship entity)
        {
            var newId = FakeDatabase.Internships.Any() ? FakeDatabase.Internships.Max(x => x.Id) + 1 : 1;
            entity.Id = newId;
            FakeDatabase.Internships.Add(entity);
            return entity;
        }

        public Internship? Update(int id, Internship entity)
        {
            var existing = GetById(id);
            if (existing == null) return null;

            existing.Title = entity.Title;
            existing.ShortDescription = entity.ShortDescription;
            existing.FullDescription = entity.FullDescription;
            existing.CategoryId = entity.CategoryId;
            existing.CompanyId = entity.CompanyId;
            existing.Deadline = entity.Deadline;
            existing.IsFeatured = entity.IsFeatured;
            existing.Remote = entity.Remote;
            existing.Location = entity.Location;

            return existing;
        }

        public bool Delete(int id)
        {
            var existing = GetById(id);
            if (existing == null) return false;
            return FakeDatabase.Internships.Remove(existing);
        }
    }
}
