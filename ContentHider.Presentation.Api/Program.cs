using ContentHider.Core.Repositories;
using ContentHider.Core.Services;
using ContentHider.DAL;
using ContentHider.Domain;
using ContentHider.Presentation.Api;
using ContentHider.Presentation.Api.Middlewares;
using Microsoft.EntityFrameworkCore;

const string connectionString = "Default";

// no behaviour with users
// simple crud

// RULES: 
// Only pure functions
// Chain until execution

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ICallerAccessor, CallerAccessor>();

builder.Services
    .AddDbContext<HiderDbContext>(options =>
        options.UseMySQL(
            builder.Configuration.GetConnectionString(connectionString) ??
            throw new InvalidOperationException("No connection string set")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IFormatService, FormatService>();
builder.Services.AddScoped<IRuleService, RuleService>();

// builder.Services.AddSwaggerGen();
var app = builder.Build();

// app.UseHttpsRedirection();
app.ConfigureExceptionHandler();
app.UseMiddleware<SimpleAuthenticationMiddleware>();

app.ConfigureOrganizationEndPoints();
app.ConfigureTextFormatEndPoints();
app.ConfigureRulesEndPoints();

app.Run();