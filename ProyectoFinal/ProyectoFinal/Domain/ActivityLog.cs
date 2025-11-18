using System;

namespace ProyectoFinal.Domain
{
    public class ActivityLog
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; } = default!;
        public string UsuarioNombre { get; set; } = default!;

        public string Accion { get; set; } = default!;
        public string Modulo { get; set; } = default!;
        public DateTime FechaHora { get; set; }
        public string Ip { get; set; } = default!;

        public string? DatosExtra { get; set; }
    }
}
