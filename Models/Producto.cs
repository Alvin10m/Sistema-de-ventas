namespace SistemaVentas.Models
{
    public class Producto
    {
        // Identificador único del producto
        public int Id { get; set; }

    // Nombre del producto
    public string Nombre { get; set; } = string.Empty;

    // Precio del producto
    public float Precio { get; set; }

    // Cantidad en stock del producto
    public float Cantidad { get; set; }

    // Categoría del producto
    public string Categoria { get; set; } = string.Empty;

    }
}   
