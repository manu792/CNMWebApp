using CNMWebApp.Authorization;
using CNMWebApp.Models;
using CNMWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CNMWebApp.Controllers
{
    [Auth(Roles = "Funcionario, Jefatura, Recursos Humanos, Director")]
    public class VacacionController : Controller
    {
        private UserService _userServicio;
        private RoleService _roleServicio;
        private SolicitudService _solicitudServicio;

        public VacacionController()
        {
            _userServicio = new UserService();
            _roleServicio = new RoleService();
            _solicitudServicio = new SolicitudService();
        }

        // GET: Vacacion
        //public ActionResult Index()
        //{
        //    // Este metodod va a retornar toda la lista de vacaciones para el usuario logeado
        //    return View();
        //}

        //// GET: Vacacion/Crear
        //public async Task<ActionResult> Crear()
        //{
        //    // Consigo la informacion del usuario logeado para mostrar en el View
        //    var usuario = await _userServicio.GetLoggedInUser();

        //    var annosLaborados = DateTime.Now.Year - usuario.FechaIngreso.Year;
        //    if (usuario.FechaIngreso > DateTime.Now.AddYears(-annosLaborados)) annosLaborados--;

        //    return View(new VacacionViewModel()
        //    {
        //        Id = usuario.Id,
        //        Nombre = usuario.Nombre,
        //        PrimerApellido = usuario.PrimerApellido,
        //        SegundoApellido = usuario.SegundoApellido,
        //        Email = usuario.Email,
        //        PhoneNumber = usuario.PhoneNumber,
        //        Role = usuario.Role,
        //        UnidadTecnica = usuario.UnidadTecnica,
        //        Categoria = usuario.Categoria,
        //        FechaIngreso = usuario.FechaIngreso,
        //        EstaActivo = usuario.EstaActivo,
        //        CantidadAnnosLaborados = annosLaborados < 0 ? 0 : annosLaborados,
        //        CantidadDiasSolicitados = 0,
                
        //        // Necesito la logica para saber calcular los dias disponibles segun fecha de ingreso 
        //        // y cantidad de vacaciones previamente solicitadas
        //        SaldoDiasDisponibles = 10 
        //    });
        //}

        //// POST: Vacacion/Crear
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Crear(VacacionViewModel solicitudVacaciones)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(solicitudVacaciones);
        //    }

        //    try
        //    {
        //        var rowsAffected = _solicitudServicio.CrearSolicitudVacaciones(solicitudVacaciones);

        //        if (rowsAffected <= 0)
        //        {
        //            ModelState.AddModelError("", "Hubo un problema al tratar de agregar la solicitud. Favor intente de nuevo más tarde.");
        //            return View(solicitudVacaciones);
        //        }

        //        return RedirectToAction("Index", "Solicitud", new { misSolicitudes = true });
        //    }
        //    catch(Exception ex)
        //    {
        //        ModelState.AddModelError("", "Hubo un problema al tratar de agregar la solicitud. Favor intente de nuevo más tarde.");
        //        return View(solicitudVacaciones);
        //    }
        //}

        // GET: Vacacion/CrearANombreDeEmpleado
        //public ActionResult CrearANombreDeEmpleado()
        //{
        //    return View();
        //}

        //// POST: Vacacion/CrearANombreDeEmpleado
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CrearANombreDeEmpleado(VacacionViewModel solicitudVacaciones)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(solicitudVacaciones);
        //    }

        //    return View(solicitudVacaciones);
        //}
    }
}