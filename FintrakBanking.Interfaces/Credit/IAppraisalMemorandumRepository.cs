using FintrakBanking.ViewModels.WorkFlow;
using FintrakBanking.ViewModels.Credit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FintrakBanking.ViewModels;
using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels.Setups.Credit;
using FintrakBanking.ViewModels.ThridPartyIntegration;
using FintrakBanking.ViewModels.Customer;
using FintrakBanking.Entities.Models;
using System;

namespace FintrakBanking.Interfaces.Credit
{
    public interface IAppraisalMemorandumRepository
    {
        Task<ProjectRiskRatingViewModel> AddProjectRiskRating(ProjectRiskRatingViewModel projectRiskRating);
        Task<bool> AddContractorTiering(ContractorTieringViewModel contractorCriteria);
        //IEnumerable<ApprovalTrailCallMemoViewModel> GetAppraisalMemorandumTrailCallMemo(int operationId);
        Task<IEnumerable<ApprovalTrailViewModel>> GetGlobalInterestRateChangeTrail(int applicationId, int operationid);
        IEnumerable<ApprovalTrailViewModel> GetCallmemoApprovalTrail(int applicationId, int operationId);
        AppraisalMemorandumViewModel GetAppraisalMemorandum(int applicationId, int staffId);

        Task<IEnumerable<DocumentationViewModel>> GetAllDocumentation(int applicationId);

        Task<AppraisalMemorandumViewModel> AddAppraisalMemorandum(AppraisalMemorandumViewModel model);

        WorkflowResponse ForwardAppraisalMemorandum(ForwardViewModel model);

        Task<WorkflowResponse> AdhocAppraisalMemorandum(ForwardViewModel model);

        Task<WorkflowResponse> LcAppraisalMemorandum(LcForwardViewModel model);

        Task<WorkflowResponse> LcReleaseMemorandum(LcForwardViewModel model);
        Task<WorkflowResponse> LcCancelationMemorandum(LcForwardViewModel model);
        Task<WorkflowResponse> LcEnhancementMemorandum(LcForwardViewModel model);
       Task<WorkflowResponse> LcIssuanceExtensionMemorandum(LcForwardViewModel model);
       Task<WorkflowResponse> LcUsanceExtensionMemorandum(LcForwardViewModel model);
       Task<WorkflowResponse> LcUssanceMemorandum(LcForwardViewModel model);
       
       WorkflowResponse LetterGenerationRequestMemorandum(LetterGenerationRequestViewModel model);
       
       String ResponseMessage(WorkflowResponse response, string itemHeading);
       //WorkflowResponse CollateralSwapMemorandum(CollateralSwapViewModel model);
   
       bool UpdateAppraisalMemorandum(AppraisalMemorandumViewModel model, int appraisalMemorandumId);
      
       Task<IEnumerable<ApprovalTrailViewModel>> GetAppraisalMemorandumTrail(int applicationId, int operationId, bool all);
       IEnumerable<ApprovalTrailViewModel> GetTrailForReferBack(int applicationId, int operationId, int currentLevelId, bool all, bool isClassified, bool isLMSCrossWorkflow = false);
        // IEnumerable<ApprovedLoanDetailViewModel> GetApprovedLoanDetail(int applicationId);
       Task<LoanApplicationDetailsViewModel> GetLoanApplicationDetail(int applicationId);
       Task<LoanApplicationDetailsViewModel> GetApprovedTrancheDetail(int bookingRequestId);
       Task<IEnumerable<LookupViewModel>> GetAllCRMSSecuredCollateralType(int companyid);
       Task<IEnumerable<LookupViewModel>> GetAllCRMSAllCollateralType(int companyid);

       Task<IEnumerable<LookupViewModel>> GetAllCRMSUnsecuredCollateralType(int companyid);
       Task<IEnumerable<LoanApplicationDetailLogViewModel>> GetLoanDetailChangeLog(int applicationId);

       Task<IEnumerable<LoanDetailsFeeViewModel>> GetLoanDetailsFee(int applicationId);

       //bool Confirmation(int type, int applicationId);

       IQueryable<LoanApplicationViewModel> GetPendingLoanApplications(int applicationId, int countryId, int branchId, int staffId, int? classId, bool isSpecific = false);
       IQueryable<LoanApplicationViewModel> GetPendingCashFlowDocumentApplication(int applicationId, int countryId, int branchId, int staffId, int? classId, bool isSpecific = false);
       Task<IEnumerable<SubsidiaryViewModel>> GetSubsidiaryPendingLoanApplications();
       Task<IEnumerable<SubsidiaryViewModel>> GetSubsidiaries();
       List<LoanApplicationViewModel> CalculateSLA(List<LoanApplicationViewModel> apps);
       IQueryable<LoanApplicationViewModel> GetPoolApplications(int operationId, int companyId, int branchId, int staffId, int? classId);
       Task<bool> AssignApplication(int approvalTrailId, int staffId, GeneralEntity entity);
       Task<bool> ChangeApplicationOwner(int loanApplicationId, int staffId, GeneralEntity entity);
       
