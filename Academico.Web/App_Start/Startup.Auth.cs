using Academico.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
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
    }

}