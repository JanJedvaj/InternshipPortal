using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InternshipPortal.API.Data.EF;

public partial class InternshipPortalContext : DbContext
{
    public InternshipPortalContext()
    {
    }

    public InternshipPortalContext(DbContextOptions<InternshipPortalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Internship> Internships { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer("Name=DefaultConnection");
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Applicat__3214EC07AC8AEACA");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Internship).WithMany(p => p.Applications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applications_Internships");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applications_Users");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07554CAA72");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Companie__3214EC07C18E3342");
        });

        modelBuilder.Entity<Internship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Internsh__3214EC07C5C66436");

            entity.Property(e => e.PostedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Category).WithMany(p => p.Internships)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Internships_Categories");

            entity.HasOne(d => d.Company).WithMany(p => p.Internships)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Internships_Companies");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0779B422BC");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
