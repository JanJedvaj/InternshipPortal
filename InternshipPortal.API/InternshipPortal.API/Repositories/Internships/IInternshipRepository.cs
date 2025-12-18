using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Repositories.Internships
{
    public interface IInternshipRepository
    {
        IEnumerable<Internship> GetAll();
        Internship? GetById(int id);
        Internship Create(Internship entity);
        Internship? Update(int id, Internship entity);
        bool Delete(int id);
    }
}
