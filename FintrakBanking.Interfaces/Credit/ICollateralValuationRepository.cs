using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface ICollateralValuationRepository
    {
        Task<ValuationPrerequisiteViewModel> GetCollateralValuerIformations(int id);
        Task<bool> UpdateCollateralNarration(ValuationPrerequisiteViewModel model);
        Task<bool> UpdateCollateralValurerInfo(ValuationPrerequisiteViewModel model);
        Task<ValuationPrerequisiteViewModel> GetAllCollateralValuerIformationById(int id);
        Task<List<ValuationPrerequisiteViewModel>> GetAllValuationPrerequisitesListById(int staffId, int collateralValuationId);
        Task<IEnumerable<ValuationPrerequisiteViewModel>> GetAllValuationRequestList();
        Task<CollateralValuationViewModel> AddCollateralValuation(CollateralValuationViewModel model);
        Task<ValuationPrerequisiteViewModel> AddValuationPrerequisite(ValuationPrerequisiteViewModel model);
        Task<bool> UpdateValuationPrerequisite(int valuationPrerequisiteId, ValuationPrerequisiteViewModel model);
        Task<CollateralValuationViewModel> GetCollateralValuation(int collteralValuationId);
        Task<List<CollateralValuationViewModel>> GetAllCollateralValuations(int collateralId);
        Task<List<ValuationPrerequisiteViewModel>> GetAllValuationPrerequisitesById(int staffId, int collateralValuationId);
        Task<WorkflowResponse> GoForCollateralValuationApproval(ValuationPrerequisiteViewModel entity);
        String ResponseMessage(WorkflowResponse response, string itemHeading);
        Task<IEnumerable<ValuationPrerequisiteViewModel>> GetAllValuationRequest(int staffId);
        Task<IEnumerable<ValuationPrerequisiteViewModel>> GetCollateralValuationRequestWaitingForApproval(int staffId);
        WorkflowResponse SubmitApproval(ValuationPrerequisiteViewModel model);
        Task<bool> AddCollateralValurerInfo(ValuationPrerequisiteViewModel model);
        Task<List<ValuationPrerequisiteViewModel>> GetAllCollateralValuerIformation();
        Task<List<ValuationPrerequisiteViewModel>> GetCollateralValuerIformation(int id);
        Task<bool> DeleteValuationPrerequisite(int valuationPrerequisiteId, UserInfo user);
        Task<List<ValuationPrerequisiteViewModel>> GetCollateralValuationPrerequisiteById(int staffId, int valuationPrerequisiteId);
        Task<bool> UpdateValuationPrerequisiteStatus(int valuationPrerequisiteId, UserInfo user);
        Task<List<ValuationPrerequisiteViewModel>> SearchForCollateralValuation(string searchString);
    }
}
