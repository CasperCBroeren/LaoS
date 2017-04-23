using System.Collections.Generic;
using LaoS.Models;

namespace LaoS.Interfaces
{
    public interface IChannelMessageStore
    {
        IReadOnlyList<SlackMessage> GetAllPast(int amount);

        bool StoreMessage(SlackMessage message);
        void DeleteMessage(SlackMessage message);
        SlackMessage UpdateMessage(SlackMessage message);
    }
}