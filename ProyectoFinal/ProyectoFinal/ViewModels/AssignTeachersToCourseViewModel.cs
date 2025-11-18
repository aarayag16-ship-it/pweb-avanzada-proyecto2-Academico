using System.Collections.Generic;

namespace ProyectoFinal.ViewModels
{
    public class AssignTeachersToCourseViewModel
    {
        public int CursoId { get; set; }
        public string CodigoCurso { get; set; } = default!;
        public string NombreCurso { get; set; } = default!;

        public List<TeacherAssignmentViewModel> Docentes { get; set; } = new();
    }
}
