using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using ApplicationIdentity = WindTree.Services.ApplicationIdentity;
using WindTree.Models;
using WindTree.Services;

namespace WindTree;

public partial class MainWindow : FluentWindow
{
    private readonly SettingsStore _store = new();
    private readonly WfpFilteringService _filter = new();
    private WindTreeSettings _settings = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += LoadSettingsAsync;
    }

    private async void LoadSettingsAsync(object sender, RoutedEventArgs e)
    {
        _settings = await _store.LoadAsync();
        ApplyTheme(_settings.Theme);
        Refresh();
    }

    private void ApplyTheme(ThemePreference preference)
    {
        var theme = preference switch
        {
            ThemePreference.Dark => ApplicationTheme.Dark,
            ThemePreference.Light => ApplicationTheme.Light,
            _ => ApplicationThemeManager.GetSystemTheme() == SystemTheme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light
        };
        ApplicationThemeManager.Apply(theme, WindowBackdropType.None, true);
    }
    private void Refresh()
    {
        var query = SearchBox?.Text?.Trim() ?? "";
        var applications = _settings.Applications
            .Where(a => string.IsNullOrWhiteSpace(query) || a.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || a.Path.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
        Applications.ItemsSource = applications;
        ManagedCount.Text = _settings.Applications.Count.ToString();
        BlockedCount.Text = _settings.Applications.Count(a => a.DesiredPolicy == NetworkPolicy.Blocked).ToString();
        AllowedCount.Text = _settings.Applications.Count(a => a.DesiredPolicy == NetworkPolicy.Allowed).ToString();
        AttentionCount.Text = _settings.Applications.Count(a => a.Status != RuleStatus.Confirmed).ToString();
        EmptyState.Visibility = applications.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        EnforcementText.Text = _filter.IsElevated ? "WFP enforcement ready — this session is elevated" : "Elevation required before WindTree can change WFP rules";
    }

    private void Search_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => Refresh();

    private async void AddApplication_Click(object sender, RoutedEventArgs e)
    {
        var picker = new OpenFileDialog { Filter = "Programs (*.exe)|*.exe", CheckFileExists = true };
        if (picker.ShowDialog() != true) return;
        await AddPathAsync(picker.FileName);
    }

    private async void RefreshProcesses_Click(object sender, RoutedEventArgs e)
    {
        var paths = Process.GetProcesses()
            .Select(p => { try { return p.MainModule?.FileName; } catch { return null; } finally { p.Dispose(); } })
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
            .Take(100)
            .ToList();
        var available = paths.FirstOrDefault(p => !_settings.Applications.Any(a => ApplicationIdentity.Same(a.Path, p!)));
        if (available is null) { MessageBox.Show("No un-managed process executable is available.", "WindTree"); return; }
        await AddPathAsync(available);
    }

    private async Task AddPathAsync(string selectedPath)
    {
        try
        {
            var path = ApplicationIdentity.Normalize(selectedPath);
            if (_settings.Applications.Any(a => ApplicationIdentity.Same(a.Path, path))) { MessageBox.Show("This executable is already managed.", "WindTree"); return; }
            var choice = MessageBox.Show($"Allow network access for:\n\n{path}\n\nYes = Allow\nNo = Block\nCancel = Decide later", "Manage application", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (choice == MessageBoxResult.Cancel) return;
            var app = ManagedApplication.Create(path, choice == MessageBoxResult.Yes ? NetworkPolicy.Allowed : NetworkPolicy.Blocked);
            _settings.Applications.Add(app);
            await ApplyAsync(app);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "WindTree", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void Allow_Click(object sender, RoutedEventArgs e) => await ChangeSelectedAsync(NetworkPolicy.Allowed);
    private async void Block_Click(object sender, RoutedEventArgs e) => await ChangeSelectedAsync(NetworkPolicy.Blocked);

    private async void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (Applications.SelectedItem is not ManagedApplication app) return;
        var result = await _filter.RemoveAsync(app);
        if (!result.Success) { MessageBox.Show(result.Message, "WindTree", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
        _settings.Applications.Remove(app);
        await _store.SaveAsync(_settings);
        Refresh();
    }

    private async Task ChangeSelectedAsync(NetworkPolicy policy)
    {
        if (Applications.SelectedItem is not ManagedApplication app) return;
        var previous = app.DesiredPolicy;
        app.DesiredPolicy = policy;
        var result = await ApplyAsync(app);
        if (!result) app.DesiredPolicy = previous;
        Refresh();
    }

    private async Task<bool> ApplyAsync(ManagedApplication app)
    {
        app.Status = RuleStatus.Pending;
        Refresh();
        var result = await _filter.ApplyAsync(app);
        app.Status = result.Success ? RuleStatus.Confirmed : RuleStatus.Error;
        app.Diagnostic = result.Message;
        if (result.Success) await _store.SaveAsync(_settings);
        else MessageBox.Show(result.Message, "WindTree", MessageBoxButton.OK, MessageBoxImage.Warning);
        Refresh();
        return result.Success;
    }
}





