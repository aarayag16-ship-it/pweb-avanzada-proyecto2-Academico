using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Academico.Web.Models
{
    public class Matricula
    {
        public int Id { get; set; }
        [Required] public int EstudianteId { get; set; }
        [Required] public int TerminoId { get; set; }
        public virtual Estudiante Estudiante { get; set; }
        public virtual Termino Termino { get; set; }
        public virtual ICollection<MatriculaCurso> Cursos { get; set; } = new List<MatriculaCurso>();
    }
}