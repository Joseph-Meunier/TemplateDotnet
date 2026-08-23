using Scalar.AspNetCore;
using Template.Api.Errors;
using Template.Modules.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.CreateSchemaReferenceId = type =>
        type.Type.FullName?.Replace("+", ".");
});

// Add ProblemDetails middleware for standardized error responses
builder.Services.AddProblemDetails();

// Add global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add UserModule services
builder.Services.AddUsersModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Map UserModule endpoints
app.MapUsersModule();

// Use ProblemDetails middleware to handle exceptions and return standardized error responses
app.UseExceptionHandler();

app.Run();

// Required for integration tests
public partial class Program;