using Microsoft.Owin;
using Owin;


[assembly: OwinStartupAttribute(typeof(chatbot_demo.Startup))]
namespace chatbot_demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);


            var command = new System.Diagnostics.Process();
           

            
        }
    }
}
