using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BrettTestWeb.Startup))]
namespace BrettTestWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
