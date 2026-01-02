using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Services.Internships.Sorting;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace InternshipPortal.API.Services.Internships.Search
{

    public class InternshipSearchFacade : IInternshipSearchFacade
    {
        private readonly InternshipPortalContext _context;
        private readonly IReadOnlyDictionary<string, IInternshipSortingStrategy> _sortingStrategies;

        public InternshipSearchFacade(
            InternshipPortalContext context,
            IEnumerable<IInternshipSortingStrategy> sortingStrategies)
        {
            _context = context;


            _sortingStrategies = sortingStrategies
                .GroupBy(s => s.Name.ToLower())
                .ToDictionary(g => g.Key, g => g.First());
        }

        public InternshipSearchResult Search(InternshipSearchCriteria criteria)
        {
            if (criteria == null)
                criteria = new InternshipSearchCriteria();

            if (criteria.Page <= 0) criteria.Page = 1;
            if (criteria.PageSize <= 0) criteria.PageSize = 10;

            IQueryable<Internship> query = _context.Internships.AsNoTracking();


            if (!string.IsNullOrWhiteSpace(criteria.Keyword))
            {
                string keyword = criteria.Keyword.Trim().ToLower();

                query = query.Where(i =>
                    i.Title != null && i.Title.ToLower().Contains(keyword) ||
                    i.ShortDescription != null && i.ShortDescription.ToLower().Contains(keyword) ||
                    i.FullDescription != null && i.FullDescription.ToLower().Contains(keyword)
                );
            }

            if (criteria.CategoryId.HasValue)
                query = query.Where(i => i.CategoryId == criteria.CategoryId.Value);

            if (criteria.CompanyId.HasValue)
                query = query.Where(i => i.CompanyId == criteria.CompanyId.Value);

            if (criteria.Remote.HasValue)
                query = query.Where(i => i.Remote == criteria.Remote.Value);

            if (!string.IsNullOrWhiteSpace(criteria.Location))
            {
                string loc = criteria.Location.Trim().ToLower();
                query = query.Where(i => i.Location != null && i.Location.ToLower().Contains(loc));
            }

            if (criteria.OnlyActive)
            {
                var today = DateTime.UtcNow.Date;
                query = query.Where(i =>
                    !i.Deadline.HasValue || i.Deadline.Value.Date >= today);
            }

            int totalCount = query.Count();



            bool desc = criteria.SortDescending;
            string sortKey = (criteria.SortBy ?? "date").ToLower();

            if (!_sortingStrategies.TryGetValue(sortKey, out var strategy))
            {

                if (!_sortingStrategies.TryGetValue("date", out strategy))
                {

                    var unsortedItems = query
                        .Skip((criteria.Page - 1) * criteria.PageSize)
                        .Take(criteria.PageSize)
                        .ToList();

                    return new InternshipSearchResult
                    {
                        Items = unsortedItems,
                        TotalCount = totalCount,
                        Page = criteria.Page,
                        PageSize = criteria.PageSize
                    };
                }
            }

            IOrderedQueryable<Internship> orderedQuery = strategy.Apply(query, desc);

            int skip = (criteria.Page - 1) * criteria.PageSize;

            var items = orderedQuery
                .Skip(skip)
                .Take(criteria.PageSize)
                .ToList();

            return new InternshipSearchResult
            {
                Items = items,
                TotalCount = totalCount,
                Page = criteria.Page,
                PageSize = criteria.PageSize
            };
        }
    }
}
