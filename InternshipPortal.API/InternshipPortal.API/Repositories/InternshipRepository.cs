using InternshipApi.Data;
using InternshipApi.Models;

namespace InternshipApi.Repositories
{
    public class InternshipRepository : IRepository<Internship>
    {
        public IEnumerable<Internship> GetAll() =>
            FakeDatabase.Internships;

        public Internship GetById(int id) =>
            FakeDatabase.Internships.FirstOrDefault(x => x.Id == id);

        public Internship Create(Internship entity)
        {
            entity.Id = FakeDatabase.Internships.Any()
                ? FakeDatabase.Internships.Max(x => x.Id) + 1
                : 1;

            FakeDatabase.Internships.Add(entity);
            return entity;
        }

        public Internship Update(int id, Internship updated)
        {
            var item = GetById(id);
            if (item == null) return null;

            item.Title = updated.Title;
            item.ShortDescription = updated.ShortDescription;
            item.FullDescription = updated.FullDescription;
            item.CategoryId = updated.CategoryId;
            item.CompanyId = updated.CompanyId;
            item.IsFeatured = updated.IsFeatured;
            item.Remote = updated.Remote;
            item.Location = updated.Location;
            item.Deadline = updated.Deadline;

            return item;
        }

        public bool Delete(int id)
        {
            var item = GetById(id);
            if (item == null) return false;

            FakeDatabase.Internships.Remove(item);
            return true;
        }
    }
}
