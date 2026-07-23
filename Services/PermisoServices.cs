using System;
using System.Linq;
using Npgsql;
using SistemaVentas.Data;
using SistemaVentas.Helpers;

namespace SistemaVentas.Services
{
    public static class PermisoService
    {
        public static void CargarPermisos(int idUsuario)
        {
            // Limpiar los permisos de una sesión anterior.
            SesionUsuario.Permisos.Clear();

            var conexionBD = new ConexionBD();

            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            using var comando = new NpgsqlCommand(
                @"SELECT p.nombre
                  FROM permiso AS p
                  INNER JOIN usuario_permiso AS up
                      ON up.id_permiso = p.id
                  WHERE up.id_usuario = @idUsuario",
                conexion);

            comando.Parameters.AddWithValue("idUsuario", idUsuario);
            
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                string nombrePermiso = lector.GetString(0);

                SesionUsuario.Permisos.Add(nombrePermiso);
            }
        }

        public static bool TienePermiso(string nombrePermiso)
        {
            if(string.IsNullOrWhiteSpace(nombrePermiso))
            {
                return false;
            }

            return SesionUsuario.Permisos.Any(
                permiso => permiso.Equals(
                    nombrePermiso, 
                    StringComparison.OrdinalIgnoreCase));
        }
    }
}