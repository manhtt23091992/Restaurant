using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PromotionCMS.Startup))]
namespace PromotionCMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
