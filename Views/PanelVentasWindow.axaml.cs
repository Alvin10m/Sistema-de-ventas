using Avalonia.Controls;

namespace SistemaVentas.Views
{
    public partial class PanelVentasWindow : Window
    {
        public PanelVentasWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => WindowState = WindowState.Maximized;
        }
    }
}