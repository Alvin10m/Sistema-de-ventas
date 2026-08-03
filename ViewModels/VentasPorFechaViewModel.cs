using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaVentas.Models;
using SistemaVentas.Services;
using System;
using System.Collections.ObjectModel;

namespace SistemaVentas.ViewModels
{
    public partial class VentasPorFechaViewModel : ObservableObject
    {
        private readonly VentaService ventaService;

        // Ventas encontradas en la fecha seleccionada.
        public ObservableCollection<VentaPorFechaItem> Ventas { get; } = new();

        // Fecha elegida por el usuario.
        [ObservableProperty]
        private DateTime fechaSeleccionada = DateTime.Today;

        // Venta seleccionada por el usuario en la lista.
        [ObservableProperty]
        private VentaPorFechaItem? ventaSeleccionada;

        // Información completa de la venta seleccionada
        [ObservableProperty]
        private VentaConsulta? detalleVentaSeleccionada;

        // Cantidad de ventas encontradas.
        [ObservableProperty]
        private int cantidadVentas;

        // Monto total generado durante la fecha consultada.
        [ObservableProperty]
        private decimal totalGenerado;

        // Mensaje de aviso o error.
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Indicar si el mensaje corresponde a un error.
        [ObservableProperty]
        private bool esError;

        public VentasPorFechaViewModel()
        {
            ventaService = new VentaService();
        }

        // Buscar las ventas realizadas en la fecha seleccionada.
        [RelayCommand]
        private void BuscarVentasPorFecha()
        {
            try
            {
                Ventas.Clear();
                CantidadVentas = 0;
                TotalGenerado = 0;
                Mensaje = string.Empty;
                EsError = false;

                var ventasEncontradas =
                    ventaService.ObtenerVentasPorFecha(FechaSeleccionada);

                if (ventasEncontradas.Count == 0)
                {
                    Mensaje =
                        "No se encontraron ventas para la fecha seleccionada.";
                    EsError = false;
                    return;
                }

                foreach (var venta in ventasEncontradas)
                {
                    Ventas.Add(venta);
                    TotalGenerado += venta.Total;
                }

                CantidadVentas = Ventas.Count;
            }
            catch (Exception ex)
            {
                Mensaje =
                    "No fue posible consultar las ventas por fecha. " + ex.Message;
                EsError = true;
            }
        }

        // Consultar los detalles de la venta seleccionada.
        [RelayCommand]
        private void ConsultarDetalleVenta()
        {
            if (VentaSeleccionada is null)
            {
                Mensaje = "Debe seleccionar una venta.";
                EsError = true;
                return;
            }

            try
            {
                DetalleVentaSeleccionada = ventaService.BuscarVentaPorId(VentaSeleccionada.IdVenta);

                if (DetalleVentaSeleccionada is null)
                {
                    Mensaje = "Venta no encontrada.";
                    EsError = true;
                    return;

                }
                Mensaje = string.Empty;
                EsError = false;
            }
            catch (Exception ex)
            {
                Mensaje = "No fue posible consultar los detalles de la venta. " + ex.Message;
                EsError = true;
            }
        }

        // Cerrar los detalles de la venta sin borrar las búsqueda realizada.
        [RelayCommand]
        private void CerrarDetalleVenta()
        {
            DetalleVentaSeleccionada = null;
            VentaSeleccionada = null;

            Mensaje = string.Empty;
            EsError = false;
        }

    }
}