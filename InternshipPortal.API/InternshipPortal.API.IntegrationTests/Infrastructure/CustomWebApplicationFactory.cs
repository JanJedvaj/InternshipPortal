using InternshipPortal.API.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InternshipPortal.API.IntegrationTests.Infrastructure
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        // Svaki factory dobije svoj DB name -> nema sudara između testova/hostova
        private readonly string _dbName = $"InternshipPortal_TestDb_{Guid.NewGuid():N}";
        private readonly InMemoryDatabaseRoot _dbRoot = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // 1) JWT konfiguracija (da Program.cs ne baci "Jwt:Key nije konfiguriran")
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var jwtInMemory = new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "TEST_TEST_TEST_TEST_TEST_TEST_TEST_TEST_1234567890",
                    ["Jwt:Issuer"] = "InternshipPortal.Api",
                    ["Jwt:Audience"] = "InternshipPortal.Client",
                    ["Jwt:ExpiresMinutes"] = "60",
                };

                config.AddInMemoryCollection(jwtInMemory);
            });

            builder.ConfigureServices(services =>
            {
                // 2) Makni postojeći DbContext (Npgsql) i ubaci InMemory (unique name)
                services.RemoveAll(typeof(DbContextOptions<InternshipPortalContext>));

                services.AddDbContext<InternshipPortalContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName, _dbRoot);
                });

                // 3) Seed
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<InternshipPortalContext>();
                db.Database.EnsureCreated();

                Seed(db);
            });
        }

        private static void Seed(InternshipPortalContext db)
        {
            // Ako se Seed ikad pozove više puta, ovo osigurava idempotentnost
            if (db.Categories.Any() || db.Companies.Any() || db.Internships.Any() || db.Users.Any())
                return;

            db.Categories.Add(new Category { Id = 1, Name = "IT" });
            db.Companies.Add(new Company { Id = 1, Name = "TestCompany" });

            db.Internships.AddRange(
                new Internship
                {
                    Id = 1,
                    Title = "Backend Developer",
                    ShortDescription = "Short",
                    FullDescription = "Full",
                    Location = "Zagreb",
                    Remote = true,
                    IsFeatured = false,
                    PostedAt = DateTime.UtcNow.AddDays(-3),
                    Deadline = DateTime.UtcNow.AddDays(10),
                    CompanyId = 1,
                    CategoryId = 1
                },
                new Internship
                {
                    Id = 2,
                    Title = "Frontend Developer",
                    ShortDescription = "Short2",
                    FullDescription = "Full2",
                    Location = "Zagreb",
                    Remote = false,
                    IsFeatured = false,
                    PostedAt = DateTime.UtcNow.AddDays(-2),
                    Deadline = DateTime.UtcNow.AddDays(12),
                    CompanyId = 1,
                    CategoryId = 1
                }
            );

            db.Users.Add(new User
            {
                Id = 1,
                Username = "test",
                Password = "test12345",
                Role = "Student"
            });

            db.SaveChanges();
        }
    }
}
