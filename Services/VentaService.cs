using SistemaVentas.Data;
using Npgsql;
using SistemaVentas.ViewModels;
using System.Collections.ObjectModel;
using System;
using SistemaVentas.Models;
using System.Collections.Generic;


namespace SistemaVentas.Services
{
    public class VentaService
    {
        private readonly ConexionBD conexionBD = new ConexionBD();

        public void GuardarVenta(
            ObservableCollection<DetalleVentaItem> productos,
            decimal subtotal,
            decimal descuento,
            decimal itbis,
            decimal total,
            string comentario,
            string vendedor)
        {
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            using var transaccion = conexion.BeginTransaction();
            
            try
            {
                string codigoVenta = "V" + DateTime.Now.ToString("mmss");

                string sqlVenta= @"
                    INSERT INTO ventas
                    (codigo, fecha, hora,vendedor, subtotal, descuento, itbis, total, comentario)
                    VALUES
                    (@codigo, @fecha, @hora, @vendedor, @subtotal, @descuento, @itbis, @total, @comentario)
                    RETURNING id;";

                using var comando = new NpgsqlCommand(sqlVenta, conexion, transaccion);

                comando.Parameters.AddWithValue("codigo", codigoVenta);
                comando.Parameters.AddWithValue("fecha", DateTime.Today);
                comando.Parameters.AddWithValue("hora", DateTime.Now.TimeOfDay);
                comando.Parameters.AddWithValue("vendedor", vendedor);
                comando.Parameters.AddWithValue("subtotal", subtotal);
                comando.Parameters.AddWithValue("descuento", descuento);
                comando.Parameters.AddWithValue("itbis", itbis);
                comando.Parameters.AddWithValue("total", total);
                comando.Parameters.AddWithValue("comentario", comentario);

                int idVenta =(int)comando.ExecuteScalar()!;
            
                foreach (var producto in productos)
                {
                    string sqlDetalle = @"
                        INSERT INTO detalleventas
                        (idventa, idproducto, cantidad, preciounitario, descuento, itbis, subtotal)
                        VALUES
                        (@idventa, @idproducto, @cantidad, @precio, @descuento, @itbis, @subtotal);";
                
                    using var comandoDetalle = new NpgsqlCommand(sqlDetalle, conexion, transaccion);

                    comandoDetalle.Parameters.AddWithValue("idventa", idVenta);
                    comandoDetalle.Parameters.AddWithValue("Idproducto", producto.IdProductoBD);
                    comandoDetalle.Parameters.AddWithValue("cantidad", producto.Cantidad);
                    comandoDetalle.Parameters.AddWithValue("precio", producto.PrecioUnitario);
                    comandoDetalle.Parameters.AddWithValue("descuento", producto.Descuento);
                    comandoDetalle.Parameters.AddWithValue("itbis", producto.Itbis);
                    comandoDetalle.Parameters.AddWithValue("subtotal", producto.Subtotal);

                    comandoDetalle.ExecuteNonQuery();

                    string sqlStock = @"
                        UPDATE productos
                        SET stock = stock - @cantidad
                        WHERE id = @idproducto;";

                    using var comandoStock = new NpgsqlCommand(sqlStock, conexion, transaccion);

                    comandoStock.Parameters.AddWithValue("cantidad", producto.Cantidad);
                    comandoStock.Parameters.AddWithValue("idproducto", producto.IdProductoBD);

                    comandoStock.ExecuteNonQuery();
            }


            transaccion.Commit();
        }
        catch
            {
                transaccion.Rollback();
                throw;
            }
        }

