using System.ComponentModel.DataAnnotations;

namespace InternshipPortal.BL.DTOi
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Lozinka mora imati barem 8 znakova.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrda lozinke je obavezna.")]
        [Compare("Password", ErrorMessage = "Lozinka i potvrda lozinke se ne podudaraju.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
