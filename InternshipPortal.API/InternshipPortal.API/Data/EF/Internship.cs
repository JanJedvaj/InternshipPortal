using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InternshipPortal.API.Data.EF;

[Index("CategoryId", Name = "IX_Internships_CategoryId")]
[Index("CompanyId", Name = "IX_Internships_CompanyId")]
public partial class Internship
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(500)]
    public string ShortDescription { get; set; } = null!;

    public string FullDescription { get; set; } = null!;

    public bool IsFeatured { get; set; }

    public bool Remote { get; set; }

    [StringLength(100)]
    public string Location { get; set; } = null!;

    [Precision(0)]
    public DateTime PostedAt { get; set; }

    [Precision(0)]
    public DateTime? Deadline { get; set; }

    public int CompanyId { get; set; }

    public int CategoryId { get; set; }

    [InverseProperty("Internship")]
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Internships")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("CompanyId")]
    [InverseProperty("Internships")]
    public virtual Company Company { get; set; } = null!;
}
