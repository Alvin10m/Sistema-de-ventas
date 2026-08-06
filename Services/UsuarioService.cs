using Npgsql;
using SistemaVentas.Data;
using SistemaVentas.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SistemaVentas.Services
{
    public class UsuarioService
    {
        // Conexión a la base de datos
        private readonly ConexionBD conexionBD = new ConexionBD();

        // Generar el código único del usuario
        public string GenerarCodigoUsuario()
        {
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            string sql = @"
                SELECT codigo
                FROM usuario
                WHERE codigo IS NOT NULL
                ORDER BY ID DESC
                LIMIT 1;";
            
            using var comando = new NpgsqlCommand(sql, conexion);

            object? resultado = comando.ExecuteScalar();

            if (resultado is null || resultado == DBNull.Value)
                return "U00001";
            
            string ultimoCodigo = resultado.ToString()!;

            int numero = int.Parse(ultimoCodigo.Substring(1));
            numero++;

            return $"U{numero:D5}";
        }

        // Validar los requisitos de seguridad de la contraseña
        public string? ValidarContrasena(string contrasena)
        {
            if (string.IsNullOrWhiteSpace(contrasena))
                return "La contraseña es obligatoria";

            if (contrasena.Length < 8)
            {
                return "La contraseña debe tener al menos 8 caracteres, " +
                       "una letra mayúscula, una letra minúscula y un carácter especial.";
            }

            if (!Regex.IsMatch(contrasena, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                return "La contraseña debe tener al menos 8 caracteres, " +
                       "una letra mayúscula, una letra minúscula y un carácter especial.";
            }

            return null;
        }

        // Validar los datos necesarios para crear un usuario
        public string? ValidarUsuario(
            Usuario usuario,
            string confirmacionContrasena,
            List<int> permisosSeleccionados)
        {
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
                return "El nombre de usuario es obligatorio";

            if (string.IsNullOrWhiteSpace(usuario.Rol))
                return "Debe seleccionar un rol.";
            
            if (permisosSeleccionados.Count == 0)
                return "Debe seleccionar por lo menos un permiso.";

            string? errorContrasena = ValidarContrasena(usuario.Contrasena);

            if (errorContrasena is not null)
                return errorContrasena;

            if (usuario.Contrasena != confirmacionContrasena)
                return "Las contraseñas no coinciden.";

            return null;
        }

        // Cifrar la contraseña antes de guardarla en DB
        public string CifrarContrasena(string contrasena)
        {
            return BCrypt.Net.BCrypt.HashPassword(contrasena);
        }

        // Guardar usuario y sus permisos en DB
        public void GuardarUsuario(
            Usuario usuario,
            List<int> permisosSeleccionados)
        {
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            using var transaccion = conexion.BeginTransaction();

            try
            {
                string sqlUsuario = @"
                    INSERT INTO usuario
                    (
                        codigo,
                        nombre,
                        contrasena,
                        rol,
                        activo
                    )
                    VALUES
                    (
                        @codigo,
                        @nombre,
                        @contrasena,
                        @rol,
                        @activo
                    )
                    RETURNING id;";
                using var comandoUsuario = new NpgsqlCommand(sqlUsuario, conexion, transaccion);

                comandoUsuario.Parameters.AddWithValue("codigo", usuario.Codigo);
                comandoUsuario.Parameters.AddWithValue("nombre", usuario.NombreUsuario);
                comandoUsuario.Parameters.AddWithValue("contrasena", CifrarContrasena(usuario.Contrasena));
                comandoUsuario.Parameters.AddWithValue("rol", usuario.Rol);
                comandoUsuario.Parameters.AddWithValue("activo", usuario.Activo);

                int idUsuario = (int)comandoUsuario.ExecuteScalar()!;

                string sqlPermiso = @"
                    INSERT INTO usuario_permiso
                    (
                        id_usuario,
                        id_permiso
                    )
                    VALUES
                    (
                        @id_usuario,
                        @id_permiso
                    );";
                
                foreach (int idPermiso in permisosSeleccionados)
                {
                    using var comandoPermiso = new NpgsqlCommand(sqlPermiso, conexion, transaccion);

                    comandoPermiso.Parameters.AddWithValue("id_usuario", idUsuario);

                    comandoPermiso.Parameters.AddWithValue("id_permiso", idPermiso);

                    comandoPermiso.ExecuteNonQuery();
                }

                transaccion.Commit();

            }
            catch
            {
                transaccion.Rollback();
                throw;
            }
        }

        // Obtener todos los permisos registrado
        public List<PermisoSeleccionItem> ObtenerPermisos()
        {
            var permisos = new List<PermisoSeleccionItem>();

            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            string sql = @"
                SELECT id, nombre FROM permiso ORDER BY nombre;";

            using var comando = new NpgsqlCommand(sql, conexion);
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                permisos.Add(new PermisoSeleccionItem
                {
                    Id = lector.GetInt32(0),
                    Nombre = lector.GetString(1)
                });
            }
            return permisos;
        }
    }
}
