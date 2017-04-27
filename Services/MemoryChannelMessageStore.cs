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
                return this.store.Values.ToList().OrderBy(x=>x.Event_Ts).ToList();
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
            var tsOfPrev = message.Previous_Message != null ? message.Previous_Message.Ts : message.Message.Ts;
            if (this.store.ContainsKey(tsOfPrev.ToString()))
            {
                var update = this.store[tsOfPrev.ToString()];
                update.Text = message.Message.Text;
                update.Subtype = message.Subtype;
                update.Attachments = message.Message.Attachments;
                update.Previous_Message = message.Previous_Message;
                return update;
            }
            else
            {
                this.StoreMessage(message);
            }
            return message;
        }
    }
}