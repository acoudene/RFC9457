using Microsoft.AspNetCore.Http.Features;
using MyApi;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Adds services for using Problem Details format
builder.Services.AddProblemDetails(options =>
{
  options.CustomizeProblemDetails = context =>
  {
    context.ProblemDetails.Instance =
        $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

    Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
    context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
  };
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Converts unhandled exceptions into Problem Details responses
app.UseExceptionHandler();

// Returns the Problem Details response for (empty) non-successful responses
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.MapProductEndpoints();
app.MapErrorEndpoints();

app.Run();