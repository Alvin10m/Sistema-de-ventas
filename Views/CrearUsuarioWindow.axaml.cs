using Avalonia.Controls;
using SistemaVentas.ViewModels;

namespace SistemaVentas.Views
{
    public partial class CrearUsuarioWindow : Window
    {
        public CrearUsuarioWindow()
        {
            InitializeComponent();

            var viewModel = new CrearUsuarioViewModel();

            viewModel.SolicitarCerrar += () =>
            {
                Close();
            };

            DataContext = viewModel;
        }
    }
}