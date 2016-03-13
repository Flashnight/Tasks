using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClassLibrary.Core.Startup))]
namespace ClassLibrary.Core
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
