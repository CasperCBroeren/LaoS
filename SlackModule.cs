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
        public SlackModule(IChannelMessageStore messageStore,
                          IClientSocketHandler clientSocketHandler)
        {
            this.messageStore = messageStore;
            this.clientSocketHandler = clientSocketHandler;

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
                    var message = this.Bind<Message>();
                    return HandleMessage(message);
                } 
            });
        }

        private Task<string> HandleMessage(Message message)
        {
            this.messageStore.StoreMessage(message);
            this.clientSocketHandler.SendMessageToClients(message);
            return Task.FromResult("OK");
        }

        private Task<string> HandleValidation(VerificationRequest validation)
        {  
            return Task.FromResult(validation.Challenge);
        }
    }
}
