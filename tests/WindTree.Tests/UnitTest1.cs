using System; using System.IO; using Xunit;
using WindTree.Models;
using WindTree.Services;

namespace WindTree.Tests;

public sealed class SettingsTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "WindTree.Tests", Guid.NewGuid().ToString("N"));
    private string SettingsPath => Path.Combine(_directory, "settings.json");

    [Fact]
    public void NormalizesExecutable()
    {
        var path = WindTree.Services.ApplicationIdentity.Normalize(Environment.ProcessPath!);
        Assert.EndsWith(".exe", path, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PersistsRulesProfilesAndTheme()
    {
        var settings = new WindTreeSettings { Theme = ThemePreference.Dark };
        settings.Applications.Add(ManagedApplication.Create(Environment.ProcessPath!, NetworkPolicy.Blocked));
        settings.Profiles.Add(new WindTreeProfile { Name = "Travel" });
        var store = new SettingsStore(SettingsPath);
        await store.SaveAsync(settings);
        var loaded = await store.LoadAsync();
        Assert.Equal(ThemePreference.Dark, loaded.Theme);
        Assert.Single(loaded.Applications);
        Assert.Contains(loaded.Profiles, p => p.Name == "Travel");
    }

    [Fact]
    public async Task CorruptSettingsUseSafeDefaults()
    {
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(SettingsPath, "not json");
        var loaded = await new SettingsStore(SettingsPath).LoadAsync();
        Assert.Empty(loaded.Applications);
        Assert.Equal(ThemePreference.System, loaded.Theme);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory)) Directory.Delete(_directory, true);
    }
}

