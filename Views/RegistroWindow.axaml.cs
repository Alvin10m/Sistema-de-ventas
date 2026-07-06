using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SistemaVentas.Views
{
    public partial class RegistroWindow : Window
    {
        public RegistroWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}