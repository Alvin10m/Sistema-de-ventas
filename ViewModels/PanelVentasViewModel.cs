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
        public decimal Cantidad { get; set; }

        // Precio por unidad
        public decimal PrecioUnitario { get; set; }

        // Descuento aplicado
        public decimal Descuento { get; set; }

        // Subtotal = Cantidad * PrecioUnitario - Descuento
        public decimal Subtotal { get; set; }
    }

    public partial class PanelVentasViewModel : ObservableObject
{
    [ObservableProperty]
    private string fecha = "";

    [ObservableProperty]
    private string hora = "";

    [ObservableProperty]
    private string vendedor = "";

    // Lista de productos en la venta
    [ObservableProperty]
    private ObservableCollection<DetalleVentaItem> productosVenta = new();

    // Subtotal de todos los productos
    [ObservableProperty]
    private decimal subtotalVenta;

    // Total de descuentos aplicados
    [ObservableProperty]
    private decimal descuentoVenta;

    // ITBIS de la venta
    [ObservableProperty]
    private decimal itbisVenta;

    // Total final de la venta
    [ObservableProperty]
    private decimal totalVenta;

    // Código o nombre del producto escrito por el vendedor
    [ObservableProperty]
    private string busquedaProducto = string.Empty;

    // Cantidad del producto a vender
    [ObservableProperty]
    private decimal cantidadProducto = 1;

    // Comentario de la venta
    [ObservableProperty]
    private string comentarioVenta = string.Empty;



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

    // Método para calcular los totales de la venta
    private void CalcularTotales()
    {
        decimal subtotal = 0;
        decimal descuento = 0;

        // Recorrer todos los productos agregados a la venta
        foreach (DetalleVentaItem producto in ProductosVenta)
        {
            subtotal += producto.Subtotal;
            descuento += producto.Descuento;
        }

        // Guardar resultados
        SubtotalVenta = subtotal;
        DescuentoVenta = descuento;

        // Calcular ITBIS (18%)
        ItbisVenta = SubtotalVenta * 0.18m;

        // Total de la venta
        TotalVenta = SubtotalVenta + ItbisVenta;
    }

    // Agragar producto
    public void AgregarProducto(DetalleVentaItem producto)
    {
        // Asignar el número de la fila
        producto.Numero = ProductosVenta.Count + 1;

        // Agregar el producto a la lista
        ProductosVenta.Add(producto);

        //Recalcurlar los totales
        CalcularTotales();
    }

    // Eliminar producto de la lista
    public void EliminarProducto(DetalleVentaItem producto)
        {
            ProductosVenta.Remove(producto);

            // Renumerar las filas
            for (int i = 0; i < ProductosVenta.Count; i++)
            {
                ProductosVenta[i].Numero = i + 1;
            }
            CalcularTotales();
        }


    
}
}