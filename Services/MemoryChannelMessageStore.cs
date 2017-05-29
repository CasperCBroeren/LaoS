using System.Collections.Generic;
using LaoS.Models;
using LaoS.Interfaces;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace LaoS.Services
{
    public class MemoryChannelMessageStore : IChannelMessageStore
    {
        private Dictionary<string, SlackMessage> store { get; set; } = new Dictionary<string, SlackMessage>();

        public Task DeleteMessage(SlackMessage message)
        {
            if (this.store.ContainsKey(message.Deleted_Ts))
            {
                this.store.Remove(message.Deleted_Ts);
            }
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<SlackMessage>> GetAllPast(string channel, int amount)
        {
            IReadOnlyList<SlackMessage> result = null;
            if (this.store.Count > amount)
            {
                result = this.store.Values.ToList().GetRange(0, amount);
            }
            else
            {
                result = this.store.Values.ToList().OrderBy(x=>x.Event_Ts).ToList();
            }
            return Task.FromResult(result);
        }

        public Task<SlackMessage> GetMessage(string channel, string ts)
        {
            return Task.FromResult(this.store.ContainsKey(ts) ? this.store[ts] : null);
        }

        public Task<bool> StoreMessage(SlackMessage message)
        {
            if (!this.store.ContainsKey(message.Event_Ts))
            {
                this.store.Add(message.Event_Ts, message);
            }
            return Task.FromResult(true);
        }

        public Task<SlackMessage> UpdateMessage(SlackMessage message)
        {
            SlackMessage result = message;
            var tsOfPrev = message.Previous_Message != null ? message.Previous_Message.Ts : message.Message.Ts;
            if (this.store.ContainsKey(tsOfPrev))
            {
                var original = this.store[tsOfPrev];
                original.Text = message.Message.Text;
                original.Subtype = message.Subtype;
                original.Attachments = message.Message.Attachments;
                original.Previous_Message = message.Previous_Message;
                result = original;
            }
            else
            {
                this.StoreMessage(message);
            }
            return Task.FromResult(message);
        }
    }
}