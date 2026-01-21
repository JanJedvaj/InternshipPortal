using Moq;
using Xunit;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Categories;

namespace InternshipPortal.API.UnitTests.Services.Categories
{
    public class DefaultCategoryFactoryTests
    {
        [Fact]
        public void CreateNew_Null_ThrowsValidationException()
        {
            var nameStrategy = new Mock<ICategoryNameStrategy>();
            var sut = new DefaultCategoryFactory(nameStrategy.Object);

            Assert.Throws<ValidationException>(() => sut.CreateNew(null!));
        }

        [Fact]
        public void CreateNew_UsesStrategyValidateAndNormalize_ReturnsNewEntity()
        {
            var nameStrategy = new Mock<ICategoryNameStrategy>();
            nameStrategy.Setup(s => s.Normalize("  IT  ")).Returns("IT");

            var sut = new DefaultCategoryFactory(nameStrategy.Object);

            var input = new Category { Id = 123, Name = "  IT  " };
            var result = sut.CreateNew(input);

            nameStrategy.Verify(s => s.Validate("  IT  "), Times.Once);
            nameStrategy.Verify(s => s.Normalize("  IT  "), Times.Once);

            Assert.NotSame(input, result);
            Assert.Equal("IT", result.Name);
            Assert.Equal(0, result.Id); // EF će dodijeliti
        }

        [Fact]
        public void ApplyUpdates_ExistingNull_ThrowsValidationException()
        {
            var nameStrategy = new Mock<ICategoryNameStrategy>();
            var sut = new DefaultCategoryFactory(nameStrategy.Object);

            Assert.Throws<ValidationException>(() => sut.ApplyUpdates(null!, new Category { Name = "X" }));
        }

        [Fact]
        public void ApplyUpdates_UpdatesNull_ThrowsValidationException()
        {
            var nameStrategy = new Mock<ICategoryNameStrategy>();
            var sut = new DefaultCategoryFactory(nameStrategy.Object);

            Assert.Throws<ValidationException>(() => sut.ApplyUpdates(new Category { Id = 1, Name = "Old" }, null!));
        }

        [Fact]
        public void ApplyUpdates_UsesStrategy_AndMutatesExisting()
        {
            var nameStrategy = new Mock<ICategoryNameStrategy>();
            nameStrategy.Setup(s => s.Normalize("  New  ")).Returns("New");

            var sut = new DefaultCategoryFactory(nameStrategy.Object);

            var existing = new Category { Id = 1, Name = "Old" };
            var updates = new Category { Name = "  New  " };

            var result = sut.ApplyUpdates(existing, updates);

            nameStrategy.Verify(s => s.Validate("  New  "), Times.Once);
            nameStrategy.Verify(s => s.Normalize("  New  "), Times.Once);

            Assert.Same(existing, result);
            Assert.Equal("New", existing.Name);
        }
    }
}
