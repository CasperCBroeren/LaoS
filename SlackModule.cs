using System;
using LaoS.Models;
using Nancy.ModelBinding;
using System.Threading.Tasks;
using LaoS.Interfaces;
using Nancy.Extensions;
using Nancy.IO;
using System.Threading;

namespace LaoS
{
    public class SlackModule : Nancy.NancyModule
    {
        private ISocketClientManager clientManager;
        private IChannelMessageStore messageStore;
        private ISlackApi slackApi;
        private IAccountService accountService;

        public SlackModule(IChannelMessageStore messageStore,
                          ISocketClientManager clientManager,
                          ISlackApi slackApi,
                          IAccountService accountService)
        {
            this.messageStore = messageStore;
            this.clientManager = clientManager;
            this.slackApi = slackApi;


            this.accountService = accountService;

            Get("/", x => View["register"]);
            Get("/authorize", async (args) =>
            {
                if (!String.IsNullOrEmpty(Request.Query["code"]))
                {
                    var authAttempt = await slackApi.DoAuthentication(Request.Query["code"], Request.Query["state"], "https://laos.now.sh/main/authorize");
                    if (authAttempt != null && !String.IsNullOrEmpty(authAttempt.Access_Token))
                    {
                        var account = new Account(authAttempt.Team_Id, authAttempt.Team_Name, authAttempt.Access_Token);
                        await accountService.SaveAccountForTeam(account);
                        var channels = await this.slackApi.GetChannelList(account.TeamId);
                        return View["channelSelection", new StepTwoRegisterModel()
                        {
                            TeamId = account.TeamId,
                            Channels = channels
                        }];
                    }
                    else
                    {
                        return "Ohnoes, no data from slack";
                    }
                }
                else
                {
                    return "Ohnoes, no code from slack";
                }
            });
            Post("/registerOk", async args =>
            {
                var teamId = Request.Form["team_id"];
                var settings = await accountService.GetAccountForTeam(teamId);
                settings.ChannelId = Request.Form["channelSelection"];
                await accountService.SaveAccountForTeam(settings);
                return View["registerOK", teamId];
            });
            Get("/test", async args =>
            {
                var teamId = Request.Query["for"];
                var settings = await accountService.GetAccountForTeam(teamId);
                return View["test", settings];
            });
            Post("/eventhandler", async (args) =>
           {
               string raw = string.Empty;
               try
               {
                   raw = RequestStream.FromStream(Request.Body).AsString();
                   //Console.WriteLine(raw);
                   var validation = this.Bind<VerificationRequest>();

                   if (validation.Type == "url_verification")
                   {
                       return await HandleValidation(validation);
                   }
                   else
                   {
                       new Thread(async () =>
                       {
                           Thread.CurrentThread.IsBackground = true;
                           try
                           {
                              var liteMessage = this.Bind<EventCallback<LiteMessage>>();
                               if (liteMessage.Event.Type == "reaction_added" || liteMessage.Event.Type == "reaction_removed")
                               {
                                   Console.WriteLine(raw);
                                   var reactionEvent = this.Bind<EventCallback<ReactionEvent>>();
                                   await HandleReaction(reactionEvent);
                               }
                               else
                               {
                                   var message = this.Bind<EventCallback<SlackMessage>>();
                                   await HandleMessage(message);
                               }
                           }
                           catch (Exception exc)
                           {
                               Console.WriteLine(exc.Message);
                               Console.WriteLine(exc.StackTrace);
                           }
                       }).Start();
                       return "OK";

                   }
               }
               catch (Exception exc)
               {
                   Console.WriteLine("Raw: " + raw);
                   Console.WriteLine(exc.Message);
                   Console.WriteLine(exc.StackTrace);
                   return "OK";
               }
           });

        }

        private async Task HandleReaction(EventCallback<ReactionEvent> eventCallback)
        {
            var accountSettings = await this.accountService.GetAccountForTeam(eventCallback.Team_Id);
            var reaction = eventCallback.Event;
            Console.WriteLine(reaction.Type);
            if (reaction.Item.Channel == accountSettings.ChannelId)
            {
                var message = await this.messageStore.GetMessage(reaction.Item.Channel, reaction.Item.Ts);
                Console.WriteLine($"<---");
                var fullUser = await this.slackApi.GetUser(eventCallback.Team_Id, reaction.User);
                Console.WriteLine($"Message {message.Text} got reaction from {fullUser.Name}: {reaction.Reaction}!");
                if (reaction.Type == "reaction_added")
                {
                    if (message.Reactions == null) message.Reactions = new System.Collections.Generic.List<Reaction>();
                    message.Reactions.Add(new Reaction() { Name = reaction.Reaction, User = reaction.User, FullUser = fullUser.Real_Name });
                }
                else
                {
                    Console.WriteLine($"Remove {message.Reactions.RemoveAll(x => x.Name == reaction.Reaction && x.User == reaction.User)}");
                }
                Console.WriteLine($"--->");
                message = await this.messageStore.UpdateMessage(message);
                await this.clientManager.SendMessageToClients(message, eventCallback.Team_Id);
                Console.WriteLine($"storred");
            }
        }

        private async Task<string> HandleMessage(EventCallback<SlackMessage> eventCallback)
        {
            var accountSettings = await this.accountService.GetAccountForTeam(eventCallback.Team_Id);
            var message = eventCallback.Event;
            if (message.Channel == accountSettings.ChannelId)
            {
                if (!String.IsNullOrEmpty(message.User))
                {
                    message.FullUser = await this.slackApi.GetUser(eventCallback.Team_Id, message.User);
                }
                if (message.Hidden && message.Subtype == "message_deleted")
                {
                    await this.messageStore.DeleteMessage(message);
                }
                else if (message.Hidden && message.Subtype == "message_changed")
                {
                    if (message.Message.Edited != null && !String.IsNullOrEmpty(message.Message.Edited.User))
                    {
                        message.Message.Edited.FullUser = await this.slackApi.GetUser(eventCallback.Token, message.Message.Edited.User);
                    }
                    message = await this.messageStore.UpdateMessage(message);
                }
                else
                {
                    await this.messageStore.StoreMessage(message);
                }
                await this.clientManager.SendMessageToClients(message, eventCallback.Team_Id);
                Console.WriteLine($"{message.FullUser?.Name}: {message.Text} ");
            }
            return "OK";

        }

        private Task<string> HandleValidation(VerificationRequest validation)
        {
            return Task.FromResult(validation.Challenge);
        }
    }

    
}
