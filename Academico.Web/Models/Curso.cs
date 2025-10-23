using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Academico.Web.Models
{
    public class Curso
    {
        public int Id { get; set; }
        [Required, StringLength(20)]
        public string Codigo { get; set; }      // único
        [Required, StringLength(100)]
        public string Nombre { get; set; }
    }
}