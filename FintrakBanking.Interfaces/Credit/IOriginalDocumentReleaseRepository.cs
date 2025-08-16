using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface IOriginalDocumentReleaseRepository
    {
        Task<IEnumerable<CollateralCashReleaseViewModel>> GetCashSecurityReleaseSearch(string searchString);
        WorkflowResponse SubmitCashSecurityReleaseApproval(CollateralCashReleaseViewModel model);
        Task<IEnumerable<CollateralCashReleaseViewModel>> GetRejectedAndReferredCashSecurityRelease(int staffId);
        Task<IEnumerable<CollateralCashReleaseViewModel>> GetCashSecurityReleaseForApproval(int staffId);
        Task <WorkflowResponse> GoForGuaranteeCashApproval(CollateralCashReleaseViewModel entity);
        Task<WorkflowResponse> GoForCashSecurityApproval(CollateralCashReleaseViewModel entity);
        Task<IEnumerable<OriginalDocumentReleaseViewModel>> GetSecurityReleaseSearch(string searchString);
        bool AddOriginalDocumentGuaranteeRelease(IEnumerable<OriginalDocumentReleaseViewModel> model);
        Task<WorkflowResponse> GoForGuaranteeApproval(IEnumerable<OriginalDocumentReleaseViewModel> entity);
        bool AddOriginalDocumentRelease(IEnumerable<OriginalDocumentReleaseViewModel> model);
        IEnumerable<OriginalDocumentReleaseViewModel> GetOriginalAllDocmentRelease(int id);
        bool saveChanges();
        Task<IEnumerable<OriginalDocumentReleaseViewModel>> GetLeaseDocumentForApproval(int staffId);
        WorkflowResponse GoForApproval(IEnumerable<OriginalDocumentReleaseViewModel> model);
        Task<WorkflowResponse> GoForApproval(OriginalDocumentReleaseViewModel model);

        Task<WorkflowResponse> SubmitApproval(OriginalDocumentReleaseViewModel model);
        bool UpdateOriginalDocumentRelease(OriginalDocumentReleaseViewModel mod);
        Task<IEnumerable<OriginalDocumentReleaseViewModel>> GetRejectedAndReferredSecurityRelease(int staffId);
        bool reinitiateSecurityRelease(int id, int staffId, int companyId);
        IEnumerable<DocumentUploadViewModel> GetReleasedDocUploadIds(int operationId, int targetId, int staffId);
        IEnumerable<DocumentUploadViewModel> GetAvailableDocumentsForReleease(int operationId, int targetId, int staffId);
    }
}
