using System.Windows;
using Wpf.Ui.Appearance;

namespace WindTree;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        ApplicationThemeManager.ApplySystemTheme(true);
        base.OnStartup(e);
    }
}
