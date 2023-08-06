using ControlFinanzas.Models.Entidades;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlFinanzas.Models
{
    public class CuentaCreacionViewModel:Cuenta
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
