using FluentAssertions;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Services.Internships.Search;
using InternshipPortal.API.Services.Internships.Sorting;
using Microsoft.EntityFrameworkCore;

namespace InternshipPortal.API.UnitTests.Services.Internships.Search;

public class InternshipSearchFacadeTests
{
    private static InternshipPortalContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<InternshipPortalContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new InternshipPortalContext(options);
    }

    private static void SeedInternships(InternshipPortalContext context, params Internship[] internships)
    {
        context.Internships.AddRange(internships);
        context.SaveChanges();
    }

    
    private static Internship ValidInternship(
        int id,
        string title,
        DateTime postedAt,
        DateTime? deadline = null,
        int categoryId = 1,
        int companyId = 1,
        bool remote = false,
        string location = "Zagreb",
        string shortDescription = "Short",
        string fullDescription = "Full")
    {
        return new Internship
        {
            Id = id,
            Title = title,
            ShortDescription = shortDescription,
            FullDescription = fullDescription,
            Location = location,
            PostedAt = postedAt,
            Deadline = deadline,
            CategoryId = categoryId,
            CompanyId = companyId,
            Remote = remote
        };
    }

    [Fact]
    public void Search_WhenCriteriaIsNull_UsesDefaults_AndReturnsPagedItems()
    {
        
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);

        SeedInternships(context,
            ValidInternship(1, "A", new DateTime(2026, 1, 1)),
            ValidInternship(2, "B", new DateTime(2026, 1, 2)),
            ValidInternship(3, "C", new DateTime(2026, 1, 3))
        );

        var strategies = new IInternshipSortingStrategy[]
        {
            new PostedAtSortingStrategy(), 
            new DeadlineSortingStrategy(),
            new TitleSortingStrategy()
        };

        var facade = new InternshipSearchFacade(context, strategies);

        
        var result = facade.Search(criteria: null!);

        
        result.TotalCount.Should().Be(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Items.Should().HaveCount(3);
    }

    [Fact]
    public void Search_WhenPageOrPageSizeAreInvalid_NormalizesToDefaults()
    {
        
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);

        SeedInternships(context,
            ValidInternship(1, "A", new DateTime(2026, 1, 1))
        );

        var facade = new InternshipSearchFacade(
            context,
            new IInternshipSortingStrategy[] { new PostedAtSortingStrategy() });

        var criteria = new InternshipSearchCriteria
        {
            Page = 0,       
            PageSize = -10, 
            SortBy = "date",
            SortDescending = true,
            OnlyActive = false
        };

        
        var result = facade.Search(criteria);

      
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public void Search_WhenKeywordProvided_FiltersByTitleOrDescriptions()
    {
        
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);

        SeedInternships(context,
            ValidInternship(
                id: 1,
                title: "Backend Developer",
                postedAt: new DateTime(2026, 1, 1),
                shortDescription: "C# .NET",
                fullDescription: "Work on APIs",
                location: "Zagreb"
            ),
            ValidInternship(
                id: 2,
                title: "Frontend Developer",
                postedAt: new DateTime(2026, 1, 2),
                shortDescription: "React",
                fullDescription: "Work on UI",
                location: "Split"
            )
        );

        var facade = new InternshipSearchFacade(
            context,
            new IInternshipSortingStrategy[] { new PostedAtSortingStrategy() });

        var criteria = new InternshipSearchCriteria
        {
            Keyword = "backend",
            OnlyActive = false
        };

        
        var result = facade.Search(criteria);

        
        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle();
        result.Items.First().Id.Should().Be(1);
    }

    [Fact]
    public void Search_WhenOnlyActiveTrue_FiltersOutExpiredDeadlines_ButKeepsNullDeadline()
    {
        
        var today = DateTime.UtcNow.Date;

        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);

        SeedInternships(context,
            ValidInternship(1, "Expired", new DateTime(2026, 1, 1), deadline: today.AddDays(-1)),
            ValidInternship(2, "Active", new DateTime(2026, 1, 2), deadline: today.AddDays(5)),
            ValidInternship(3, "No deadline", new DateTime(2026, 1, 3), deadline: null)
        );

        var facade = new InternshipSearchFacade(
            context,
            new IInternshipSortingStrategy[] { new PostedAtSortingStrategy() });

        var criteria = new InternshipSearchCriteria
        {
            OnlyActive = true,
            SortBy = "date",
            SortDescending = false
        };

      
        var result = facade.Search(criteria);

        
        result.TotalCount.Should().Be(2);
        result.Items.Select(i => i.Id).Should().BeEquivalentTo(new[] { 2, 3 });
    }

    [Fact]
    public void Search_WhenSortKeyUnknown_FallsBackToDateStrategy_IfAvailable()
    {
        
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);

        SeedInternships(context,
            ValidInternship(1, "A", new DateTime(2026, 1, 1)),
            ValidInternship(2, "B", new DateTime(2026, 1, 3)),
            ValidInternship(3, "C", new DateTime(2026, 1, 2))
        );

        var strategies = new IInternshipSortingStrategy[]
        {
            new PostedAtSortingStrategy() 
        };

        var facade = new InternshipSearchFacade(context, strategies);

        var criteria = new InternshipSearchCriteria
        {
            SortBy = "unknown-sort",
            SortDescending = true,
            OnlyActive = false
        };

        
        var result = facade.Search(criteria);

        
        result.Items.Select(i => i.Id).Should().ContainInOrder(2, 3, 1); // PostedAt desc
    }

    [Fact]
    public void Search_WhenSortKeyUnknown_AndDateStrategyMissing_UsesUnsortedBranch()
    {
        
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);

       
        SeedInternships(context,
            ValidInternship(1, "First", new DateTime(2026, 1, 1)),
            ValidInternship(2, "Second", new DateTime(2026, 1, 2)),
            ValidInternship(3, "Third", new DateTime(2026, 1, 3))
        );

        var strategies = new IInternshipSortingStrategy[]
        {
            new DeadlineSortingStrategy() 
        };

        var facade = new InternshipSearchFacade(context, strategies);

        var criteria = new InternshipSearchCriteria
        {
            SortBy = "unknown-sort",
            SortDescending = true,
            OnlyActive = false,
            Page = 1,
            PageSize = 2
        };

       
        var result = facade.Search(criteria);

        
        result.TotalCount.Should().Be(3);
        result.Items.Should().HaveCount(2);
        result.Items.Select(i => i.Id).Should().ContainInOrder(1, 2); 
    }

    [Fact]
    public void Search_WhenPagingApplied_ReturnsCorrectPageSlice()
    {
        
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);

        SeedInternships(context,
            ValidInternship(1, "A", new DateTime(2026, 1, 1)),
            ValidInternship(2, "B", new DateTime(2026, 1, 2)),
            ValidInternship(3, "C", new DateTime(2026, 1, 3)),
            ValidInternship(4, "D", new DateTime(2026, 1, 4))
        );

        var facade = new InternshipSearchFacade(
            context,
            new IInternshipSortingStrategy[] { new PostedAtSortingStrategy() });

        var criteria = new InternshipSearchCriteria
        {
            OnlyActive = false,
            SortBy = "date",
            SortDescending = false,
            Page = 2,
            PageSize = 2
        };

        
        var result = facade.Search(criteria);

        
        result.TotalCount.Should().Be(4);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.Items.Select(i => i.Id).Should().ContainInOrder(3, 4);
    }
}
