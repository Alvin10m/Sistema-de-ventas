namespace SistemaVentas.Models
{
    public class DetalleVentaConsulta
    {
        // Nombre del producto
        public string Producto { get; set; } = string.Empty;

        // Cantidad vendida
        public decimal Cantidad { get; set; }

        // precio unitario
        public decimal PrecioUnitario { get; set; }

        // Descuento aplicado al producto
        public decimal Descuento { get; set; }

        // ITBIS aplicado al producto
        public decimal Itbis { get; set; }

        //Subtotal del producto
        public decimal Subtotal { get; set; }
    }
}