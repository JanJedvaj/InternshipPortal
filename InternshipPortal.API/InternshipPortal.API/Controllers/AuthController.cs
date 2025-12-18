using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InternshipPortal.API.Data.EF;
using InternshipPortal.BL.DTOi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace InternshipPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly InternshipPortalContext _context;

        public AuthController(
            IConfiguration configuration,
            ILogger<AuthController> logger,
            InternshipPortalContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

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
                    _logger.LogWarning("Login request je null.");
                    return BadRequest("Neispravno tijelo zahtjeva.");
                }

                if (string.IsNullOrWhiteSpace(request.Username) ||
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest("Username i password su obavezni.");
                }

                var user = _context.Users.FirstOrDefault(u =>
                    u.Username == request.Username);

                if (user == null || user.Password != request.Password)
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
                    return StatusCode(StatusCodes.Status500InternalServerError, "Greška u konfiguraciji JWT-a.");
                }

                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

                var role = string.IsNullOrWhiteSpace(user.Role) ? "Student" : user.Role;

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

                return Ok(new LoginResponse
                {
                    Token = tokenString,
                    ExpiresAtUtc = expires,
                    Username = user.Username,
                    Role = role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neočekivana greška prilikom prijave korisnika {Username}.", request?.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "Dogodila se greška prilikom prijave.");
            }
        }

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
                    _logger.LogWarning("Register request je null.");
                    return BadRequest("Neispravno tijelo zahtjeva.");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var normalizedUsername = request.Username.Trim();

                var exists = _context.Users.Any(u => u.Username == normalizedUsername);
                if (exists)
                    return Conflict("Korisničko ime je već zauzeto. Odaberite drugo korisničko ime.");

                var newUser = new User
                {
                    Username = normalizedUsername,
                    Password = request.Password, 
                    Role = "Student"
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

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
                _logger.LogError(ex, "Neočekivana greška prilikom registracije korisnika {Username}.", request?.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "Dogodila se greška prilikom registracije.");
            }
        }
    }
}
