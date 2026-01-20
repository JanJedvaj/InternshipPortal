using System;
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
    public class CategoryCreateControllerTests
    {
        private readonly Mock<ICategoryService> _service = new();
        private readonly Mock<ILogger<CategoryCreateController>> _logger = new();

        private CategoryCreateController CreateSut()
            => new CategoryCreateController(_service.Object, _logger.Object);

        [Fact]
        public void Create_Valid_ReturnsCreatedAt()
        {
            var input = new Category { Name = "Design" };
            var created = new Category { Id = 10, Name = "Design" };

            _service.Setup(s => s.Create(input)).Returns(created);

            var sut = CreateSut();
            var result = sut.Create(input);

            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            var payload = Assert.IsType<Category>(createdAt.Value);
            Assert.Equal(10, payload.Id);
        }

        [Fact]
        public void Create_ValidationException_ReturnsBadRequest()
        {
            _service.Setup(s => s.Create(It.IsAny<Category>()))
                .Throws(new ValidationException("bad"));

            var sut = CreateSut();
            var result = sut.Create(new Category());

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void Create_GenericException_Returns500()
        {
            _service.Setup(s => s.Create(It.IsAny<Category>()))
                .Throws(new Exception("boom"));

            var sut = CreateSut();
            var result = sut.Create(new Category { Name = "X" });

            var obj = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, obj.StatusCode);
        }
    }
}
