using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using BCrypt.Net;
using Npgsql;
using SistemaVentas.Data;
using System;

namespace SistemaVentas.ViewModels
{
    public partial class RegistroViewModel : ObservableObject
    {
        [ObservableProperty]
        private string nombreUsuario = string.Empty;
        [ObservableProperty]
        private string contrasena = string.Empty;
        [ObservableProperty]
        private string confirmarContrasena = string.Empty;
        [ObservableProperty]
        private string mensajeError = string.Empty;
        [ObservableProperty]
        private bool hayError = false;

        [RelayCommand]
        private void Registrar()
        {
            // Limpieza del error anterior antes de validar
            HayError = false;
            MensajeError = string.Empty;

            // Validación 1: verificar que ningún campo esté vacío
            if (string.IsNullOrEmpty(NombreUsuario) ||
                string.IsNullOrEmpty(Contrasena) ||
                string.IsNullOrEmpty(ConfirmarContrasena))
            {
                MensajeError = "Todos los campos son obligatorios";
                HayError = true;
                return;
            }

            // Validación 2: verificar que la contraseña tenga caracteres especiales
            if (!Regex.IsMatch(Contrasena, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                MensajeError = "La contraseña debe tener mayúscula, carácter especial y mínimo 8 caracteres.";
                HayError = true;
                return;
            }

            // Validación 3: verificar que las contraseñas coincidan
            if (Contrasena != ConfirmarContrasena)
            {
                MensajeError = "La contraseña no coincide con la confirmación.";
                HayError = true;
                return;
            }

            // Cifrar la contraseña antes de guardarla
            string contrasenaCifrada = BCrypt.Net.BCrypt.HashPassword(Contrasena);

            // Conectar con la base de datos y guardar el usuario
            var conexionBD = new ConexionBD();
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            using var verificar = new NpgsqlCommand(
            "SELECT COUNT(*) FROM usuario WHERE nombre = @nombre",
            conexion);

            verificar.Parameters.AddWithValue("nombre", NombreUsuario);

            long cantidad = (long)verificar.ExecuteScalar()!;

            // Verificar si el usuario existe en la base de datos
            if (cantidad > 0)
            {
                MensajeError = "Ese nombre de usuario ya existe.";
                HayError = true;
                return;
            }


            using var comando = new NpgsqlCommand(
                "INSERT INTO usuario (nombre, contrasena) VALUES (@usuario, @contrasena)",
                conexion);

            comando.Parameters.AddWithValue("usuario", NombreUsuario);
            comando.Parameters.AddWithValue("contrasena", contrasenaCifrada);


            comando.ExecuteNonQuery();

            MensajeError = "Usuario registrado correctamente.";
            HayError = false;
        }

        [RelayCommand]
        private void Cancelar()
        {
            NombreUsuario = string.Empty;
            Contrasena = string.Empty;
            ConfirmarContrasena = string.Empty;
            MensajeError = string.Empty;
            HayError = false;
        }
    }
}