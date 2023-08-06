using ControlFinanzas.Models.Entidades;

namespace ControlFinanzas.Models
{
    public class IndiceCuentasViewModel
    {
        public string TipoCuenta { get; set; }

        public IEnumerable<Cuenta> cuentas { get; set; }

        public decimal Balance => cuentas.Sum(x => x.Balance);
    }
}
