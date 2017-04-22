using LaoS.Interfaces;
using LaoS.Services;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.Conventions;

namespace LaoS
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        public static TinyIoCContainer Container;
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            container.Register<IChannelMessageStore, MemoryChannelMessageStore>().AsSingleton();
            container.Register<ISocketClientManager, SocketClientManager>().AsSingleton();
            container.Register<ISlackApi, SlackApi>().AsSingleton();
            container.Register<IAccountService, AzureAccountService>().AsSingleton();
            container.Register<IAppSettings, JsonFileIAppSettings>().AsSingleton();
            Container = container;
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("wwwroot", @"wwwroot"));
            base.ConfigureConventions(nancyConventions);
        }
    }
}
