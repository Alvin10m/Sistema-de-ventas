using Avalonia.Controls;
using SistemaVentas.ViewModels;
using Avalonia.Interactivity;

namespace SistemaVentas.Views
{
    public partial class PanelAdministracionWindow : Window
    {
        public PanelAdministracionWindow()
        {
            InitializeComponent();

            var viewModel = new PanelAdministracionViewModel();

            viewModel.SolicitarAbrirCrearUsuario += () =>
            {
                var ventanaCrearUsuario = new CrearUsuarioWindow();

                ventanaCrearUsuario.ShowDialog(this);
            };

            DataContext = viewModel;

        }

        private void VolverMenuPrincipal_Click(object? sender, RoutedEventArgs e)
        {
            var menuPrincipal = new MenuPrincipalWindow();

            menuPrincipal.Show();

            Close();
        }

        private void CerrarSesion_Click(object? sender,RoutedEventArgs e)
        {
            var inicioSesion = new LoginWindow();

            inicioSesion.Show();

            Close();
        }
    }
}