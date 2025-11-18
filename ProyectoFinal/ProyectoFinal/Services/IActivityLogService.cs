using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProyectoFinal.Services
{
    public interface IActivityLogService
    {
        Task LogAsync(string usuarioId, string usuarioNombre, string accion, string modulo, string? datosExtra, HttpContext httpContext);
    }
}
