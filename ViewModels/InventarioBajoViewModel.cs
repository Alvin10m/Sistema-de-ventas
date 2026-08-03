using CommunityToolkit.Mvvm.ComponentModel;
using SistemaVentas.Models;
using SistemaVentas.Services;
using System;
using System.Collections.ObjectModel;

namespace SistemaVentas.ViewModels
{
    public partial class InventarioBajoViewModel : ObservableObject
    {
        private readonly ProductoService productoService;

        // Lista de productos con inventario bajo
        public ObservableCollection<InventarioItem> Productos { get; } = new();

        // Mensaje de aviso o error
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Indicar si el mensaje corresponde a un error
        [ObservableProperty]
        private bool esError;

        public InventarioBajoViewModel()
        {
            productoService = new ProductoService();

            CargarInventarioBajo();
        }

        // Consultar los productos con inventario bajo
        private void CargarInventarioBajo()
        {
            try
            {
                Productos.Clear();

                var productos = productoService.ObtenerInventarioBajo();

                if (productos.Count == 0)
                {
                    Mensaje = "No existen productos con inventario bajo.";
                    EsError = false;
                    return;
                }

                foreach (var producto in productos)
                {
                    Productos.Add(producto);
                }

                Mensaje = string.Empty;
                EsError = false;
            }
            catch (Exception ex)
            {
                Mensaje = "No fue posible consultar el inventario bajo. " + ex.Message;
                EsError = true;
            }
        }
    }
}