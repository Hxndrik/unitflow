using Npgsql;

namespace Unitflow.Infrastructure.Database;

public static class PostgresConnectionString
{
    public static string? Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            return value;

        var uri = new Uri(value);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
            Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'))
        };

        foreach (var (key, val) in ParseQuery(uri.Query))
            builder[key] = val;

        return builder.ConnectionString;
    }

    private static IEnumerable<KeyValuePair<string, string>> ParseQuery(string query)
    {
        if (string.IsNullOrEmpty(query)) yield break;
        var trimmed = query.StartsWith('?') ? query[1..] : query;
        foreach (var part in trimmed.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var idx = part.IndexOf('=');
            if (idx < 0) continue;
            yield return new KeyValuePair<string, string>(
                Uri.UnescapeDataString(part[..idx]),
                Uri.UnescapeDataString(part[(idx + 1)..]));
        }
    }
}
