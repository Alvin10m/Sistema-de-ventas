using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaVentas.Models;
using SistemaVentas.Services;
using System;

namespace SistemaVentas.ViewModels
{
    public partial class ActivarDesactivarUsuarioViewModel : ObservableObject
    {
        private readonly UsuarioService usuarioService;

        public ActivarDesactivarUsuarioViewModel()
        {
            usuarioService = new UsuarioService();
        }

        // Código visible del usuario
        [ObservableProperty]
        private string codigoUsuario = string.Empty;

        // Nombre del usuario encontrado
        [ObservableProperty]
        private string nombreUsuario = string.Empty;

        // Estado actual del usuario
        [ObservableProperty]
        private bool usuarioActivo;

        // Mensaje mostrado al administrador
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Indica si el mensaje corresponde a un error
        [ObservableProperty]
        private bool esError;

        // Usuario encontrado en la búsqueda
        private UsuarioRolConsulta? usuarioEncontrado;

        // Buscar usuario por su código visible
        [RelayCommand]
        private void BuscarUsuario()
        {
            if (string.IsNullOrWhiteSpace(CodigoUsuario))
            {
                Mensaje = "Debe ingresar el código del usuario.";
                EsError = true;
                return;
            }

            try
            {
                usuarioEncontrado =
                    usuarioService.BuscarUsuarioPorCodigo(CodigoUsuario);

                if (usuarioEncontrado is null)
                {
                    NombreUsuario = string.Empty;
                    Mensaje = "No hay usuario con ese código.";
                    EsError = true;
                    return;
                }

                NombreUsuario = usuarioEncontrado.NombreUsuario;
                UsuarioActivo = usuarioEncontrado.Activo;

                Mensaje = string.Empty;
                EsError = false;
            }
            catch (Exception ex)
            {
                Mensaje =
                    "No fue posible buscar el usuario. " + ex.Message;
                EsError = true;
            }
        }

        // Activar el usuario encontrado
        [RelayCommand]
        private void ActivarUsuario()
        {
            if (usuarioEncontrado is null)
            {
                Mensaje = "Debe buscar un usuario.";
                EsError = true;
                return;
            }

            if (usuarioEncontrado.Activo)
            {
                Mensaje = "El usuario se encuentra activo.";
                EsError = false;
                return;
            }

            try
            {
                usuarioService.CambiarEstadoUsuario(
                    usuarioEncontrado.Codigo,
                    true);

                usuarioEncontrado.Activo = true;
                UsuarioActivo = true;

                Mensaje = "Usuario activado correctamente.";
                EsError = false;
            }
            catch (Exception ex)
            {
                Mensaje =
                    "No fue posible activar el usuario. " + ex.Message;
                EsError = true;
            }
        }

        // Desactivar el usuario encontrado
        [RelayCommand]
        private void DesactivarUsuario()
        {
            if (usuarioEncontrado is null)
            {
                Mensaje = "Debe buscar un usuario.";
                EsError = true;
                return;
            }

            if (!usuarioEncontrado.Activo)
            {
                Mensaje = "Este usuario se encuentra inactivo.";
                EsError = false;
                return;
            }

            try
            {
                usuarioService.CambiarEstadoUsuario(
                    usuarioEncontrado.Codigo,
                    false);

                usuarioEncontrado.Activo = false;
                UsuarioActivo = false;

                Mensaje = "Usuario desactivado correctamente.";
                EsError = false;
            }
            catch (Exception ex)
            {
                Mensaje =
                    "No fue posible desactivar el usuario. " + ex.Message;
                EsError = true;
            }
        }
    }
}