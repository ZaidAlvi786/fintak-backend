using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface IFacilityModificationRepository
    {
        Task<FacilityModificationViewModel> GetFacilityModification(int id);

        Task<IEnumerable<FacilityModificationViewModel>> GetFacilityModificationsForApproval(int staffId);
        Task<WorkflowResponse> AddFacilityModification(FacilityModificationViewModel model);
        Task<WorkflowResponse> ApproveFacilityModification(ForwardViewModel model);
        bool UpdateFacilityModification(FacilityModificationViewModel model, int id, UserInfo user);

        bool DeleteFacilityModification(int id, UserInfo user);
    }
}
