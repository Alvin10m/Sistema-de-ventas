using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaVentas.Models;
using System.Collections.ObjectModel;
using System;
using SistemaVentas.Services;

namespace SistemaVentas.ViewModels
{
    public partial class DetallesVentaViewModel : ObservableObject
    {
        private readonly VentaService ventaService;
        public ObservableCollection<VentaListaItem> Ventas {get; set;} = new();

        public DetallesVentaViewModel()
        {
            ventaService = new VentaService();

            CargarVentas();
        }

        private void CargarVentas()
        {
            try
            {
                Ventas.Clear();

                var ventasRegistradas = ventaService.ObtenerVentas();

                foreach (var venta in ventasRegistradas)
                {
                    Ventas.Add(venta);
                }

                Mensaje = ventasRegistradas.Count == 0
                    ? "No hay ventas registradas."
                    : string.Empty;

                EsError = false;
            }
            catch
            {
                Mensaje = "No fue posible consultar las ventas.";
                EsError = true;
            }
        }

        // Buscar una venta por el código ingresado por el usuario
        [RelayCommand]
        private void BuscarVenta()
        {
            Mensaje = string.Empty;
            EsError = false;
            VentaEncontrada = null;
            Productos.Clear();

            // Validar que el usuario haya escrito un código
            if (string.IsNullOrWhiteSpace(CodigoVentaBusqueda))
            {
                Mensaje = "Debe Ingresar el código de la venta.";
                EsError = true;
                return;
            }

            // Buscar la venta dentro de la lista cargada
            VentaListaItem? ventaLista = BuscarVentaEnLista(CodigoVentaBusqueda);

            // Verificar si la venta existe
            if (ventaLista is null)
            {
                Mensaje = "Venta no encontrada.";
                EsError = true;
                return;
            }

            // Cargar todos los detalles de la venta encontrada
            CargarDetalleVenta(ventaLista.IdVenta);
        }

        // Buscar una venta dentro de la lista utilizando su codigo
        private VentaListaItem? BuscarVentaEnLista(string codigo)
        {
            foreach (var venta in Ventas)
            {
                if (venta.CodigoVenta.Equals(
                    codigo.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return venta;
                }
            }

            return null;
        }
        
        // Consultar y cargar todos los detalles de una venta
        private void CargarDetalleVenta(int idVenta)
        {
            try
            {
                VentaConsulta? venta = ventaService.BuscarVentaPorId(idVenta);

                if (venta is null)
                {
                    Mensaje = "Venta no encontrada.";
                    EsError = true;
                    return;
                }

                // Guardar la información general de la venta
                VentaEncontrada = venta;

                // Limpiar los productos de una consulta anterior
                Productos.Clear();

                // Cargar los productos pertenecientes a la venta
                foreach (var producto in venta.Productos)
                {
                    Productos.Add(producto);
                }

                Mensaje = string.Empty;
                EsError = false;

            }
            catch
            {
                Mensaje = "No fue posible consultar los detalles de la venta.";
                EsError = true;
            }
        }

        // Mostrar el detalle de una venta seleccionada en la lista
        [RelayCommand]
        private void SeleccionarVenta(VentaListaItem? venta)
        {
            if (venta is null)
                return;

            VentaSeleccionada = venta;

            CargarDetalleVenta(venta.IdVenta);
        }

        // Código escrito por el usuario para realizar la búsqueda
        [ObservableProperty]
        private string codigoVentaBusqueda = string.Empty;

        // Venta seleccionada por el usuario en la lista
        [ObservableProperty]
        private VentaListaItem? ventaSeleccionada;

        // Venta encontrada en la base de datos
        [ObservableProperty]
        private VentaConsulta? ventaEncontrada;

        // Productos de la venta encontrada
        public ObservableCollection<DetalleVentaConsulta> Productos { get; } = new();

        // Mensajede éxito, aviso o error.
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Indicar si el el mensaje corresponde a un error.
        [ObservableProperty]
        private bool esError;
    }
}