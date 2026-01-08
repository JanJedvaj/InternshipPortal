using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.IntegrationTests.Infrastructure;
using InternshipPortal.BL.DTOi.Companies;
using Xunit;

namespace InternshipPortal.API.IntegrationTests.Controllers;

public class CompaniesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CompaniesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Returns200_AndList()
    {
        // Act
        var response = await _client.GetAsync("/api/Companies");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Controller vraća listu iz service-a; u tvojoj implementaciji najčešće su to Company entiteti.
        var items = await response.Content.ReadFromJsonAsync<List<Company>>();
        items.Should().NotBeNull();
        items!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_WhenExists_Returns200()
    {
        // Act
        var response = await _client.GetAsync("/api/Companies/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var company = await response.Content.ReadFromJsonAsync<Company>();
        company.Should().NotBeNull();
        company!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/Companies/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_WhenIdInvalid_Returns400()
    {
        // Act (negativan id bi trebao baciti ValidationException u service-u)
        var response = await _client.GetAsync("/api/Companies/-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WhenBodyInvalid_Returns400()
    {
        // Arrange: namjerno nevalidno (npr. Name null/prazno)
        var badRequest = new CompanyRequestDTO
        {
            Name = null!,
            Website = "https://example.com",
            Location = "Zagreb"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Companies", badRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WhenValid_Returns201_AndCanGetById()
    {
        // Arrange
        var createRequest = new CompanyRequestDTO
        {
            Name = $"Test Company {Guid.NewGuid():N}",
            Website = "https://test-company.example",
            Location = "Zagreb"
        };

        // Act
        var createResponse = await _client.PostAsJsonAsync("/api/Companies", createRequest);

        // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<Company>();
        created.Should().NotBeNull();
        created!.Id.Should().BeGreaterThan(0);
        created.Name.Should().Be(createRequest.Name);
        created.Website.Should().Be(createRequest.Website);
        created.Location.Should().Be(createRequest.Location);

        // Provjeri GET
        var getResponse = await _client.GetAsync($"/api/Companies/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<Company>();
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Name.Should().Be(createRequest.Name);
    }

    [Fact]
    public async Task Update_WhenExists_Returns200_AndUpdatesEntity()
    {
        // Arrange (update na seeded Id=1)
        var updateRequest = new CompanyRequestDTO
        {
            Name = "Company UPDATED",
            Website = "https://updated.example",
            Location = "Split"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/Companies/1", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<Company>();
        updated.Should().NotBeNull();
        updated!.Id.Should().Be(1);
        updated.Name.Should().Be("Company UPDATED");
        updated.Website.Should().Be("https://updated.example");
        updated.Location.Should().Be("Split");

        // Opcionalno: provjeri persist kroz GET
        var get = await _client.GetAsync("/api/Companies/1");
        get.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await get.Content.ReadFromJsonAsync<Company>();
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(1);
        fetched.Name.Should().Be("Company UPDATED");
    }

    [Fact]
    public async Task Update_WhenNotFound_Returns404()
    {
        // Arrange
        var updateRequest = new CompanyRequestDTO
        {
            Name = "Does not matter",
            Website = "https://does-not-matter.example",
            Location = "Nowhere"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/Companies/99999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WhenBodyInvalid_Returns400()
    {
        // Arrange
        var badRequest = new CompanyRequestDTO
        {
            Name = "", // očekivano nevalidno
            Website = "https://x.example",
            Location = "Zagreb"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/Companies/1", badRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_WhenExists_Returns204_AndThenGetReturns404()
    {
        // Arrange: prvo kreiraj novu company koju smijemo obrisati
        var createRequest = new CompanyRequestDTO
        {
            Name = $"To be deleted {Guid.NewGuid():N}",
            Website = "https://delete-me.example",
            Location = "Osijek"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/Companies", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<Company>();
        created.Should().NotBeNull();
        var id = created!.Id;

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/Companies/{id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getAfter = await _client.GetAsync($"/api/Companies/{id}");
        getAfter.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenNotFound_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/Companies/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenIdInvalid_Returns400()
    {
        // Act
        var response = await _client.DeleteAsync("/api/Companies/-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
