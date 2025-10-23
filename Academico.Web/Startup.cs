using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Academico.Web.Startup))]
namespace Academico.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
