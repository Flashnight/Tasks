using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Tasks.Startup))]
namespace Tasks
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
