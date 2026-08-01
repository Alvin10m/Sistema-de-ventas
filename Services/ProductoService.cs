using Npgsql;
using SistemaVentas.Data;
using SistemaVentas.ViewModels;
using SistemaVentas.Models;
using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;

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

        public void AgregarProducto(Producto producto)
        {
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();
            
            string sql = @"
                INSERT INTO productos
                (
                    codigo,
                    nombre,
                    precio,
                    stock,
                    stock_minimo,
                    activo,
                    aplica_itbis,
                    porcentaje_descuento,
                    id_categoria
                )
                VALUES
                (
                    @codigo,
                    @nombre,
                    @precio,
                    @stock,
                    @stock_minimo,
                    @activo,
                    @aplica_itbis,
                    @porcentaje_descuento,
                    @id_categoria
                );";

                using var comando = new NpgsqlCommand(sql, conexion);

                comando.Parameters.AddWithValue("codigo", producto.Codigo);
                comando.Parameters.AddWithValue("nombre", producto.Nombre);
                comando.Parameters.AddWithValue("precio", producto.Precio);
                comando.Parameters.AddWithValue("stock", producto.Cantidad);
                comando.Parameters.AddWithValue("stock_minimo", producto.StockMinimo);
                comando.Parameters.AddWithValue("activo", producto.Activo);
                comando.Parameters.AddWithValue("aplica_itbis", producto.AplicaItbis);
                comando.Parameters.AddWithValue("porcentaje_descuento", producto.PorcentajeDescuento);
                comando.Parameters.AddWithValue("id_categoria", (object?)producto.IdCategoria ?? DBNull.Value);

                comando.ExecuteNonQuery();
        }

        public string GenerarCodigoProducto()
        {
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            string sql = @"
                SELECT codigo
                FROM productos
                ORDER BY id DESC
                LIMIT 1;";

            using var comando = new NpgsqlCommand(sql, conexion);

            var resultado = comando.ExecuteScalar();
            if (resultado == null || resultado == DBNull.Value)
                return "P0001";

            string ultimoCodigo = resultado.ToString()!;

            int numero = int.Parse(ultimoCodigo.Substring(1));

            numero++;

            return $"P{numero:D4}";
        }
        
        // Validar que el campo de nombre no esté vacío
        public string? ValidarNombreProducto(Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return "Este campo es obligatorio";

            return null;
        }

        // Validar que el stock mínimo no sea un número negativo
        public string? ValidarStockMinimoProducto(Producto producto)
        {
            if (producto.StockMinimo <= 0)
            return "El stock mínimo no puede ser negativo.";

            return null;
        }
        // Validar que el precio no sea menor a 0
        public string? ValidarPrecioProducto(Producto producto)
        {
            if (producto.Precio <= 0)
                return "El precio debe ser mayor que cero";

            return null;
        }
        
        // Validar que la cantidad del producto a agreagar no sea negativo
        public string? ValidarStockProducto(Producto producto)
        {
            if (producto.Cantidad < 0)
                return "El stock no puede ser negativo.";
            
            return null;
        }

        // Validar la categoría del producto a agregar
        public string? ValidarCategoriaProducto(Producto producto)
        {
            if (producto.IdCategoria == null)
                return "Debe seleccionar una categoría.";

            return null;
        }

        // Ver que todas las condiciones se cumplan
        public string? ValidarProducto(Producto producto)
        {
            string? error;

            error = ValidarNombreProducto(producto);
            if (error != null)
                return error;

            error = ValidarPrecioProducto(producto);
            if (error != null)
                return error;

            error = ValidarStockProducto(producto);
            if (error != null)
                return error;

            error = ValidarStockMinimoProducto(producto);
            if (error != null)
                return error;

            error = ValidarCategoriaProducto(producto);
            if (error != null)
                return error;

            error = ValidarCategoriaProducto(producto);
            if (error != null)
                return error;

            error = ValidarDescuentoProducto(producto);
            if (error != null)
                return error;
            return null;
        }

        // Validar descuento
        public string? ValidarDescuentoProducto(Producto producto)
        {
            if (producto.PorcentajeDescuento < 0 || producto.PorcentajeDescuento > 100)
                return "El descuento debe estar entre 0 y 100";

            return null;

        }

        //Obtener todas las categorias disponibles registrada en la base de datos, ordenadas alfabéticamente 
        public List<Categoria> ObtenerCategorias()
        {
            // Lista para almacenar las categorías obtenidas
            var categorias = new List<Categoria>();

            // Establecer la conexión con la base de datos
            using var conexion = conexionBD.ObtenerConexion();
            conexion.Open();

            // Consulta las categorías activas ordenadas por nombre
            string sql = @"
                SELECT id, nombre
                FROM categorias
                WHERE activo = TRUE
                ORDER BY nombre;";

            using var comando = new NpgsqlCommand(sql, conexion);
            using var lector = comando.ExecuteReader();

            // Recorrer los registros obtenidos
            while (lector.Read())
            {
                categorias.Add(new Categoria
                {
                    Id = (int)lector["id"],
                    Nombre = lector["nombre"].ToString()!
                });
            }

            // Devolver la lista de categorías
            return categorias;
        }

    }
}