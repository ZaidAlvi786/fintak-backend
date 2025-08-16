using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.General;
using FintrakBanking.ViewModels.WorkFlow;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.Approval
{
    public interface IBusinessRuleRepository
    {
        Task<IEnumerable<BusinessRuleViewModel>> GetBusinessRule(int companyId);

        Task<BusinessRuleViewModel> GetBusinessRuleById(int businessRuleId);

        Task<bool> AddBusinessRule(BusinessRuleViewModel model);

        Task<bool> UpdateBusinessRule(BusinessRuleViewModel model, int businessRuleId, UserInfo user);

        Task<bool> DeleteBusinessRule(int businessRuleId, UserInfo user);
    }
}