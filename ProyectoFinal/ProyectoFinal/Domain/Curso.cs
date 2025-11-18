using System;
using System.Collections.Generic;

namespace ProyectoFinal.Domain
{
    public class Curso
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string Descripcion { get; set; } = default!;
        public int Creditos { get; set; }

        public int TerminoId { get; set; }
        public Termino Termino { get; set; } = default!;

        public ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
        public ICollection<DocenteCurso> Docentes { get; set; } = new List<DocenteCurso>();

        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreadorId { get; set; } = default!;
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioModificadorId { get; set; }
    }
}
