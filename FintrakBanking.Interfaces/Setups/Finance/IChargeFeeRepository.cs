using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.Finance;
using FintrakBanking.ViewModels.WorkFlow;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.Finance
{
    public interface IChargeFeeRepository
    {
        bool GoForApproval(ApprovalViewModel entity);
        bool GoForFeeApproval(ApprovalViewModel entity);
        Task<IEnumerable<ChargeFeeViewModel>> GetChargeFeeAwaitingApprovals(int staffId, int companyId);
        Task<IEnumerable<ChargeFeeViewModel>> GetChargeFeeAwaitingAdminApprovals(int staffId, int companyId);
        Task<ChargeFeeViewModel> GetChargeFee(int chargeFeeId);
        IEnumerable<ChargeFeeViewModel> GetAllChargeFee();
        Task<IEnumerable<ChargeFeeViewModel>> GetAllChargeFeeByCompanyId(int companyId);
        IEnumerable<LookupViewModel> GetAllPostingType();
        IEnumerable<LookupViewModel> GetAllFeeType();
        IEnumerable<LookupViewModel> GetAllCRMSFeeType();
        IEnumerable<LookupViewModel> GetAllChargeFeeDetailType();
        Task<IEnumerable<LookupViewModel>> GetAllChargeFeeDetailClass();
        Task<bool> AddChargeFee(ChargeFeeViewModel model);
        Task<bool> UpdateChargeFee(ChargeFeeViewModel model, int chargeFeeId);
        Task<bool> DeleteChargeFee(int chargeFeeId, UserInfo user);
    }

}
