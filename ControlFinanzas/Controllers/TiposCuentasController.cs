using ControlFinanzas.Models.Entidades;
using ControlFinanzas.Services;
using ControlFinanzas.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControlFinanzas.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IReporsitorioTiposCuentas reporsitorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        public TiposCuentasController(IReporsitorioTiposCuentas reporsitorioTiposCuentas, 
            IServicioUsuarios servicioUsuarios)
        {
            this.reporsitorioTiposCuentas = reporsitorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await reporsitorioTiposCuentas.Obtener(usuarioId);
            return View (tiposCuentas);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);

            }

            tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();

            var existe = await reporsitorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId); // validamos si ya existe el tipo de cuenta en el usuario

            if (existe)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre),
                    $"El nombre {tipoCuenta.Nombre} ya existe.");

                return View(tipoCuenta);

            }

            await reporsitorioTiposCuentas.Crear(tipoCuenta); // si no existe -> Crear cuenta

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await reporsitorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Homer");
            }
            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuentaExiste = await  reporsitorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if(tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Homer");
            }

            await reporsitorioTiposCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("Index");
        }

        public async Task <IActionResult>Borrar(int id)
        {
            var UsuarioId =  servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await reporsitorioTiposCuentas.ObtenerPorId(id, UsuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var UsuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await reporsitorioTiposCuentas.ObtenerPorId(id, UsuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await reporsitorioTiposCuentas.Borrar(id);
            return RedirectToAction("Index");

        }


        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var existeTipoCuenta = await reporsitorioTiposCuentas.Existe(nombre, usuarioId);

            if (existeTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }

       

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await reporsitorioTiposCuentas.Obtener(usuarioId);
            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);

            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoPertenecenAlUsuario.Count > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor, indice) =>
                new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await reporsitorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }

    }
}
