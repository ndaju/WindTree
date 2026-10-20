# Investigation

## Launch failure (fixed)

The process stayed alive with no main window because the WPF UI rewrite removed `StartupUri="MainWindow.xaml"` from `App.xaml`. Restoring it exposed a second runtime exception: WPF UI 4.3.0 rejects `WindowBackdropType="Mica"` while `ExtendsContentIntoTitleBar` is false. The application now uses the default FluentWindow backdrop and its normal title bar. This preserves WPF UI styling without a startup-only visual effect.

## WFP boundary

Source inspection confirms the app uses documented WFP entry points, a dedicated provider/sub-layer, app identity derived from an executable path, and four persistent ALE authorization filters. It refuses filter deletion unless the retrieved filter's provider equals the WindTree provider.

No elevated live-network test has been conducted in this environment. Source inspection does not establish effective network behavior.
