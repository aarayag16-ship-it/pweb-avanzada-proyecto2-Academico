using System;
using System.Collections.Generic;



namespace ProyectoFinal.Domain
{
    public class Estudiante
    {
        public int Id { get; set; }

        public string CodigoMatricula { get; set; } = default!;
        public string NombreCompleto { get; set; } = default!;
        public string Identificacion { get; set; } = default!;
        public DateTime FechaNacimiento { get; set; }

        public string Correo { get; set; } = default!;
        public string? UserId { get; set; }    // Id de AspNetUsers

        // Relaciones
        public ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
    }
}
