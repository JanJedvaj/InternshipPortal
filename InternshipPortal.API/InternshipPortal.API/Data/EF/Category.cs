using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InternshipPortal.API.Data.EF;

public partial class Category
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("Category")]
    public virtual ICollection<Internship> Internships { get; set; } = new List<Internship>();
}
