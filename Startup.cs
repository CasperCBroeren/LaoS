using LaoS.Interfaces;
using Microsoft.AspNetCore.Builder;
using Nancy.Owin;

namespace LaoS
{
    internal class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Map("/main", branch => branch.UseOwin(x => x.UseNancy()));

            app.Map("/socket", branch =>
            branch.UseMiddleware<ClientSocketHandler>(CustomBootstrapper.Container.Resolve<ISocketClientManager>())
            .UseWebSockets()
            );
        }
    }
}