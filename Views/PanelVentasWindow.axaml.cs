using Avalonia.Controls;
using System.Globalization;
using System.Threading;

namespace SistemaVentas.Views
{
    public partial class PanelVentasWindow : Window
    {
        public PanelVentasWindow()
        {
            // Poner los números decimales en el apartado de cantidad
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
            
            InitializeComponent();
            this.Loaded += (s, e) => WindowState = WindowState.Maximized;
        }
    }
}