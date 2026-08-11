using CommunityToolkit.Mvvm.ComponentModel;
using SistemaVentas.Models;
using SistemaVentas.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace SistemaVentas.ViewModels
{
    public partial class AgregarProductoViewModel : ObservableObject
    {
        private readonly ProductoService productoService;

        public AgregarProductoViewModel()
        {
            productoService = new ProductoService();
            InicializarFormulario(); 
        }

        private void InicializarFormulario()
        {
            Codigo = productoService.GenerarCodigoProducto();

            Categorias.Clear();
            foreach (var categoria in productoService.ObtenerCategorias())
            {
                Categorias.Add(categoria);
            }
        
        }

        // Limpiar los campos editables del formulario
        [RelayCommand]
        public void LimpiarFormulario()
        {
            Nombre = string.Empty;
            Precio = null;
            Stock = null;
            StockMinimo = null;
            AplicaItbis = false;
            PorcentajeDescuento = null;
            CategoriaSeleccionada = null;
            Mensaje = string.Empty;
            EsError = false;
        }

        // Guardar un nuevo producto en la base de datos
        [RelayCommand]
        private void GuardarProducto()
        {   
            if (CategoriaSeleccionada is null)
            {
                Mensaje = "Debe seleccionar una categoría";
                EsError = true;
                return;
            }

            Producto producto = new Producto
            {
                Codigo = Codigo,
                Nombre = Nombre,
                Precio = Precio ?? 0,
                Cantidad = Stock ?? 0,
                StockMinimo = StockMinimo ?? 0,
                AplicaItbis = AplicaItbis,
                PorcentajeDescuento = PorcentajeDescuento ?? 0,
                IdCategoria = CategoriaSeleccionada!.Id
            };

            try
            {
                // Comprobar que los datos del producto cumplan las reglas del sistema
                string? errorValidacion = productoService.ValidarProducto(producto);

                if (errorValidacion != null)
                {
                    Mensaje = errorValidacion;
                    EsError = true;
                    return;
                }

                // Insertar el producto en la base de datos
                productoService.AgregarProducto(producto);

                // Mostrar mensaje de éxito
                Mensaje = "Producto guardado correctamente";
                EsError = false;

                // Preparar el formulario para registrar otro producto
                LimpiarFormulario();
                Codigo = productoService.GenerarCodigoProducto();
            }
            catch (Exception ex)
            {
                // Mostrar el motivo del error
                Mensaje = ex.Message;
                EsError = true;
            }
        }   

        // Generar código de producto de manera automática
        [ObservableProperty]
        private string codigo = string.Empty;

        // Nombre del producto
        [ObservableProperty]
        private string nombre = string.Empty;

        // Precio de venta.
        [ObservableProperty]
        private decimal? precio;

        // Cantidad disponible en el inventario
        [ObservableProperty]
        private decimal? stock;

        // Cantidad mínima antes de considerar que el producto tiene stock bajo
        [ObservableProperty]
        private decimal? stockMinimo;

        // Indicar si el producto paga ITBIS
        [ObservableProperty]
        private bool aplicaItbis;

        // Porcentaje de descuento.
        [ObservableProperty]
        private decimal? porcentajeDescuento;
        
        // Categoría seleccionada
        [ObservableProperty]
        private Categoria? categoriaSeleccionada;

        // Mensaje para mostar al usuario luego de haber agragado un producto
        [ObservableProperty]
        private string mensaje = string.Empty;

        // Indicar si el mensaje corresponde a un error.
        [ObservableProperty]
        private bool esError;

        // Lista de categorías
        public ObservableCollection<Categoria> Categorias { get; } = new();
    }
}