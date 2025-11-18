using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Security;
using ProyectoFinal.ViewModels;

namespace ProyectoFinal.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class BitacoraController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BitacoraController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Bitacora
        public async Task<IActionResult> Index()
        {
            // Vista inicial sin filtros → últimos 50 registros como ejemplo
            var logs = await _context.ActivityLogs
                .OrderByDescending(l => l.FechaHora)
                .Take(50)
                .ToListAsync();

            var model = new ActivityLogFilterViewModel
            {
                Resultados = logs.Select(l => new ActivityLogItemViewModel
                {
                    Id = l.Id,
                    FechaHora = l.FechaHora,
                    UsuarioNombre = l.UsuarioNombre,
                    Accion = l.Accion,
                    Modulo = l.Modulo,
                    DatosExtra = l.DatosExtra,
                    Ip = l.Ip
                }).ToList()
            };

            return View(model);
        }

        // POST: /Bitacora
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ActivityLogFilterViewModel model)
        {
            var query = _context.ActivityLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Usuario))
            {
                var usuario = model.Usuario.Trim();
                query = query.Where(l =>
                    l.UsuarioNombre.Contains(usuario) ||
                    l.UsuarioId.Contains(usuario));
            }

            if (!string.IsNullOrWhiteSpace(model.Accion))
            {
                var accion = model.Accion.Trim();
                query = query.Where(l => l.Accion.Contains(accion));
            }

            if (!string.IsNullOrWhiteSpace(model.Modulo))
            {
                var modulo = model.Modulo.Trim();
                query = query.Where(l => l.Modulo.Contains(modulo));
            }

            if (!string.IsNullOrWhiteSpace(model.Ip))
            {
                var ip = model.Ip.Trim();
                query = query.Where(l => l.Ip.Contains(ip));
            }

            if (model.FechaDesde.HasValue)
            {
                var desde = model.FechaDesde.Value.Date;
                query = query.Where(l => l.FechaHora >= desde);
            }

            if (model.FechaHasta.HasValue)
            {
                var hasta = model.FechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(l => l.FechaHora <= hasta);
            }

            // Para no traer demasiado, limitamos a 500 registros
            var logs = await query
                .OrderByDescending(l => l.FechaHora)
                .Take(500)
                .ToListAsync();

            model.Resultados = logs.Select(l => new ActivityLogItemViewModel
            {
                Id = l.Id,
                FechaHora = l.FechaHora,
                UsuarioNombre = l.UsuarioNombre,
                Accion = l.Accion,
                Modulo = l.Modulo,
                DatosExtra = l.DatosExtra,
                Ip = l.Ip
            }).ToList();

            return View(model);
        }
    }
}

