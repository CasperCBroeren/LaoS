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

            Get("/", args => "Hello from LaoS; Look at our Slack");
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

        private Task<string> HandleMessage(EventCallback<Message> eventCallback)
        {
            Console.WriteLine($"{eventCallback.Event.User}: {eventCallback.Event.Text} ");
            this.messageStore.StoreMessage(eventCallback.Event);
            this.clientSocketHandler.SendMessageToClients(eventCallback.Event);
            return Task.FromResult("OK");
        }

        private Task<string> HandleValidation(VerificationRequest validation)
        {               
            return Task.FromResult(validation.Challenge);
        }
    }
}
