using InternshipPortal.API.Data.EF;
using System.Linq;

namespace InternshipPortal.API.Services.Internships
{
    /// <summary>
    /// Sortiranje po roku prijave (Deadline).
    /// Name = "deadline"
    /// </summary>
    public class DeadlineSortingStrategy : IInternshipSortingStrategy
    {
        public string Name => "deadline";

        public IOrderedQueryable<Internship> Apply(IQueryable<Internship> query, bool descending)
        {
            return descending
                ? query.OrderByDescending(i => i.Deadline)
                : query.OrderBy(i => i.Deadline);
        }
    }
}
