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
    public class CategoryUpdateControllerTests
    {
        private readonly Mock<ICategoryService> _service = new();
        private readonly Mock<ILogger<CategoryUpdateController>> _logger = new();

        private CategoryUpdateController CreateSut()
            => new CategoryUpdateController(_service.Object, _logger.Object);

        [Fact]
        public void Update_Valid_ReturnsOk()
        {
            _service.Setup(s => s.Update(1, It.IsAny<Category>()))
                .Returns(new Category { Id = 1, Name = "Updated" });

            var sut = CreateSut();
            var result = sut.Update(1, new Category { Name = "Updated" });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var payload = Assert.IsType<Category>(ok.Value);
            Assert.Equal("Updated", payload.Name);
        }

        [Fact]
        public void Update_ValidationException_ReturnsBadRequest()
        {
            _service.Setup(s => s.Update(1, It.IsAny<Category>()))
                .Throws(new ValidationException("bad"));

            var sut = CreateSut();
            var result = sut.Update(1, new Category());

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void Update_NotFoundException_ReturnsNotFound()
        {
            _service.Setup(s => s.Update(99, It.IsAny<Category>()))
                .Throws(new NotFoundException("nf"));

            var sut = CreateSut();
            var result = sut.Update(99, new Category());

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void Update_GenericException_Returns500()
        {
            _service.Setup(s => s.Update(1, It.IsAny<Category>()))
                .Throws(new Exception("boom"));

            var sut = CreateSut();
            var result = sut.Update(1, new Category());

            var obj = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, obj.StatusCode);
        }
    }
}
