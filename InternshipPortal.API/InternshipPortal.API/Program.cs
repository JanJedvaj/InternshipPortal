using InternshipPortal.API.Data.EF;

using InternshipPortal.API.Repositories.Categories;
using InternshipPortal.API.Repositories.Companies;
using InternshipPortal.API.Repositories.Internships;

using InternshipPortal.API.Services.Categories;
using InternshipPortal.API.Services.Companies;
using InternshipPortal.API.Services.Internships;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using InternshipPortal.API.Services.Internships.Factories;
using InternshipPortal.API.Services.Internships.Search;
using InternshipPortal.API.Services.Internships.Sorting;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

/*  builder.Services.AddDbContext<InternshipPortalContext>(options =>
    options.UseSqlServer(connectionString));   */ //Microsoft sql

builder.Services.AddDbContext<InternshipPortalContext>(options =>
    options.UseNpgsql(connectionString));


var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured (Jwt:Key).");
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

/*
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Query", LogLevel.Information);
builder.Logging.AddFilter("Npgsql", LogLevel.Information);  */


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "InternshipPortal.API",
        Version = "v1"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Upišite: Bearer {token}",

        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = signingKey,

            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});


builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IInternshipRepository, InternshipRepository>();



builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

builder.Services.AddScoped<IInternshipFactory, DefaultInternshipFactory>();
builder.Services.AddScoped<IInternshipService, InternshipService>();


builder.Services.AddScoped<IInternshipSearchFacade, InternshipSearchFacade>();


builder.Services.AddScoped<ICategoryNameStrategy, DefaultCategoryNameStrategy>();
builder.Services.AddScoped<ICategoryFactory, DefaultCategoryFactory>();
builder.Services.AddScoped<ICategoryFacade, CategoryFacade>();


builder.Services.AddScoped<ICategorySortingStrategy, MostUsedCategorySortingStrategy>();
builder.Services.AddScoped<CategorySortingStrategyResolver>();



builder.Services.AddScoped<IInternshipSortingStrategy, PostedAtSortingStrategy>();
builder.Services.AddScoped<IInternshipSortingStrategy, DeadlineSortingStrategy>();
builder.Services.AddScoped<IInternshipSortingStrategy, TitleSortingStrategy>();

var app = builder.Build();


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            Error = "Dogodila se greška na serveru. Pogledaj logove za detalje."
        });
    });
});


/*  app.UseSwagger();
    app.UseSwaggerUI(); */

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "InternshipPortal.API v1");
    c.RoutePrefix = "swagger"; // forces /swagger
});


app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

app.Run();

public partial class Program { }
