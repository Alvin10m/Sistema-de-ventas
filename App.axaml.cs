using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SistemaVentas.ViewModels;
using SistemaVentas.Views;
using Npgsql;
using SistemaVentas.Data;

namespace SistemaVentas;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            bool existeUsuario;

            var conexionBD = new ConexionBD();

            using (var conexion = conexionBD.ObtenerConexion())
            {
                conexion.Open();

                using var comando = new NpgsqlCommand("SELECT COUNT(*) FROM usuario", conexion);

                existeUsuario = Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }

            if (!existeUsuario)
            {
                desktop.MainWindow = new RegistroWindow
                {
                    DataContext = new RegistroViewModel()
                };
            }
            else
            {
                desktop.MainWindow = new LoginWindow
                {
                    DataContext = new LoginViewModel()
                };
            }
        }
        base.OnFrameworkInitializationCompleted();
    }
}