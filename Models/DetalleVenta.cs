namespace SistemaVentas.Models
{
    public class DetalleVenta
    {
        // Número identificador del detalle de la venta
        public int DetalleVentaId { get; set; }

        // Número identificador de la venta
        public int VentaId { get; set; } 

        // Identificador del producto
        public int ProductoId { get; set; }

        // Cantidad del producto
        public float CantidadProducto { get; set; }

        // Valor unitario del producto
        public float ValorUnitarioProducto { get; set; }

        // Subtotal dada la cantidad de productos por el valor unitario
        public float SubTotal {get; set; }

    }
}