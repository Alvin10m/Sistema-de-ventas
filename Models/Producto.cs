namespace SistemaVentas.Models
{
    public class Producto
    {
        // Identificador único del producto
        public int Id { get; set; }

        // Código del producto
        public string Codigo { get; set; } = string.Empty;

        // Nombre del producto
        public string Nombre { get; set; } = string.Empty;

        // Precio del producto
        public float Precio { get; set; }

        // Cantidad en stock del producto
        public float Cantidad { get; set; }

        //Indicar si el producto está activo
        public bool Activo { get; set; } = true;

        // Indica si al producto se le aplica ITBIS
        public bool AplicaItbis { get; set; } 

        // Porcentaje de descuento del producto
        public float PorcentajeDescuento { get; set; }

        // Indicar la categoría del producto
        public int? IdCategoria { get; set; }

        // Categoría del producto
        public string Categoria { get; set; } = string.Empty;

    }
}   
