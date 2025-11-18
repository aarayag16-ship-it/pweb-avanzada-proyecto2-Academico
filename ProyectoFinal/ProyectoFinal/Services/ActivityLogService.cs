using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProyectoFinal.Data;
using ProyectoFinal.Domain;

namespace ProyectoFinal.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string usuarioId, string usuarioNombre, string accion, string modulo, string? datosExtra, HttpContext httpContext)
        {
            var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "N/D";

            var log = new ActivityLog
            {
                UsuarioId = usuarioId,
                UsuarioNombre = usuarioNombre,
                Accion = accion,
                Modulo = modulo,
                DatosExtra = datosExtra,
                Ip = ip,
                FechaHora = DateTime.UtcNow
            };

            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
