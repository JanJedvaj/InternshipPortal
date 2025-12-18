using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Services.Internships
{
    public interface IInternshipService
    {
        IEnumerable<Internship> GetAll();
        Internship GetById(int id);
        Internship Create(Internship internship);
        Internship Update(int id, Internship internship);
        void Delete(int id);
    }
}
