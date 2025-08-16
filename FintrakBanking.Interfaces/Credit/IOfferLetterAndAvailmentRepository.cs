using FintrakBanking.ViewModels.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels;
using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels.WorkFlow;

namespace FintrakBanking.Interfaces.Credit
{
    public interface IOfferLetterAndAvailmentRepository
    {
        IQueryable<CamProcessedLoanViewModel> GetApplicationsAtOfferLetter(int staffId, int companyId);
        Task<bool> AddCRMSCollateralType(int applicationId, ApprovedLoanDetailViewModel model);

        IQueryable<CamProcessedLoanViewModel> GetApplicationsAtOfferLetter(int staffId, int branchId, int companyId);

        IQueryable<CamProcessedLoanViewModel> GetApplicationsDueForAvailment(int staffId, int companyId);

        OfferLetterTemplateViewModel GenerateOfferLetterTemplate(string applicationRefNumber);

        OfferLetterTemplateViewModel GetDraftOfferLetterByApplRefNumber(string applicationRefNumber);

        IEnumerable<OfferLetterTemplateViewModel> GetAllDraftOfferLetters();

        Task<bool> SaveDraftOfferLetter(OfferLetterTemplateViewModel model);

        Task<bool> UpdateDraftOfferLetter(int documentId, OfferLetterTemplateViewModel model);

        bool SaveFinalOfferLetter(int loanApplicationId, OfferLetterTemplateViewModel model);

        Task<int> ApproveLoanAvailmentDecision(LoanAvailmentApprovalViewModel entity);

        IQueryable<OfferLetterTemplateViewModel> GetAllFinalOfferLetters();

        OfferLetterTemplateViewModel GetFinalOfferLetterByApplRefNumber(int loanApplicationId);

        //bool LogApplicationForApprovalDuringAvailment(LoanAvailmentApprovalViewModel model);

        Task<Form3800ViewModel> GenerateForm3800Template(string applicationRefNumber);

        Task<WorkflowResponse> ApproveOfferLetterGeneration(LoanAvailmentApprovalViewModel entity);

        bool ForwardBondsAndGuarantee(ForwardViewModel entity);

        bool UpdateFinalOfferLetter(int loanApplicationId, OfferLetterTemplateViewModel model);

        Task<bool> OfferLetterRejection(ForwardViewModel entity);

       // bool OfferLetterReferBack(ApprovalViewModel model);

        Task<IEnumerable<CommentOnLoanAvailmentViewModel>> GetCommentOnLoanAvailment(string applicationRefNumber);

        Task<Form3800ViewModel> GenerateForm3800TemplateLMS(string refNumber);

        Task<bool> SendBackToBusinessAvailment(LoanAvailmentApprovalViewModel entity);

       void AddOfferLetterClauses(int applicationId, int staffId,bool isLMS, bool callSaveChanges);
        Task<bool> EditOfferLetterTitle(int custimerId, string data, int staffId, int branchId);
        Task<bool> EditOfferLetterSalutation(int custimerId, string data, int staffId, int branchId);
        Task<bool> EditOfferLetterAcceptance(int applicationId, string data, bool isLMS, int staffId, int branchId);
        Task<bool> EditOfferLetterClause(int applicationId, string data, bool isLMS, int staffId, int branchId);

        Task<OfferLetterViewModel> GetOfferLetterTitle(int custimerId);
        OfferLetterViewModel GetOfferLetterSalutation(int custimerId);
        Task<OfferLetterViewModel> GetOfferLetterAcceptance(int applicationId);
        Task<OfferLetterViewModel> GetOfferLetterClause(int applicationId);
        Task<bool> ReferBackOneStep(LoanAvailmentApprovalViewModel entity);
        List<CamProcessedLoanViewModel> GetApplicationsDueForAvailmentRoute(int staffId, int companyId);
        bool IsOfferLetterGenerated(int templateId, int loanApplicationId, int staffId, int branchId);
        Task<bool> ApplyTemplateToOfferLetter(int templateId, int loanApplicationId, int staffId, int branchId);
    }
}
