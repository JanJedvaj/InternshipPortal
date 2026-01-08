using Xunit;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Categories;

namespace InternshipPortal.API.UnitTests.Services.Categories
{
    public class DefaultCategoryNameStrategyTests
    {
        [Fact]
        public void Validate_Empty_ThrowsValidationException()
        {
            var sut = new DefaultCategoryNameStrategy();
            Assert.Throws<ValidationException>(() => sut.Validate("   "));
        }

        [Fact]
        public void Validate_TooLong_ThrowsValidationException()
        {
            var sut = new DefaultCategoryNameStrategy();
            var longName = new string('a', 101);

            Assert.Throws<ValidationException>(() => sut.Validate(longName));
        }

        [Fact]
        public void Validate_Valid_DoesNotThrow()
        {
            var sut = new DefaultCategoryNameStrategy();
            sut.Validate("IT");
        }

        [Fact]
        public void Normalize_Trims()
        {
            var sut = new DefaultCategoryNameStrategy();
            var result = sut.Normalize("  IT  ");

            Assert.Equal("IT", result);
        }
    }
}
