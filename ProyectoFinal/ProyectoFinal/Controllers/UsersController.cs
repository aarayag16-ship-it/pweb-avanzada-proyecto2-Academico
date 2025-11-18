using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Security;
using ProyectoFinal.Services;
using ProyectoFinal.ViewModels;

namespace ProyectoFinal.Controllers
{
    [Authorize(Roles = RoleNames.Admin)] // Solo administradores
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IActivityLogService _activityLog;

        public UsersController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IActivityLogService activityLog)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _activityLog = activityLog;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoles(EditUserRolesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.Roles.Where(r => r.Selected).Select(r => r.Name).ToList();

            var rolesToAdd = selectedRoles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(selectedRoles).ToList();

            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", "Error al agregar roles.");
                    return View(model);
                }

                // Registrar en bitácora
                foreach (var role in rolesToAdd)
                {
                    await _activityLog.LogAsync(
                        usuarioId: User.FindFirst("sub")?.Value ?? User.Identity?.Name ?? "Admin",
                        usuarioNombre: User.Identity?.Name ?? "Admin",
                        accion: "AgregarRol",
                        modulo: "Usuarios",
                        datosExtra: $"UsuarioDestino={user.Email}; RolAgregado={role}",
                        httpContext: HttpContext
                    );
                }
            }

            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError("", "Error al quitar roles.");
                    return View(model);
                }

                // Registrar en bitácora
                foreach (var role in rolesToRemove)
                {
                    await _activityLog.LogAsync(
                        usuarioId: User.FindFirst("sub")?.Value ?? User.Identity?.Name ?? "Admin",
                        usuarioNombre: User.Identity?.Name ?? "Admin",
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
    }
}
