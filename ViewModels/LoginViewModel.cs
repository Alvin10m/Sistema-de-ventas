using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using SistemaVentas.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Npgsql;
using SistemaVentas.Data;

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

            // Búsqueda de datos en la base de datos y devolución de la contraseña del usuario
            using var comando = new NpgsqlCommand(
                "SELECT contrasena FROM usuario WHERE nombre = @nombre", conexion);
            comando.Parameters.AddWithValue("nombre", NombreUsuario);

            var resultado = comando.ExecuteScalar();

            // Validación 2: si el resultado de la consulta es nulo, el usuario no existe
            if (resultado == null)
            {
                MensajeError = "El usuario no existe en la base de datos.";
                HayError = true;
                return;
            }

            // Validación 3: verificar que la contraseña sea correcta
            if (resultado is not string contrasenaCifrada)
            {
                MensajeError = "Error al obtener la contraseña del usuario.";
                HayError = true;
                return;
            }

            if (!BCrypt.Net.BCrypt.Verify(Contrasena, contrasenaCifrada))
            {
                MensajeError = "La contraseña no es correcta.";
                HayError = true;
                return;
            }

            // Si pasa todas las validaciones, guarda el usuario que inició sesión
            UsuarioActual = NombreUsuario;

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
            NombreUsuario = string.Empty;
            Contrasena = string.Empty;
            MensajeError = string.Empty;
            HayError = false;
        }
    }
}