using Academico.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Academico.Web.App_Start
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync()
        {
            using (var ctx = ApplicationDbContext.Create())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));

                if (!await roleManager.RoleExistsAsync("Docente"))
                    await roleManager.CreateAsync(new IdentityRole("Docente"));

                var email = "admin.docente@demo.com";
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                    await userManager.CreateAsync(user, "Admin#1234");
                    await userManager.AddToRoleAsync(user.Id, "Docente");
                }
            }
        }
    }

}