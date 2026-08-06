using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaVentas.Models;
using SistemaVentas.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SistemaVentas.ViewModels
{
    public partial class CrearUsuarioViewModel : ObservableObject
    {
        private readonly UsuarioService usuarioService;

        public CrearUsuarioViewModel()
        {
            usuarioService = new UsuarioService();
            Codigo = usuarioService.GenerarCodigoUsuario();
        }

        // Código generado automáticamente.
        [ObservableProperty]
        private string codigo = string.Empty;

        // Nombre del usuario.
        [ObservableProperty]
        private string nombreUsuario = string.Empty;

        // Contraseña.
        [ObservableProperty]
        private string contrasena = string.Empty;

        // Confirmación de contraseña.
        [ObservableProperty]
        private string confirmacionContrasena = string.Empty;

        // Rol seleccionado.
        [ObservableProperty]
        private string rolSeleccionado = string.Empty;

        // Mensaje de éxito o error.
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Indica si el mensaje es un error.
        [ObservableProperty]
        private bool esError;

        // Permisos disponibles para seleccionar.
        public ObservableCollection<PermisoSeleccionItem> Permisos { get; } = new();

        // Guardar un nuevo usuario.
        [RelayCommand]
        private void GuardarUsuario()
        {
            try
            {
                List<int> permisosSeleccionados = Permisos
                    .Where(p => p.Seleccionado)
                    .Select(p => p.Id)
                    .ToList();

                Usuario usuario = new Usuario
                {
                    Codigo = Codigo,
                    NombreUsuario = NombreUsuario,
                    Contrasena = Contrasena,
                    Rol = RolSeleccionado,
                    Activo = true
                };

                string? error = usuarioService.ValidarUsuario(
                    usuario,
                    ConfirmacionContrasena,
                    permisosSeleccionados);

                if (error is not null)
                {
                    Mensaje = error;
                    EsError = true;
                    return;
                }

                usuarioService.GuardarUsuario(
                    usuario,
                    permisosSeleccionados);

                Mensaje = "Usuario creado correctamente.";
                EsError = false;

                LimpiarFormulario();
                Codigo = usuarioService.GenerarCodigoUsuario();
            }
            catch (Exception ex)
            {
                Mensaje = "No fue posible crear el usuario. " + ex.Message;
                EsError = true;
            }
        }

        // Limpiar los campos sin guardar.
        [RelayCommand]
        private void LimpiarFormulario()
        {
            NombreUsuario = string.Empty;
            Contrasena = string.Empty;
            ConfirmacionContrasena = string.Empty;
            RolSeleccionado = string.Empty;

            foreach (var permiso in Permisos)
            {
                permiso.Seleccionado = false;
            }
        }
    }
}