       Task<bool> SelfAssignMultpleApplication(List<ForwardViewModel> models, GeneralEntity userEntity);
       Task<bool> ReassignMultipleRequests(List<int> models, GeneralEntity userEntity, int staffId);

       Task<bool> ReturnAssignApplicationToPool(int approvalTrailId, GeneralEntity model);

       Task<IQueryable<LoanApplicationViewModel>> GetPendingAdhocApplications(int applicationId, int countryId, int branchId, int staffId, int? classId);

       Task<IEnumerable<CurrentCommitteeViewModel>> GetCurrentCommittee(int loanApplicationId);

       Task<bool> SecretariatForwardAppraisalMemorandum(ForwardCommitteeCamViewModel entity);

       IQueryable<RegionLoanApplicationViewModel> GetRegionalLoanApplications(int staffId);

       List<PendingProductProgramViewModel> GetPendingProductProgram(UserInfo user);

       bool GetUntenoredStatus(int applicationId);

       PrivilegeViewModel GetUserPrivilege(AuthoritySignatureViewModel entity);

       PrivilegeViewModel GetUserPrivilegeByCode(AuthoritySignatureViewModel entity);

        Task<IEnumerable<MonitoringTriggersViewModel>> GetApplicationMonitoringTriggers(int applicationId);

       Task<IEnumerable<MonitoringTriggersViewModel>> SaveApplicationMonitoringTriggers(int applicationId, List<MonitoringTriggersViewModel> entity, int staffId);

       Task<bool> WorkflowTest();
       Task<string> GetAllOldApplicationReference(string data);
       Task<ApprovalTrailViewModel> GetapprovalTrailByTrailId(int approvalTrailId);

       Task<IEnumerable<RepaymentScheduleTermsViewModel>> SaveRepaymentScheduleAndTerms(RepaymentScheduleTermsViewModel entity);
       Task<IEnumerable<RepaymentScheduleTermsViewModel>> GetRepaymentScheduleAndTerms(int applicationId);
        IEnumerable<RepaymentScheduleTermSetupViewModel> GetAllSetupRepaymentTerms();
       Task<List<ProductLimitValidationViewModel>> SaveProductLimitValidation(ProductLimitValidationViewModel entity);
       Task<List<ProductLimitValidationViewModel>> GetProductLimitValidation(int applicationId, int classId);

       Task<List<RecommendedCollateralViewModel>> GetRecommendedCollateral(int applicationId, int staffId);
       Task<List<RecommendedCollateralViewModel>> AddRecommendedCollateral(RecommendedCollateralViewModel entity);
       Task<List<RecommendedCollateralViewModel>> UpdateRecommendedCollateral(RecommendedCollateralViewModel entity);
       Task<IEnumerable<MonitoringTriggersViewModel>> GetApplicationMonitoringTriggersLms(int applicationId);
       Task<IEnumerable<MonitoringTriggersViewModel>> SaveApplicationMonitoringTriggersLms(int applicationId, List<MonitoringTriggersViewModel> entity, int getStaffId);
       Task<List<RepaymentScheduleTermsViewModel>> SaveRepaymentScheduleAndTermsLms(RepaymentScheduleTermsViewModel entity);
       Task<List<RecommendedCollateralViewModel>> UpdateRecommendedCollateralLms(RecommendedCollateralViewModel entity);
       Task<List<RecommendedCollateralViewModel>> AddRecommendedCollateralLms(RecommendedCollateralViewModel entity);
       Task<List<RecommendedCollateralViewModel>> GetRecommendedCollateralLms(int applicationId);
       Task<bool> saveTranchDisbursmentApprovalLevel(TranchDisbursmentViewModel entity);
       Task<List<RecommendedCollateralViewModel>> GetRecommendedCollateralHistory(int applicationId);
       Task<List<RecommendedCollateralViewModel>> GetRecommendedCollateralHistoryLms(int applicationId);
       Task<LoanApplicationDetailsViewModel> GetSingleLoanApplicationDetail(int detailId);
       Task<LoanApplicationDetailsViewModel> GetLMSLoanApplicationDetail(int applicationId);

       Task<WorkflowResponse> GetWorkflowNextStatus(ForwardViewModel model);

       Task<WorkflowResponse> GetWorkflowNextStatusLms(ForwardReviewViewModel model);
       
       Task<IEnumerable<MonitoringTriggersViewModel>> GetApplicationMonitoringTriggersByOperationId(int operationId, int applicationDetailId);
       Task<LoanApplicationDetailsViewModel> GetLoanApplicationDetailByRefNo(string applicationReferenceNumber);
       
       void LoanStatusChangeThroughAPI(TBL_LOAN_APPLICATION loanApplication, string comment, int staffId, string statusCode, string status);
       Task<IEnumerable<FailedTransactionViewModel>> GetFailedGroupOfficeTransactions();

    }
}