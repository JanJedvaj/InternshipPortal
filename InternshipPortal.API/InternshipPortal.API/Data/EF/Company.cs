using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InternshipPortal.API.Data.EF;

public partial class Company
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    public string? Website { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    [InverseProperty("Company")]
    public virtual ICollection<Internship> Internships { get; set; } = new List<Internship>();
}
