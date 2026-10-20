# WindTree

WindTree is a Windows 10/11 .NET 8 desktop application for managing per-executable Windows Filtering Platform (WFP) network block rules. It is an application-level network filter, not a VPN or a Wi-Fi controller.

## Features

- Fluent WPF interface using [WPF UI 4.3.0](https://github.com/lepoco/wpfui): `FluentWindow`, cards, buttons, and Fluent System Icons.
- System-theme-aware appearance, with persisted `System`, `Dark`, and `Light` preference model values.
- Browse for an executable or choose an accessible running process.
- Explicit Ask / Allow / Block decision before an app is managed.
- Text search, confirmed/pending/error status, and removal controls.
- Persistent configuration at `%LOCALAPPDATA%\WindTree\settings.json`, saved atomically.
- Persistent starter profiles: Default, Work, Gaming, and Privacy.

## Build and run

```powershell
dotnet build WindTree.slnx
dotnet test WindTree.slnx
dotnet run --project src/WindTree
```

Publish a self-contained executable:

```powershell
dotnet publish src\WindTree\WindTree.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish\WindTree-win-x64
```

Run WindTree elevated to install or remove WFP filters. When it is not elevated, it reports the failure and retains the previously confirmed state.

## WFP behavior

For a blocked executable, WindTree resolves the supported WFP application identity through `FwpmGetAppIdFromFileName0`. It adds four persistent terminating block filters to its own WFP provider and sublayer:

- `ALE_AUTH_CONNECT_V4` and `ALE_AUTH_CONNECT_V6` for outbound authorization
- `ALE_AUTH_RECV_ACCEPT_V4` and `ALE_AUTH_RECV_ACCEPT_V6` for inbound accept authorization

Filter changes run inside a WFP transaction. WindTree persists only IDs that it received from WFP, and validates the provider before deleting a filter. Allowing an application removes WindTree’s own blocking filters; other Windows Firewall and security-product policy still applies. Existing sockets can remain connected until Windows reauthorizes them.

## Limitations and verification

The published build is smoke-tested for launch and has unit tests for path validation and settings persistence. It has **not** had an elevated, controlled live-network enforcement test in this environment. Validate first with a non-critical executable and local endpoint: block it, confirm it cannot make a new IPv4/IPv6 connection, confirm another program still can, then allow it and check the WindTree filters are removed.

Profile editing/application, tray controls, close-to-tray behavior, and a settings screen are not implemented yet. WFP rules persist when the application exits and require an elevated WindTree session to remove.

## Contributing

Use `dotnet build WindTree.slnx` and `dotnet test WindTree.slnx` before submitting changes. Keep all WFP calls within the networking service and never delete filters that are not owned by WindTree.
