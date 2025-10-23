using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Academico.Web.Models
{
    public class Termino
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Nombre { get; set; }      // "2025-Q3"
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

    }
}