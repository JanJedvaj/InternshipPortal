using InternshipApi.API.Models;
using InternshipApi.Models;

namespace InternshipApi.Data
{
    public static class FakeDatabase
    {
        public static List<Category> Categories = new()
        {
            new Category { Id = 1, Name = "IT & Programiranje" },
            new Category { Id = 2, Name = "Marketing" },
            new Category { Id = 3, Name = "Dizajn" },
            new Category { Id = 4, Name = "Financije" }
        };

        public static List<Company> Companies = new()
        {
            new Company { Id = 1, Name = "TechCorp", Website = "https://techcorp.example", Location = "Zagreb" },
            new Company { Id = 2, Name = "Digital Agency", Website = "https://digital.example", Location = "Split" },
            new Company { Id = 3, Name = "Creative Studio", Website = "https://creative.example", Location = "Rijeka" }
        };

        public static List<Internship> Internships = new()
        {
            new Internship
            {
                Id = 1,
                Title = "Frontend Developer Praksa",
                ShortDescription = "React + TypeScript praksa.",
                FullDescription = "Rad na modernim frontend tehnologijama.",
                CategoryId = 1,
                CompanyId = 1,
                Deadline = DateTime.UtcNow.AddDays(30),
                IsFeatured = true,
                Remote = false,
                Location = "Zagreb",
                PostedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Internship
            {
                Id = 2,
                Title = "Marketing Stručnjak – Praksa",
                ShortDescription = "Digital marketing praksa.",
                FullDescription = "SEO, content, social media…",
                CategoryId = 2,
                CompanyId = 2,
                Deadline = DateTime.UtcNow.AddDays(20),
                IsFeatured = true,
                Remote = true,
                Location = "Split",
                PostedAt = DateTime.UtcNow.AddDays(-8)
            },
            new Internship
            {
                Id = 3,
                Title = "UX/UI Dizajner Praksa",
                ShortDescription = "Dizajn interakcija i prototipiranje.",
                FullDescription = "Rad u Figma-i, kreiranje prototipova.",
                CategoryId = 3,
                CompanyId = 3,
                Deadline = DateTime.UtcNow.AddDays(40),
                IsFeatured = false,
                Remote = false,
                Location = "Rijeka",
                PostedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        
        public static List<User> Users = new()
        {
            new User
            {
                Id = 1,
                Username = "student",
                Password = "student123",
                Role = "Student"
            },
            new User
            {
                Id = 2,
                Username = "company",
                Password = "company123",
                Role = "Company"
            },
            new User
            {
                Id = 3,
                Username = "admin",
                Password = "admin123",
                Role = "Admin"
            }
        };
    }
}
