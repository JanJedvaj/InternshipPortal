using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Services.Internships
{

    public interface IInternshipFactory
    {
      
        Internship CreateNew(Internship internship);

       
        Internship ApplyUpdates(Internship existing, Internship updates);
    }
}
