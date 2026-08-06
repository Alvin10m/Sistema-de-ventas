namespace SistemaVentas.Models
{
    // Clase que representa un usuario del sistema
    public class Usuario
    {
        // Identificador único del usuario
        public int Id { get; set; }

        // Código visible del usuario
        public string Codigo { get; set; } = string.Empty;


        // Nombre con el que el usuario inicia sesión
        public string NombreUsuario { get; set; } = string.Empty;

        // Contraseña encriptada del usuario
        public string Contrasena { get; set; } = string.Empty;

        // Rol del usuario
        public string Rol { get; set; } = string.Empty;

        // Indicar si el usuario está activo
        public bool Activo { get; set;} = true;
    }
}