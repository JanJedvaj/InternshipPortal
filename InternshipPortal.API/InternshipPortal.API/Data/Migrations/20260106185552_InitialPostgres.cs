using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InternshipPortal.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__3214EC07554CAA72", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Companie__3214EC07C18E3342", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3214EC0779B422BC", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Internships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ShortDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FullDescription = table.Column<string>(type: "text", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    Remote = table.Column<bool>(type: "boolean", nullable: false),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PostedAt = table.Column<DateTime>(type: "timestamp(0) with time zone", precision: 0, nullable: false, defaultValueSql: "timezone('utc', now())"),
                    Deadline = table.Column<DateTime>(type: "timestamp(0) with time zone", precision: 0, nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Internsh__3214EC07C5C66436", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Internships_Categories",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Internships_Companies",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InternshipId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CoverLetter = table.Column<string>(type: "text", nullable: true),
                    CvUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp(0) with time zone", precision: 0, nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Applicat__3214EC07AC8AEACA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_Internships",
                        column: x => x.InternshipId,
                        principalTable: "Internships",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Applications_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_InternshipId",
                table: "Applications",
                column: "InternshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_UserId",
                table: "Applications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Internships_CategoryId",
                table: "Internships",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Internships_CompanyId",
                table: "Internships",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__536C85E4E7CA1934",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Internships");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
