using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaVentas.Models;
using System.Collections.ObjectModel;
using System;
using SistemaVentas.Services;

namespace SistemaVentas.ViewModels
{
    public partial class DetallesVentaViewModel : ObservableObject
    {
        private readonly VentaService ventaService;

        public DetallesVentaViewModel()
        {
            ventaService = new VentaService();
        }

        // ID escrito por el usuario para buscar la venta.
        [ObservableProperty]
        private string idVentaBusqueda = string.Empty;

        // Venta encontrada en la base de datos
        [ObservableProperty]
        private VentaConsulta? ventaEncontrada;

        // Productos de la venta encontrada
        public ObservableCollection<DetalleVentaConsulta> Productos { get; } = new();

        // Mensajede éxito, aviso o error.
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Indicar si el el mensaje corresponde a un error.
        [ObservableProperty]
        private bool esError;
    }
}