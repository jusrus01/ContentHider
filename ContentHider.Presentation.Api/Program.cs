using ContentHider.Presentation.Api;

// RULES:
// Only pure functions
// Chain until execution

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddSwaggerGen();
var app = builder.Build();

// app.UseHttpsRedirection();
// app.UseAuthorization();
app.ConfigureExceptionHandler();
app.ConfigureOrganizationEndPoints();

app.Run();
