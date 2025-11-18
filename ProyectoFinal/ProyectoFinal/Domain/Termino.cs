using System;
using System.Collections.Generic;

namespace ProyectoFinal.Domain
{
    public class Termino
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = default!; // Ej: 2025-Q3
        public string Descripcion { get; set; } = default!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
        public ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
    }
}
