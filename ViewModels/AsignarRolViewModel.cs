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
    public partial class AsignarRolViewModel : ObservableObject
    {
        private readonly UsuarioService usuarioService;

        // Permisos disponibles
        public ObservableCollection<PermisoSeleccionItem> Permisos { get; } = new();

        // Código visible ingresado para buscar el usuario
        [ObservableProperty]
        private string codigoUsuario = string.Empty;

        // Nombre del usuario encontrado
        [ObservableProperty]
        private string nombreUsuario = string.Empty;

        // Rol seleccionado
        [ObservableProperty]
        private string rolSeleccionado = string.Empty;

        // Mensaje mostrado al usuario
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Indicar si el mensaje corresponde a un error
        [ObservableProperty]
        private bool esError;

        // ID interno del usuario encontrado
        private int idUsuario;

        public AsignarRolViewModel()
        {
            usuarioService = new UsuarioService();

            foreach (var permiso in usuarioService.ObtenerPermisos())
            {
                Permisos.Add(permiso);
            }
        }

        // Buscar el usuario mediante su código 
        [RelayCommand]
        private void BuscarUsuario()
        {
            if (string.IsNullOrWhiteSpace(CodigoUsuario))
            {
                Mensaje = "Debe Ingresar el código del usuario.";
                EsError = true;
                return;
            }


            try
            {
                var usuario = usuarioService.BuscarUsuarioPorCodigo(CodigoUsuario);

                if (usuario is null)
                {
                    Mensaje = "No existe un usuario con ese código";
                    EsError = true;
                    return;
                }

                idUsuario = usuario.Id;
                NombreUsuario = usuario.NombreUsuario;
                RolSeleccionado = usuario.Rol;

                foreach (var permiso in Permisos)
                {
                    permiso.Seleccionado = usuario.IdsPermisos.Contains(permiso.Id);
                }

                Mensaje = string.Empty;
                EsError = false;

            }
            catch (Exception ex)
            {
                Mensaje = "No fue posible buscar el usuario. " + ex.Message;
                EsError = true;
            }
        }

        // Actualizar el rol y los permisos
        [RelayCommand]
        private void Aceptar()
        {
            if (idUsuario == 0)
            {
                Mensaje = "Debe buscar un usuario.";
                EsError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(RolSeleccionado))
            {
                Mensaje = "Debe seleccionar un rol.";
                EsError = true;
                return;

            }

            List<int> permisosSeleccionados = Permisos
                .Where(p => p.Seleccionado)
                .Select(p => p.Id)
                .ToList();

            if (permisosSeleccionados.Count == 0)
            {
                Mensaje = "Debe seleccionar al menos un permiso.";
                EsError = true;
                return;

            }
            try
            {
                usuarioService.ActualizarRolYPermisos(
                    idUsuario,
                    RolSeleccionado,
                    permisosSeleccionados);
                
                Mensaje = "Rol y permisos actualizados correctamente.";
                EsError = false;

            }
            catch (Exception ex)
            {
                Mensaje = "No fue posible actualizar  el rol y los permisos. " + ex.Message;
                EsError = true;
            }
            
        }
    }
}