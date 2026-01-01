using System.ComponentModel.DataAnnotations;

namespace InternshipPortal.BL.DTOi.Categories
{
    public class CategoryRequestDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
