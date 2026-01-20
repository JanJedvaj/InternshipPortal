using System;
using InternshipPortal.API.Controllers;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InternshipPortal.API.UnitTests.Controllers.Categories
{
    public class CategoryDeleteControllerTests
    {
        private readonly Mock<ICategoryFacade> _facade = new();
        private readonly Mock<ILogger<CategoryDeleteController>> _logger = new();

        private CategoryDeleteController CreateSut()
            => new CategoryDeleteController(_facade.Object, _logger.Object);

        [Fact]
        public void Delete_Valid_ReturnsNoContent()
        {
            var sut = CreateSut();
            var result = sut.Delete(1);

            Assert.IsType<NoContentResult>(result);
            _facade.Verify(f => f.DeleteCategorySafely(1), Times.Once);
        }

        [Fact]
        public void Delete_ValidationException_ReturnsBadRequest()
        {
            _facade.Setup(f => f.DeleteCategorySafely(0))
                .Throws(new ValidationException("bad"));

            var sut = CreateSut();
            var result = sut.Delete(0);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Delete_NotFoundException_ReturnsNotFound()
        {
            _facade.Setup(f => f.DeleteCategorySafely(99))
                .Throws(new NotFoundException("nf"));

            var sut = CreateSut();
            var result = sut.Delete(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Delete_GenericException_Returns500()
        {
            _facade.Setup(f => f.DeleteCategorySafely(1))
                .Throws(new Exception("boom"));

            var sut = CreateSut();
            var result = sut.Delete(1);

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }
    }
}
