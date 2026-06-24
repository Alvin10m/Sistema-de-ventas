using System;

namespace SistemaVentas.Models
{
    public class Venta
    {
        // Identificador de la venta
        public int Id { get; set; }

        // Fecha de la venta
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Método de pago
        public string MetodoPago { get; set; } = string.Empty;

        // Ingreso generado por venta
        public float Total { get; set; }

    }
}