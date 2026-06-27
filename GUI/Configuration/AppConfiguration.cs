using System.IO;
using System.Text.Json;

namespace GUI.Configuration;

public static class AppConfiguration
{
    public static string GetConnectionString(string name = "DefaultConnection")
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException("Cannot find appsettings.json in application output folder.", configPath);
        }

        using var document = JsonDocument.Parse(File.ReadAllText(configPath));

        if (document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings)
            && connectionStrings.TryGetProperty(name, out var connectionString))
        {
            return connectionString.GetString() ?? string.Empty;
        }

        throw new InvalidOperationException($"Connection string '{name}' was not found.");
    }
}

