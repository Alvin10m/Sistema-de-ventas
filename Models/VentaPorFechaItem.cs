using System;

namespace SistemaVentas.Models
{
    public class VentaPorFechaItem
    {
        // ID de la venta
        public int IdVenta { get; set; }

        // Fecha en que se realizó  la venta
        public DateTime Fecha { get; set; }

        // Hora en que se realizó la venta
        public TimeSpan Hora { get; set; } 

        // Usuario que realizó la venta
        public string Usuario { get; set; } = string.Empty;

        // Total generado de la venta
        public decimal Total { get; set; } 
    }
}