using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UpAndRunningAzure.Startup))]
namespace UpAndRunningAzure
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
