using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

using InternshipPortal.API.Controllers;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Categories;

namespace InternshipPortal.API.UnitTests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryService> _service = new();
        private readonly Mock<ICategoryFacade> _facade = new();
        private readonly Mock<ILogger<CategoriesController>> _logger = new();

        private CategoriesController CreateSut(IEnumerable<ICategorySortingStrategy> strategies = null)
        {
            strategies ??= new List<ICategorySortingStrategy>
            {
                new FakeMostUsedStrategy()
            };

            var resolver = new CategorySortingStrategyResolver(strategies);
            return new CategoriesController(_service.Object, _facade.Object, resolver, _logger.Object);
        }

        // GET ALL

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

            Assert.Equal(new[] { 2, 1 }, payload.Select(x => x.Id).ToArray());
        }

        [Fact]
        public void GetAll_WithUnknownSort_ResolverThrows_Returns500()
        {
            _service.Setup(s => s.GetAll()).Returns(new List<Category>
            {
                new Category { Id = 1, Name = "IT" }
            });

            var sut = CreateSut(); 
            var result = sut.GetAll("unknownSort");

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

        // GET BY ID
        [Fact]
        public void Get_Ok_ReturnsOk()
        {
            _service.Setup(s => s.GetById(1))
                .Returns(new Category { Id = 1, Name = "IT" });

            var sut = CreateSut();
            var result = sut.Get(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<Category>(ok.Value);
            Assert.Equal(1, payload.Id);
        }

        [Fact]
        public void Get_ValidationException_ReturnsBadRequest()
        {
            _service.Setup(s => s.GetById(0))
                .Throws(new ValidationException("Id mora biti veći od nule."));

            var sut = CreateSut();
            var result = sut.Get(0);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Id", bad.Value?.ToString() ?? "");
        }

        [Fact]
        public void Get_NotFoundException_ReturnsNotFound()
        {
            _service.Setup(s => s.GetById(99))
                .Throws(new NotFoundException("not found"));

            var sut = CreateSut();
            var result = sut.Get(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Get_GenericException_Returns500()
        {
            _service.Setup(s => s.GetById(5)).Throws(new Exception("boom"));

            var sut = CreateSut();
            var result = sut.Get(5);

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        // CREATE
        [Fact]
        public void Create_Ok_ReturnsCreatedAt()
        {
            var input = new Category { Name = "IT" };
            var created = new Category { Id = 10, Name = "IT" };

            _service.Setup(s => s.Create(input)).Returns(created);

            var sut = CreateSut();
            var result = sut.Create(input);

            var createdAt = Assert.IsType<CreatedAtActionResult>(result);
            var payload = Assert.IsType<Category>(createdAt.Value);
            Assert.Equal(10, payload.Id);
        }

        [Fact]
        public void Create_ValidationException_ReturnsBadRequest()
        {
            var input = new Category { Name = "" };

            _service.Setup(s => s.Create(input))
                .Throws(new ValidationException("Name je obavezan."));

            var sut = CreateSut();
            var result = sut.Create(input);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Create_GenericException_Returns500()
        {
            var input = new Category { Name = "IT" };
            _service.Setup(s => s.Create(input)).Throws(new Exception("boom"));

            var sut = CreateSut();
            var result = sut.Create(input);

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        // UPDATE

        [Fact]
        public void Update_Ok_ReturnsOk()
        {
            var input = new Category { Id = 0, Name = "New" };
            var updated = new Category { Id = 1, Name = "New" };

            _service.Setup(s => s.Update(1, input)).Returns(updated);

            var sut = CreateSut();
            var result = sut.Update(1, input);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<Category>(ok.Value);
            Assert.Equal("New", payload.Name);
        }

        [Fact]
        public void Update_ValidationException_ReturnsBadRequest()
        {
            var input = new Category { Name = "" };
            _service.Setup(s => s.Update(1, input)).Throws(new ValidationException("Name je obavezan."));

            var sut = CreateSut();
            var result = sut.Update(1, input);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Update_NotFoundException_ReturnsNotFound()
        {
            var input = new Category { Name = "X" };
            _service.Setup(s => s.Update(123, input)).Throws(new NotFoundException("not found"));

            var sut = CreateSut();
            var result = sut.Update(123, input);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Update_GenericException_Returns500()
        {
            var input = new Category { Name = "X" };
            _service.Setup(s => s.Update(1, input)).Throws(new Exception("boom"));

            var sut = CreateSut();
            var result = sut.Update(1, input);

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        // DELETE

        [Fact]
        public void Delete_Ok_ReturnsNoContent()
        {
            var sut = CreateSut();
            var result = sut.Delete(1);

            _facade.Verify(f => f.DeleteCategorySafely(1), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Delete_ValidationException_ReturnsBadRequest()
        {
            _facade.Setup(f => f.DeleteCategorySafely(0)).Throws(new ValidationException("bad"));

            var sut = CreateSut();
            var result = sut.Delete(0);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Delete_NotFoundException_ReturnsNotFound()
        {
            _facade.Setup(f => f.DeleteCategorySafely(99)).Throws(new NotFoundException("nf"));

            var sut = CreateSut();
            var result = sut.Delete(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Delete_GenericException_Returns500()
        {
            _facade.Setup(f => f.DeleteCategorySafely(1)).Throws(new Exception("boom"));

            var sut = CreateSut();
            var result = sut.Delete(1);

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        private class FakeMostUsedStrategy : ICategorySortingStrategy
        {
            public string Key => "mostUsed";

            public IEnumerable<Category> Sort(IEnumerable<Category> categories)
                => categories.OrderBy(c => c.Name);
        }
    }
}
