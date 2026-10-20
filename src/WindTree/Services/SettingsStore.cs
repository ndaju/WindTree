using System; using System.IO; using System.Threading.Tasks; using System.Text.Json;
using WindTree.Models;

namespace WindTree.Services;

public sealed class SettingsStore
{
    private static readonly JsonSerializerOptions Json = new() { WriteIndented = true };
    private readonly string _filePath;

    public SettingsStore(string? filePath = null) => _filePath = filePath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WindTree", "settings.json");

    public async Task<WindTreeSettings> LoadAsync()
    {
        try
        {
            var settings = JsonSerializer.Deserialize<WindTreeSettings>(await File.ReadAllTextAsync(_filePath), Json) ?? new();
            settings.Applications ??= [];
            settings.Profiles ??= [];
            return settings;
        }
        catch (FileNotFoundException) { return new(); }
        catch (JsonException) { return new(); }
    }

    public async Task SaveAsync(WindTreeSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        var temporary = _filePath + ".tmp";
        await File.WriteAllTextAsync(temporary, JsonSerializer.Serialize(settings, Json));
        File.Move(temporary, _filePath, true);
    }
}

