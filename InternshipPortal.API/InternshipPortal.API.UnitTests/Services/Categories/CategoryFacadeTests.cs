using System.Collections.Generic;
using Moq;
using Xunit;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Repositories.Internships;
using InternshipPortal.API.Services.Categories;

namespace InternshipPortal.API.UnitTests.Services.Categories
{
    public class CategoryFacadeTests
    {
        [Fact]
        public void DeleteCategorySafely_IdInvalid_ThrowsValidationException()
        {
            var service = new Mock<ICategoryService>();
            var repo = new Mock<IInternshipRepository>();

            var sut = new CategoryFacade(service.Object, repo.Object);

            Assert.Throws<ValidationException>(() => sut.DeleteCategorySafely(0));
        }

        [Fact]
        public void DeleteCategorySafely_CategoryHasInternships_ThrowsValidationException()
        {
            var service = new Mock<ICategoryService>();
            service.Setup(s => s.GetById(1)).Returns(new Category { Id = 1, Name = "IT" });

            var repo = new Mock<IInternshipRepository>();
            repo.Setup(r => r.GetAll()).Returns(new List<Internship>
            {
                new Internship { Id = 1, CategoryId = 1 }
            });

            var sut = new CategoryFacade(service.Object, repo.Object);

            Assert.Throws<ValidationException>(() => sut.DeleteCategorySafely(1));
            service.Verify(s => s.Delete(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void DeleteCategorySafely_NoInternships_CallsDelete()
        {
            var service = new Mock<ICategoryService>();
            service.Setup(s => s.GetById(5)).Returns(new Category { Id = 5, Name = "X" });

            var repo = new Mock<IInternshipRepository>();
            repo.Setup(r => r.GetAll()).Returns(new List<Internship>());

            var sut = new CategoryFacade(service.Object, repo.Object);

            sut.DeleteCategorySafely(5);

            service.Verify(s => s.Delete(5), Times.Once);
        }
    }
}
