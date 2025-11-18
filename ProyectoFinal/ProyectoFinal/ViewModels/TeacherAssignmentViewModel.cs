namespace ProyectoFinal.ViewModels
{
    public class TeacherAssignmentViewModel
    {
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string NombreUsuario { get; set; } = default!;
        public bool Asignado { get; set; }

        // Opcional: un campo para horario si luego lo quieres usar
        public string? Horario { get; set; }
    }
}
