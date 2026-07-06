using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Threading;

namespace SistemaVentas.ViewModels
{
    public partial class PanelVentasViewModel : ObservableObject
    {
        [ObservableProperty]
        private string fecha = "";

        [ObservableProperty]
        private string hora = "";

        [ObservableProperty]
        private string vendedor = "";

        public PanelVentasViewModel()
        {
            // Fecha actual
            Fecha = DateTime.Now.ToString("dd/MM/yyyy");

            // Hora actual
            Hora = DateTime.Now.ToString("HH:mm:ss tt");

            // Temporal, luego vendrá del inicio de sesión
            Vendedor = "Usuario Actual";

            // Actualizar la hora cada segundo
            DispatcherTimer timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromSeconds(1);

            timer.Tick += (sender, e) =>
            {
                Hora = DateTime.Now.ToString("HH:mm:ss tt");
            };

            timer.Start();
        }
    }
}