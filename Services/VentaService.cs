using SistemaVentas.Data;
using Npgsql;
using SistemaVentas.ViewModels;
using System.Collections.ObjectModel;
using System;


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
                        (idventa, idproducto, cantidad, preciounitario, descuento, subtotal)
                        VALUES
                        (@idventa, @idproducto, @cantidad, @precio, @descuento, @subtotal);";
                
                    using var comandoDetalle = new NpgsqlCommand(sqlDetalle, conexion, transaccion);

                    comandoDetalle.Parameters.AddWithValue("idventa", idVenta);
                    comandoDetalle.Parameters.AddWithValue("Idproducto", producto.IdProductoBD);
                    comandoDetalle.Parameters.AddWithValue("cantidad", producto.Cantidad);
                    comandoDetalle.Parameters.AddWithValue("precio", producto.PrecioUnitario);
                    comandoDetalle.Parameters.AddWithValue("descuento", producto.Descuento);
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
    }
}