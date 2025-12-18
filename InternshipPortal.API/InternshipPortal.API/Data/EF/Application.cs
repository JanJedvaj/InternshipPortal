using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InternshipPortal.API.Data.EF;

[Index("InternshipId", Name = "IX_Applications_InternshipId")]
[Index("UserId", Name = "IX_Applications_UserId")]
public partial class Application
{
    [Key]
    public int Id { get; set; }

    public int InternshipId { get; set; }

    public int UserId { get; set; }

    public string? CoverLetter { get; set; }

    [StringLength(500)]
    public string? CvUrl { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("InternshipId")]
    [InverseProperty("Applications")]
    public virtual Internship Internship { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Applications")]
    public virtual User User { get; set; } = null!;
}
