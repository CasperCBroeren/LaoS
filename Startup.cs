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
            container.Register<IClientSocketHandler, WebsiteClientModule>().AsSingleton();
            container.Register<ISlackApi, SlackApi>().AsSingleton();
            app.UseOwin(x => x.UseNancy());
        }
    }
}