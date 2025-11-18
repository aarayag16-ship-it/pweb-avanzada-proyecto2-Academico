using System;

namespace ProyectoFinal.ViewModels
{
    public class ActivityLogItemViewModel
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string UsuarioNombre { get; set; } = default!;
        public string Accion { get; set; } = default!;
        public string Modulo { get; set; } = default!;
        public string? DatosExtra { get; set; }
        public string Ip { get; set; } = default!;
    }
}
