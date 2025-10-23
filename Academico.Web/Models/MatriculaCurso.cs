using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Academico.Web.Models
{
    public class MatriculaCurso
    {
        public int Id { get; set; }
        public int MatriculaId { get; set; }
        public int CursoId { get; set; }
        public virtual Matricula Matricula { get; set; }
        public virtual Curso Curso { get; set; }
    }
}