using System;
using System.Collections.Generic;

namespace ProyectoFinal.ViewModels
{
    public class AcademicRecordEntryViewModel
    {
        public string TerminoDescripcion { get; set; } = default!;
        public string CursoCodigo { get; set; } = default!;
        public string CursoNombre { get; set; } = default!;
        public decimal NotaFinal { get; set; }
        public string Estado { get; set; } = default!;
        public DateTime FechaRegistro { get; set; }
    }

    public class AcademicHistoryViewModel
    {
        public int EstudianteId { get; set; }
        public string NombreCompleto { get; set; } = default!;
        public string CodigoMatricula { get; set; } = default!;
        public string Identificacion { get; set; } = default!;
        public string Correo { get; set; } = default!;

        public List<AcademicRecordEntryViewModel> Registros { get; set; } = new();
    }
}
