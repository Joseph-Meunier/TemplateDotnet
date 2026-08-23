using Scalar.AspNetCore;
using Template.Modules.Sample;
using Template.Api.Errors;

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

// Add SampleModule services and endpoints
builder.Services.AddSampleModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Map SampleModule endpoints
app.MapSampleModule();

// Use ProblemDetails middleware to handle exceptions and return standardized error responses
app.UseExceptionHandler();

app.Run();

// Required for integration tests
public partial class Program;