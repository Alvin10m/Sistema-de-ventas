using CommunityToolkit.Mvvm.ComponentModel;

namespace SistemaVentas.Models
{
    public partial class PermisoSeleccionItem : ObservableObject
    {
        // Identificador del permiso
        public int Id { get; set; }

        // Nombre del permiso
        public string Nombre { get; set; } = string.Empty;

        // Indica si el permiso está seleccionado
        [ObservableProperty]
        private bool seleccionado;
    }
}