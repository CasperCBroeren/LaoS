using LaoS.Models;
using System.Threading.Tasks;

namespace LaoS.Interfaces
{
    public interface IAccountService
    {
        Task<Account> GetAccountForTeam(string teamId);
        Task<bool> SaveAccountForTeam(Account account);
    }
}
