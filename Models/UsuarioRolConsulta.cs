using System.Collections.Generic;
namespace SistemaVentas.Models
{
    public class UsuarioRolConsulta
    {
        // Identificador interno del usuario
        public int Id { get; set; }

        // Código visible del usuario
        public string Codigo { get; set; } = string.Empty;

        // Nombre del usuario
        public string NombreUsuario { get; set; } = string.Empty;

        // Rol actual del usuario 
        public string Rol { get; set; } = string.Empty;

        // Permisos actuales del usuario
        public List<int> IdsPermisos { get; set; } = new ();

        // Indicar si el usuario se encuentra activo
        public bool Activo { get; set; }
    }
}