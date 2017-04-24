using System;
using LaoS.Models;
using Nancy.ModelBinding;
using System.Threading.Tasks; 
using LaoS.Interfaces;
using Nancy.Extensions;
using Nancy.IO;

namespace LaoS
{
    public class SlackModule : Nancy.NancyModule
    {
        private ISocketClientManager clientManager;
        private IChannelMessageStore messageStore;
        private ISlackApi slackApi;
        private IAccountService settingService;

        public SlackModule(IChannelMessageStore messageStore,
                          ISocketClientManager clientManager,
                          ISlackApi slackApi,
                          IAccountService settingService)
        {
            this.messageStore = messageStore;
            this.clientManager = clientManager;
            this.slackApi = slackApi;
            this.settingService = settingService; 
 
            Get("/", args => "Hello from LaoS; Look at our Slack");
            Get("/test", x => View["index", Guid.NewGuid()]);
            Post("/eventhandler", async (args) =>  
            {
                try
                {
                    string raw = RequestStream.FromStream(Request.Body).AsString();
                    var validation = this.Bind<VerificationRequest>();
                    if (validation.Type == "url_verification")
                    {
                        return await HandleValidation(validation);
                    }
                    else
                    {
                        var message = this.Bind<EventCallback<SlackMessage>>();
                        return await HandleMessage(message);
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    Console.WriteLine(exc.StackTrace);
                    return "OK";
                }
            });
        }

        private async Task<string> HandleMessage(EventCallback<SlackMessage> eventCallback)
        {               
            var message = eventCallback.Event;
            var settings = await this.settingService.GetSettings(eventCallback.Token);
            if (settings.Channel == message.Channel)
            {
                if (!String.IsNullOrEmpty(message.User))
                {
                    message.FullUser = await this.slackApi.GetUser(eventCallback.Token, message.User);
                }
                if (message.Hidden && message.Subtype == "message_deleted")
                {
                    this.messageStore.DeleteMessage(message);
                }
                else if (message.Hidden && message.Subtype == "message_changed")
                { 
                    if (!String.IsNullOrEmpty(message.Message.Edited.User))
                    {
                        message.Message.Edited.FullUser = await this.slackApi.GetUser(eventCallback.Token, message.Message.Edited.User);
                    }
                    message = this.messageStore.UpdateMessage(message);
                }
                else
                {
                    this.messageStore.StoreMessage(message);
                }
                await this.clientManager.SendMessageToClients(message);
                Console.WriteLine($"{message.FullUser.Name}: {message.Text} ");
            }
            return "OK";
        }

        private Task<string> HandleValidation(VerificationRequest validation)
        {               
            return Task.FromResult(validation.Challenge);
        }
    }
}
