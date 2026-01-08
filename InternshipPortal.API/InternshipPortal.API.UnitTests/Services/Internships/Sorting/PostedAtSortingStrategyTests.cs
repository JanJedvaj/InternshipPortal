using FluentAssertions;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Services.Internships.Sorting;

namespace InternshipPortal.API.UnitTests.Services.Internships.Sorting;

public class PostedAtSortingStrategyTests
{
    [Fact]
    public void Name_ReturnsDate()
    {
        var strategy = new PostedAtSortingStrategy();
        strategy.Name.Should().Be("date");
    }

    [Fact]
    public void Apply_WhenDescendingIsFalse_SortsByPostedAtAscending()
    {
        var strategy = new PostedAtSortingStrategy();

        var data = new List<Internship>
        {
            new Internship { Id = 1, PostedAt = new DateTime(2026, 1, 10) },
            new Internship { Id = 2, PostedAt = new DateTime(2026, 1, 5) },
            new Internship { Id = 3, PostedAt = new DateTime(2026, 1, 20) }
        }.AsQueryable();

        var ordered = strategy.Apply(data, descending: false).ToList();

        ordered.Select(x => x.Id).Should().ContainInOrder(2, 1, 3);
    }

    [Fact]
    public void Apply_WhenDescendingIsTrue_SortsByPostedAtDescending()
    {
        var strategy = new PostedAtSortingStrategy();

        var data = new List<Internship>
        {
            new Internship { Id = 1, PostedAt = new DateTime(2026, 1, 10) },
            new Internship { Id = 2, PostedAt = new DateTime(2026, 1, 5) },
            new Internship { Id = 3, PostedAt = new DateTime(2026, 1, 20) }
        }.AsQueryable();

        var ordered = strategy.Apply(data, descending: true).ToList();

        ordered.Select(x => x.Id).Should().ContainInOrder(3, 1, 2);
    }
}
