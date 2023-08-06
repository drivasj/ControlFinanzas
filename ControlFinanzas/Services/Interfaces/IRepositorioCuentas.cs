using ControlFinanzas.Models;
using ControlFinanzas.Models.Entidades;

namespace ControlFinanzas.Services.Interfaces
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);

        Task Crear(Cuenta cuenta);

        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }
}
