using ContentHider.Core.Repositories;
using ContentHider.DAL;
using ContentHider.Domain;
using ContentHider.Presentation.Api;

// RULES:
// Only pure functions
// Chain until execution

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<DelayedFunction>();

// builder.Services.AddSwaggerGen();
var app = builder.Build();

// app.UseHttpsRedirection();
// app.UseAuthorization();
app.ConfigureExceptionHandler();
app.ConfigureOrganizationEndPoints();

app.Run();
