using System;
using LaoS.Models;
using Nancy.ModelBinding;
using System.Threading.Tasks;
using Slack.Webhooks.Core;
using LaoS.Interfaces;

namespace LaoS
{
    public class SlackModule : Nancy.NancyModule
    {
        private IClientSocketHandler clientSocketHandler;
        private IChannelMessageStore messageStore;
        private ISlackApi slackApi;
        private IAccountService settingService;

        public SlackModule(IChannelMessageStore messageStore,
                          IClientSocketHandler clientSocketHandler,
                          ISlackApi slackApi,
                          IAccountService settingService)
        {
            this.messageStore = messageStore;
            this.clientSocketHandler = clientSocketHandler;
            this.slackApi = slackApi;
            this.settingService = settingService;
            var task = this.slackApi.GetUser("XunhEnXMHkUKKUmQuKRfaBEx", "U4QFAM1RU");
            task.Wait();
            Get("/", args => "Hello from LaoS; Look at our Slack");
            Get("/test", x => View["index"]);
            Post("/main", args =>
            { 
                var validation = this.Bind<VerificationRequest>();
                if (validation.Type == "url_verification")
                {
                    return HandleValidation(validation);
                }
                else
                {
                    var message = this.Bind<EventCallback<Message>>();
                    return HandleMessage(message);
                } 
            });
        }

        private async Task<string> HandleMessage(EventCallback<Message> eventCallback)
        {      
            var message = eventCallback.Event;
            message.FullUser = await this.slackApi.GetUser(eventCallback.Token, message.User);
            this.messageStore.StoreMessage(message);
            this.clientSocketHandler.SendMessageToClients(message);
            Console.WriteLine($"{message.FullUser.Name}: {message.Text} ");
            return "OK";
        }

        private Task<string> HandleValidation(VerificationRequest validation)
        {               
            return Task.FromResult(validation.Challenge);
        }
    }
}
