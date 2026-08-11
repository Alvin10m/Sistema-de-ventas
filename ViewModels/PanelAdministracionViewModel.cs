using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SistemaVentas.ViewModels
{
    public partial class PanelAdministracionViewModel : ObservableObject
    {
        // Indicar si el menú Productos está desplegado
        [ObservableProperty]
        private bool productosExpandido;

        // Indicar si el menú Facturas está desplegado
        [ObservableProperty]
        private bool facturasExpandido;

        // Indicar si el menú Reportes está desplegado
        [ObservableProperty]
        private bool reportesExpandido;

        // Indicar si el menú Usuarios está desplegado
        [ObservableProperty]
        private bool usuariosExpandido;

        // Mostrar u ocultar el menú Productos
        [RelayCommand]
        private void AlternarProductos()
        {
            ProductosExpandido = !ProductosExpandido;
        }

        // Mostrar u ocultar el menú Facturas
        [RelayCommand]
        private void AlternarFacturas()
        {
            FacturasExpandido = !FacturasExpandido;
        }

        // Mostrar u ocultar el menú Reportes
        [RelayCommand]
        private void AlternarReportes()
        {
            ReportesExpandido = !ReportesExpandido;
        }

        // Mostrar u ocultar el menú Usuarios
        [RelayCommand]
        private void AlternarUsuarios()
        {
            UsuariosExpandido = !UsuariosExpandido;
        }

        // Contenido a mostrar en el área derecha del panel
        [ObservableProperty]
        private object? contenidoActual;

        // Mostrar la pantalla para agregar productos
        [RelayCommand]
        private void MostrarAgregarProducto()
        {
            ContenidoActual = new AgregarProductoViewModel();
        }

        // Mostrar la pantalla de consulta de inventario
        [RelayCommand]
        private void MostrarConsultarInventario()
        {
            ContenidoActual = new ConsultarInventarioViewModel();
        }

        // Mostrar la pantalla de buscar ventas
        [RelayCommand]
        private void MostrarBuscarVentas()
        {
            ContenidoActual = new BuscarVentasViewModel();
        }

    }
}