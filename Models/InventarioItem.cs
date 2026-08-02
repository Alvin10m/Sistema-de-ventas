namespace SistemaVentas.Models
{
    public class InventarioItem
    {
        // Código visible del producto
        public string Codigo { get; set; } = string.Empty;

        // Nombre del producto
        public string Nombre { get; set; } = string.Empty;

        // Cantidad disponible actualmente
        public decimal Stock { get; set; } 

        // Estado calculado del inventario
        public string EstadoInventario { get; set; } = string.Empty;
    }
}