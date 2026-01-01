using System;
using System.Collections.Generic;
using System.Linq;

namespace InternshipPortal.API.Services.Categories
{
    public class CategorySortingStrategyResolver
    {
        private readonly Dictionary<string, ICategorySortingStrategy> _strategies;

        public CategorySortingStrategyResolver(IEnumerable<ICategorySortingStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(
                s => s.Key,
                s => s,
                StringComparer.OrdinalIgnoreCase);
        }

        public ICategorySortingStrategy Resolve(string sortKey)
        {
            if (string.IsNullOrWhiteSpace(sortKey))
                throw new ArgumentException("Sort key nije poslan.");

            if (_strategies.TryGetValue(sortKey, out var strategy))
                return strategy;

            throw new ArgumentException($"Nepoznat sort key: '{sortKey}'.");
        }
    }
}
