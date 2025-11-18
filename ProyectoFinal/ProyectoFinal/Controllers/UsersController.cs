using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Security;
using ProyectoFinal.Services;
using ProyectoFinal.ViewModels;
using ProyectoFinal.Data;
using ProyectoFinal.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ProyectoFinal.Controllers
{
    [Authorize(Roles = RoleNames.Admin)] // Solo administradores
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IActivityLogService _activityLog;
        private readonly ApplicationDbContext _context;

        public UsersController(
      UserManager<IdentityUser> userManager,
      RoleManager<IdentityRole> roleManager,
      IActivityLogService activityLog,
      ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _activityLog = activityLog;
            _context = context;
        }


        // GET: /Users
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var model = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserWithRolesViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Roles = roles
                });
            }

            return View(model);
        }

        // GET: /Users/EditRoles/{id}
        // GET: /Users/EditRoles/{id}
        public async Task<IActionResult> EditRoles(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var allRoles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? ""
            };

            foreach (var role in allRoles)
            {
                model.Roles.Add(new RoleSelectionViewModel
                {
                    Name = role.Name!,
                    Selected = userRoles.Contains(role.Name!)
                });
            }

            return View(model);
        }


        // POST: /Users/EditRoles
        // POST: /Users/EditRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoles(string userId, string[] selectedRoles)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Roles actuales del usuario
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Si no llega nada, dejamos selectedRoles como lista vacía
            selectedRoles ??= Array.Empty<string>();

            var rolesToAdd = selectedRoles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(selectedRoles).ToList();

            // Agregar nuevos roles
            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", "Error al agregar roles.");
                    // Recargamos el modelo para mostrar la vista de nuevo
                    return await RecargarEditRolesViewConErrores(user, selectedRoles);
                }

                // Bitácora
                var usuarioActual = await _userManager.GetUserAsync(User);
                foreach (var role in rolesToAdd)
                {
                    await _activityLog.LogAsync(
                        usuarioId: usuarioActual?.Id ?? "Sistema",
                        usuarioNombre: usuarioActual?.Email ?? "Sistema",
                        accion: "AgregarRol",
                        modulo: "Usuarios",
                        datosExtra: $"UsuarioDestino={user.Email}; RolAgregado={role}",
                        httpContext: HttpContext
                    );
                }
            }

            // Quitar roles
            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError("", "Error al quitar roles.");
                    return await RecargarEditRolesViewConErrores(user, selectedRoles);
                }

                var usuarioActual = await _userManager.GetUserAsync(User);
                foreach (var role in rolesToRemove)
                {
                    await _activityLog.LogAsync(
                        usuarioId: usuarioActual?.Id ?? "Sistema",
                        usuarioNombre: usuarioActual?.Email ?? "Sistema",
                        accion: "RemoverRol",
                        modulo: "Usuarios",
                        datosExtra: $"UsuarioDestino={user.Email}; RolRemovido={role}",
                        httpContext: HttpContext
                    );
                }
            }

            TempData["Message"] = "Roles actualizados correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para recargar la vista en caso de error
        private async Task<IActionResult> RecargarEditRolesViewConErrores(IdentityUser user, string[] selectedRoles)
        {
            var allRoles = _roleManager.Roles.ToList();
            var model = new EditUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? ""
            };

            foreach (var role in allRoles)
            {
                model.Roles.Add(new RoleSelectionViewModel
                {
                    Name = role.Name!,
                    Selected = selectedRoles.Contains(role.Name!)
                });
            }

            return View("EditRoles", model);
        }



        // GET: /Users/VincularEstudiante/{id}
        public async Task<IActionResult> VincularEstudiante(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var estudiantes = await _context.Estudiantes
                .OrderBy(e => e.NombreCompleto)
                .ToListAsync();

            var model = new LinkStudentUserViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email ?? "",
                Estudiantes = estudiantes.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.NombreCompleto} ({e.CodigoMatricula})"
                }).ToList()
            };

            return View(model);
        }

        // POST: /Users/VincularEstudiante
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VincularEstudiante(LinkStudentUserViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserId) || !model.EstudianteId.HasValue)
            {
                ModelState.AddModelError("", "Debe seleccionar un estudiante.");
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                var estudiantes = await _context.Estudiantes
                    .OrderBy(e => e.NombreCompleto)
                    .ToListAsync();

                model.UserEmail = user.Email ?? "";
                model.Estudiantes = estudiantes.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.NombreCompleto} ({e.CodigoMatricula})"
                }).ToList();

                return View(model);
            }

            // Limpiar cualquier vínculo previo de ese estudiante y/o usuario
            var estudiante = await _context.Estudiantes.FindAsync(model.EstudianteId.Value);
            if (estudiante == null) return NotFound();

            estudiante.UserId = model.UserId;
            await _context.SaveChangesAsync();

            // Asegurar rol Estudiante
            if (!await _userManager.IsInRoleAsync(user, RoleNames.Estudiante))
            {
                await _userManager.AddToRoleAsync(user, RoleNames.Estudiante);
            }

            // Registrar en bitácora
            var usuarioActual = await _userManager.GetUserAsync(User);
            await _activityLog.LogAsync(
                usuarioId: usuarioActual?.Id ?? "Sistema",
                usuarioNombre: usuarioActual?.Email ?? "Sistema",
                accion: "VincularEstudianteUsuario",
                modulo: "Usuarios",
                datosExtra: $"User={user.Email}; EstudianteId={estudiante.Id}; Matricula={estudiante.CodigoMatricula}",
                httpContext: HttpContext
            );

            TempData["Message"] = "Estudiante vinculado correctamente al usuario.";
            return RedirectToAction(nameof(Index));
        }





    }
}
