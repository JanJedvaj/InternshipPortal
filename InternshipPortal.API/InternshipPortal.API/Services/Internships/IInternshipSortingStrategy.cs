using InternshipPortal.API.Data.EF;
using System.Linq;

namespace InternshipPortal.API.Services.Internships
{
 
    public interface IInternshipSortingStrategy
    {
        
        string Name { get; }

        
        IOrderedQueryable<Internship> Apply(IQueryable<Internship> query, bool descending);
    }
}
