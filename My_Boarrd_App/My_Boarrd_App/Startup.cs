using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(My_Boarrd_App.Startup))]
namespace My_Boarrd_App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
