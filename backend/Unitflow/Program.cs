using Serilog;
using Unitflow.Features.Health;
using Unitflow.Infrastructure.Logging;
using Unitflow.Infrastructure.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddControllers();
builder.Services.AddOpenApiDocumentation();
builder.Services.AddHealthFeature();

var app = builder.Build();

app.UseRequestLogging();
app.UseOpenApiDocumentation();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
