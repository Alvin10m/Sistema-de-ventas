using System.Collections.Generic;

namespace SistemaVentas.Helpers
{
    public static class SesionUsuario
    {
        public static int Id { get; set; }

        public static string Nombre { get; set; } = string.Empty;

        public static string Rol { get; set; } = string.Empty;

        public static bool Activo { get; set; }

        public static List<string> Permisos { get; } = new();


    }
}