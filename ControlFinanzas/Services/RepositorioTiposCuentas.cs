using ControlFinanzas.Models.Entidades;
using ControlFinanzas.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ControlFinanzas.Services
{
    public class RepositorioTiposCuentas: IReporsitorioTiposCuentas
    {
        private readonly string connectionString;

        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Crear Tipo de cuenta
        /// </summary>
        /// <param name="tipoCuenta"></param>
        /// <returns></returns>
        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>
                                            ("TiposCuentasInsertar", new { tipoCuenta.UsuarioId, tipoCuenta.Nombre },
                                                commandType: System.Data.CommandType.StoredProcedure); ;
            tipoCuenta.Id = id;
        }
        /// <summary>
        /// Validation for Nombre do not repeat
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        public async Task <bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                    @"SELECT 1
                                    FROM TiposCuentas
                                    WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId;",
                                    new { nombre, usuarioId });
                                    return existe == 1;
                
        }

        /// <summary>
        /// Listado de  TiposCuentas
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden " +
                "                                           FROM TiposCuentas" +
                "                                           WHERE UsuarioId = @UsuarioId " +
                "                                           ORDER BY Orden", new {usuarioId});
        }

        public async Task Actualizar (TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas SET Nombre = @Nombre WHERE Id = @Id", tipoCuenta); // Permite ejecutar script que no retorne nada
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden 
                                                             FROM TiposCuentas 
                                                             WHERE UsuarioId = @UsuarioId
                                                             AND Id = @Id",                                                       
                                                           new { id,usuarioId });
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE TiposCuentas WHERE Id = @Id", new {id});
        }

        public async  Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            var query = "UPDATE  TiposCuentas SET  Orden = @Orden WHERE Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }
    }
}
