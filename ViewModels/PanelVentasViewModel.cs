using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using SistemaVentas.Services;
using System.Linq;
using SistemaVentas.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace SistemaVentas.ViewModels
{
    // Clase que representa cada fila de la tabla de productos
    public partial class DetalleVentaItem : ObservableObject
    {
        // Número de fila
        public int Numero { get; set; }

        //ID númerico interno del producto en PostgreSQL
        public int IdProductoBD { get; set; }

        // ID del producto
        public string IdProducto { get; set; } = string.Empty;

        // Nombre del producto
        public string Producto { get; set; } = string.Empty;

        // Cantidad vendida
        [ObservableProperty]
        private decimal cantidad;

        // Precio por unidad
        public decimal PrecioUnitario { get; set; }

        // Descuento aplicado
        [ObservableProperty]
        private decimal descuento;

        // Subtotal = Cantidad * PrecioUnitario - Descuento
        [ObservableProperty]
        private decimal subtotal;

        // Cantidad disponible en inventario
        public decimal Stock { get; set; }

        // Porcentaje de descuento del producto
        public decimal PorcentajeDescuento { get; set; }

        // Indicador en caso de que el producto pague ITBIS
        public bool AplicaItbis { get; set; }
    }

    public partial class PanelVentasViewModel : ObservableObject
{
    public string VendedorActual => LoginViewModel.UsuarioActual;

    [ObservableProperty]
    private string fecha = "";

    [ObservableProperty]
    private string hora = "";

    [ObservableProperty]
    private string vendedor = "";

    // Lista de productos en la venta
    [ObservableProperty]
    private ObservableCollection<DetalleVentaItem> productosVenta = new();

    // Subtotal de todos los productos
    [ObservableProperty]
    private decimal subtotalVenta;

    // Total de descuentos aplicados
    [ObservableProperty]
    private decimal descuentoVenta;

    // ITBIS de la venta
    [ObservableProperty]
    private decimal itbisVenta;

    // Total final de la venta
    [ObservableProperty]
    private decimal totalVenta;

    // Código o nombre del producto escrito por el vendedor
    [ObservableProperty]
    private string busquedaProducto = string.Empty;

    // Cantidad del producto a vender
    [ObservableProperty]
    private decimal? cantidadProducto = 1;

    // Comentario de la venta
    [ObservableProperty]
    private string comentarioVenta = string.Empty;

    // Mensajes de errores para el uruario
    [ObservableProperty]
    private string mensajeVenta = string.Empty;

    // Controlar que el mensaje de error esté visible
    [ObservableProperty]
    private bool mostrarMensajeVenta;

    // Indicar si el mensaje es de error
    [ObservableProperty]
    private bool mensajeVentaEsError;

    // Servicio para consultar los productos en la base de datos
    private readonly ProductoService productoService = new ProductoService();

    // Servicio para registrar las ventas
    private readonly VentaService ventaService = new VentaService();


    public PanelVentasViewModel()
    {
        // Fecha actual
        Fecha = DateTime.Now.ToString("dd/MM/yyyy");

        // Hora actual
        Hora = DateTime.Now.ToString("HH:mm:ss tt");

        // Vendedor actual
        Vendedor = VendedorActual;

        // Actualizar la hora cada segundo
        DispatcherTimer timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);

        timer.Tick += (sender, e) =>
        {
            Hora = DateTime.Now.ToString("HH:mm:ss tt");
        };

        timer.Start();
    }

    // Método para calcular los totales de la venta
    private void CalcularTotales()
    {
        decimal subtotal = 0;
        decimal descuento = 0;

        // Recorrer todos los productos agregados a la venta
        foreach (DetalleVentaItem producto in ProductosVenta)
        {
            subtotal += producto.Subtotal;
            descuento += producto.Descuento;
        }

        // Guardar resultados
        SubtotalVenta = subtotal;
        DescuentoVenta = descuento;

        // Calcular ITBIS a los productos que le aplican
        decimal itbis = 0;

        foreach (DetalleVentaItem producto in ProductosVenta)
        {
            if (producto.AplicaItbis)
                {
                    itbis += producto.Subtotal * 0.18m;
                }
        }        
        ItbisVenta = itbis;

        // Total de la venta
        TotalVenta = SubtotalVenta + ItbisVenta;
    }

    // Agragar producto
    public void AgregarProducto(DetalleVentaItem producto)
    {
        // Asignar el número de la fila
        producto.Numero = ProductosVenta.Count + 1;

        // Agregar el producto a la lista
        ProductosVenta.Add(producto);

        //Recalcurlar los totales
        CalcularTotales();
    }

    [RelayCommand]
    private void BuscarYAgregarProducto()
        {
            // Verificar que el usuario haya escrito un código o nombre
            if (string.IsNullOrWhiteSpace(BusquedaProducto))
            {
                MensajeVenta = "Debe ingresar el código o nombre del producto";
                MostrarMensajeVenta = true;
                MensajeVentaEsError = true;
                return;
            }

            // Espacio para el código PostgreSQL (Buscar producto en la base de datos)

            DetalleVentaItem? productoEncontrado = productoService.BuscarProducto(BusquedaProducto);

            // Si no existe el producto
            if (productoEncontrado == null)
            {
                MensajeVenta = "El producto no existe o está inactivo";
                MostrarMensajeVenta = true;
                MensajeVentaEsError = true;
                return;
            }

            // Verificar que la cantidad sea válida
            if (CantidadProducto == null || CantidadProducto <= 0)
            {
                MensajeVenta = "La cantidad debe ser mayor a cero";
                MostrarMensajeVenta = true;
                MensajeVentaEsError = true;
                return;
            }

            decimal cantidad = CantidadProducto.Value;

            // Verificar que exista suficiente cantidad en el inventario
            if (cantidad > productoEncontrado.Stock)
            {
                MensajeVenta = $"Solo hay {productoEncontrado.Stock} unidades disponibles.";
                MostrarMensajeVenta = true;
                MensajeVentaEsError = true;
                return;
            }

            // Verificar si el producto ya fue agregado a la venta
            DetalleVentaItem? productoExistente = ProductosVenta.FirstOrDefault(p => p.IdProducto == productoEncontrado.IdProducto);

            if (productoExistente != null)
            {   
                // Verificar que la cantidad acumulada no supere el stock
                if (productoExistente.Cantidad + cantidad > productoEncontrado.Stock)
                {
                    MensajeVenta = $"La cantidad supera el stock disponible de {productoEncontrado.Stock}";
                    MostrarMensajeVenta = true;
                    MensajeVentaEsError = true;
                    return;
                }
                productoExistente.Cantidad += cantidad;

                productoExistente.Descuento = productoExistente.Cantidad *
                    productoExistente.PrecioUnitario * productoExistente.PorcentajeDescuento / 100;

                productoExistente.Subtotal = productoExistente.Cantidad *
                    productoExistente.PrecioUnitario - productoExistente.Descuento;

                CalcularTotales();

                BusquedaProducto = "";
                CantidadProducto = 1;
                
                return;
            }

            // Agregar producto nuevo
            productoEncontrado.Cantidad= cantidad;

            productoEncontrado.Descuento = productoEncontrado.Cantidad *
                productoEncontrado.PrecioUnitario * productoEncontrado.PorcentajeDescuento / 100;

            productoEncontrado.Subtotal = productoEncontrado.Cantidad *
                productoEncontrado.PrecioUnitario - productoEncontrado.Descuento;

            AgregarProducto(productoEncontrado);

            // Actualizar los totales de la venta
            CalcularTotales();

            // Limpiar la búsqueda
            BusquedaProducto = "";
            CantidadProducto = 1;

            MensajeVenta = "";
            MostrarMensajeVenta = false;
            MensajeVentaEsError = false;
        }

    // Elimina un producto de la venta y actualia la numeración y los totales

    [RelayCommand]
    public void EliminarProducto(DetalleVentaItem producto)
        {
            ProductosVenta.Remove(producto);

            // Renumerar las filas
            for (int i = 0; i < ProductosVenta.Count; i++)
            {
                ProductosVenta[i].Numero = i + 1;
            }
            CalcularTotales();
        }

        // Limpiar todos los datos de la venta actual
        [RelayCommand]
        private void LimpiarVenta()
        {
            ProductosVenta.Clear();

            BusquedaProducto = string.Empty;
            CantidadProducto = 1;
            ComentarioVenta = string.Empty;

            // Recalculará todo poniendo el contenedor con los totales de la venta en 0
            CalcularTotales();

            MensajeVenta = string.Empty;
            MostrarMensajeVenta = false;
            MensajeVentaEsError = false;
        }

        // Finaliza la venta actual
        [RelayCommand]
        private void FinalizarVenta()
        {
            // No permitir guardar una venta sin productos
            if (ProductosVenta.Count == 0)
            {
                MensajeVenta = "No hay productod en la venta";
                MostrarMensajeVenta = true;
                MensajeVentaEsError = true;
                return;
            }
            
            try
            {
                ventaService.GuardarVenta(
                    ProductosVenta,
                    SubtotalVenta,
                    DescuentoVenta,
                    ItbisVenta,
                    TotalVenta,
                    ComentarioVenta,
                    Vendedor
                );

            LimpiarVenta();

            MensajeVenta = "Venta registrada exitosamente";
            MostrarMensajeVenta = true;
            MensajeVentaEsError = false;
        }
        catch (Exception ex)
            {
                MensajeVenta = ex.Message;
                MostrarMensajeVenta = true;
                MensajeVentaEsError = true;
            }
        }
        // Cerrar sesión y volver al Login
        [RelayCommand]
        private void CerrarSesion()
        {
            // Limpiar el usuario que inició sesión
            LoginViewModel.UsuarioActual = string.Empty;

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var ventanaActual = desktop.MainWindow;

                var login = new LoginWindow
                {
                    DataContext = new LoginViewModel()
                };

                desktop.MainWindow = login;

                login.Show();

                ventanaActual?.Close();
            }
        }

        // Volver al menú principal
        [RelayCommand]
        private void VolverAlMenu()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var ventanaActual = desktop.MainWindow;

                var menu = new MenuPrincipalWindow
                {
                    DataContext = new MenuPrincipalViewModel()
                };

                desktop.MainWindow = menu;
                menu.Show();

                ventanaActual?.Close();
            }
        }

    
}
}