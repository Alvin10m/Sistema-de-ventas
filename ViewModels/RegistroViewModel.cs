using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using SistemaVentas.Views;
using SistemaVentas.Helpers;
using SistemaVentas.Models;
using SistemaVentas.Services;

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
            // Limpiar errores anteriores
            HayError = false;
            MensajeError = string.Empty;

            // Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(NombreUsuario) ||
                string.IsNullOrWhiteSpace(Contrasena) ||
                string.IsNullOrWhiteSpace(ConfirmarContrasena))
            {
                MensajeError = "Todos los campos son obligatorios.";
                HayError = true;
                return;
            }

            // Validar requisitos de la contraseña
            if (!Regex.IsMatch(
                Contrasena,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                MensajeError =
                    "La contraseña debe tener mayúscula, minúscula, número, carácter especial y mínimo 8 caracteres.";

                HayError = true;
                return;
            }

            // Validar confirmación de contraseña
            if (Contrasena != ConfirmarContrasena)
            {
                MensajeError = "La contraseña no coincide con la confirmación.";
                HayError = true;
                return;
            }

            try
            {
                // Crear el servicio de usuarios
                var usuarioService = new UsuarioService();

                // Generar el código del usuario
                string codigoUsuario =
                    usuarioService.GenerarCodigoUsuario();

                // Obtener todos los permisos registrados
                var permisosDisponibles =
                    usuarioService.ObtenerPermisos();

                // Obtener los ID de los permisos
                var idsPermisos = permisosDisponibles
                    .Select(p => p.Id)
                    .ToList();

                // Verificar que existan permisos
                if (idsPermisos.Count == 0)
                {
                    MensajeError =
                        "No existen permisos registrados en el sistema.";

                    HayError = true;
                    return;
                }

                // Crear el primer usuario como administrador
                var usuario = new Usuario
                {
                    Codigo = codigoUsuario,
                    NombreUsuario = NombreUsuario,
                    Contrasena = Contrasena,
                    Rol = "Administrador",
                    Activo = true
                };

                // Guardar el usuario y asignarle todos los permisos
                usuarioService.GuardarUsuario(
                    usuario,
                    idsPermisos
                );

                // Buscar el usuario para recuperar su ID interno
                var usuarioRegistrado =
                    usuarioService.BuscarUsuarioPorCodigo(
                        codigoUsuario
                    );

                if (usuarioRegistrado is null)
                {
                    MensajeError =
                        "No se pudo obtener la información del usuario registrado.";

                    HayError = true;
                    return;
                }

                // Guardar los datos del usuario en la sesión
                LoginViewModel.UsuarioActual =
                    usuarioRegistrado.NombreUsuario;

                SesionUsuario.Id =
                    usuarioRegistrado.Id;

                SesionUsuario.Nombre =
                    usuarioRegistrado.NombreUsuario;

                SesionUsuario.Rol =
                    usuarioRegistrado.Rol;

                SesionUsuario.Activo =
                    usuarioRegistrado.Activo;

                // Cargar sus permisos en la sesión
                PermisoService.CargarPermisos(
                    usuarioRegistrado.Id
                );

                // Abrir el menú principal
                if (Application.Current?.ApplicationLifetime
                    is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var ventanaRegistro =
                        desktop.MainWindow;

                    var menuPrincipal =
                        new MenuPrincipalWindow
                        {
                            DataContext =
                                new MenuPrincipalViewModel()
                        };

                    desktop.MainWindow =
                        menuPrincipal;

                    menuPrincipal.Show();

                    ventanaRegistro?.Close();
                }
            }
            catch (Exception ex)
            {
                MensajeError =
                    "No fue posible registrar el usuario. " +
                    ex.Message;

                HayError = true;
            }
        }

        [RelayCommand]
        private void Cancelar()
        {
            if (Application.Current?.ApplicationLifetime
                is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }
    }
}
