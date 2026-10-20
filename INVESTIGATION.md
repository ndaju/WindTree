# Investigation

## Baseline

- The repository was initially a standalone WPF .NET 8 project without Git metadata.
- `dotnet build WindTree.slnx` and its two tests passed before the WPF UI integration.
- `WPF-UI` 4.3.0 was restored from NuGet and exposes `FluentWindow`, `Button`, `Card`, `SymbolIcon`, and `ApplicationThemeManager` for `net8.0-windows`.

## WFP boundary

Source inspection confirms the app uses documented WFP entry points, a dedicated provider/sub-layer, app identity derived from an executable path, and four persistent ALE authorization filters. It refuses filter deletion unless the retrieved filter's provider equals the WindTree provider.

No elevated live-network test has been conducted in this environment. Source inspection does not establish effective network behavior.
