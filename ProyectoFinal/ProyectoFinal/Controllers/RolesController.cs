using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Security;
using ProyectoFinal.Services;

namespace ProyectoFinal.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IActivityLogService _activityLog;

        public RolesController(RoleManager<IdentityRole> roleManager, IActivityLogService activityLog)
        {
            _roleManager = roleManager;
            _activityLog = activityLog;
        }

        // GET: /Roles
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // GET: /Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("", "El nombre del rol es obligatorio.");
                return View();
            }

            if (await _roleManager.RoleExistsAsync(name))
            {
                ModelState.AddModelError("", "Ya existe un rol con ese nombre.");
                return View();
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(name));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error al crear el rol.");
                return View();
            }

            await _activityLog.LogAsync(
                usuarioId: User.Identity?.Name ?? "Admin",
                usuarioNombre: User.Identity?.Name ?? "Admin",
                accion: "CrearRol",
                modulo: "Usuarios",
                datosExtra: $"RolCreado={name}",
                httpContext: HttpContext
            );

            TempData["Message"] = "Rol creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
