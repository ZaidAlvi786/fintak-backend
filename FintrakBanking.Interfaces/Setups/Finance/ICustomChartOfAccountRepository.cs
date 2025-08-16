using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.Finance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.Finance
{
    public interface ICustomChartOfAccountRepository
    {
        Task<CustomChartOfAccountViewModel> GetCustomChartOfAccount(int customChartOfAccountId);

        IEnumerable<CustomChartOfAccountViewModel> GetAllCustomChartOfAccount();

        IEnumerable<CustomChartOfAccountViewModel> GetAllCustomChartOfAccountByCompanyId(int companyId);

        IEnumerable<CustomChartOfAccountViewModel> GetnostroCustomChartOfAccountByCompanyId(int companyId);

        Task<bool> AddCustomChartOfAccount(CustomChartOfAccountViewModel model);

        Task<bool> UpdateCustomChartOfAccount(CustomChartOfAccountViewModel model, int customChartOfAccountId);
    }
}