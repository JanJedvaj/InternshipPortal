using InternshipPortal.API.Models;

namespace InternshipPortal.API.Services.Abstractions
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
