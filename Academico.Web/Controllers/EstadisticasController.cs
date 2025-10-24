using Academico.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Academico.Web.Controllers
{

    [Authorize(Roles = "Docente")]
    public class EstadisticasController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewBag.TerminoId = new SelectList(_db.Terminos, "Id", "Nombre");
            ViewBag.CursoId = new SelectList(_db.Cursos, "Id", "Nombre");
            return View();
        }

        [HttpGet]
        public ActionResult Kpis(int terminoId, int cursoId)
        {
            var evals = _db.Evaluaciones
                .Where(e => e.TerminoId == terminoId && e.CursoId == cursoId);

            var total = evals.Count();
            var aprob = evals.Count(e => e.Estado == "Aprobado");
            var reprob = evals.Count(e => e.Estado == "Reprobado");

            // participación (contar por TipoParticipacion)
            var alta = evals.Count(e => e.TipoParticipacion == "Alta");
            var media = evals.Count(e => e.TipoParticipacion == "Media");
            var baja = evals.Count(e => e.TipoParticipacion == "Baja");

            var kpis = new
            {
                total,
                aprob,
                reprob,
                porcAprob = total == 0 ? 0 : (aprob * 100.0 / total),
                porcReprob = total == 0 ? 0 : (reprob * 100.0 / total),
                participacion = new { alta, media, baja }
            };
            return Json(kpis, JsonRequestBehavior.AllowGet);
        }
    }


}
