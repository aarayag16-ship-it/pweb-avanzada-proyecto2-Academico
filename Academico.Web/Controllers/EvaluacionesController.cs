using Academico.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Academico.Web.Controllers
{

    [Authorize(Roles = "Docente")]
    public class EvaluacionesController : Controller
       
    
    {





        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Index() => View(); // mostrará caja de búsqueda y resultados con AJAX

        [HttpGet]
        public ActionResult Buscar(string q)
        {
            var data = _db.Estudiantes
                .Where(e => q == null
                    || e.Nombre.Contains(q)
                    || e.Apellidos.Contains(q)
                    || e.Identificacion.Contains(q))
                .Select(e => new { e.Id, Nombre = e.Nombre + " " + e.Apellidos, e.Identificacion, e.Correo })
                .Take(20)
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }






        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Crear(Evaluacion model)
        {
            if (!ModelState.IsValid) return Json(new { ok = false, msg = "Datos inválidos" });

            bool dup = _db.Evaluaciones.Any(x => x.EstudianteId == model.EstudianteId
                                               && x.TerminoId == model.TerminoId
                                               && x.CursoId == model.CursoId);
            if (dup) return Json(new { ok = false, msg = "Ya existe evaluación para este término/curso." });

            _db.Evaluaciones.Add(model);
            _db.SaveChanges();
            return Json(new { ok = true, msg = "Evaluación registrada." });
        }
    }






}
