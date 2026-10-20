# Project memory

- WindTree targets `net8.0-windows` and uses `WPF-UI` 4.3.0.
- The UI starts in WPF UI's system theme. Persisted theme settings are modeled but no settings screen exists yet.
- The persistent WFP filter policy is intentionally not removed when the app exits.
- Settings are saved atomically under `%LOCALAPPDATA%\WindTree\settings.json`.
