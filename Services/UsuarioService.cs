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

        // Buscar un usuario por su código
        public UsuarioRolConsulta? BuscarUsuarioPorCodigo(string codigo)
        {
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            string sqlUsuario = @"
                SELECT id, codigo, nombre, rol, activo
                FROM usuario
                WHERE codigo = @codigo;";

            using var comandoUsuario = new NpgsqlCommand(sqlUsuario, conexion);
            comandoUsuario.Parameters.AddWithValue("codigo", codigo.Trim());

            using var lectorUsuario = comandoUsuario.ExecuteReader();

            if (!lectorUsuario.Read())
                return null;

            var usuario = new UsuarioRolConsulta
            {
                Id = lectorUsuario.GetInt32(0),
                Codigo = lectorUsuario.GetString(1),
                NombreUsuario = lectorUsuario.GetString(2),
                Rol = lectorUsuario.GetString(3),
                Activo = lectorUsuario.GetBoolean(4)
            };

            lectorUsuario.Close();

            string sqlPermisos = @"
                SELECT id_permiso
                FROM usuario_permiso
                WHERE id_usuario = @id_usuario;";

            using var comandoPermisos = new NpgsqlCommand(sqlPermisos, conexion);
            comandoPermisos.Parameters.AddWithValue("id_usuario", usuario.Id);

            using var lectorPermisos = comandoPermisos.ExecuteReader();

            while (lectorPermisos.Read())
            {
                usuario.IdsPermisos.Add(lectorPermisos.GetInt32(0));
            }

            return usuario;
        }

        // Cambiar el estado activo o inactivo de un usuario
        public void CambiarEstadoUsuario(string codigoUsuario, bool nuevoEstado)
        {
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            string sql = @"
                UPDATE usuario
                SET activo = @activo
                WHERE codigo = @codigo;";

            using var comando = new NpgsqlCommand(sql, conexion);

            comando.Parameters.AddWithValue("activo", nuevoEstado);
            comando.Parameters.AddWithValue("codigo", codigoUsuario.Trim());

            comando.ExecuteNonQuery();
        }

        // Verificar que la contraseña actual del usuario sea correcta
        public bool VerificarContrasenaActual(string codigoUsuario, string contrasenaActual)
        {
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            string sql = @"
                SELECT contrasena
                FROM usuario
                WHERE codigo = @codigo
                LIMIT 1;";
            
            using var comando = new NpgsqlCommand(sql, conexion);
            comando.Parameters.AddWithValue("codigo", codigoUsuario.Trim());

            object? resultado = comando.ExecuteScalar();

            if (resultado is null || resultado == DBNull.Value)
                return false;

            string contrasenaCifrada = resultado.ToString()!;

            return BCrypt.Net.BCrypt.Verify(
                contrasenaActual,
                contrasenaCifrada
            );
        }

        // Actualizar la contraseña de un usuario
        public void CambiarContrasena(
            string codigoUsuario,
            string nuevaContrasena)
        {
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            string contrasenaCifrada = CifrarContrasena(nuevaContrasena);

            string sql = @"
                UPDATE usuario
                SET contrasena = @contrasena
                WHERE codigo = @codigo;";

            using var comando = new NpgsqlCommand(sql, conexion);

            comando.Parameters.AddWithValue(
                "contrasena",
                contrasenaCifrada
            );

            comando.Parameters.AddWithValue(
                "codigo",
                codigoUsuario.Trim()
            );

            comando.ExecuteNonQuery();
        }

        // Actualizar rol y permisos de un usuario
        public void ActualizarRolYPermisos(
            int idUsuario,
            string nuevoRol,
            List<int> permisosSeleccionados)
        {
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            using var transaccion = conexion.BeginTransaction();

            try
            {
                string sql = @"
                    UPDATE usuario
                    SET rol = @rol
                    WHERE id = @id_usuario;";

                using var comandoRol = new NpgsqlCommand(sql,conexion, transaccion);

                comandoRol.Parameters.AddWithValue("rol", nuevoRol);
                comandoRol.Parameters.AddWithValue("id_usuario", idUsuario);

                comandoRol.ExecuteNonQuery();

                string sqlEliminarPermisos = @"
                    DELETE FROM usuario_permiso
                    WHERE id_usuario = @id_usuario;";

                using var comandoEliminarPermisos =
                    new NpgsqlCommand(
                        sqlEliminarPermisos,
                        conexion,
                        transaccion
                    );

                comandoEliminarPermisos.ExecuteNonQuery();

                string sqlAgregarPermiso = @"
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
                    using var comandoPermiso =
                        new NpgsqlCommand(
                            sqlAgregarPermiso,
                            conexion,
                            transaccion
                        );
                    
                    comandoPermiso.Parameters.AddWithValue(
                        "id_usuario",
                        idUsuario
                    );

                    comandoPermiso.Parameters.AddWithValue(
                        "id_permiso",
                        idPermiso

                    );
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
        
    }
}
