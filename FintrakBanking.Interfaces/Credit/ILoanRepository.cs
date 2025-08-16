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
    public interface ILoanRepository
    {
        Task<WorkflowResponse> saveMultipleRetailLoanUnAssignmentToAgent(List<GlobalExposureApplicationViewModel> model, UserInfo user);
        Task<WorkflowResponse> saveRetailBulkLoanUnAssignmentToAgent(LoanRecoveryAssignmentViewModel model, UserInfo user);
        Task<bool> saveBulkLoanReAssignmentToAgentRem(LoanRecoveryAssignmentViewModel model, UserInfo user);
        Task<bool> RetailLoanRecoveryCommissionInternal(RetailLoanRecoveryCommissionViewModel models, UserInfo user);
        Task<bool> RetailLoanRecoveryReportCollection(RetailLoanRecoveryCommissionViewModel models, UserInfo user);
        Task<WorkflowResponse> saveMultipleLoanUnAssignmentToAgent(List<GlobalExposureApplicationViewModel> model, UserInfo user);
        Task<WorkflowResponse> saveBulkLoanUnAssignmentToAgent(LoanRecoveryAssignmentViewModel model, UserInfo user);
        Task<bool> saveBulkLoanAssignmentToAgentRem(List<GlobalExposureApplicationViewModel> models, int accreditedConsultant, DateTime? expCompletionDate, string source, string assignmentType, UserInfo user);
        Task<bool> RetailLoanRecoveryCommission(RetailLoanRecoveryCommissionViewModel models, UserInfo user);
        Task<WorkflowResponse> saveBulkLoanReAssignmentToAgent(GlobalExposureApplicationViewModel model, UserInfo user);
        Task<WorkflowResponse> saveMultipleLoanReAssignmentToAgent(List<GlobalExposureApplicationViewModel> model, UserInfo user, DateTime expCompletionDate, int accreditedConsultant, string source);
        Task<WorkflowResponse> saveMultipleRetailLoanReAssignmentToAgent(List<GlobalExposureApplicationViewModel> model, UserInfo user, DateTime expCompletionDate, int accreditedConsultant, string source);
        Task<IEnumerable<FacilityModificationViewModel>> GetLMSFacilityModificationsForApproval(int staffId);
        Task<WorkflowResponse> ApproveLMSFacilityModification(ForwardViewModel model);
        Task<FacilityModificationViewModel> GetLMSFacilityModification(int facilityModificationId);
        Task<WorkflowResponse> AddFacilityModification(FacilityModificationViewModel model);
        Task<bool> saveBulkLoanRecoveryCommission(List<LoanRecoveryCommissionBatchViewModel> models, UserInfo user);
        Task<WorkflowResponse> bulkLoanRecoveryCommissionGoForApproval(LoanRecoveryCommissionApprovalViewModel models, UserInfo user);
        Task<WorkflowResponse> bulkLoanRecoveryReportingGoForApproval(LoanRecoveryReportApprovalViewModel models, UserInfo user);
        Task<bool> saveBulkLoanRecoveryReporting(List<LoanRecoveryReportBatchViewModel> models, UserInfo user);
        Task<IEnumerable<LoanViewModel>> GetProcessLoanReviewData(int companyId, int staffId, string searchString);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetBookedLoanApplicationForBookingVerificationParam(int staffId, int companyId, string searchString);
        Task<IEnumerable<CamProcessedLoanViewModel>> getLoanFacilitiesAwaitingApprovalByParam(int companyId, int staffId, string searchString);
        Task<int> AddCollateralLiquidationRecoveryWithoutFile(CollateralLiquidationRecoveryViewModel model);
        Task<WorkflowResponse> bulkLoanAssignmentToAgentGoForApproval(GlobalExposureApplicationViewModel models, UserInfo user);
        Task<RemoveLienViewModel> GetLienRemovalLetter(int lienRemovalId);
        Task<CollateralLiquidationRecoveryViewModel> GetLiquidationReceipt(int liquidationRecoveryReceiptId);
        Task<int> AddCollateralLiquidationRecovery(CollateralLiquidationRecoveryViewModel model, byte[] buffer);
        Task<WorkflowResponse> saveBulkLoanAssignmentToAgent(List<GlobalExposureApplicationViewModel> models, int accreditedConsultant, DateTime? expCompletionDate, string source, string assignmentType, UserInfo user);
        void LogEmailAlert(string messageBody, string alertSubject, List<string> recipients, string referenceCode, int targetId, string operationMehtod);
        Task<IQueryable<LoanViewModel>> LoanPrepaymentApprovalList();
        Task<CasaBalanceViewModel> GetCASABalanceById(int casaAccountId, int companyId);
        Task<IQueryable<LoanViewModel>> SearchForLoanPrepaymentReversal(string searchQuery);
        Task<List<OverrideItemVeiwModel>> getBookingOverride(string customerCode);
        Task<LoanViewModel> GetReferedBookingFacilityRecordsById(CamProcessedLoanViewModel model);
        Task<WorkflowResponse> ReferBackBooking(ApprovalViewModel model);
        Task<IEnumerable<LoanCovenantDetailViewModel>> GetLoanApplicationDetailCovenantById(int applicationDetailId);
        Task<IEnumerable<TransactionDynamicsViewModel>> GetLoanTransactionDynamics(int loanApplicationDetailId);
        Task<decimal> getDailyInterest(decimal principal, double interestRate, int daysInAYear);
        Task<List<ProductFeeViewModel>> GetLoanProductFees(int loanBookingRequestId);
        Task<CurrencyExchangeRateViewModel> GetExchangeRate(string fromCurrencyCode, string toCurrencyCode, string rateCode);
    
        decimal getTotalInterest(decimal principal, double interestRate, int interestDaysPeriod, DayCountConventionEnum dayCountConventionId);

        //int getDaysInLoanPeriod(DateTime startDate, DateTime endDate);

        Task<IEnumerable<LookupViewModel>> GetLoanApplicationTypes();

        Task<IQueryable<LoanViewModel>> SearchForLoan(string searchQuery);
        Task<IQueryable<LoanViewModel>> SearchForLoanPrepayment(string searchQuery);
        Task<IQueryable<LoanViewModel>> SearchForLoanContingent(string searchQuery);
        Task<IQueryable<LoanViewModel>> SearchForLoanInactiveContingent(string searchQuery);

        Task<IQueryable<LoanViewModel>> SearchForFXRevolvingLoan(string searchQuery);

        IEnumerable<LoanViewModel> GetApprovedLoanReview(int companyId, int staffId);

        Task<IEnumerable<LoanViewModel>> GetApprovedLoanReviewRemedial(int userId, int companyId);
        Task<LoanViewModel> GetUnDisbursedLoanByLoanId(int loanId, int loanType);

        LoanViewModel GetDisbursedLoanByLoanId(int loanId, int loanType);

        Task<IQueryable<LoanViewModel>> SearchForFullAndFinalLoan(string searchQuery);

        Task<bool> CancelFullAndFinal(int loanId);
        Task<bool> AddExistingLoan(LoanViewModel entity);

        Task<List<LoanViewModel>> getDisbursedCommercialLoanTrancheDetailsById(int loanId);

        LoanViewModel GetDisbursedLoanByLoanId(int loanId);

        Task<LoanViewModel> GetGroupLoanByLoanId(int loanId);

        IQueryable<LoanRepaymentScheduleViewModel> RunningLoans(int customerId, int companyId);

        Task<IEnumerable<CamProcessedLoanViewModel>> GetLoanApplicationDetails(int loanApplicationDetailId, int companyId);

        Task<List<ApprovalLevelStaffViewModel>> GetLoanOperationApprovers(int operation, int companyId);

        Task<string> AddLoanBooking(LoanViewModel entity);

        bool UpdateFacilityLineStatus(LoanViewModel entity);

        Task<IEnumerable<LoanViewModel>> GetLoanByCustomer(int customerId);

        Task<LoanViewModel> GetLoan(int loanId);
        Task<List<LoanMonitoringTriggerViewModel>> GetLoanMonitoringTrigger();

        Task<IEnumerable<LoanViewModel>> FindLoan(string referenceNumberOrName, int companyId);

        IEnumerable<LoanViewModel> LoanSearch(int companyId, LoanSearchViewModel searchModel);

        Task<IEnumerable<CamProcessedLoanViewModel>> ApprovedLoansForIFF(int companyId, int staffId, int branchId);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetAvailedLoanApplicationsDueForInitiateBooking(int companyId, int staffId, int branchId);
        Task<IEnumerable<CamProcessedLoanViewModel>> getApplicationsToBeAdhocApprovedForInitiateBooking(int companyId, int staffId, int branchId);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetAvailedLoanApplicationsReadyForCrmsCode(int companyId, int staffId);

        IEnumerable<CamProcessedLoanViewModel> GetAvailedLoanApplicationsReadyForBooking(int companyId, int staffId);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetAvailedContingentFacilityBooking(int companyId, int staffId);

        Task<IEnumerable<CamProcessedLoanViewModel>> GetAvailedLoanApplicationDetailById(int staffId, int companyId, int applicationDetailId, int loanBookingRequestId);

        Task<IEnumerable<LoanViewModel>> GetBookedLoanDetails(int companyId);

        Task<IEnumerable<LoanViewModel>> GetBookedLoanDetailsByCustomerCode(string customerCode, int companyId);

        Task<IEnumerable<LoanViewModel>> GetBookedLoanDetailsByLoanReferenceNumber(string loanReferenceNumber, int companyId);

        Task<IEnumerable<LoanChargeFeeViewModel>> GetProductFees(int productId);

        Task<IEnumerable<LoanChargeFeeViewModel>> GetLoanProductChargeFee(int chargeFeeId, int productId);
        Task<IEnumerable<LoanViewModel>> GetLoanByCustomerGroup(int customerGroupId);

        Task<IEnumerable<LoanViewModel>> GetLoanFacilityBookingAwaitingApproval(int staffId, int companyId);

        Task<IEnumerable<CamProcessedLoanViewModel>> GetBookedLoanApplicationForBookingVerification(int staffId, int companyId);

        Task<IEnumerable<CamProcessedLoanViewModel>> GetFacilityLineAwaitingMaintenanceApproval(int staffId, int companyId);

        Task<IEnumerable<CamProcessedLoanViewModel>> GetdisbursedLoansApplicationDetails(int staffId, int companyId);

        Task<IEnumerable<RevolvingLoanViewModel>> GetRevolvingFacilityBookingAwaitingApproval(int staffId, int companyId);

        Task<IEnumerable<ContingentLoanViewModel>> GetContingentFacilityBookingAwaitingApproval(int staffId, int companyId);

       Task<int> GoForApproval(ApprovalViewModel entity, int loanBookingRequestId, bool isManual = false);
       Task<bool> GoForFeeOverrideApproval(ApprovalViewModel entity);

       void PostLoanFees(LoanViewModel entity);

       Task<AppraisalMemorandumLoanDetailViewModel> GetAppraisalMemorandumLoanUpdates(int appraisalMemorandumId);

       Task<IQueryable<CustomerSearchItemViewModels>> SearchCustomerCollateral(int companyId, string searchQuery);

       Task<IQueryable<CustomerViewModels>> SearchForCustomerCollateral(int companyId, string searchQuery);
 
       Task<List<loanApplicationColateralViewModel>> GetLoanApplicationCollateralsByApplicationId(int loanApplicationId);

       Task<List<CasaViewModel>> GetLoanCustomerAccounts(int customerId, int loanApplicationDetailId);

        Task<List<LoanMonitoringTriggerViewModel>> GetLoanMonitoringTriggerByLoanApplicationDetailId(int loanApplicationDetailId);
        Task<List<LoanChargeFeeViewModel>> GetLoanChargeFee(int loanId);
        List<LoanChargeFeeViewModel> GetLoanChargeFeeODF(int loanId);

        Task<List<LoanCovenantDetailViewModel>> GetLoanCovenant(int loanId);
        List<LoanChargeFeeViewModel> GetLoanChargeFee(int loanId, int loanType);
        List<LoanCovenantDetailViewModel> GetLoanCovenant(int loanId, int loanType);
        List<CollateralLoanApplication> GetLoanCollateral(int loanId, int loanType);
        Task<List<LoanDisbursementViewModel>> GetForeignLoanBeneficiaryNaration(int loanId);
        Task<List<LoanMonitoringTriggerViewModel>> GetLoanMonitoringTriggers(int loanId, int loanSystemTypeId);
        Task<bool> VerifyLegalContingentCode(string legalContingentCode, int loanApplicationDetailId);

        List<CurrentCustomerExposure> GetCurrentCustomerExposure(List<CustomerExposure> customer, int loanTypeId, int companyId);
        Task<List<LoanCAMSOLViewModel>> GetCurrentCamsolByCustomer(List<CustomerExposure> customer, int loanTypeId, int companyId);

        IEnumerable<LoanPaymentSchedulePeriodicViewModel> GetLoanScheduleByLoanId(int loanId);
        Task<IEnumerable<LoanViewModel>> GetBookedLoanDetailsWithParameters(int companyId, string param);

        Task<IEnumerable<WorkflowTrackerViewModel>> GetApprovalTrailByOperationIdAndTargetId(int operationId, int targetId, int companyId, int staffId);
        Task<IEnumerable<LoanViewModel>> SearchForLoanAndRevolvingLoan(string searchQuery, int statusId = 0);
        Task<IEnumerable<LoanViewModel>> SearchForLoanAndRevolvingLoanFeeCharge(int loanSystemTypeId, string searchQuery);

        string GenerateLoanReferenceNumber(int customerId, int productId, int productTypeId);

        Task<IEnumerable<LoanViewModel>> GetLoanReviewApplicationOverDraft(int staffId, int companyId);

        Task<LoanViewModel> GetOverdraftDetailsByLoanId(int revolvingLoanId);

        Task<IEnumerable<LoanViewModel>> GetBookedLoanDetails(int companyId, ReportSearchParamViewModel param);
        Task<IQueryable<LoanViewModel>> SearchRunningCommercialAndFXLoans(string searchQuery);
        Task<IEnumerable<RevolvingLoanViewModel>> GetRevolvingLoanTypes();
        Task<IEnumerable<RevolvingLoanViewModel>> GetTemporaryOverdrafts();
        Task<IEnumerable<LoanViewModel>> GetLoanStatus(int companyId);
        Task<IEnumerable<CustomerCompanyInfomationViewModels>> getLoanCustomerCompanyInformation(int customerId);

        #region Loan Disbursement 
        Task<IEnumerable<LoanDisbursementViewModel>> GetAllLoanDisbursement(int loanId);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetEmployerRelatedData(int staffId, int companyId, DateRange dateRange);
        // bool AddUpdateLoanDisbursement(LoanDisbursementViewModel entity);
        #endregion

        Task<List<LookupViewModel>> GetAllFrequencyType();
        Task<List<ProductViewModel>> GetLoanCommercialLoans(int companyId);
        LoanPaymentScheduleInputViewModel BuildScheduleModel(int targetId, int createdBy);
        LoanViewModel BuildDisbursementModel(int loanId, LoanPaymentScheduleInputViewModel loanInputModel, int staffId);

        void DisburseLoan(LoanViewModel entity, TwoFactorAutheticationViewModel twoFactorAuthDetails = null);

        Task<IEnumerable<CamProcessedLoanViewModel>> GetBookingRequestAwaitingApproval(int staffId, int companyId, bool isInitiation);

        Task<int> GoForBookingRequestApproval(ApprovalViewModel entity, int loanBookingRequestId);
        //WorkflowResponse GoForBookingRequestApproval(ApprovalViewModel entity, int loanBookingRequestId);

        Task<IEnumerable<LoanViewModel>> GetApprovedNonTermLoansForReview(int staffId, int companyId);
        Task<IEnumerable<LoanViewModel>> GetApprovedNonTermLoansForReviewAwaitingApproval(int staffId, int companyId);

        // IEnumerable<LoanViewModel> GetApprovedFXRevolvingLoanReview();
        // IEnumerable<LookupViewModel> GetAllCRMSRepaymentAgreementType();

        Task<List<LoanViewModel>> GetLoanApplicationExistingLoans(int applicationId);

        Task<List<CurrentCustomerExposure>> GetApplicationFacilitySummary(int applicationId);
        IEnumerable<CamProcessedLoanViewModel> GetApprovedLineReview(int staffId, int companyId);
        Task<IEnumerable<DailyInterestAccrualViewModel>> ProcessBackDatedTeamLoansInterestAccrual(DateTime effectiveDate, int loanId);
        Task<IEnumerable<LoanViewModel>> GetContingentApprovedExpiredApplication(int staffId, int companyId);
        IEnumerable<LoanViewModel> GetContingentApprovedApplication(int staffId, int companyId);
        Task<LoanViewModel> GetContingentByLoanId(int revolvingLoanId);
        // IEnumerable<LookupViewModel> GetAllCRMSRepaymentAgreementType();
        Task<IEnumerable<LoanViewModel>> GetCommercialLoanByApplicationDetailId(int loanApplicationDetailId);
        Task<IEnumerable<LoanViewModel>> GetLoanByApplicationDetailId(int loanApplicationDetailId);
        Task<IEnumerable<LoanViewModel>> GetLoanHistoryByLoanAccountNumber(string loanReferenceNumber);
        Task<IEnumerable<LoanBookingRequestViewModel>> GetLoanRequestsByApplicationDetailId(int loanApplicationDetailId);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetCustomerLines(int customerId);
        Task<decimal> getLoanInterestRateAmount(decimal principal, double interestRate, DateTime startDate, DateTime endDate, DayCountConventionEnum dayCountConventionId);
        List<LookupViewModel> GetLoanRepricingModes();
        Task<List<LoanViewModel>> GetCompletedLoans();
        Task<List<LoanViewModel>> GetCompletedLoan(string searchValue);
        Task<bool> GetChangeLoanStatusOfACompletedLoan(int loanId);
        Task<IEnumerable<LookupViewModel>> GetAllLoanStatus();
        IQueryable<LoanViewModel> SearchAllOverdraft(string searchQuery);
        Task<AccountBalanceViewModel> GetLoanBalances(int loanId, int companyId);
        Task<Tuple<List<multipleDisbursementOutputViewModel>, bool>> preBulkLoanDisbursement(byte[] file, UserInfo user, bool isFinal);
        Task<Tuple<List<MultipleInsuranceOutputViewModel>, bool>> preBulkInsurance(byte[] file, UserInfo user, bool isFinal);
        Task<IEnumerable<WorkflowTrackerViewModel>> GetApprovalTrailByOperationIdAndTargetId(int operationId, int targetId, int companyId);
       // IEnumerable<WorkflowTrackerViewModel> GetApprovalTrailByOperationIdAndTargetId(int operationId, int targetId, int companyId, int staffId);

        Task<List<multipleDisbursementOutputViewModel>> startBulkLoanDisbursement(List<multipleDisbursementOutputViewModel> models, UserInfo user);
        Task<bool> saveBulkLoanDisbursementEntries(List<multipleDisbursementOutputViewModel> models, UserInfo user);
        Task<IEnumerable<multipleDisbursementOutputViewModel>> GetpendingMultipleDisbursement();
        List<LoanViewModel> GetApprovedLoanReviewAwaitingRoute(int staffId, int companyId);
        List<LoanViewModel> GetLoanReviewApplicationOverDraftRouteAndOperations(int staffId, int companyId);
        Task<IEnumerable<LookupViewModel>> GetFullAndFinalStatus();
        Task<bool> CancelFullAndFinal(int loanId, int statusId);
        Task<List<LoanViewModel>> GetApprovedLoanReviewAwaitingOperation(int staffId, int companyId);
        Task<WorkflowResponse> saveBulkInsurancePolicyEntries(List<MultipleInsuranceOutputViewModel> models, UserInfo user);

    }
}