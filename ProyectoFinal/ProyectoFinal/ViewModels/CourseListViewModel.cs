using System;

namespace ProyectoFinal.ViewModels
{
    public class CourseListViewModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string TerminoDescripcion { get; set; } = default!;
        public int Creditos { get; set; }
        public int CantidadEvaluaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
