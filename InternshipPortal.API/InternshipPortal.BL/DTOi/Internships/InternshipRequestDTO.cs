using System;
using System.ComponentModel.DataAnnotations;

namespace InternshipPortal.BL.DTOi.Internships
{
    public class InternshipRequestDTO
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string ShortDescription { get; set; } = string.Empty;

        [Required]
        public string FullDescription { get; set; } = string.Empty;

        public bool IsFeatured { get; set; }
        public bool Remote { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        public DateTime? Deadline { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
