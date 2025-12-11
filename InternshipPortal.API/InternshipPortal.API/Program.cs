using InternshipApi.Models;
using InternshipApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS za React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});


builder.Services.AddSingleton<IRepository<Category>, CategoryRepository>();
builder.Services.AddSingleton<IRepository<Company>, CompanyRepository>();
builder.Services.AddSingleton<IRepository<Internship>, InternshipRepository>();

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


app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
