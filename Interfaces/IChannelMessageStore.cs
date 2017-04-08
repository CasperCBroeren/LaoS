using System.Collections.Generic;
using LaoS.Models;

namespace LaoS.Interfaces
{
    public interface IChannelMessageStore
    {
        IReadOnlyList<Message> GetAllPast(int amount);

        bool StoreMessage(Message message);
    }
}