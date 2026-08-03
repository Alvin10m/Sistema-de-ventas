using CommunityToolkit.Mvvm.ComponentModel;
using SistemaVentas.Models;
using SistemaVentas.Services;
using System;
using System.Collections.ObjectModel;

namespace SistemaVentas.ViewModels
{
    public partial class IngresosViewModel : ObservableObject
    {
        private readonly VentaService ventaService;

        // Lista de ventas para calcular los ingresos
        public ObservableCollection<VentaListaItem> Ventas { get; } = new ();

        // Cantidad total de las ventas registradas.
        [ObservableProperty]
        private int cantidadVentas;

        // Monto total acumulado de ingresos
        [ObservableProperty]
        private decimal totalIngresos;

        // Mensaje de aviso o error
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Indicar si el mensaje corresponde a un error
        [ObservableProperty]
        private bool esError;

        public IngresosViewModel()
        {
            ventaService = new VentaService();

            CargarIngresos();
        }

        // Consultar las ventas y carcular los ingresos acumulados
        private void CargarIngresos()
        {
            try
            {
                Ventas.Clear();
                CantidadVentas = 0;
                TotalIngresos = 0;
                Mensaje = string.Empty;
                EsError = false;

                var ventasRegistradas = ventaService.ObtenerVentas();

                if (ventasRegistradas.Count == 0)
                {
                    Mensaje = "No hay ventas registradas.";
                    EsError = false;
                    return;
                }
                foreach (var venta in ventasRegistradas)
                {
                    Ventas.Add(venta);
                    TotalIngresos += venta.Total;
                }

                CantidadVentas = Ventas.Count;

            }
            catch (Exception ex)
            {
                Mensaje = "No fue posible consultar ingresos. " + ex.Message;
                EsError = true;
            }
        }
    }
}