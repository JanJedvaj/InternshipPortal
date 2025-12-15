using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InternshipPortal.API.Models;
using InternshipPortal.API.Data;
using InternshipPortal.BL.DTOi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // ===================== LOGIN 

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest? request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("Login request je null (prazno tijelo ili neispravan JSON).");
                    return BadRequest("Neispravno tijelo zahtjeva.");
                }

                if (string.IsNullOrWhiteSpace(request.Username) ||
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest("Username i password su obavezni.");
                }

                var users = FakeDatabase.Users;
                if (users == null)
                {
                    _logger.LogError("FakeDatabase.Users je null – nije inicijaliziran.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Interna greška: korisnici nisu inicijalizirani.");
                }

                var user = users.FirstOrDefault(u =>
                    u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == request.Password);

                if (user == null)
                {
                    return Unauthorized("Neispravno korisničko ime ili lozinka.");
                }

                var jwtSection = _configuration.GetSection("Jwt");
                var key = jwtSection["Key"];
                var issuer = jwtSection["Issuer"];
                var audience = jwtSection["Audience"];
                var expiresMinutes = int.TryParse(jwtSection["ExpiresMinutes"], out var minutes)
                    ? minutes
                    : 60;

                if (string.IsNullOrWhiteSpace(key))
                {
                    _logger.LogError("JWT Key nije konfiguriran (Jwt:Key).");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Greška u konfiguraciji JWT-a.");
                }

                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

                var role = string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role;

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, role)
                };

                var now = DateTime.UtcNow;
                var expires = now.AddMinutes(expiresMinutes);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    notBefore: now,
                    expires: expires,
                    signingCredentials: credentials);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                var response = new LoginResponse
                {
                    Token = tokenString,
                    ExpiresAtUtc = expires,
                    Username = user.Username,
                    Role = role
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Neočekivana greška prilikom prijave korisnika {Username}.",
                    request?.Username);

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom prijave.");
            }
        }

        // == Registracija

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult Register([FromBody] RegisterRequest? request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("Register request je null (prazno tijelo ili neispravan JSON).");
                    return BadRequest("Neispravno tijelo zahtjeva.");
                }

                // Validacija modela (data anotacije9
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrWhiteSpace(request.Username) ||
                    string.IsNullOrWhiteSpace(request.Password) ||
                    string.IsNullOrWhiteSpace(request.ConfirmPassword))
                {
                    return BadRequest("Korisničko ime, lozinka i potvrda lozinke su obavezni.");
                }

                // Should already be validated by [Compare], ali provjera ne škodi
                if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
                {
                    return BadRequest("Lozinka i potvrda lozinke se ne podudaraju.");
                }

                var users = FakeDatabase.Users;
                if (users == null)
                {
                    _logger.LogError("FakeDatabase.Users je null – nije inicijaliziran.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Interna greška: korisnici nisu inicijalizirani.");
                }

                var normalizedUsername = request.Username.Trim();

                // Provjera da li korisničko ime već postoji
                var existingUser = users.FirstOrDefault(u =>
                    u.Username.Equals(normalizedUsername, StringComparison.OrdinalIgnoreCase));

                if (existingUser != null)
                {
                    return Conflict("Korisničko ime je već zauzeto. Odaberite drugo korisničko ime.");
                }

                // Odredi novi ID
                var newId = users.Any() ? users.Max(u => u.Id) + 1 : 1;

                // Samoregistrirani korisnici -> rola "Student"
                var newUser = new User
                {
                    Id = newId,
                    Username = normalizedUsername,
                    Password = request.Password,  // napomena: za produkciju koristiti hash
                    Role = "Student"
                };

                users.Add(newUser);

                _logger.LogInformation("Novi korisnik registriran: {Username} sa ID-em {UserId}.",
                    newUser.Username, newUser.Id);

                // Nema auto-login: samo vratimo 201 Created + osnovne info
                return StatusCode(StatusCodes.Status201Created, new
                {
                    Message = "Korisnik je uspješno registriran.",
                    UserId = newUser.Id,
                    Username = newUser.Username,
                    Role = newUser.Role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Neočekivana greška prilikom registracije korisnika {Username}.",
                    request?.Username);

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom registracije.");
            }
        }
    }
}
