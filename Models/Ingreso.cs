using System;

namespace SistemaVentas.Models
{
    public class Ingresos
    {
        // Número identificador de los ingresos
        public int IdIngresos { get; set; }

        // Total de ingresos generados
        public float TotalIngresos { get; set; }

        // Ingresos en efectivo
        public float TotalEfectivo { get; set; }

        // Ingresos en tarjeta
        public float TotalTarjeta { get; set; }

        // Fecha de actualización
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }
}