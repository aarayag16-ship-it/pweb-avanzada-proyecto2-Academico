using Academico.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Academico.Web.Controllers
{

    [Authorize(Roles = "Docente")]
    public class EstudiantesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Crear()
        {
            ViewBag.TerminoId = new SelectList(_db.Terminos, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearAjax(Estudiante model, int terminoId, int[] cursosIds)
        {
            if (!ModelState.IsValid)
                return Json(new { ok = false, msg = "Datos inválidos" });

            // Validar duplicados
            bool dup = _db.Estudiantes.Any(e => e.Identificacion == model.Identificacion || e.Correo == model.Correo);
            if (dup) return Json(new { ok = false, msg = "Identificación o correo ya existe." });

            _db.Estudiantes.Add(model);
            _db.SaveChanges();

            var mat = new Matricula { EstudianteId = model.Id, TerminoId = terminoId };
            _db.Matriculas.Add(mat);
            _db.SaveChanges();

            if (cursosIds != null)
            {
                foreach (var cid in cursosIds)
                    _db.MatriculaCursos.Add(new MatriculaCurso { MatriculaId = mat.Id, CursoId = cid });
                _db.SaveChanges();
            }

            return Json(new { ok = true, msg = "Estudiante registrado con éxito." });
        }
    }



}
