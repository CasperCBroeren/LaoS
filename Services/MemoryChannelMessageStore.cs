using System.Collections.Generic;
using System.Linq;
using LaoS.Models;
using LaoS.Interfaces;

namespace LaoS.Services
{
    public class MemoryChannelMessageStore : IChannelMessageStore
    {
        private List<Message> store { get; set; } = new List<Message>();
        public IReadOnlyList<Message> GetAllPast(int amount)
        {
            return this.store.GetRange(0,amount);
        }
        public bool StoreMessage(Message message)
        {
            this.store.Add(message);
            return true;
        }
    }
}