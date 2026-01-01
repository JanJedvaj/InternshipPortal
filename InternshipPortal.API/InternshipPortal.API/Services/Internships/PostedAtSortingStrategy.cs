using InternshipPortal.API.Data.EF;
using System.Linq;

namespace InternshipPortal.API.Services.Internships
{
 
    public class PostedAtSortingStrategy : IInternshipSortingStrategy
    {
        public string Name => "date";

        public IOrderedQueryable<Internship> Apply(IQueryable<Internship> query, bool descending)
        {
            return descending
                ? query.OrderByDescending(i => i.PostedAt)
                : query.OrderBy(i => i.PostedAt);
        }
    }
}
