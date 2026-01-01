using System;
using System.Collections.Generic;
using System.Linq;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Repositories.Internships;

namespace InternshipPortal.API.Services.Categories
{
    /// Sortira kategorije po broju oglasa/praksi (Internships) u toj kategoriji (DESC).
    /// Strategy pattern (Behavioral).

    public class MostUsedCategorySortingStrategy : ICategorySortingStrategy
    {
        public string Key => "mostUsed";

        private readonly IInternshipRepository _internshipRepository;

        public MostUsedCategorySortingStrategy(IInternshipRepository internshipRepository)
        {
            _internshipRepository = internshipRepository;
        }

        public IEnumerable<Category> Sort(IEnumerable<Category> categories)
        {
            if (categories == null) return Enumerable.Empty<Category>();

            // Brojanje internshipa po CategoryId
            var counts = _internshipRepository
                .GetAll()
                .GroupBy(i => i.CategoryId)
                .ToDictionary(g => g.Key, g => g.Count());

            // Sort: najviše oglasa prvo, pa Name
            return categories
                .OrderByDescending(c => counts.ContainsKey(c.Id) ? counts[c.Id] : 0)
                .ThenBy(c => c.Name);
        }
    }
}
