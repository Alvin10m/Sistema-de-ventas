using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Threading;

namespace SistemaVentas.ViewModels
{
    // Clase que representa cada fila de la tabla de productos
    public class DetalleVentaItem
    {
        // Número de fila
        public int Numero { get; set; }

        // ID del producto
        public string IdProducto { get; set; } = string.Empty;

        // Nombre del producto
        public string Producto { get; set; } = string.Empty;

        // Cantidad vendida
        public double Cantidad { get; set; }

        // Precio por unidad
        public double PrecioUnitario { get; set; }

        // Descuento aplicado
        public double Descuento { get; set; }

        // Subtotal = Cantidad * PrecioUnitario - Descuento
        public double Subtotal { get; set; }
    }

    public partial class PanelVentasViewModel : ObservableObject
    {
        [ObservableProperty]
        private string fecha = "";

        [ObservableProperty]
        private string hora = "";

        [ObservableProperty]
        private string vendedor = "";

        // Lista de productos en la venta — notifica a la tabla cuando cambia
        [ObservableProperty]
        private ObservableCollection<DetalleVentaItem> productosVenta = new();

        public PanelVentasViewModel()
        {
            // Fecha actual
            Fecha = DateTime.Now.ToString("dd/MM/yyyy");

            // Hora actual
            Hora = DateTime.Now.ToString("HH:mm:ss tt");

            // Temporal, luego vendrá del inicio de sesión
            Vendedor = "Usuario Actual";

            // Actualizar la hora cada segundo
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, e) =>
            {
                Hora = DateTime.Now.ToString("HH:mm:ss tt");
            };
            timer.Start();
        }
    }
}