using FluentAssertions;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Repositories.Internships;
using InternshipPortal.API.Services.Internships;
using InternshipPortal.API.Services.Internships.Factories;
using Moq;

namespace InternshipPortal.API.UnitTests.Services.Internships;

public class InternshipServiceTests
{
    [Fact]
    public void Delete_WhenIdIsLessOrEqualZero_ThrowsValidationException()
    {
        // Arrange
        var repoMock = new Mock<IInternshipRepository>();
        var factoryMock = new Mock<IInternshipFactory>();
        var service = new InternshipService(repoMock.Object, factoryMock.Object);

        // Act
        var act = () => service.Delete(0);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Id mora biti veći od nule.");

        // Repo se ne smije ni zvati kad je id neispravan
        repoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Delete_WhenRepoReturnsFalse_ThrowsNotFoundException()
    {
        // Arrange
        const int id = 123;

        var repoMock = new Mock<IInternshipRepository>();
        repoMock.Setup(r => r.Delete(id)).Returns(false);

        var factoryMock = new Mock<IInternshipFactory>();

        var service = new InternshipService(repoMock.Object, factoryMock.Object);

        // Act
        var act = () => service.Delete(id);

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage($"Praksa s Id={id} nije pronađena.");

        repoMock.Verify(r => r.Delete(id), Times.Once);
    }

    [Fact]
    public void Delete_WhenRepoReturnsTrue_DoesNotThrow()
    {
        // Arrange
        const int id = 123;

        var repoMock = new Mock<IInternshipRepository>();
        repoMock.Setup(r => r.Delete(id)).Returns(true);

        var factoryMock = new Mock<IInternshipFactory>();

        var service = new InternshipService(repoMock.Object, factoryMock.Object);

        // Act
        var act = () => service.Delete(id);

        // Assert
        act.Should().NotThrow();

        repoMock.Verify(r => r.Delete(id), Times.Once);
    }

    [Fact]
    public void GetById_WhenIdIsLessOrEqualZero_ThrowsValidationException()
    {
        // Arrange
        var repoMock = new Mock<IInternshipRepository>();
        var factoryMock = new Mock<IInternshipFactory>();
        var service = new InternshipService(repoMock.Object, factoryMock.Object);

        // Act
        var act = () => service.GetById(0);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Id mora biti veći od nule.");

        repoMock.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void GetById_WhenRepoReturnsNull_ThrowsNotFoundException()
    {
        // Arrange
        const int id = 123;

        var repoMock = new Mock<IInternshipRepository>();
        repoMock.Setup(r => r.GetById(id)).Returns((Internship?)null);

        var factoryMock = new Mock<IInternshipFactory>();
        var service = new InternshipService(repoMock.Object, factoryMock.Object);

        // Act
        var act = () => service.GetById(id);

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage($"Praksa s Id={id} nije pronađena.");

        repoMock.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public void GetById_WhenRepoReturnsEntity_ReturnsThatEntity()
    {
        // Arrange
        const int id = 123;

        var expected = new Internship
        {
            Id = id
        };

        var repoMock = new Mock<IInternshipRepository>();
        repoMock.Setup(r => r.GetById(id)).Returns(expected);

        var factoryMock = new Mock<IInternshipFactory>();
        var service = new InternshipService(repoMock.Object, factoryMock.Object);

        // Act
        var result = service.GetById(id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);

        repoMock.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public void Update_WhenIdIsLessOrEqualZero_ThrowsValidationException()
    {
        // Arrange
        var repoMock = new Mock<IInternshipRepository>();
        var factoryMock = new Mock<IInternshipFactory>();
        var service = new InternshipService(repoMock.Object, factoryMock.Object);

        // Act
        var act = () => service.Update(0, new Internship());

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Id mora biti veći od nule.");

        repoMock.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
        repoMock.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Internship>()), Times.Never);
        factoryMock.Verify(f => f.ApplyUpdates(It.IsAny<Internship>(), It.IsAny<Internship>()), Times.Never);
    }

    [Fact]
    public void Update_WhenExistingEntityNotFound_ThrowsNotFoundException()
    {
        // Arrange
        const int id = 123;

        var repoMock = new Mock<IInternshipRepository>();
        repoMock.Setup(r => r.GetById(id)).Returns((Internship?)null);

        var factoryMock = new Mock<IInternshipFactory>();

        var service = new InternshipService(repoMock.Object, factoryMock.Object);

        // Act
        var act = () => service.Update(id, new Internship());

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage($"Praksa s Id={id} nije pronađena.");

        repoMock.Verify(r => r.GetById(id), Times.Once);
        factoryMock.Verify(f => f.ApplyUpdates(It.IsAny<Internship>(), It.IsAny<Internship>()), Times.Never);
        repoMock.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Internship>()), Times.Never);
    }

