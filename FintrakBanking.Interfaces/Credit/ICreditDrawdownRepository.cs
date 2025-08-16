using FintrakBanking.Common.Enum;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Setups.Approval;
using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.CASA;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Customer;
using FintrakBanking.ViewModels.Finance;
using FintrakBanking.ViewModels.Report;
using FintrakBanking.ViewModels.Reports;
using FintrakBanking.ViewModels.Setups.General;
using FintrakBanking.ViewModels.WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace FintrakBanking.Interfaces.Credit
{
    public interface ICreditDrawdownRepository
    {
        Task<IEnumerable<TransactionDynamicsViewModel>> GetLoanTransactionDynamics(int loanApplicationDetailId);
        Task<bool> LogApproval(ForwardViewModel model, int operationId, bool externalInitialization, int ApprovalStatusId);
        Task<WorkflowResponse> LogApprovalForMessage(ForwardViewModel model, bool externalInitialization, bool saveChanges = false);
        Task<int> GetNextLevelForBookingRequest(int applicationStatusId, List<LoanBookingRequestViewModel> entity);
        Task<CurrentCustomerExposure> GetCurrentCompanyExposure();
        Task<WorkflowResponse> GoForBookingRequestApproval(ApprovalViewModel entity, int loanBookingRequestId);
        IEnumerable<CamProcessedLoanViewModel> GetBookingRequestAwaitingApproval(int staffId, int companyId, bool isInitiation = false);
        IEnumerable<CamProcessedLoanViewModel> GetBookingRequestAwaitingAvailment(int staffId, int companyId, bool isInitiation = false);
        Task<int> GetDrawdownOperationId(int applicationDetailId);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetGlobalEmployerLoansDueForInitiateBooking(int companyId, string searchString);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetAvailedLoanApplicationsDueForInitiateBooking(int companyId, int staffId, int branchId, int customerId = 0, bool getAll = false);
        Task<IEnumerable<CamProcessedLoanViewModel>> getApplicationsToBeAdhocApprovedForInitiateBooking(int companyId, int staffId, int branchId);
        Task<WorkflowResponse> AddLoanBookingRequest(int applicationStatusId, List<LoanBookingRequestViewModel> models);
        Task<bool> setLineFacilityLegalDocumentStatus(RecommendedCollateralViewModel entity, int loanBookingRequestId, bool value);

      //  Task<IEnumerable<WorkflowTrackerViewModel>> GetApprovalTrailByOperationIdAndTargetId(int operationId, int targetId, int companyId, int staffId);
    }
}
