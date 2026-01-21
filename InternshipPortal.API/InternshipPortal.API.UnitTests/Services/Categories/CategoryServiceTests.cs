using System.Collections.Generic;
using Moq;
using Xunit;

using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Repositories.Categories;
using InternshipPortal.API.Services.Categories;

namespace InternshipPortal.API.UnitTests.Services.Categories
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _repo = new();
        private readonly Mock<ICategoryFactory> _factory = new();

        private CategoryService CreateSut()
            => new CategoryService(_repo.Object, _factory.Object);

        [Fact]
        public void GetAll_WhenRepoReturnsNull_ReturnsEmpty()
        {
            _repo.Setup(r => r.GetAll()).Returns(Enumerable.Empty<Category>());


            var sut = CreateSut();
            var result = sut.GetAll();

            Assert.Empty(result);
        }

        [Fact]
        public void GetById_IdInvalid_ThrowsValidationException()
        {
            var sut = CreateSut();
            Assert.Throws<ValidationException>(() => sut.GetById(0));
        }

        [Fact]
        public void GetById_NotFound_ThrowsNotFoundException()
        {
            _repo.Setup(r => r.GetById(10)).Returns((Category)null!);

            var sut = CreateSut();
            Assert.Throws<NotFoundException>(() => sut.GetById(10));
        }

        [Fact]
        public void GetById_Found_ReturnsEntity()
        {
            _repo.Setup(r => r.GetById(1)).Returns(new Category { Id = 1, Name = "IT" });

            var sut = CreateSut();
            var result = sut.GetById(1);

            Assert.Equal(1, result.Id);
            Assert.Equal("IT", result.Name);
        }

        [Fact]
        public void Create_UsesFactoryAndRepo()
        {
            var input = new Category { Name = "  IT  " };
            var entity = new Category { Name = "IT" };
            var saved = new Category { Id = 5, Name = "IT" };

            _factory.Setup(f => f.CreateNew(input)).Returns(entity);
            _repo.Setup(r => r.Create(entity)).Returns(saved);

            var sut = CreateSut();
            var result = sut.Create(input);

            _factory.Verify(f => f.CreateNew(input), Times.Once);
            _repo.Verify(r => r.Create(entity), Times.Once);

            Assert.Equal(5, result.Id);
        }

        [Fact]
        public void Update_IdInvalid_ThrowsValidationException()
        {
            var sut = CreateSut();
            Assert.Throws<ValidationException>(() => sut.Update(0, new Category { Name = "X" }));
        }

        [Fact]
        public void Update_NotFound_ThrowsNotFoundException()
        {
            _repo.Setup(r => r.GetById(7)).Returns((Category)null!);

            var sut = CreateSut();
            Assert.Throws<NotFoundException>(() => sut.Update(7, new Category { Name = "X" }));
        }

        [Fact]
        public void Update_Found_UsesFactoryApplyUpdates_AndRepoUpdate()
        {
            var existing = new Category { Id = 7, Name = "Old" };
            var updates = new Category { Name = "New" };
            var afterFactory = new Category { Id = 7, Name = "New" };
            var afterRepo = new Category { Id = 7, Name = "New" };

            _repo.Setup(r => r.GetById(7)).Returns(existing);
            _factory.Setup(f => f.ApplyUpdates(existing, updates)).Returns(afterFactory);
            _repo.Setup(r => r.Update(7, afterFactory)).Returns(afterRepo);

            var sut = CreateSut();
            var result = sut.Update(7, updates);

            _factory.Verify(f => f.ApplyUpdates(existing, updates), Times.Once);
            _repo.Verify(r => r.Update(7, afterFactory), Times.Once);

            Assert.Equal("New", result.Name);
        }

        [Fact]
        public void Delete_IdInvalid_ThrowsValidationException()
        {
            var sut = CreateSut();
            Assert.Throws<ValidationException>(() => sut.Delete(0));
        }

        [Fact]
        public void Delete_WhenRepoReturnsFalse_ThrowsNotFoundException()
        {
            _repo.Setup(r => r.Delete(9)).Returns(false);

            var sut = CreateSut();
            Assert.Throws<NotFoundException>(() => sut.Delete(9));
        }

        [Fact]
        public void Delete_WhenRepoReturnsTrue_DoesNotThrow()
        {
            _repo.Setup(r => r.Delete(9)).Returns(true);

            var sut = CreateSut();
            sut.Delete(9);

            _repo.Verify(r => r.Delete(9), Times.Once);
        }
    }
}
