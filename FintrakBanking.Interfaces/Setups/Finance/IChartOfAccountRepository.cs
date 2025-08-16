using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.WorkFlow;
using FintrakBanking.ViewModels.Setups.Finance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.Finance
{
    public interface IChartOfAccountRepository
    {
        Task<IEnumerable<ChartOfAccountViewModel>> GetAllAccounts();
        Task<IEnumerable<ChartOfAccountViewModel>> GetAccountsByCategory(short accountCategoryId);
        Task<IEnumerable<LookupViewModel>> GetFinancialSatementCaptionLookup();
        Task<ChartOfAccountViewModel> GetAccountByAccountId(short accountId);
        ChartOfAccountViewModel GetTempAccountDetail(int accountId);
        Task<IEnumerable<ChartOfAccountViewModel>> GetAccountsAwaitingApprovals(int staffId, int companyId);
        bool GoForApproval(ApprovalViewModel entity);
        Task<bool> AddTempAccount(ChartOfAccountViewModel account);
        Task<bool> IsAccountCodeAlreadyExist(string accountCode);
        Task<bool> IsTempAccountExist(string accountCode);
        Task<bool> UpdateAccount(short accountId, ChartOfAccountViewModel account);
        Task<bool> DeleteAccount(short accountId, UserInfo user);
        Task<IEnumerable<ChartOfAccountClassViewModel>> GetChartOfAccountClasses();
        Task<string> GetAccountNameByAccountCode(string accountCode);
        Task<int> GetAccountDefaultCurrency(int glAccountId, int companyId);
    }
}
