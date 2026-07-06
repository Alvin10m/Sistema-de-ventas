using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
    }
}