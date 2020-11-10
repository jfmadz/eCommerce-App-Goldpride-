using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UserRoles.Startup))]
namespace UserRoles
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
