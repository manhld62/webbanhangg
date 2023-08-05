using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VanilaBakery.Startup))]
namespace VanilaBakery
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
