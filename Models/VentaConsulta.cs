using System;
using System.Collections.Generic;

namespace SistemaVentas.Models
{
    public class VentaConsulta
    {
        // Identificador de la venta
        public int IdVenta { get; set; }

        // Código visible de la venta
        public string CodigoVenta { get; set; } = string.Empty;

        // Usuario que realizó la venta
        public string Usuario { get; set; } = string.Empty;

        // Fecha y hora de la venta
        public DateTime FechaHora { get; set; }

        // Subtotal de la venta e ITBIS
        public decimal Subtotal {get; set; }

        // Descuento total aplicado a la venta
        public decimal DescuentoTotal { get; set; }

        //ITBIS total de la venta
        public decimal ItbisTotal { get; set; }

        // Monto total de la venta
        public decimal Total { get; set; }

        // Comentario de la venta
        public string Comentario { get; set; } = string.Empty;

        // Productos que forman parte de la venta
        public List<DetalleVentaConsulta> Productos { get; set; } = new();
    }
}