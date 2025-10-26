using Academico.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Academico.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Cargar config de JSON
            var path = HttpContext.Current.Server.MapPath("~/App_Data/authsettings.json");
            dynamic cfg = Newtonsoft.Json.JsonConvert.DeserializeObject(System.IO.File.ReadAllText(path));
            int maxAttempts = cfg.MaxFailedAccessAttempts;
            int lockoutMin = cfg.DefaultLockoutMinutes;

            //var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //userManager.UserLockoutEnabledByDefault = true;
            //userManager.MaxFailedAccessAttemptsBeforeLockout = maxAttempts;
            //userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(lockoutMin);

            // Cookie Auth
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                //CookieSecure = CookieSecureOption.SameAsRequest // En producción: Always + HTTPS
                 CookieSecure = CookieSecureOption.Always,          // exige HTTPS en producción
                CookieHttpOnly = true,                             // no accesible desde JS


                ExpireTimeSpan = TimeSpan.FromMinutes(60),         // duración del ticket
                SlidingExpiration = true,                          // renueva si hay actividad

                // ► Validación de identidad (opcional pero recomendable)
                Provider = new CookieAuthenticationProvider
                {
                    // Regenera identidad tras cambios de seguridad (password/roles)
                    OnValidateIdentity = SecurityStampValidator
                    .OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(20),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }

            });


        }


        public async Task<string> SeedRolesUsers()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var roleMgr = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));
                var userMgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));

                if (!await roleMgr.RoleExistsAsync("Docente"))
                    await roleMgr.CreateAsync(new IdentityRole("Docente"));

                var email = "aarayag16@gmail.com";   // usa tu correo real
                var user = await userMgr.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                    await userMgr.CreateAsync(user, "Qwer1234+");  // cámbiala luego
                }

                if (!await userMgr.IsInRoleAsync(user.Id, "Docente"))
                    await userMgr.AddToRoleAsync(user.Id, "Docente");

                await ctx.SaveChangesAsync();
            }
            return "OK";
        }


    }

}