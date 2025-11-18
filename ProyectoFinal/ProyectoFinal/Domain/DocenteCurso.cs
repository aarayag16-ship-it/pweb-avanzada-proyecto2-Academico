using System;

namespace ProyectoFinal.Domain
{
    public class DocenteCurso
    {
        public int Id { get; set; }

        public string DocenteId { get; set; } = default!; // FK a ApplicationUser.Id

        public int CursoId { get; set; }
        public Curso Curso { get; set; } = default!;

        public string? Horario { get; set; }
        public DateTime FechaAsignacion { get; set; }
    }
}
