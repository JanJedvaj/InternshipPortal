using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Repositories.Internships;
using InternshipPortal.API.Services.Categories;

namespace InternshipPortal.API.UnitTests.Services.Categories
{
    public class MostUsedCategorySortingStrategyTests
    {
        [Fact]
        public void Sort_NullCategories_ReturnsEmpty()
        {
            var repo = new Mock<IInternshipRepository>();
            repo.Setup(r => r.GetAll()).Returns(new List<Internship>());

            var sut = new MostUsedCategorySortingStrategy(repo.Object);
            var result = sut.Sort(null);

            Assert.Empty(result);
        }

        [Fact]
        public void Sort_SortsByInternshipCountDesc_ThenByName()
        {
            var repo = new Mock<IInternshipRepository>();
            repo.Setup(r => r.GetAll()).Returns(new List<Internship>
            {
                new Internship { Id = 1, CategoryId = 1 },
                new Internship { Id = 2, CategoryId = 1 },
                new Internship { Id = 3, CategoryId = 2 }
            });

            var categories = new List<Category>
            {
                new Category { Id = 2, Name = "B" }, // count 1
                new Category { Id = 1, Name = "A" }, // count 2
                new Category { Id = 3, Name = "C" }  // count 0
            };

            var sut = new MostUsedCategorySortingStrategy(repo.Object);
            var result = sut.Sort(categories).ToList();

            Assert.Equal(new[] { 1, 2, 3 }, result.Select(c => c.Id).ToArray());
        }
    }
}
