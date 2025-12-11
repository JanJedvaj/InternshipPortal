using InternshipApi.Models;
using InternshipApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Repos
builder.Services.AddSingleton<IRepository<Category>, CategoryRepository>();
builder.Services.AddSingleton<IRepository<Company>, CompanyRepository>();
builder.Services.AddSingleton<IRepository<Internship>, InternshipRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
