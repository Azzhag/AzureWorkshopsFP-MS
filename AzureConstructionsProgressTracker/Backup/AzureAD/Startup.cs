using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzureConstructionsProgressTracker.Startup))]
namespace AzureConstructionsProgressTracker
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
