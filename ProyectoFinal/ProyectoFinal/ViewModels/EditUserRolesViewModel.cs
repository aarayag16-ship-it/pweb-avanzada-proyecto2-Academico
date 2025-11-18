using System.Collections.Generic;

namespace ProyectoFinal.ViewModels
{
    public class RoleSelectionViewModel
    {
        public string Name { get; set; } = default!;
        public bool Selected { get; set; }
    }

    public class EditUserRolesViewModel
    {
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;

        public List<RoleSelectionViewModel> Roles { get; set; } = new();
    }
}
