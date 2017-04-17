﻿using LaoS.Interfaces;
using LaoS.Services;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

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
    }
}
