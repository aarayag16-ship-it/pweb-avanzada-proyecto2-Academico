using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace ProyectoFinal.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Roles = ProyectoFinal.Security.RoleNames.Docente + "," + ProyectoFinal.Security.RoleNames.Coordinador)]
    public class HistorialApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HistorialApiController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/HistorialApi/Estudiante/5
        [HttpGet("Estudiante/{id:int}")]
        public async Task<IActionResult> GetPorEstudiante(int id)
        {
            // Verifica que el estudiante exista
            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estudiante == null)
            {
                return NotFound(new { message = "Estudiante no encontrado." });
            }

            // Carga evaluaciones
            var evaluaciones = await _context.Evaluaciones
                .Include(ev => ev.Curso)
                .Include(ev => ev.Termino)
                .Where(ev => ev.EstudianteId == id)
                .OrderBy(ev => ev.Termino.FechaInicio)
                .ThenBy(ev => ev.Curso.Nombre)
                .ToListAsync();

            var resultado = evaluaciones.Select(ev => new
            {
                estudianteId = estudiante.Id,
                estudianteNombre = estudiante.NombreCompleto,
                termino = ev.Termino.Descripcion,
                cursoCodigo = ev.Curso.Codigo,
                cursoNombre = ev.Curso.Nombre,
                notaFinal = ev.NotaFinal,
                estado = ev.Estado,
                fechaRegistro = ev.FechaRegistro
            });

            return Ok(resultado);
        }
    }
}

