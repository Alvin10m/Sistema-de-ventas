using Npgsql;
namespace SistemaVentas.Data
{
    public class ConexionBD
    {
        // Cadena de conexión para indicar cómo encontrar la base de datos
        private readonly string cadenaConexion =
            "Host=localhost;Port=5432;Database=sistema_ventas;Username=postgres;Password=Alvin102023";
        
        // Devuelve la conexión lista para usar en cualquier parte del programa
        public NpgsqlConnection ObtenerConexion()
        {
            return new NpgsqlConnection(cadenaConexion);
        }
    }
}