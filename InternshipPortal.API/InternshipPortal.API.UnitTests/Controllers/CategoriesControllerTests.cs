using System;
using System.Collections.Generic;
using System.Linq;
using InternshipPortal.API.Controllers;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InternshipPortal.API.UnitTests.Controllers.Categories
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryService> _service = new();
        private readonly Mock<ILogger<CategoriesController>> _logger = new();

        private CategoriesController CreateSut(IEnumerable<ICategorySortingStrategy> strategies = null)
        {
            strategies ??= new ICategorySortingStrategy[] { new FakeMostUsedStrategy() };
            var resolver = new CategorySortingStrategyResolver(strategies);

            return new CategoriesController(_service.Object, resolver, _logger.Object);
        }


        [Fact]
        public void GetAll_NoSort_ReturnsOk()
        {
            _service.Setup(s => s.GetAll())
                .Returns(new List<Category> { new Category { Id = 1, Name = "IT" } });

            var sut = CreateSut();
            var result = sut.GetAll(null);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsAssignableFrom<IEnumerable<Category>>(ok.Value);
            Assert.Single(payload);
        }

        [Fact]
        public void GetAll_WithSortMostUsed_AppliesStrategy()
        {
            _service.Setup(s => s.GetAll()).Returns(new List<Category>
            {
                new Category { Id = 1, Name = "B" },
                new Category { Id = 2, Name = "A" }
            });

            var sut = CreateSut();
            var result = sut.GetAll("mostUsed");

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsAssignableFrom<IEnumerable<Category>>(ok.Value).ToList();

            // Fake strategy sortira po Name desc => B pa A
            Assert.Equal(new[] { 1, 2 }, payload.Select(x => x.Id).ToArray());
        }

        [Fact]
        public void GetAll_UnknownSort_ResolverThrows_Returns500()
        {
            _service.Setup(s => s.GetAll())
                .Returns(new List<Category> { new Category { Id = 1, Name = "IT" } });

            var sut = CreateSut(); // resolver baca ArgumentException za unknown
            var result = sut.GetAll("unknown");

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        [Fact]
        public void GetAll_ServiceThrows_Returns500()
        {
            _service.Setup(s => s.GetAll()).Throws(new Exception("boom"));

            var sut = CreateSut();
            var result = sut.GetAll(null);

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }


        [Fact]
        public void GetById_Valid_ReturnsOk()
        {
            _service.Setup(s => s.GetById(1))
                .Returns(new Category { Id = 1, Name = "IT" });

            var sut = CreateSut();
            var result = sut.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var payload = Assert.IsType<Category>(ok.Value);
            Assert.Equal(1, payload.Id);
        }

        [Fact]
        public void GetById_ValidationException_ReturnsBadRequest()
        {
            _service.Setup(s => s.GetById(0))
                .Throws(new ValidationException("bad"));

            var sut = CreateSut();
            var result = sut.GetById(0);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void GetById_NotFoundException_ReturnsNotFound()
        {
            _service.Setup(s => s.GetById(99))
                .Throws(new NotFoundException("nf"));

            var sut = CreateSut();
            var result = sut.GetById(99);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void GetById_GenericException_Returns500()
        {
            _service.Setup(s => s.GetById(1)).Throws(new Exception("boom"));

            var sut = CreateSut();
            var result = sut.GetById(1);

            var obj = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, obj.StatusCode);
        }

        private class FakeMostUsedStrategy : ICategorySortingStrategy
        {
            public string Key => "mostUsed";
            public IEnumerable<Category> Sort(IEnumerable<Category> categories)
                => categories.OrderByDescending(c => c.Name);
        }
    }
}
