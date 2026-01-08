using FluentAssertions;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Services.Internships.Sorting;

namespace InternshipPortal.API.UnitTests.Services.Internships.Sorting;

public class TitleSortingStrategyTests
{
    [Fact]
    public void Name_ReturnsTitle()
    {
        var strategy = new TitleSortingStrategy();
        strategy.Name.Should().Be("title");
    }

    [Fact]
    public void Apply_WhenDescendingIsFalse_SortsByTitleAscending()
    {
        var strategy = new TitleSortingStrategy();

        var data = new List<Internship>
        {
            new Internship { Id = 1, Title = "Zebra" },
            new Internship { Id = 2, Title = "Alpha" },
            new Internship { Id = 3, Title = "Mango" }
        }.AsQueryable();

        var ordered = strategy.Apply(data, descending: false).ToList();

        ordered.Select(x => x.Id).Should().ContainInOrder(2, 3, 1);
    }

    [Fact]
    public void Apply_WhenDescendingIsTrue_SortsByTitleDescending()
    {
        var strategy = new TitleSortingStrategy();

        var data = new List<Internship>
        {
            new Internship { Id = 1, Title = "Zebra" },
            new Internship { Id = 2, Title = "Alpha" },
            new Internship { Id = 3, Title = "Mango" }
        }.AsQueryable();

        var ordered = strategy.Apply(data, descending: true).ToList();

        ordered.Select(x => x.Id).Should().ContainInOrder(1, 3, 2);
    }
}
