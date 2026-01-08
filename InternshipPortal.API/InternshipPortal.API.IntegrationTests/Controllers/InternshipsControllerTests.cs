using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InternshipPortal.API.IntegrationTests.Infrastructure;
using InternshipPortal.BL.DTOi.Internships;
using Xunit;

namespace InternshipPortal.API.IntegrationTests.Controllers;

public class InternshipsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public InternshipsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Returns200_AndList()
    {
        // Act
        var response = await _client.GetAsync("/api/Internships");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<InternshipResponseDTO>>();
        items.Should().NotBeNull();
        items!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_WhenExists_Returns200()
    {
        // Act
        var response = await _client.GetAsync("/api/Internships/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<InternshipResponseDTO>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/Internships/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WhenBodyInvalid_Returns400()
    {
        // Arrange: namjerno krivo (npr. Title null)
        var badRequest = new InternshipRequestDTO
        {
            Title = null!,
            ShortDescription = "x",
            FullDescription = "x",
            Location = "x",
            CompanyId = 1,
            CategoryId = 1,
            Remote = true,
            IsFeatured = false,
            Deadline = DateTime.UtcNow.Date.AddDays(3)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Internships", badRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_Returns200()
    {
        // Act
        var response = await _client.GetAsync("/api/Internships/search?keyword=backend&onlyActive=false");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_WhenExists_Returns200_AndUpdatesEntity()
    {
        // Arrange (napravi update na seeded Id=1)
        var updateRequest = new InternshipRequestDTO
        {
            Title = "Backend Developer UPDATED",
            ShortDescription = "C# .NET UPDATED",
            FullDescription = "Full desc UPDATED",
            Location = "Zagreb",
            CompanyId = 1,
            CategoryId = 1,
            Remote = true,
            IsFeatured = true,
            Deadline = DateTime.UtcNow.Date.AddDays(20)
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/Internships/1", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // (Opcionalno) provjeri da GET vraća promjene
        var get = await _client.GetAsync("/api/Internships/1");
        get.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await get.Content.ReadFromJsonAsync<InternshipResponseDTO>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(1);
        dto.Title.Should().Be("Backend Developer UPDATED");
    }

    [Fact]
    public async Task Delete_WhenExists_Returns204_AndThenGetReturns404()
    {
        // Arrange: prvo kreiraj novi internship
        var createRequest = new InternshipRequestDTO
        {
            Title = "To be deleted",
            ShortDescription = "x",
            FullDescription = "x",
            Location = "Zagreb",
            CompanyId = 1,
            CategoryId = 1,
            Remote = true,
            IsFeatured = false,
            Deadline = DateTime.UtcNow.Date.AddDays(7)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/Internships", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<InternshipResponseDTO>();
        created.Should().NotBeNull();
        var id = created!.Id;

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/Internships/{id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getAfter = await _client.GetAsync($"/api/Internships/{id}");
        getAfter.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


}
