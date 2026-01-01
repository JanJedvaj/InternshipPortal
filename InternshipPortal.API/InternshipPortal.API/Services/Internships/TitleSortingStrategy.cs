using InternshipPortal.API.Data.EF;
using System.Linq;

namespace InternshipPortal.API.Services.Internships
{
    /// <summary>
    /// Sortiranje po naslovu oglasa (Title).
    /// Name = "title"
    /// </summary>
    public class TitleSortingStrategy : IInternshipSortingStrategy
    {
        public string Name => "title";

        public IOrderedQueryable<Internship> Apply(IQueryable<Internship> query, bool descending)
        {
            return descending
                ? query.OrderByDescending(i => i.Title)
                : query.OrderBy(i => i.Title);
        }
    }
}
