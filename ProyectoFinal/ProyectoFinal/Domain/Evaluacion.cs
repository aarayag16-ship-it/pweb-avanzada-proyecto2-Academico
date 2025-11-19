using System;

namespace ProyectoFinal.Domain
{
    public class Evaluacion
    {
        public int Id { get; set; }

        public int EstudianteId { get; set; }
        public Estudiante Estudiante { get; set; } = default!;

        public int CursoId { get; set; }
        public Curso Curso { get; set; } = default!;

        public int TerminoId { get; set; }
        public Termino Termino { get; set; } = default!;

        public decimal NotaFinal { get; set; }   // 0-100
        public string Estado { get; set; } = default!; // Aprobado/Reprobado
        public string? Observaciones { get; set; } //Observaciones

        public DateTime FechaRegistro { get; set; }
    }
}
