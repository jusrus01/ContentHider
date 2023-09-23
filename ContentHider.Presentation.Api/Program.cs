using ContentHider.Core.Repositories;
using ContentHider.Core.Services;
using ContentHider.DAL;
using ContentHider.Domain;
using ContentHider.Presentation.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

const string connectionString = "Default";

// RULES:
// Only pure functions
// Chain until execution

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<HiderDbContext>(options =>
        options.UseMySQL(
            builder.Configuration.GetConnectionString(connectionString) ??
            throw new InvalidOperationException("No connection string set")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<DelayedFunction>();

// builder.Services.AddSwaggerGen();
var app = builder.Build();

// app.UseHttpsRedirection();
// app.UseAuthorization();
app.ConfigureExceptionHandler();
app.ConfigureOrganizationEndPoints();

app.Run();
