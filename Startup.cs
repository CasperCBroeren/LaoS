using LaoS.Interfaces;
using LaoS.Services;
using Microsoft.AspNetCore.Builder;
using Nancy.Owin;
using Nancy.TinyIoc;

namespace LaoS
{
    internal class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            var container = TinyIoCContainer.Current;
            container.Register<IChannelMessageStore, MemoryChannelMessageStore>().AsSingleton();
            container.Register<IClientSocketHandler, ClientSocketHandler>().AsSingleton();
            container.Register<ISlackApi, SlackApi>().AsSingleton();
            container.Register<IAccountService, AzureAccountService>().AsSingleton();
            container.Register<IAppSettings, JsonFileIAppSettings>().AsSingleton();

            app.UseWebSockets();
            app.UseMiddleware<ClientSocketHandler>();

            app.UseOwin(x => x.UseNancy());

        }
    }
}