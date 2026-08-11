using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace SistemaVentas.Views
{
    public partial class MenuPrincipalWindow : Window
    {
        public MenuPrincipalWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void AbrirPanelAdministracion_Click(object? sender, RoutedEventArgs e)
        {
            var panelAdministracion = new PanelAdministracionWindow();

            panelAdministracion.Show();
            
            Close();
        }
    }
}