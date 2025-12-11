using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InternshipApi.Data;
using InternshipApi.API.Models;        // for User (FakeDatabase.Users)
using InternshipPortal.BL.DTOi;        // for LoginRequest / LoginResponse
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InternshipApi.Controllers
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

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest? request)
        {
            try
            {
                // 1) Defensive: null body / invalid JSON
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

                // 2) Defensive: users list
                var users = FakeDatabase.Users;
                if (users == null)
                {
                    _logger.LogError("FakeDatabase.Users je null – nije inicijaliziran.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Interna greška: korisnici nisu inicijalizirani.");
                }

                // 3) Look up user
                var user = users.FirstOrDefault(u =>
                    u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == request.Password);

                if (user == null)
                {
                    return Unauthorized("Neispravno korisničko ime ili lozinka.");
                }

                // 4) Read JWT config
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

                // 5) Build token
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

                //var claims = new List<Claim>
                //{
                //    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                //    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                //    new Claim(ClaimTypes.Name, user.Username),
                //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //    new Claim(ClaimTypes.Role, user.Role)
                //};

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
                    Role = user.Role
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Neočekivana greška prilikom prijave korisnika {Username}.",
                    request?.Username);

                // privremeno možemo vratiti detaljniji tekst dok debugiraš,
                // ali za produkciju bi ostao generički message
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Dogodila se greška prilikom prijave.");
            }
        }
    }
}
