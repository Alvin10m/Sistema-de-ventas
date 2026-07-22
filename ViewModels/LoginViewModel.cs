using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using SistemaVentas.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Npgsql;
using SistemaVentas.Data;
using SistemaVentas.Helpers;

namespace SistemaVentas.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        // usuario que ha iniciado sesión
        public static string UsuarioActual { get; set; } = string.Empty;

        // Campo para el nombre del usuario
        [ObservableProperty]
        private string nombreUsuario = string.Empty;

        // Campo para la contraseña del usuario
        [ObservableProperty]
        private string contrasena = string.Empty;

        // Campo para el mensaje de error
        [ObservableProperty]
        private string mensajeError = string.Empty;

        [ObservableProperty]
        private bool hayError = false;

        // Lógica para el botón de "Iniciar sesión"
        [RelayCommand]
        private void IniciarSesion()
        {
            // Limpiar error anterior 
            HayError = false;
            MensajeError = string.Empty;

            // Validación 1: campos vacíos
            if (string.IsNullOrEmpty(NombreUsuario) || string.IsNullOrEmpty(Contrasena))
            {
                MensajeError = "Todos los campos son obligatorios.";
                HayError = true;
                return;
            }

            // Confirmar que los datos ingresados se encuentran en la base de datos
            var conexionBD = new ConexionBD();
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            // Buscar los datos del usuario en la base de datos
            using var comando = new NpgsqlCommand(
                @"SELECT id, nombre, contrasena, rol, activo
                  FROM usuario WHERE nombre = @nombre", conexion);
            
            comando.Parameters.AddWithValue("nombre", NombreUsuario);

            using var lector = comando.ExecuteReader();

            // Validación 2: verificar que el usuario exista
            if (!lector.Read())
            {
                MensajeError = "El usuario no existe en la base de datos";
                HayError = true;
                return;
            }

            // Obtener los datos del usuario
            int IdUsuario = lector.GetInt32(0);
            string nombre = lector.GetString(1);
            string contrasenaCifrada = lector.GetString(2);
            string rol = lector.GetString(3);
            bool activo = lector.GetBoolean(4);

            //Validación 3: verificar que el usuario esté activo
            if (!activo)
            {
                MensajeError = "Este usuario está desactivado.";
                HayError = true;
                return;
            }

            // Validación 4: verificar la contraseña
            if (!BCrypt.Net.BCrypt.Verify(Contrasena, contrasenaCifrada))
            {
                MensajeError = "La contraseña es incorrecta.";
                HayError = true;
                return;
            }

            // Guardar los datos del usuario que inició sesion
            UsuarioActual = nombre;

            SesionUsuario.Id = IdUsuario;
            SesionUsuario.Nombre = nombre;
            SesionUsuario.Rol = rol;
            SesionUsuario.Activo = activo;
            SesionUsuario.Permisos.Clear();

            // Abrir el menú principal si el inicio de sesión es exitoso
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var ventanaLogin = desktop.MainWindow;

                var menuPrincipal = new MenuPrincipalWindow
                {
                    DataContext = new MenuPrincipalViewModel() 
                };
                
                desktop.MainWindow = menuPrincipal;
                menuPrincipal.Show();
                ventanaLogin?.Close();
            }
            
        }

        // Lógica para el botón "Cancelar"
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