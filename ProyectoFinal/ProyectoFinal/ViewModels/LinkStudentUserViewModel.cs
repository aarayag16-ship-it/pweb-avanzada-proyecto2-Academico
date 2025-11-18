using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoFinal.ViewModels
{
    public class LinkStudentUserViewModel
    {
        public string UserId { get; set; } = default!;
        public string UserEmail { get; set; } = default!;

        public int? EstudianteId { get; set; }

        public List<SelectListItem> Estudiantes { get; set; } = new();
    }
}
