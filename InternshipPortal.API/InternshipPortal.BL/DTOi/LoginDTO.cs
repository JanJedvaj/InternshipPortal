using System;
using System.ComponentModel.DataAnnotations;

namespace InternshipPortal.BL.DTOi
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Korisnicko ime je obavezno.")]
       
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
