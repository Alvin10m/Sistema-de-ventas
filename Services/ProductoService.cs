using Npgsql;
using SistemaVentas.Data;
using SistemaVentas.ViewModels;

namespace SistemaVentas.Services
{
    public class ProductoService
    {
        // Conexión a la base de datos
        private readonly ConexionBD conexionBD = new ConexionBD();

        public DetalleVentaItem? BuscarProducto(string busqueda)
        {
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            string sql = @"
                SELECT id, codigo, nombre, precio, stock, porcentaje_descuento, aplica_itbis
                FROM productos
                WHERE activo = TRUE
                AND (codigo = @busqueda OR nombre ILIKE @nombre)
                LIMIT 1;";

            using var comando = new NpgsqlCommand(sql, conexion);

            comando.Parameters.AddWithValue("busqueda", busqueda);
            comando.Parameters.AddWithValue("nombre", "%" + busqueda + "%");

            using var lector = comando.ExecuteReader();

            if (!lector.Read())
                return null;
            
            return new DetalleVentaItem
            {   
                IdProductoBD =(int)lector["id"],
                IdProducto = lector["codigo"].ToString()!,
                Producto = lector["nombre"].ToString()!,
                Cantidad = 1,
                PrecioUnitario = (decimal)lector["precio"],
                Stock = (decimal)lector["stock"],
                PorcentajeDescuento = (decimal)lector["porcentaje_descuento"],
                AplicaItbis = (bool)lector["aplica_itbis"],
                Descuento = 0,
                Subtotal = 0

            };
        }
    }
}