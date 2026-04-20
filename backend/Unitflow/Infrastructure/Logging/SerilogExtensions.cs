using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Unitflow.Infrastructure.Database;

namespace Unitflow.Infrastructure.Logging;

public static class SerilogExtensions
{
    public static WebApplication UseRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (ctx, _, ex) =>
            {
                if (ex is not null || ctx.Response.StatusCode >= 500) return LogEventLevel.Error;
                if (ctx.Response.StatusCode >= 400) return LogEventLevel.Warning;
                return LogEventLevel.Information;
            };

            options.EnrichDiagnosticContext = (diag, ctx) =>
            {
                diag.Set("ClientIp", ctx.Connection.RemoteIpAddress?.ToString());
                diag.Set("UserAgent", ctx.Request.Headers.UserAgent.ToString());
                diag.Set("Scheme", ctx.Request.Scheme);
                diag.Set("Host", ctx.Request.Host.Value);
                if (ctx.User.Identity?.IsAuthenticated == true)
                    diag.Set("UserName", ctx.User.Identity.Name);
            };
        });

        return app;
    }

    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, _, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.With<ActivityEnricher>()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File("logs/unitflow-.log", rollingInterval: RollingInterval.Day);

            var connectionString = PostgresConnectionString.Normalize(
                context.Configuration.GetConnectionString("Postgres"));

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                configuration.WriteTo.PostgreSQL(
                    connectionString,
                    "logs",
                    PostgresColumnWriters,
                    needAutoCreateTable: true);
            }
        });

        return builder;
    }

    private static IDictionary<string, ColumnWriterBase> PostgresColumnWriters => new Dictionary<string, ColumnWriterBase>
    {
        { "message", new RenderedMessageColumnWriter() },
        { "message_template", new MessageTemplateColumnWriter() },
        { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
        { "timestamp", new TimestampColumnWriter() },
        { "exception", new ExceptionColumnWriter() },
        { "properties", new LogEventSerializedColumnWriter() }
    };
}
