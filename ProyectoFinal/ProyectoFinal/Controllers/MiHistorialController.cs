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
    [Authorize(Roles = RoleNames.Estudiante)]
    public class MiHistorialController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IActivityLogService _activityLog;

        public MiHistorialController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IActivityLogService activityLog)
        {
            _context = context;
            _userManager = userManager;
            _activityLog = activityLog;
        }

        // GET: /MiHistorial
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.UserId == user.Id);

            if (estudiante == null)
            {
                ViewBag.Mensaje = "Su cuenta no está asociada a un registro de estudiante. Consulte con el administrador.";
                return View("SinEstudiante");
            }

            var evaluaciones = await _context.Evaluaciones
                .Include(ev => ev.Curso)
                .Include(ev => ev.Termino)
                .Where(ev => ev.EstudianteId == estudiante.Id)
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

            await _activityLog.LogAsync(
                usuarioId: user.Id,
                usuarioNombre: user.Email ?? "",
                accion: "VerMiHistorial",
                modulo: "MiHistorial",
                datosExtra: $"EstudianteId={estudiante.Id};Matricula={estudiante.CodigoMatricula}",
                httpContext: HttpContext
            );

            return View(model);
        }

        // GET: /MiHistorial/Datos
        [HttpGet]
        public async Task<IActionResult> Datos()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.UserId == user.Id);

            if (estudiante == null) return NotFound();

            var evaluaciones = await _context.Evaluaciones
                .Include(ev => ev.Curso)
                .Include(ev => ev.Termino)
                .Where(ev => ev.EstudianteId == estudiante.Id)
                .OrderBy(ev => ev.Termino.FechaInicio)
                .ThenBy(ev => ev.Curso.Nombre)
                .ToListAsync();

            var datos = evaluaciones.Select(ev => new
            {
                terminoDescripcion = ev.Termino.Descripcion,
                cursoNombre = ev.Curso.Nombre,
                notaFinal = ev.NotaFinal,
                estado = ev.Estado,
                fechaRegistro = ev.FechaRegistro
            });

            return Ok(datos);
        }
    }
}
