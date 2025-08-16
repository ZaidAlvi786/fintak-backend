using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.Approval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.Approval
{
    public interface IApprovalGroupMappingRepository
    {
       Task <int> AddApprovalGroupMapping(ApprovalGroupMappingViewModel entity);

       bool UpdateApprovalGroupMapping(int operationMappingId, ApprovalGroupMappingViewModel entity);

       bool DeleteApprovalGroupMapping(int operationMappingId, UserInfo user);

       IQueryable<ApprovalGroupMappingViewModel> GetAllApprovalGroupMapping();

        Task <ApprovalGroupMappingViewModel> GetApprovalGroupMapping(int operationMappingId);

        Task<IEnumerable<ApprovalGroupMappingViewModel>> GetApprovalGroupMapping(int operationId, short? productClassId, short? productId);
        int GoForApproval(ApprovalGroupMappingViewModel model);
        Task<List<ApprovalGroupMappingViewModel>> GetTempApprovalGroupForApproval(int staffId);

    }
}
