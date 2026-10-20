namespace WindTree.Models;

public enum NetworkPolicy { Allowed, Blocked }
public enum RuleStatus { Pending, Confirmed, Error, Unmanaged }
public enum ThemePreference { System, Dark, Light }

public sealed class ManagedApplication
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DisplayName { get; set; } = "";
    public string Path { get; set; } = "";
    public NetworkPolicy DesiredPolicy { get; set; }
    public RuleStatus Status { get; set; } = RuleStatus.Pending;
    public string? Diagnostic { get; set; }
    public List<ulong> FilterIds { get; set; } = [];
    public static ManagedApplication Create(string path, NetworkPolicy policy) => new() { Path = path, DisplayName = System.IO.Path.GetFileNameWithoutExtension(path), DesiredPolicy = policy };
}

public sealed class WindTreeProfile { public string Name { get; set; } = "Default"; public Dictionary<Guid, NetworkPolicy> Policies { get; set; } = []; }
public sealed class WindTreeSettings
{
    public int Version { get; set; } = 2;
    public ThemePreference Theme { get; set; } = ThemePreference.System;
    public List<ManagedApplication> Applications { get; set; } = [];
    public List<WindTreeProfile> Profiles { get; set; } = [new() { Name = "Default" }, new() { Name = "Work" }, new() { Name = "Gaming" }, new() { Name = "Privacy" }];
}
