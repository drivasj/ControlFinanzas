using ControlFinanzas.Models.Entidades;
using ControlFinanzas.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ControlFinanzas.Services
{
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;

        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        /// <summary>
        /// Crear
        /// </summary>
        /// <param name="transaccion"></param>
        /// <returns></returns>
        
        public async Task Crear (Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar",
                new
            {
                transaccion.UsuarioId,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CategoriaId,
                transaccion.CuentaId,
                transaccion.Nota
            },
            commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentasId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"
                SELECT 
                	t.id, t.Monto,
                    t.FechaTransaccion, 
                    c.Nombre AS Categoria, 
                    cu.Nombre AS Cuenta,
                    c.TipoOperacionId
                FROM Transacciones t INNER JOIN Categorias c ON c.Id = t.CategoriaId
                					 INNER JOIN Cuentas cu  ON cu.id = t.CuentaId
                WHERE t.CuentaId = @CuentaId
                AND t.UsuarioId = @UsuarioId
                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin",modelo);
        }

        /// <summary>
        /// Actualizar
        /// </summary>
        /// <param name="transaccion"></param>
        /// <param name="montoAnterior"></param>
        /// <param name="cuentaAnterior"></param>
        /// <returns></returns>
        
        public async Task Actualizar (Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar", new
            {
                transaccion.Id,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CategoriaId,
                transaccion.CuentaId,
                transaccion.Nota,
                montoAnterior,
                cuentaAnteriorId
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        /// <summary>
        /// ObtenerPorId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        
        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                     @"SELECT Transacciones.*, CAT.TipoOperacionId 
                     FROM Transacciones INNER JOIN Categorias cat ON cat.Id = Transacciones.CategoriaId
                     WHERE Transacciones.Id = @Id AND Transacciones.UsuarioId = @UsuarioId", new {id,usuarioId});
        }

        /// <summary>
        /// Borrar
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Borrar", new {id}, commandType: System.Data.CommandType.StoredProcedure);
        }


    }
}
