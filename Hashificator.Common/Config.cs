using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hashificator.Common;

public class Config
{
    public int ThreadCount { get; set; } = 1;

    public int BufferSizeKiB { get; set; } = 64;

    public static async Task<Config> LoadConfig(string directory, string fileName)
    {
        Config config;

        try
        {
            return JsonSerializer.Deserialize<Config>(await File.ReadAllTextAsync(Path.Combine(directory, fileName)));
        }
        catch (Exception ex) when (ex is JsonException or FileNotFoundException or DirectoryNotFoundException)
        {
            _ = Directory.CreateDirectory(directory);
            config = new Config { ThreadCount = Math.Max(Environment.ProcessorCount / 4, 1) };
            await SaveConfig(directory, fileName, config);
        }

        return config;
    }

    public static Task SaveConfig(string directory, string fileName, Config config)
    {
        var jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        return File.WriteAllTextAsync(Path.Combine(directory, fileName), jsonString);
    }
}