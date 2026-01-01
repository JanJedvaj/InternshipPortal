using InternshipPortal.API.Data.EF;
using System.Linq;

namespace InternshipPortal.API.Services.Internships
{
    /// <summary>
    /// Strategy sučelje za različite algoritme sortiranja Internship entiteta.
    /// </summary>
    public interface IInternshipSortingStrategy
    {
        /// <summary>
        /// Jedinstveni naziv strategije (npr. "date", "deadline", "title").
        /// Koristi se za odabir strategije na temelju parametra SortBy.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Primjenjuje sortiranje nad upitom.
        /// </summary>
        IOrderedQueryable<Internship> Apply(IQueryable<Internship> query, bool descending);
    }
}
