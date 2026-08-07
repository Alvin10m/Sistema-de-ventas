using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaVentas.Services;
using System;

namespace SistemaVentas.ViewModels
{
    public partial class CambiarContrasenaViewModel : ObservableObject
    {
        private readonly UsuarioService usuarioService;

        public CambiarContrasenaViewModel()
        {
            usuarioService = new UsuarioService();
        }

        // Código visible del usuario
        [ObservableProperty]
        private string codigoUsuario = string.Empty;

        // Contraseña actual
        [ObservableProperty]
        private string contrasenaActual = string.Empty;

        // Nueva contraseña
        [ObservableProperty]
        private string nuevaContrasena = string.Empty;

        // Confirmación de la nueva contraseña
        [ObservableProperty]
        private string confirmarNuevaContrasena = string.Empty;

        // Mensaje mostrado al usuario
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Indicar si el mensaje corresponde a un error
        [ObservableProperty]
        private bool esError;

        // Guardar el cambio de contraseña
        [RelayCommand]
        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(CodigoUsuario))
            {
                Mensaje = "Debe ingresar el código del usuario.";
                EsError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(ContrasenaActual))
            {
                Mensaje = "Debe ingresar la contraseña actual.";
                EsError = true;
                return;
            }

            try
            {
                // Comprobar que el usuario exista
                var usuario =
                    usuarioService.BuscarUsuarioPorCodigo(CodigoUsuario);

                if (usuario is null)
                {
                    Mensaje = "No existe un usuario con ese código.";
                    EsError = true;
                    return;
                }

                // Comprobar la contraseña actual
                bool contrasenaCorrecta =
                    usuarioService.VerificarContrasenaActual(
                        CodigoUsuario,
                        ContrasenaActual);

                if (!contrasenaCorrecta)
                {
                    Mensaje = "La contraseña actual es incorrecta.";
                    EsError = true;
                    return;
                }

                // Validar la nueva contraseña
                string? errorContrasena =
                    usuarioService.ValidarContrasena(NuevaContrasena);

                if (errorContrasena is not null)
                {
                    Mensaje = errorContrasena;
                    EsError = true;
                    return;
                }

                // Comprobar la confirmación
                if (NuevaContrasena != ConfirmarNuevaContrasena)
                {
                    Mensaje = "Las contraseñas no coinciden.";
                    EsError = true;
                    return;
                }

                // Guardar la nueva contraseña
                usuarioService.CambiarContrasena(
                    CodigoUsuario,
                    NuevaContrasena);

                Mensaje = "Contraseña actualizada correctamente.";
                EsError = false;

                LimpiarCampos();
            }
            catch (Exception ex)
            {
                Mensaje =
                    "No fue posible cambiar la contraseña. " +
                    ex.Message;

                EsError = true;
            }
        }

        // Limpiar los campos
        [RelayCommand]
        private void LimpiarCampos()
        {
            CodigoUsuario = string.Empty;
            ContrasenaActual = string.Empty;
            NuevaContrasena = string.Empty;
            ConfirmarNuevaContrasena = string.Empty;
        }
    }
}