        public VentaConsulta? BuscarVentaPorId(int idVenta)
        {
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            // Consultar los datos generales de la venta
            string sqlVenta = @"
                SELECT
                    id,
                    codigo,
                    vendedor,
                    fecha,
                    hora,
                    subtotal,
                    descuento,
                    itbis,
                    total,
                    comentario
                FROM ventas
                WHERE id = @idventa;";

            using var comandoVenta = new NpgsqlCommand(sqlVenta, conexion);

            comandoVenta.Parameters.AddWithValue("idventa", idVenta);

            using var lectorVenta = comandoVenta.ExecuteReader();

            // Si no existe una venta con ese ID, devolver null
            if (!lectorVenta.Read())
            {
                return null;
            }

            DateTime fecha = lectorVenta.GetDateTime(3);
            TimeSpan hora = lectorVenta.GetTimeSpan(4);

            // Crear el objeto con los datos generales de la venta
            var venta = new VentaConsulta
            {
                IdVenta = lectorVenta.GetInt32(0),
                CodigoVenta = lectorVenta.GetString(1),
                Usuario = lectorVenta.GetString(2),
                FechaHora = fecha.Date + hora,
                Subtotal = lectorVenta.GetDecimal(5),
                DescuentoTotal = lectorVenta.GetDecimal(6),
                ItbisTotal = lectorVenta.GetDecimal(7),
                Total = lectorVenta.GetDecimal(8),
                Comentario = lectorVenta.IsDBNull(9)
                    ? string.Empty
                    : lectorVenta.GetString(9)
            };

            lectorVenta.Close();

            // Consultar los productos pertenecientes a la venta
            string sqlDetalles = @"
                SELECT
                    p.codigo,
                    p.nombre,
                    dv.cantidad,
                    dv.preciounitario,
                    dv.descuento,
                    dv.itbis,
                    dv.subtotal
                FROM detalleventas dv
                INNER JOIN productos p
                    ON p.id = dv.idproducto
                WHERE dv.idventa = @idventa
                ORDER BY dv.id;";

            using var comandoDetalles = new NpgsqlCommand(sqlDetalles, conexion);

            comandoDetalles.Parameters.AddWithValue("idventa", idVenta);

            using var lectorDetalles = comandoDetalles.ExecuteReader();

            while (lectorDetalles.Read())
            {
                venta.Productos.Add(new DetalleVentaConsulta
                {
                    CodigoProducto = lectorDetalles.GetString(0),
                    Producto = lectorDetalles.GetString(1),
                    Cantidad = lectorDetalles.GetDecimal(2),
                    PrecioUnitario = lectorDetalles.GetDecimal(3),
                    Descuento = lectorDetalles.GetDecimal(4),
                    Itbis = lectorDetalles.GetDecimal(5),
                    Subtotal = lectorDetalles.GetDecimal(6)
                });
            }

            return venta;
        }
        
        // Obtener todas las ventas registradas 
        public List<VentaListaItem> ObtenerVentas()
        {
            var ventas = new List<VentaListaItem>();

            using var conexion= conexionBD.ObtenerConexion();
            conexion.Open();

            string sql = @"
                SELECT id, codigo, fecha, hora, vendedor, total
                FROM ventas
                ORDER BY fecha DESC, hora DESC;";

            using var comando = new NpgsqlCommand(sql, conexion);
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                ventas.Add(new VentaListaItem
                {
                    IdVenta = lector.GetInt32(0),
                    CodigoVenta = lector.GetString(1),
                    Fecha = lector.GetDateTime(2),
                    Hora = lector.GetTimeSpan(3),
                    Usuario = lector.GetString(4),
                    Total = lector.GetDecimal(5)
                });

            }

            return ventas;
        }

        // Obtener las ventas realizadas en una fecha específica
        public List<VentaPorFechaItem> ObtenerVentasPorFecha(DateTime fecha)
        {
            var ventas = new List<VentaPorFechaItem>();

            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            string sql = @"
            SELECT id, fecha, hora, vendedor, total
            FROM ventas
            WHERE fecha = @fecha
            ORDER BY hora DESC;";

            using var comando = new NpgsqlCommand(sql, conexion);
            comando.Parameters.AddWithValue("fecha", fecha.Date);

            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                ventas.Add(new VentaPorFechaItem
                {
                    IdVenta = lector.GetInt32(0),
                    Fecha = lector.GetDateTime(1),
                    Hora = lector.GetTimeSpan(2),
                    Usuario = lector.GetString(3),
                    Total = lector.GetDecimal(4)
                });
            }

            return ventas;
        }

    }

}