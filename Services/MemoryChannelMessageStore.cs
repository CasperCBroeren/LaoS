using System.Collections.Generic;
using LaoS.Models;
using LaoS.Interfaces;
using System.Linq;
using System;

namespace LaoS.Services
{
    public class MemoryChannelMessageStore : IChannelMessageStore
    {
        private Dictionary<string, SlackMessage> store { get; set; } = new Dictionary<string, SlackMessage>();

        public void DeleteMessage(SlackMessage message)
        {
            if (this.store.ContainsKey(message.Deleted_Ts.ToString()))
            {
                this.store.Remove(message.Deleted_Ts.ToString());
            }
        }

        public IReadOnlyList<SlackMessage> GetAllPast(int amount)
        {
            if (this.store.Count > amount)
            {
                return this.store.Values.ToList().GetRange(0, amount);
            }
            else
            {
                return this.store.Values.ToList();
            }
        }
        public bool StoreMessage(SlackMessage message)
        {
            if (!this.store.ContainsKey(message.Event_Ts.ToString()))
            {
                this.store.Add(message.Event_Ts.ToString(), message);
            }
            return true;
        }

        public SlackMessage UpdateMessage(SlackMessage message)
        {
            if (this.store.ContainsKey(message.Message.Ts.ToString()))
            {
                var update = this.store[message.Message.Ts.ToString()];
                update.Text = message.Message.Text;
                update.Subtype = message.Subtype;
                return update;
            }
            return message;
        }
    }
}