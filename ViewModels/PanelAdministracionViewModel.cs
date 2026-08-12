using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaVentas.Services;

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

        // Validar que el usuario tenga permiso para realizar la acción deseada
        private bool ValidarPermiso(string nombrePermiso)
        {
            if (PermisoService.TienePermiso(nombrePermiso))
            {
                Mensaje = string.Empty;
                EsError = false;
                return true;
            }

            Mensaje = $"Usted no tiene rango para realizar la acción: {nombrePermiso}, busque su puesto.";
            EsError = true;
            return false;
        }

        // Mostrar la pantalla para agregar productos
        [RelayCommand]
        private void MostrarAgregarProducto()
        {
            if (!ValidarPermiso("Agregar producto"))
                return;

            ContenidoActual = new AgregarProductoViewModel();
        }

        // Mostrar la pantalla de consulta de inventario
        [RelayCommand]
        private void MostrarConsultarInventario()
        {
            if (!ValidarPermiso("Consultar inventario"))
                return;
            ContenidoActual = new ConsultarInventarioViewModel();
        }

        // Mostrar la pantalla de buscar ventas
        [RelayCommand]
        private void MostrarBuscarVentas()
        {
            if (!ValidarPermiso("Buscar ventas"))
                return;
            ContenidoActual = new BuscarVentasViewModel();
        }

        // Mostrar la pantalla de detalles de ventas
        [RelayCommand]
        private void MostrarDetallesVenta()
        {
            if (!ValidarPermiso("Detalles de venta"))
                return;
            ContenidoActual = new DetallesVentaViewModel();
        }

        // Mostrar la pestaña Crear usuario
        [RelayCommand]
        private void MostrarCrearUsuario()
        {
            if (!ValidarPermiso("Crear usuario"))
                return;

            SolicitarAbrirCrearUsuario?.Invoke();
        }

        // Solicitar la apertura de la pestaña Crear usuario
        public event Action? SolicitarAbrirCrearUsuario;

        // Mensaje relacionado con el acceso a las funcionalidades
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Idicar si ocurrió un error de acceso
        [ObservableProperty]
        private bool esError;

    }
}