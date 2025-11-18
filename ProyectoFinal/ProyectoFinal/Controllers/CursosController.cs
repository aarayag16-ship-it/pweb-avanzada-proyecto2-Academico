using ProyectoFinal.ViewModels; 
using ProyectoFinal.Security;
using ProyectoFinal.Services;
using ProyectoFinal.Domain;
using ProyectoFinal.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace ProyectoFinal.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IActivityLogService _activityLog;
        private readonly RoleManager<IdentityRole> _roleManager;


        public CursosController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,      // ← AGREGAR
            IActivityLogService activityLog)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;                 // ← ASIGNAR
            _activityLog = activityLog;
        }


        // GET: /Cursos
        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Cursos
                .Include(c => c.Termino)
                .Include(c => c.Evaluaciones)
                .ToListAsync();

            var model = cursos.Select(c => new CourseListViewModel
            {
                Id = c.Id,
                Codigo = c.Codigo,
                Nombre = c.Nombre,
                Creditos = c.Creditos,
                TerminoDescripcion = c.Termino.Descripcion,
                CantidadEvaluaciones = c.Evaluaciones.Count,
                FechaCreacion = c.FechaCreacion
            }).ToList();

            return View(model);
        }

        // GET: /Cursos/Create
        public async Task<IActionResult> Create()
        {
            var vm = new CourseFormViewModel
            {
                Terminos = await _context.Terminos
                    .OrderBy(t => t.FechaInicio)
                    .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Descripcion
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        // POST: /Cursos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Terminos = await _context.Terminos
                    .OrderBy(t => t.FechaInicio)
                    .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Descripcion
                    })
                    .ToListAsync();

                return View(model);
            }

            // Validar código único (otra capa además del índice)
            bool codigoExiste = await _context.Cursos.AnyAsync(c => c.Codigo == model.Codigo);
            if (codigoExiste)
            {
                ModelState.AddModelError("Codigo", "Ya existe un curso con este código.");
                model.Terminos = await _context.Terminos
                    .OrderBy(t => t.FechaInicio)
                    .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Descripcion
                    })
                    .ToListAsync();
                return View(model);
            }

            var usuarioActual = await _userManager.GetUserAsync(User);

            var curso = new Curso
            {
                Codigo = model.Codigo,
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Creditos = model.Creditos,
                TerminoId = model.TerminoId,
                FechaCreacion = System.DateTime.UtcNow,
                UsuarioCreadorId = usuarioActual?.Id ?? "Sistema"
            };

            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            // Bitácora
            await _activityLog.LogAsync(
                usuarioId: usuarioActual?.Id ?? "Sistema",
                usuarioNombre: usuarioActual?.Email ?? "Sistema",
                accion: "CrearCurso",
                modulo: "Cursos",
                datosExtra: $"CursoId={curso.Id}; Codigo={curso.Codigo}",
                httpContext: HttpContext
            );

            TempData["Message"] = "Curso creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Cursos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var curso = await _context.Cursos
                .Include(c => c.Evaluaciones)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null) return NotFound();

            bool tieneEstudiantes = curso.Evaluaciones.Any();

            var vm = new CourseFormViewModel
            {
                Id = curso.Id,
                Codigo = curso.Codigo,
                Nombre = curso.Nombre,
                Descripcion = curso.Descripcion,
                Creditos = curso.Creditos,
                TerminoId = curso.TerminoId,
                TieneEstudiantesInscritos = tieneEstudiantes,
                Terminos = await _context.Terminos
                    .OrderBy(t => t.FechaInicio)
                    .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Descripcion
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        // POST: /Cursos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            var curso = await _context.Cursos
                .Include(c => c.Evaluaciones)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null) return NotFound();

            bool tieneEstudiantes = curso.Evaluaciones.Any();

            if (!ModelState.IsValid)
            {
                model.TieneEstudiantesInscritos = tieneEstudiantes;
                model.Terminos = await _context.Terminos
                    .OrderBy(t => t.FechaInicio)
                    .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Descripcion
                    })
                    .ToListAsync();
                return View(model);
            }

            // Si tiene estudiantes, NO permitimos cambiar el código
            if (tieneEstudiantes && model.Codigo != curso.Codigo)
            {
                ModelState.AddModelError("Codigo", "No se puede cambiar el código de un curso con estudiantes inscritos.");
                model.TieneEstudiantesInscritos = tieneEstudiantes;
                model.Terminos = await _context.Terminos
                    .OrderBy(t => t.FechaInicio)
                    .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Descripcion
                    })
                    .ToListAsync();
                return View(model);
            }

            // Validar código único en caso de cursos sin estudiantes
            if (!tieneEstudiantes && model.Codigo != curso.Codigo)
            {
                bool codigoExiste = await _context.Cursos.AnyAsync(c => c.Codigo == model.Codigo && c.Id != model.Id);
                if (codigoExiste)
                {
                    ModelState.AddModelError("Codigo", "Ya existe un curso con este código.");
                    model.TieneEstudiantesInscritos = tieneEstudiantes;
                    model.Terminos = await _context.Terminos
                        .OrderBy(t => t.FechaInicio)
                        .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                        {
                            Value = t.Id.ToString(),
                            Text = t.Descripcion
                        })
                        .ToListAsync();
                    return View(model);
                }
            }

            var usuarioActual = await _userManager.GetUserAsync(User);

            curso.Nombre = model.Nombre;
            curso.Descripcion = model.Descripcion;
            curso.Creditos = model.Creditos;
            curso.TerminoId = model.TerminoId;
            curso.FechaModificacion = System.DateTime.UtcNow;
            curso.UsuarioModificadorId = usuarioActual?.Id ?? "Sistema";

            if (!tieneEstudiantes)
            {
                curso.Codigo = model.Codigo;
            }

            await _context.SaveChangesAsync();

            await _activityLog.LogAsync(
                usuarioId: usuarioActual?.Id ?? "Sistema",
                usuarioNombre: usuarioActual?.Email ?? "Sistema",
                accion: "EditarCurso",
                modulo: "Cursos",
                datosExtra: $"CursoId={curso.Id}; Codigo={curso.Codigo}",
                httpContext: HttpContext
            );

            TempData["Message"] = "Curso actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Cursos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var curso = await _context.Cursos
                .Include(c => c.Termino)
                .Include(c => c.Evaluaciones)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null) return NotFound();

            if (curso.Evaluaciones.Any())
            {
                TempData["Error"] = "No se puede eliminar un curso que tiene estudiantes/evaluaciones registradas.";
                return RedirectToAction(nameof(Index));
            }

            return View(curso);
        }

        // POST: /Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _context.Cursos
                .Include(c => c.Evaluaciones)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null) return NotFound();

            if (curso.Evaluaciones.Any())
            {
                TempData["Error"] = "No se puede eliminar un curso que tiene estudiantes/evaluaciones registradas.";
                return RedirectToAction(nameof(Index));
            }

            var usuarioActual = await _userManager.GetUserAsync(User);

            _context.Cursos.Remove(curso);
            await _context.SaveChangesAsync();

            await _activityLog.LogAsync(
                usuarioId: usuarioActual?.Id ?? "Sistema",
                usuarioNombre: usuarioActual?.Email ?? "Sistema",
                accion: "EliminarCurso",
                modulo: "Cursos",
                datosExtra: $"CursoId={curso.Id}; Codigo={curso.Codigo}",
                httpContext: HttpContext
            );

            TempData["Message"] = "Curso eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Cursos/AsignarDocentes/5
        public async Task<IActionResult> AsignarDocentes(int id)
        {
            var curso = await _context.Cursos
                .Include(c => c.Docentes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null) return NotFound();

            // Usuarios con rol Docente
            var docentesRole = await _roleManager.FindByNameAsync(RoleNames.Docente);
            if (docentesRole == null)
            {
                TempData["Error"] = "No existe el rol 'Docente'.";
                return RedirectToAction(nameof(Index));
            }

            var docentesIds = await _context.UserRoles
                .Where(ur => ur.RoleId == docentesRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var docentes = await _userManager.Users
                .Where(u => docentesIds.Contains(u.Id))
                .ToListAsync();

            // Docentes ya asignados a este curso
            var asignacionesExistentes = await _context.DocenteCursos
                .Where(dc => dc.CursoId == id)
                .ToListAsync();

            var vm = new AssignTeachersToCourseViewModel
            {
                CursoId = curso.Id,
                CodigoCurso = curso.Codigo,
                NombreCurso = curso.Nombre
            };

            foreach (var docente in docentes)
            {
                var asignacion = asignacionesExistentes.FirstOrDefault(dc => dc.DocenteId == docente.Id);

                vm.Docentes.Add(new TeacherAssignmentViewModel
                {
                    UserId = docente.Id,
                    Email = docente.Email ?? "",
                    NombreUsuario = docente.UserName ?? "",
                    Asignado = asignacion != null,
                    Horario = asignacion?.Horario
                });
            }

            return View(vm);
        }

        // POST: /Cursos/AsignarDocentes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarDocentes(AssignTeachersToCourseViewModel model)
        {
            var curso = await _context.Cursos
                .Include(c => c.Docentes)
                .FirstOrDefaultAsync(c => c.Id == model.CursoId);

            if (curso == null) return NotFound();

            var usuarioActual = await _userManager.GetUserAsync(User);

            // Asignaciones existentes
            var asignacionesExistentes = await _context.DocenteCursos
                .Where(dc => dc.CursoId == model.CursoId)
                .ToListAsync();

            var seleccionados = model.Docentes.Where(d => d.Asignado).ToList();

            // Docentes a agregar (marcados ahora y que no estaban antes)
            var idsSeleccionados = seleccionados.Select(d => d.UserId).ToList();
            var idsYaAsignados = asignacionesExistentes.Select(dc => dc.DocenteId).ToList();

            var idsParaAgregar = idsSeleccionados.Except(idsYaAsignados).ToList();
            var idsParaQuitar = idsYaAsignados.Except(idsSeleccionados).ToList();

            // Agregar nuevas asignaciones
            foreach (var docenteId in idsParaAgregar)
            {
                var docenteVm = seleccionados.First(d => d.UserId == docenteId);

                var nueva = new DocenteCurso
                {
                    DocenteId = docenteId,
                    CursoId = model.CursoId,
                    Horario = docenteVm.Horario,
                    FechaAsignacion = System.DateTime.UtcNow
                };

                _context.DocenteCursos.Add(nueva);

                await _activityLog.LogAsync(
                    usuarioId: usuarioActual?.Id ?? "Sistema",
                    usuarioNombre: usuarioActual?.Email ?? "Sistema",
                    accion: "AsignarDocenteCurso",
                    modulo: "Cursos",
                    datosExtra: $"CursoId={curso.Id};Codigo={curso.Codigo};DocenteId={docenteId}",
                    httpContext: HttpContext
                );
            }

            // Quitar asignaciones
            foreach (var docenteId in idsParaQuitar)
            {
                var asignacion = asignacionesExistentes.First(dc => dc.DocenteId == docenteId);

                _context.DocenteCursos.Remove(asignacion);

                await _activityLog.LogAsync(
                    usuarioId: usuarioActual?.Id ?? "Sistema",
                    usuarioNombre: usuarioActual?.Email ?? "Sistema",
                    accion: "RemoverDocenteCurso",
                    modulo: "Cursos",
                    datosExtra: $"CursoId={curso.Id};Codigo={curso.Codigo};DocenteId={docenteId}",
                    httpContext: HttpContext
                );
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Asignaciones de docentes actualizadas correctamente.";
            return RedirectToAction(nameof(Index));
        }



    }
}
