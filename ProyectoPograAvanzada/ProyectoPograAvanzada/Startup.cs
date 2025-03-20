using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProyectoPograAvanzada.Startup))]
namespace ProyectoPograAvanzada
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
