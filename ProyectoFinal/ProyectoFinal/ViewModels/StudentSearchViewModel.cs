using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.ViewModels
{
    public class StudentSearchViewModel
    {
        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [Display(Name = "Matrícula")]
        public string? CodigoMatricula { get; set; }

        [Display(Name = "Identificación")]
        public string? Identificacion { get; set; }

        // Resultados de la búsqueda
        public List<StudentSearchResultViewModel> Resultados { get; set; } = new();
    }

    public class StudentSearchResultViewModel
    {
        public int EstudianteId { get; set; }
        public string NombreCompleto { get; set; } = default!;
        public string CodigoMatricula { get; set; } = default!;
        public string Identificacion { get; set; } = default!;
        public string Correo { get; set; } = default!;
    }
}
