using System.Collections.Generic; 
using LaoS.Models;
using LaoS.Interfaces;
using System.Linq;

namespace LaoS.Services
{
    public class MemoryChannelMessageStore : IChannelMessageStore
    {
        private Dictionary<string, Message> store { get; set; } = new Dictionary<string, Message>();
        public IReadOnlyList<Message> GetAllPast(int amount)
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
        public bool StoreMessage(Message message)
        {
            if (!this.store.ContainsKey(message.Event_Ts.ToString()))
            {
                this.store.Add(message.Event_Ts.ToString(), message);
            }
            return true;
        }
    }
}