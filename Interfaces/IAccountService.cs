using LaoS.Models;
using System.Threading.Tasks;

namespace LaoS.Interfaces
{
    public interface IAccountService
    {
        Task<Account> GetSettings(string account);
        Task<bool> SaveContractToTableStorage(Account settings);
    }
}
