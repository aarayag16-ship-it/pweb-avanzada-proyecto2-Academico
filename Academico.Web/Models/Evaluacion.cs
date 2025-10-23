using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Academico.Web.Models
{
    public class Evaluacion
    {
        public int Id { get; set; }
        [Required] public int EstudianteId { get; set; }
        [Required] public int TerminoId { get; set; }
        [Required] public int CursoId { get; set; }
        [Range(0, 100)] public decimal Nota { get; set; }
        [Required, StringLength(50)] public string TipoParticipacion { get; set; } // "Alta/Media/Baja"
        [Required, StringLength(20)] public string Estado { get; set; }            // "Aprobado"/"Reprobado"
        [StringLength(500)] public string Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public virtual Estudiante Estudiante { get; set; }
        public virtual Termino Termino { get; set; }
        public virtual Curso Curso { get; set; }
    }
}