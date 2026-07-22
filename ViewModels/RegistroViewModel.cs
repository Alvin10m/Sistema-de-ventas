using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using BCrypt.Net;
using Npgsql;
using SistemaVentas.Data;
using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using SistemaVentas.Views;
using SistemaVentas.Helpers;
using Microsoft.EntityFrameworkCore.Storage.Internal;

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
                @"INSERT INTO usuario (nombre, contrasena)
                  VALUES (@usuario, @contrasena)
                  RETURNING id, rol, activo",
                conexion
            );

            comando.Parameters.AddWithValue("usuario", NombreUsuario);
            comando.Parameters.AddWithValue("contrasena", contrasenaCifrada);

            using var lector = comando.ExecuteReader();

            if (!lector.Read())
            {
                MensajeError = "No se pudo obtener la información del usuario registrado";
                HayError = true;
                return;
            }

            int idUsuario = lector.GetInt32(0);
            string rol = lector.GetString(1);
            bool activo = lector.GetBoolean(2);

            // Guardar la sesión del usuario recién registrado
            LoginViewModel.UsuarioActual = NombreUsuario;

            SesionUsuario.Id = idUsuario;
            SesionUsuario.Nombre = NombreUsuario;
            SesionUsuario.Rol = rol;
            SesionUsuario.Activo = activo;
            SesionUsuario.Permisos.Clear();
            
            // Abrir el menú principal
            if(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var ventanaRegistro = desktop.MainWindow;

                var menuPrincipal = new MenuPrincipalWindow
                {
                    DataContext = new MenuPrincipalViewModel()
                };
                
                desktop.MainWindow = menuPrincipal;
                menuPrincipal.Show();
                ventanaRegistro?.Close();
            }
        }

        [RelayCommand]
        private void Cancelar()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }
    }
}