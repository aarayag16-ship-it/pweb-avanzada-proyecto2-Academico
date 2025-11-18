using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Security;
using ProyectoFinal.Services;
using ProyectoFinal.ViewModels;

namespace ProyectoFinal.Controllers
{
    // Solo Docente y Coordinador pueden acceder
    [Authorize(Roles = RoleNames.Docente + "," + RoleNames.Coordinador)]
    public class HistorialController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IActivityLogService _activityLog;

        public HistorialController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IActivityLogService activityLog)
        {
            _context = context;
            _userManager = userManager;
            _activityLog = activityLog;
        }

        // GET: /Historial/Buscar
        // Muestra formulario de búsqueda
        public IActionResult Buscar()
        {
            var model = new StudentSearchViewModel();
            return View(model);
        }

        // POST: /Historial/Buscar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buscar(StudentSearchViewModel model)
        {
            // Si no puso nada, devolvemos el formulario con un mensaje simple
            if (string.IsNullOrWhiteSpace(model.Nombre) &&
                string.IsNullOrWhiteSpace(model.CodigoMatricula) &&
                string.IsNullOrWhiteSpace(model.Identificacion))
            {
                ModelState.AddModelError("", "Debe ingresar al menos un criterio de búsqueda.");
                return View(model);
            }

            var query = _context.Estudiantes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Nombre))
            {
                var nombre = model.Nombre.Trim();
                query = query.Where(e => e.NombreCompleto.Contains(nombre));
            }

            if (!string.IsNullOrWhiteSpace(model.CodigoMatricula))
            {
                var mat = model.CodigoMatricula.Trim();
                query = query.Where(e => e.CodigoMatricula.Contains(mat));
            }

            if (!string.IsNullOrWhiteSpace(model.Identificacion))
            {
                var id = model.Identificacion.Trim();
                query = query.Where(e => e.Identificacion.Contains(id));
            }

            var resultados = await query
                .OrderBy(e => e.NombreCompleto)
                .Take(50) // por si acaso
                .ToListAsync();

            model.Resultados = resultados.Select(e => new StudentSearchResultViewModel
            {
                EstudianteId = e.Id,
                NombreCompleto = e.NombreCompleto,
                CodigoMatricula = e.CodigoMatricula,
                Identificacion = e.Identificacion,
                Correo = e.Correo
            }).ToList();

            return View(model);
        }

        // GET: /Historial/Detalles/5
        public async Task<IActionResult> Detalles(int id)
        {
            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estudiante == null) return NotFound();

            var evaluaciones = await _context.Evaluaciones
                .Include(ev => ev.Curso)
                .Include(ev => ev.Termino)
                .Where(ev => ev.EstudianteId == id)
                .OrderBy(ev => ev.Termino.FechaInicio)
                .ThenBy(ev => ev.Curso.Nombre)
                .ToListAsync();

            var model = new AcademicHistoryViewModel
            {
                EstudianteId = estudiante.Id,
                NombreCompleto = estudiante.NombreCompleto,
                CodigoMatricula = estudiante.CodigoMatricula,
                Identificacion = estudiante.Identificacion,
                Correo = estudiante.Correo
            };

            foreach (var ev in evaluaciones)
            {
                model.Registros.Add(new AcademicRecordEntryViewModel
                {
                    TerminoDescripcion = ev.Termino.Descripcion,
                    CursoCodigo = ev.Curso.Codigo,
                    CursoNombre = ev.Curso.Nombre,
                    NotaFinal = ev.NotaFinal,
                    Estado = ev.Estado,
                    FechaRegistro = ev.FechaRegistro
                });
            }

            // Registrar en bitácora la consulta del historial
            var usuarioActual = await _userManager.GetUserAsync(User);

            await _activityLog.LogAsync(
                usuarioId: usuarioActual?.Id ?? "Sistema",
                usuarioNombre: usuarioActual?.Email ?? "Sistema",
                accion: "VerHistorialEstudiante",
                modulo: "Historial",
                datosExtra: $"EstudianteId={estudiante.Id};Matricula={estudiante.CodigoMatricula}",
                httpContext: HttpContext
            );

            return View(model);
        }

        // GET: /Historial/HistorialDatos/5
        [HttpGet]
        public async Task<IActionResult> HistorialDatos(int id)
        {
            // Validar que el estudiante exista
            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estudiante == null)
            {
                return NotFound();
            }

            // Cargar evaluaciones del estudiante
            var evaluaciones = await _context.Evaluaciones
                .Include(ev => ev.Curso)
                .Include(ev => ev.Termino)
                .Where(ev => ev.EstudianteId == id)
                .OrderBy(ev => ev.Termino.FechaInicio)
                .ThenBy(ev => ev.Curso.Nombre)
                .ToListAsync();

            // Proyectar a un objeto simple para el gráfico
            var datos = evaluaciones.Select(ev => new
            {
                terminoDescripcion = ev.Termino.Descripcion,
                cursoCodigo = ev.Curso.Codigo,
                cursoNombre = ev.Curso.Nombre,
                notaFinal = ev.NotaFinal,
                estado = ev.Estado,
                fechaRegistro = ev.FechaRegistro
            });

            return Ok(datos);
        }



    }
}
