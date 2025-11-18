using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProyectoFinal.Security;

namespace ProyectoFinal.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // Creamos un scope para obtener servicios
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // 1) Crear roles si no existen
            string[] roles = new[]
            {
                RoleNames.Admin,
                RoleNames.Coordinador,
                RoleNames.Docente
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        // Aquí podrías registrar logs si quisieras
                        throw new Exception($"Error creando el rol '{role}': {string.Join(", ", result.Errors)}");
                    }
                }
            }

            // 2) Crear usuario administrador por defecto
            string adminEmail = "admin@proyectofinal.local";
            string adminPassword = "Admin#2025"; // cámbiala luego

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (!createUserResult.Succeeded)
                {
                    throw new Exception($"Error creando el usuario admin: {string.Join(", ", createUserResult.Errors)}");
                }
            }

            // 3) Asegurar que tiene el rol Administrador
            var isInRole = await userManager.IsInRoleAsync(adminUser, RoleNames.Admin);
            if (!isInRole)
            {
                var addRoleResult = await userManager.AddToRoleAsync(adminUser, RoleNames.Admin);
                if (!addRoleResult.Succeeded)
                {
                    throw new Exception($"Error asignando rol admin: {string.Join(", ", addRoleResult.Errors)}");
                }
            }

            // (Opcional) Podrías crear también un Docente y un Coordinador demo
        }
    }
}
