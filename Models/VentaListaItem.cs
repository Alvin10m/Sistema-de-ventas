using System;
using System.Data;

namespace SistemaVentas.Models
{
    public class VentaListaItem
    {
        // Identificador de la venta
        public int IdVenta { get; set; }
        
        // Código visible de la venta
        public string CodigoVenta { get; set; } = string.Empty;

        // Fecha de la venta 
        public DateTime Fecha { get; set; }

        // Hora de la venta
        public TimeSpan Hora { get; set; }

        // Hora visible en la pantalla
        public string HoraVisible
        {
            get
            {
                return DateTime.Today.Add(Hora).ToString("hh:mm tt");
            }
        }

        // Usuario que realizó la venta
        public string Usuario { get; set; } = string.Empty;

        // Monto total de la venta
        public decimal Total { get; set; }
    }
}