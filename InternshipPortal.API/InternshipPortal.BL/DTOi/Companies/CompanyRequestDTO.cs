using System.ComponentModel.DataAnnotations;

namespace InternshipPortal.BL.DTOi.Companies
{
    public class CompanyRequestDTO
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string Website { get; set; } = string.Empty;

        [StringLength(100)]
        public string Location { get; set; } = string.Empty;
    }
}
