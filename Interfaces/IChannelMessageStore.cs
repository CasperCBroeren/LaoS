using System.Collections.Generic;
using LaoS.Models;
using System.Threading.Tasks;

namespace LaoS.Interfaces
{
    public interface IChannelMessageStore
    {
        Task<IReadOnlyList<SlackMessage>> GetAllPast(string channel, int amount);

        Task<bool> StoreMessage(SlackMessage message);
        Task DeleteMessage(SlackMessage message);
        Task<SlackMessage> GetMessage(string channel, string ts);
        Task<SlackMessage> UpdateMessage(SlackMessage message);
    }
}