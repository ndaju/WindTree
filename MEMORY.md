# Project memory

- WindTree targets `net8.0-windows` and uses `WPF-UI` 4.3.0.
- `App.xaml` must keep `StartupUri="MainWindow.xaml"` or the dispatcher starts without a window.
- Do not set WPF UI Mica backdrops unless `ExtendsContentIntoTitleBar` is also enabled.
- The persistent WFP filter policy is intentionally not removed when the app exits.
- Settings are saved atomically under `%LOCALAPPDATA%\WindTree\settings.json`.
