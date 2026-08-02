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

        public DetallesVentaViewModel()
        {
            ventaService = new VentaService();
        }

        // Buscar una venta por el ID ingresado por el usuario
        [RelayCommand]
        private void BuscarVenta()
        {
            Mensaje = string.Empty;
            EsError = false;
            VentaEncontrada = null;
            Productos.Clear();

            // Validar que el ID sea un número entero y mayor que 0
            if (!int.TryParse(IdVentaBusqueda, out int idVenta) || idVenta <= 0)
            {
                Mensaje = "Debe ingresar un ID de venda válido.";
                EsError = true;
                return;
            }
            try
            {
                VentaConsulta? venta = ventaService.BuscarVentaPorId(idVenta);

                // Verificar si la venta existe
                if (venta is null)
                {
                    Mensaje = "Venta no encontrada";
                    EsError = true;
                    return;
                }
                // Guardar los datos generales de la venta
                VentaEncontrada = venta;

                // Limpiar los datos de una búsquda anterior
                Productos.Clear();

                foreach (var producto in venta.Productos)
                {
                    Productos.Add(producto);
                }

                Mensaje = string.Empty;
                EsError = false;

                // Cargar los productos pertenecientes a la venta
                foreach (var producto in venta.Productos)
                {
                    Productos.Add(producto);
                }

                EsError = false;
            }
            catch (Exception ex)
            {
                Mensaje = "No fue posible consultar los detalles de la venta. " + ex.Message;
                EsError = true;
            }
        }

        // ID escrito por el usuario para buscar la venta.
        [ObservableProperty]
        private string idVentaBusqueda = string.Empty;

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