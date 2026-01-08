using System;
using System.Collections.Generic;
using Xunit;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Services.Categories;

namespace InternshipPortal.API.UnitTests.Services.Categories
{
    public class CategorySortingStrategyResolverTests
    {
        [Fact]
        public void Resolve_EmptyKey_ThrowsArgumentException()
        {
            var sut = new CategorySortingStrategyResolver(new List<ICategorySortingStrategy>
            {
                new FakeStrategy()
            });

            Assert.Throws<ArgumentException>(() => sut.Resolve(null));
        }

        [Fact]
        public void Resolve_UnknownKey_ThrowsArgumentException()
        {
            var sut = new CategorySortingStrategyResolver(new List<ICategorySortingStrategy>
            {
                new FakeStrategy()
            });

            Assert.Throws<ArgumentException>(() => sut.Resolve("xyz"));
        }

        [Fact]
        public void Resolve_KnownKey_ReturnsStrategy()
        {
            var fake = new FakeStrategy();
            var sut = new CategorySortingStrategyResolver(new List<ICategorySortingStrategy> { fake });

            var result = sut.Resolve("mostUsed");
            Assert.Same(fake, result);
        }

        private class FakeStrategy : ICategorySortingStrategy
        {
            public string Key => "mostUsed";
            public IEnumerable<Category> Sort(IEnumerable<Category> categories) => categories;
        }
    }
}
