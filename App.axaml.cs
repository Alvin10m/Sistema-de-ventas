using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SistemaVentas.ViewModels;
using SistemaVentas.Views;

namespace SistemaVentas;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new PanelVentasWindow // se debe cambiar "PanelVentasWindow" por "RegistroWindow"
            {
                DataContext = new PanelVentasViewModel(), // Se debe cambiar "PanelVentasViewModel()", por "RegistroViewModel(),""
            };
        }
        base.OnFrameworkInitializationCompleted();
    }
}