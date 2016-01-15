using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(alipay01.Startup))]
namespace alipay01
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
