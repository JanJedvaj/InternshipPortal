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

    // IMPORTANT:
    // Removed OnConfiguring() to avoid hard-coding SQL Server.
    // The provider must be configured via DI in Program.cs (UseNpgsql).

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Applicat__3214EC07AC8AEACA");

            // SQL Server: (sysutcdatetime())
            // PostgreSQL equivalent (UTC):
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("timezone('utc', now())");

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

            // SQL Server: (sysutcdatetime())
            // PostgreSQL equivalent (UTC):
            entity.Property(e => e.PostedAt)
                  .HasDefaultValueSql("timezone('utc', now())");

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
