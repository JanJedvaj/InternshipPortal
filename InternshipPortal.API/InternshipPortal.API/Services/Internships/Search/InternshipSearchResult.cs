using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Services.Internships.Search
{

    public class InternshipSearchResult
    {
        public IReadOnlyCollection<Internship> Items { get; init; } = Array.Empty<Internship>();
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
    }
}
