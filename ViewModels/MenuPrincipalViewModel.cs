using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaVentas.Views;

namespace SistemaVentas.ViewModels
{
    public partial class MenuPrincipalViewModel : ObservableObject
    {
        [RelayCommand]
        private void AbrirVentas()
        {
            if (Application.Current?.ApplicationLifetime
                is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var ventanaMenu = desktop.MainWindow;

                var panelVentas = new PanelVentasWindow
                {
                    DataContext = new PanelVentasViewModel()
                };

                desktop.MainWindow = panelVentas;
                panelVentas.Show();
                ventanaMenu?.Close();
            }
        }
    }
}