using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoFinal.ViewModels
{
    public class CourseFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = default!;

        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre del curso")]
        public string Nombre { get; set; } = default!;

        [Required]
        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = default!;

        [Range(1, 20)]
        [Display(Name = "Créditos")]
        public int Creditos { get; set; }

        [Display(Name = "Cuatrimestre / Término")]
        [Required]
        public int TerminoId { get; set; }

        public IEnumerable<SelectListItem>? Terminos { get; set; }

        // Para saber en el Edit si tiene estudiantes inscritos
        public bool TieneEstudiantesInscritos { get; set; }
    }
}
