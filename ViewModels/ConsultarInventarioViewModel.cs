using CommunityToolkit.Mvvm.ComponentModel;
using SistemaVentas.Models;
using SistemaVentas.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace SistemaVentas.ViewModels
{
    public partial class ConsultarInventarioViewModel : ObservableObject
    {
        // Servicio para consultar los productos
        private readonly ProductoService productoService;
        
        // Productos con stock bajo
        public ObservableCollection<InventarioItem> ProductosStockBajo { get; } = new();
        
        // Productos con stock suficiente

        // Mensaje general relacionado con la consulta de inventario
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Indicar si occurió un error durante la consulta.
        [ObservableProperty]
        private bool esError = false;

        // Indica si no existen productos con stock bajo.
        [ObservableProperty]
        private bool noHayStockBajo;

        // Indica si no existen productos con stock suficiente.
        [ObservableProperty]
        private bool noHayStockSuficiente;
        
        public ObservableCollection<InventarioItem> ProductosStockSuficiente { get; } = new();

        public ConsultarInventarioViewModel()
        {
            productoService = new ProductoService();

            CargarInventario();
        }

        private void CargarInventario()
        {
            try
            {
                ProductosStockBajo.Clear();
                ProductosStockSuficiente.Clear();

                var inventario = productoService.ObtenerInventario();

                if (inventario.Count == 0)
                {
                    Mensaje = "No existen productos registrados en el inventario.";
                    EsError = false;
                    return;
                }

                foreach (var producto in inventario)
                {
                    if (producto.EstadoInventario == "Stock bajo")
                    {
                        ProductosStockBajo.Add(producto);
                    }
                    else
                    {
                        ProductosStockSuficiente.Add(producto);
                    }
                }

                NoHayStockBajo = ProductosStockBajo.Count == 0;
                NoHayStockSuficiente = ProductosStockSuficiente.Count == 0;

                Mensaje = string.Empty;
                EsError = false;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                EsError = true;
            }
        }        
    }
}