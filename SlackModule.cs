using System;
using LaoS.Models;
using Nancy.ModelBinding;
using System.Threading.Tasks;
using Slack.Webhooks.Core;

namespace LaoS
{
    public class SlackModule : Nancy.NancyModule
    {
        public SlackModule()
        {
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
                    var message = this.Bind<Slack.Webhooks.Core.SlackMessage>();
                    return HandleMessage(message);
                }
                
            });
        }

        private Task<string> HandleMessage(SlackMessage message)
        {
            return Task.FromResult("OK");
        }

        private Task<string> HandleValidation(VerificationRequest validation)
        {  
            return Task.FromResult(validation.Challenge);
        }
    }
}