    [Fact]
    public void Update_WhenRepoUpdateReturnsNull_ThrowsNotFoundException()
    {
        // Arrange
        const int id = 123;

        var existing = new Internship { Id = id };
        var incoming = new Internship { Id = id };

        var updatedByFactory = new Internship { Id = id };

        var repoMock = new Mock<IInternshipRepository>();
        repoMock.Setup(r => r.GetById(id)).Returns(existing);
        repoMock.Setup(r => r.Update(id, It.IsAny<Internship>())).Returns((Internship?)null);

        var factoryMock = new Mock<IInternshipFactory>();
        factoryMock.Setup(f => f.ApplyUpdates(existing, incoming)).Returns(updatedByFactory);

        var service = new InternshipService(repoMock.Object, factoryMock.Object);

        // Act
        var act = () => service.Update(id, incoming);

        // Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage($"Praksa s Id={id} nije pronađena.");

        repoMock.Verify(r => r.GetById(id), Times.Once);
        factoryMock.Verify(f => f.ApplyUpdates(existing, incoming), Times.Once);
        repoMock.Verify(r => r.Update(id, updatedByFactory), Times.Once);
    }

    [Fact]
    public void Update_WhenAllIsValid_ReturnsUpdatedEntity()
    {
        // Arrange
        const int id = 123;

        var existing = new Internship { Id = id };
        var incoming = new Internship { Id = id };

        var updatedByFactory = new Internship { Id = id };
        var updatedFromRepo = new Internship { Id = id };

        var repoMock = new Mock<IInternshipRepository>();
        repoMock.Setup(r => r.GetById(id)).Returns(existing);
        repoMock.Setup(r => r.Update(id, updatedByFactory)).Returns(updatedFromRepo);

        var factoryMock = new Mock<IInternshipFactory>();
        factoryMock.Setup(f => f.ApplyUpdates(existing, incoming)).Returns(updatedByFactory);

        var service = new InternshipService(repoMock.Object, factoryMock.Object);

        // Act
        var result = service.Update(id, incoming);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);

        repoMock.Verify(r => r.GetById(id), Times.Once);
        factoryMock.Verify(f => f.ApplyUpdates(existing, incoming), Times.Once);
        repoMock.Verify(r => r.Update(id, updatedByFactory), Times.Once);
    }
    [Fact]
    public void Create_CallsFactoryCreateNew_AndPassesResultToRepoCreate()
    {
        // Arrange
        var incoming = new Internship { Id = 0 };
        var entityFromFactory = new Internship { Id = 0 };

        var repoMock = new Mock<IInternshipRepository>();
        repoMock.Setup(r => r.Create(entityFromFactory)).Returns(entityFromFactory);

        var factoryMock = new Mock<IInternshipFactory>();
        factoryMock.Setup(f => f.CreateNew(incoming)).Returns(entityFromFactory);

        var service = new InternshipService(repoMock.Object, factoryMock.Object);

        // Act
        var result = service.Create(incoming);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(entityFromFactory);

        factoryMock.Verify(f => f.CreateNew(incoming), Times.Once);
        repoMock.Verify(r => r.Create(entityFromFactory), Times.Once);
    }

    [Fact]
    public void Create_ReturnsEntityFromRepository()
    {
        // Arrange
        var incoming = new Internship { Id = 0 };

        var entityFromFactory = new Internship { Id = 0 };
        var createdFromRepo = new Internship { Id = 999 };

        var repoMock = new Mock<IInternshipRepository>();
        repoMock.Setup(r => r.Create(entityFromFactory)).Returns(createdFromRepo);

        var factoryMock = new Mock<IInternshipFactory>();
        factoryMock.Setup(f => f.CreateNew(incoming)).Returns(entityFromFactory);

        var service = new InternshipService(repoMock.Object, factoryMock.Object);

        // Act
        var result = service.Create(incoming);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(999);

        factoryMock.Verify(f => f.CreateNew(incoming), Times.Once);
        repoMock.Verify(r => r.Create(entityFromFactory), Times.Once);
    }


}
