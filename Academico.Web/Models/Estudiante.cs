using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Academico.Web.Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        [Required, StringLength(50)] public string Nombre { get; set; }
        [Required, StringLength(80)] public string Apellidos { get; set; }
        [Required, StringLength(25)] public string Identificacion { get; set; }  // único
        [DataType(DataType.Date)] public DateTime FechaNacimiento { get; set; }
        [Required] public string Provincia { get; set; }
        [Required] public string Canton { get; set; }
        [Required] public string Distrito { get; set; }
        [Required, EmailAddress, StringLength(120)] public string Correo { get; set; } // único
    }
}