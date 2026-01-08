using FluentAssertions;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Services.Internships.Sorting;

namespace InternshipPortal.API.UnitTests.Services.Internships.Sorting;

public class DeadlineSortingStrategyTests
{
    [Fact]
    public void Name_ReturnsDeadline()
    {
        // Arrange
        var strategy = new DeadlineSortingStrategy();

        // Act
        var name = strategy.Name;

        // Assert
        name.Should().Be("deadline");
    }

    [Fact]
    public void Apply_WhenDescendingIsFalse_SortsByDeadlineAscending()
    {
        // Arrange
        var strategy = new DeadlineSortingStrategy();

        var data = new List<Internship>
        {
            new Internship { Id = 1, Deadline = new DateTime(2026, 1, 10) },
            new Internship { Id = 2, Deadline = new DateTime(2026, 1, 5) },
            new Internship { Id = 3, Deadline = new DateTime(2026, 1, 20) }
        }.AsQueryable();

        // Act
        var ordered = strategy.Apply(data, descending: false).ToList();

        // Assert
        ordered.Select(x => x.Id).Should().ContainInOrder(2, 1, 3);
    }

    [Fact]
    public void Apply_WhenDescendingIsTrue_SortsByDeadlineDescending()
    {
        // Arrange
        var strategy = new DeadlineSortingStrategy();

        var data = new List<Internship>
        {
            new Internship { Id = 1, Deadline = new DateTime(2026, 1, 10) },
            new Internship { Id = 2, Deadline = new DateTime(2026, 1, 5) },
            new Internship { Id = 3, Deadline = new DateTime(2026, 1, 20) }
        }.AsQueryable();

        // Act
        var ordered = strategy.Apply(data, descending: true).ToList();

        // Assert
        ordered.Select(x => x.Id).Should().ContainInOrder(3, 1, 2);
    }
}
