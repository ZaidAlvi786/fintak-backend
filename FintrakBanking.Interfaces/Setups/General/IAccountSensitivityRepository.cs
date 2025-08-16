using FintrakBanking.ViewModels.Setups.General;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.General
{
    public interface IAccountSensitivityRepository
    {
        Task <IEnumerable<AccountSensitivityViewModel>> GetAllAccountSensitivityLevels();

        Task <AccountSensitivityViewModel> GetAccountSensitivityLevelsByLevelId(int sensitivityId);
    }
}