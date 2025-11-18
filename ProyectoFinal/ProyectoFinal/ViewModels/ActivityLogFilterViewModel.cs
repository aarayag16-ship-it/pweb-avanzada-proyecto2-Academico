using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.ViewModels
{
    public class ActivityLogFilterViewModel
    {
        [Display(Name = "Usuario (correo o nombre)")]
        public string? Usuario { get; set; }

        [Display(Name = "Acción")]
        public string? Accion { get; set; }

        [Display(Name = "Módulo")]
        public string? Modulo { get; set; }

        [Display(Name = "IP")]
        public string? Ip { get; set; }

        [Display(Name = "Desde")]
        [DataType(DataType.Date)]
        public DateTime? FechaDesde { get; set; }

        [Display(Name = "Hasta")]
        [DataType(DataType.Date)]
        public DateTime? FechaHasta { get; set; }

        public List<ActivityLogItemViewModel> Resultados { get; set; } = new();
    }
}
