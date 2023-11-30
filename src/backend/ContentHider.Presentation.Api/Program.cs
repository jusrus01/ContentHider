using ContentHider.Core;
using ContentHider.Core.Daos;
using ContentHider.Core.Dtos.Auth;
using ContentHider.Core.Extensions;
using ContentHider.Core.Repositories;
using ContentHider.Core.Services;
using ContentHider.DAL;
using ContentHider.Domain;
using ContentHider.Presentation.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

async Task SeedAdminAsync(RoleManager<IdentityRole> roleManager, UserManager<UserDao> userManager)
{
    var adminRole = new IdentityRole(Constants.Roles.Admin);
    if (!await roleManager.RoleExistsAsync(adminRole.Name!))
    {
        await roleManager.CreateAsync(adminRole);
    }

    var admin = new UserDao
    {
        UserName = "admin",
        Email = "admin@admin.com",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true
    };

    if (await userManager.FindByEmailAsync(admin.Email) == null)
    {
        await userManager.CreateAsync(admin, "admin");
        await userManager.AddToRoleAsync(admin, adminRole.Name!);
    }
}

async Task SeedUserAsync(RoleManager<IdentityRole> roleManager, UserManager<UserDao> userManager)
{
    var userRole = new IdentityRole(Constants.Roles.User);
    if (!await roleManager.RoleExistsAsync(userRole.Name!))
    {
        await roleManager.CreateAsync(userRole);
    }

    var user = new UserDao
    {
        UserName = "user",
        Email = "user@user.com",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true
    };

    if (await userManager.FindByEmailAsync(user.Email) == null)
    {
        await userManager.CreateAsync(user, "user");
        await userManager.AddToRoleAsync(user, userRole.Name!);
    }
}

async Task SeedAsync(WebApplication webApplication)
{
    using (var scope = webApplication.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<UserDao>>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            await SeedUserAsync(roleManager, userManager);
            await SeedAdminAsync(roleManager, userManager);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occured");
            throw;
        }
    }
}

const string jwtSectionName = "Jwt";
const string connectionString = "Default";

void AddSecurity(WebApplicationBuilder webApplicationBuilder)
{
    var configuration = webApplicationBuilder.Configuration;

    webApplicationBuilder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = configuration[$"{jwtSectionName}:Issuer"],
            ValidAudience = configuration[$"{jwtSectionName}:Audience"],
            IssuerSigningKey = configuration[$"{jwtSectionName}:Key"].GetIssuerSigningKey()
        };
    });
    webApplicationBuilder.Services.AddAuthorization();
}

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//BAD
// builder.Services.Configure<JsonOptions>(options =>
// {
//     options.SerializerOptions.PropertyNameCaseInsensitive = false;
//     options.SerializerOptions.PropertyNamingPolicy = null;
//     options.SerializerOptions.WriteIndented = true;
//     options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
// });

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(jwtSectionName));
builder.Services.AddIdentity<UserDao, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 1;
        options.Password.RequiredUniqueChars = 1;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
    }).AddEntityFrameworkStores<HiderDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ICallerAccessor, CallerAccessor>();

builder.Services
    .AddDbContext<HiderDbContext>(options =>
        options.UseMySQL(
            builder.Configuration.GetConnectionString(connectionString) ??
            throw new InvalidOperationException("No connection string set")));

// Seems HttpOnly cookie setting JWT is a good idea :)

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IFormatService, FormatService>();
builder.Services.AddScoped<IRuleService, RuleService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.EnableAnnotations());

// TODO: will need to redeploy with other settings.
// first deploy front-end.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyConfig =>
        policyConfig
            .WithOrigins("http://localhost:4200")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

AddSecurity(builder);

var app = builder.Build();

// app.UseHttpsRedirection();
app.ConfigureExceptionHandler();
// app.UseMiddleware<SimpleAuthenticationMiddleware>();

app.ConfigureOrganizationEndPoints();
app.ConfigureTextFormatEndPoints();
app.ConfigureRulesEndPoints();
app.ConfigureAuthEndPoints();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

await SeedAsync(app);

app.Run();