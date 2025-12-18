using InternshipPortal.API.Repositories;
using InternshipPortal.API.Repositories.Abstractions;
using InternshipPortal.API.Services;
using InternshipPortal.API.Services.Abstractions;

using Microsoft.EntityFrameworkCore;
using InternshipPortal.API.Data.EF;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Aliasi za domenske modele (Models.*), ne EF entitete
using DomainCategory = InternshipPortal.API.Models.Category;
using DomainCompany = InternshipPortal.API.Models.Company;
using DomainInternship = InternshipPortal.API.Models.Internship;



var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<InternshipPortalContext>(options =>
    options.UseSqlServer(connectionString));


// Jwt token postavke
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured in appsettings.json (Jwt:Key).");
}

// Logiranje
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Controllers
builder.Services.AddControllers();

// Swagger + JWT
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
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
                      "Upišite: 'Bearer {token}'.\r\nPrimjer: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",

        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// Authentication & JWT
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});


//builder.Services.AddSingleton<IReadRepository<Category>, CategoryRepository>();
//builder.Services.AddSingleton<IWriteRepository<Category>, CategoryRepository>();

//builder.Services.AddSingleton<IReadRepository<Company>, CompanyRepository>();
//builder.Services.AddSingleton<IWriteRepository<Company>, CompanyRepository>();

//builder.Services.AddSingleton<IReadRepository<Internship>, InternshipRepository>();
//builder.Services.AddSingleton<IWriteRepository<Internship>, InternshipRepository>();

// Repozitoriji – koriste domenske modele (Models.*), ne EF entitete
// i moraju biti Scoped jer koriste DbContext (koji je Scoped).
builder.Services.AddScoped<IReadRepository<DomainCategory>, CategoryRepository>();
builder.Services.AddScoped<IWriteRepository<DomainCategory>, CategoryRepository>();

builder.Services.AddScoped<IReadRepository<DomainCompany>, CompanyRepository>();
builder.Services.AddScoped<IWriteRepository<DomainCompany>, CompanyRepository>();

builder.Services.AddScoped<IReadRepository<DomainInternship>, InternshipRepository>();
builder.Services.AddScoped<IWriteRepository<DomainInternship>, InternshipRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IInternshipService, InternshipService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IInternshipService, InternshipService>();


var app = builder.Build();

// Global error handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            Error = "Nešto nije uredu u API-ju. Pogledaj log za detalje."
        });
    });
});

// Middleware pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
