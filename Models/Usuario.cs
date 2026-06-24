namespace SistemaVentas.Models
{
    // Clase que representa un usuario del sistema
    public class Usuario
    {
        // Identificador único del usuario
        public int Id { get; set; }

        // Nombre con el que el usuario inicia sesión
        public string NombreUsuario { get; set; } = string.Empty;

        // Contraseña encriptada del usuario
        public string Contrasena { get; set; } = string.Empty;
    }
}