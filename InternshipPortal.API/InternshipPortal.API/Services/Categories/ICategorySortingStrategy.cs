using System.Collections.Generic;
using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Services.Categories
{
    public interface ICategorySortingStrategy
    {
        string Key { get; }  // npr. "mostUsed"
        IEnumerable<Category> Sort(IEnumerable<Category> categories);
    }
}
