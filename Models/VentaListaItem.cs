using System;

namespace SistemaVentas.Models
{
    public class VentaListaItem
    {
        // Identificador de la venta
        public int IdVenta { get; set; }

        // Fecha de la venta 
        public DateTime Fecha { get; set; }

        // Hora de la venta
        public TimeSpan Hora { get; set; }

        // Usuario que realizó la ventra
        public string Usuario { get; set; } = string.Empty;

        // Monto total de la venta
        public decimal Total { get; set; }
    }
}