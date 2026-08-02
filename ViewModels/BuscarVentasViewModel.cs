using CommunityToolkit.Mvvm.ComponentModel;
using SistemaVentas.Models;
using SistemaVentas.Services;
using System;
using System.Collections.ObjectModel;

namespace SistemaVentas.ViewModels
{
    public partial class BuscarVentasViewModel : ObservableObject
    {
        private readonly VentaService ventaService;

        // Lista de ventas a mostrará en el panel.
        public ObservableCollection<VentaListaItem> Ventas { get; } = new();

        // Mensaje de aviso o error
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Indicar si el mensaje corresponde a un error.
        [ObservableProperty]
        private bool esError;

        public BuscarVentasViewModel()
        {
            ventaService = new VentaService();

            CargarVentas();
        }

        // Consultar y cargar todas las ventas registradas 
        private void CargarVentas()
        {
            try
            {
                Ventas.Clear();

                var ventasRegistradas = ventaService.ObtenerVentas();

                if (ventasRegistradas.Count == 0)
                {
                    Mensaje = "No hay ventas registradas";
                    EsError = false;
                    return;
                }

                foreach (var venta in ventasRegistradas)
                {
                    Ventas.Add(venta);
                }

                Mensaje = string.Empty;
                EsError = false;
            }
            catch (Exception ex)
            {
                Mensaje = "No fue posible consultar las ventas. " + ex.Message;
                EsError = true;
            }
        }
    }
}