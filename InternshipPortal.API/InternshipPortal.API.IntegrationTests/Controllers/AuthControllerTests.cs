using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InternshipPortal.API.IntegrationTests.Infrastructure;
using InternshipPortal.BL.DTOi;
using Xunit;

namespace InternshipPortal.API.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WhenBodyNull_Returns400()
    {
        // Act: JSON null (Content-Type: application/json) -> request binder ulazi i request == null
        var response = await _client.PostAsJsonAsync<LoginRequest?>("/api/Auth/login", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WhenUsernameOrPasswordMissing_Returns400()
    {
        // Arrange
        var req = new LoginRequest
        {
            Username = "",
            Password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", req);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WhenInvalidCredentials_Returns401()
    {
        // Arrange
        var req = new LoginRequest
        {
            Username = "nonexistent_user_12345",
            Password = "wrong"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", req);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_WhenBodyNull_Returns400()
    {
        // Act: JSON null
        var response = await _client.PostAsJsonAsync<RegisterRequest?>("/api/Auth/register", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WhenModelInvalid_Returns400()
    {
        // Najsigurnije: pošalji prazan objekt -> ModelState invalid (gotovo sigurno)
        var response = await _client.PostAsJsonAsync("/api/Auth/register", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WhenValid_Returns201_AndThenLoginWorks()
    {
        // Arrange
        var username = $"test_user_{Guid.NewGuid():N}";
        var password = "Aa1!aaaaaa"; // "jak" password (radi kroz većinu pravila)

        var registerPayload = new
        {
            Username = username,
            Password = password,
            ConfirmPassword = password,
            Email = $"{username}@example.com"
        };

        // Act (register)
        var registerResponse = await _client.PostAsJsonAsync("/api/Auth/register", registerPayload);

        // Assert (register)
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act (login)
        var loginReq = new LoginRequest
        {
            Username = username,
            Password = password
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginReq);

        // Assert (login)
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        dto.Should().NotBeNull();
        dto!.Username.Should().Be(username);
        dto.Role.Should().NotBeNullOrWhiteSpace();
        dto.Token.Should().NotBeNullOrWhiteSpace();
        dto.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));

        // Validiraj da je token JWT formata
        var handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(dto.Token).Should().BeTrue();

        var jwt = handler.ReadJwtToken(dto.Token);
        jwt.Claims.Should().Contain(c => c.Type == "unique_name" && c.Value == username);
    }

    [Fact]
    public async Task Register_WhenUsernameAlreadyExists_Returns409()
    {
        // Arrange
        var username = $"dup_user_{Guid.NewGuid():N}";
        var password = "Aa1!aaaaaa";

        var payload = new
        {
            Username = username,
            Password = password,
            ConfirmPassword = password,
            Email = $"{username}@example.com"
        };

        // Act
        var firstResponse = await _client.PostAsJsonAsync("/api/Auth/register", payload);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondResponse = await _client.PostAsJsonAsync("/api/Auth/register", payload);

        // Assert
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
