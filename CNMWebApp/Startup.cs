using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CNMWebApp.Startup))]
namespace CNMWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
