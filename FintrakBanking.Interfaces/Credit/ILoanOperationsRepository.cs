using FintrakBanking.Common.Enum;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.CASA;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Finance;
using FintrakBanking.ViewModels.Setups.General;
using FintrakBanking.ViewModels.ThridPartyIntegration;
using FintrakBanking.ViewModels.WorkFlow;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
//using FintrakBanking.ViewModels.Operations;

namespace FintrakBanking.Interfaces.Credit
{
    public interface ILoanOperationsRepository
    {
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetBulkUnassignmentRetailRecoveryFromAgentAwaitingApproval(int staffId, int companyId);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetBulkRetailRecoveryToAgentAwaitingApproval(int staffId, int companyId);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> getAllUnassignLoansRecoveryAnalysisByAgent(int staffId, int companyId, int accreditedConsultantId, int referenceId);
        WorkflowResponse GoForBulkUnassignLoansFromAgentApproval(List<BulkRecoveryApprovalViewModel> entity, UserInfo user, int approvalStatusId, string comment);
        WorkflowResponse GoForUnassignLoansFromAgentApproval(ApprovalViewModel entity);
        IEnumerable<LoanReviewOperationApprovalViewModel> GetBulkUnassignmentRecoveryFromAgentAwaitingApproval(int staffId, int companyId);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> getAllUnassignedRecoveryOperationByAgent(string source, int staffId, int companyId);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetLoanOperationByLoanReferenceAwaitingApproval(int staffId, int companyId, string searchString);
        Task<IEnumerable<RecoveryCollectionsViewModel>> GetAllRecoveryCustomersAssignedToAgent(int recoveryAgent);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> getAllLoansRecoveryAnalysisByAgentRemedial(int staffId, int companyId, int accreditedConsultantId, string referenceId);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> getAllLoansForRecoveryAnalysisBySingleAgent(string source, int staffId, int companyId);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> getAllLoansForExternalRecoveryAnalysisByAgent(string source, int staffId, int companyId);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> getAllLoansForRecoveryAnalysisByAgent(string source, int staffId, int companyId);
        Task<IEnumerable<RetailLoanRecoveryCommissionViewModel>> getAllInternalRecoveryCommissonByAgents(int staffId, int companyId);
        Task<IEnumerable<AccreditedConsultantsViewModel>> GetAllInternalRecoveryAgents(int staffId, int companyId, DateTime month);
        Task<IEnumerable<AccreditedConsultantsViewModel>> GetAllRecoveryAgents(int staffId, int companyId);
        Task<IEnumerable<RetailLoanRecoveryCommissionViewModel>> getAllRecoveryReportCollectionByAgents(int staffId, int companyId);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> getAllPendingEmailAlert(string source, int staffId, int companyId);
        Task<bool> generateRecoveryMailToAgents(string source, int staffId, int companyId);
        WorkflowResponse GoForBulkAssignLoansToAgentApproval(List<BulkRecoveryApprovalViewModel> entity, UserInfo user, int approvalStatusId, string comment);
        Task<IEnumerable<RetailLoanRecoveryCommissionViewModel>> getAllRecoveryCommissonByAgents(int staffId, int companyId);
        IEnumerable<MultipleInsuranceOutputApprovalViewModel> GetBulkInsuranceUploadRejectedApproval(int staffId, int companyId);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> GetAllLoansRecoveredByAgents(int staffId, int companyId);
        Task<bool> GoForDocumentationFillingApproval(ApprovalViewModel entity);
        Task<bool> GoForLmsDocumentationFillingApproval(ApprovalViewModel entity);
        Task<WorkflowResponse> GoForBulkInsuranceUploadApproval(ApprovalViewModel entity);
        Task<WorkflowResponse> GoForMultipleBulkInsuranceUploadApproval(List<MultipleInsuranceOutputViewModel> entity, UserInfo user, int approvalStatusId, string comment);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetLoanOperationDocumentationLosApproval(int staffId, int companyId);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetLoanOperationDocumentationLmsApproval(int staffId, int companyId);
        Task<IEnumerable<MultipleInsuranceOutputApprovalViewModel>> GetBulkInsuranceUploadAwaitingApproval(int staffId, int companyId);
        Task<WorkflowResponse> GoForRecoveryCommissionApproval(ApprovalViewModel entity);
        Task<IEnumerable<LoanRecoveryCommissionApprovalViewModel>> GetBulkRecoveryCommissionAwaitingApproval(int staffId, int companyId);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> getAllLoansRecoveryCommissionByReference(int staffId, int companyId, string referenceId);
        Task<IEnumerable<LoanRecoveryCommissionApprovalViewModel>> GetBulkRecoveryCommissionAwaitingApprovalList(int staffId, int companyId);
        Task<IEnumerable<LoanRecoveryCommissionApprovalViewModel>> BulkRecoveryCommissionApplicationList(int staffId, int companyId);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> getAllPaymentRecoveredCommissionByAgent(int staffId, int companyId);
        Task<IEnumerable<LoanRecoveryReportApprovalViewModel>> GetBulkRecoveryReportingAwaitingApproval(int staffId, int companyId);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> GetAllLoansRecoveredByAgentForReporting(int staffId, int companyId);
        Task<IEnumerable<LoanRecoveryReportApprovalViewModel>> GetBulkRecoveryReportingAwaitingApprovalList(int staffId, int companyId);
        Task<WorkflowResponse> GoForRecoveryReportingApproval(ApprovalViewModel entity);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> getAllLoansRecoveryReportingByReference(int staffId, int companyId, string referenceId);
        Task<IEnumerable<LoanRecoveryReportApprovalViewModel>> BulkRecoveryReportingApplicationList(int staffId, int companyId);
        Task<bool> CreditDocumentationFillingLos(CreditDocumentationViewModel model);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetLoanOperationDocumentationLos(int staffId, int companyId);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetLoanOperationDocumentationLosSearch(int staffId, int companyId, string searchString);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetAllCompletedLoanOperationDocumentationLos(int staffId, int companyId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> BulkRecoveryToAgentAwaitingApprovalList(string source, int staffId, int companyId);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetBulkRecoveryToAgentAwaitingApprovalList(string source, int staffId, int companyId);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> getAllLoansRecoveryAnalysisByAgent(int staffId, int companyId, int accreditedConsultantId, string referenceId);
        IEnumerable<LoanReviewOperationApprovalViewModel> GetBulkRecoveryToAgentAwaitingApproval(int staffId, int companyId);
        WorkflowResponse GoForAssignLoansToAgentApproval(ApprovalViewModel entity);
        Task<int> GoForLienRemovalApproval(ApprovalViewModel entity);
        Task<IEnumerable<RemoveLienViewModel>> GetLienRemovalDocuments(int lienRemovalId);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetLienRemovalAwaitingApproval(int staffId, int companyId);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetLienSearchData(string searchString);
        Task<int> AddRequestUnLienODAccount(RemoveLienViewModel model, byte[] buffer);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetAllLoansOperationWriteOffAnalysis(int staffId, int companyId);

        Task<List<LoanViewModel>> GetCurrentPrepayment(int companyId);

        Task<List<LoanViewModel>> GetRunningPrepaymentLoans(int companyId, int loanId);

        Task<List<LoanViewModel>> AddBulkPrepaymentReversal(LoanReviewOperationViewModel model, int companyId);
        Task<bool> AddBulkPrepaymentReversalData(LoanViewModel data, int batchCode, DateTime applicationDate);
        string GetTransactionReferenceNo();
        Task<CollectionsRetailComputationVariableSetupViewModel> getAllRecoveryComputationVariables(int staffId, int companyId);
        IEnumerable<GlobalExposureApplicationViewModel> GetAllLoansRecoveredByAgent(int staffId, int companyId);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> getAllLoansOperationRecoveryAnalysisByAgent(string source, int staffId, int companyId);
        Task<IEnumerable<GlobalExposureApplicationViewModel>> GetLoanOperationRecoveryAnalysis(int staffId, int companyId);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetLoanOperationDocumentation(int staffId, int companyId);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetLoanOperationDocumentationSearch(int staffId, int companyId, string searchString);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetCompletedLoanOperationDocumentation(int staffId, int companyId, DateTime startDate, DateTime endDate);
        Task<bool> CreditDocumentationFilling(CreditDocumentationViewModel model);
        bool UpdateLoanClassification(DateTime applicationDate, int companyId);
        bool DoesChargeFeeExist(int loanId, int operationTypeId, int chargeFeeId);
        bool DoesOperationExist(int loanId, int operationTypeId, int loanSystemTypeId);
        bool IsOperationReffered(int loanId, int operationTypeId, int loanSystemTypeId);

        Task<WorkflowResponse> GoForApproval(ApprovalViewModel entity);
        bool AddCollateralSearchLien(CasaLienViewModel model);
        string GetCollateralLoanNewRefernceNumber(ApprovalViewModel model);

        Task<LoanViewModel> getPrincipalAndInterestDate(int companyId, string refNo, DateTime effectiveDate);

        bool DailyWrittenOffFacilityAccrual(DateTime applicationDate, int companyId, int staffId);

        TBL_LOAN GetLoanInformation(int loanid);

        Task<bool> GetRepaymentDate(int loanId);

        bool FullAndFinalCompleteWriteOff(int loanId, LoanPaymentRestructureScheduleInputViewModel loanInput, TwoFactorAutheticationViewModel twoFactorAuth, DateTime applicationDate, int staffId);

        decimal GetCollateralSearchChargeAmount(int stateId);
        bool AddOperationReview(LoanReviewOperationViewModel model);
        Task<IEnumerable<LoanOperationTypeViewModel>> GetOperationType(bool isFinalOperation);

        bool UpdateLoanClassification(DateTime applicationDate, int companyId, int staffId);

        bool ProcessGlobalInterestRepricing(DateTime effectiveDate, int productPriceIndexID, short staffId);

        bool ProcessReleaseLien(DateTime applicationDate, int companyId, int staffId);

        bool ProcessContingentLiabilityTerminationAtMaturity(DateTime date);

        void ProcessAutomaticInterestRepricing(DateTime applicationDate, int staffId, int companyId);

        bool LoanRecoveryCompletion(int loanId, LoanPaymentRestructureScheduleInputViewModel loanInput, TwoFactorAutheticationViewModel twoFactorAuth, DateTime applicationDate, int staffId);

        LoanViewModel GetRunningLoanOpeningBalance(int companyId, string refNo, DateTime effectiveDate);

        bool ContingentLiabilityTenorExtension(TwoFactorAutheticationViewModel twoFactorAuth, LoanPaymentRestructureScheduleInputViewModel model, string approvalComment);

        bool ContingentLiabilityAmountReduction(TwoFactorAutheticationViewModel twoFactorAuth, LoanPaymentRestructureScheduleInputViewModel model, string approvalComment);

        bool LoanRecoveryPayment(int loanId, LoanPaymentRestructureScheduleInputViewModel loanInput, TwoFactorAutheticationViewModel twoFactorAuth, DateTime applicationDate, int staffId);

        List<LoanPaymentSchedulePeriodicViewModel> GeneratePrepaymentSchedule(LoanPaymentScheduleInputViewModel loanInput);

        List<DailyInterestAccrualViewModel> ProcessDailyTermLoansInterestAccrual(DateTime applicationDate, int companyId, int staffId, FinTrakBankingContext context);
        IEnumerable<DailyInterestAccrualViewModel> ProcessDailyAuthorisedOverdraftInterestAccrual(DateTime applicationDate);
        IEnumerable<DailyInterestAccrualViewModel> ProcessDailyUnauthorisedOverdraftInterestAccrual(DateTime applicationDate);
        IEnumerable<DailyInterestAccrualViewModel> ProcessDailyInterestOnPastDueInterestAccrual(DateTime applicationDate, int companyId, int staffId);
        IEnumerable<DailyInterestAccrualViewModel> ProcessDailyInterestOnPastDuePrincipalAccrual(DateTime applicationDate, int companyId, int staffId);
        IEnumerable<DailyInterestAccrualViewModel> ProcessDailyTaxAccrual(DateTime applicationDate);
        IEnumerable<DailyInterestAccrualViewModel> ProcessDailyFeeAccrual(DateTime applicationDate, int companyId, int staffId);

        IEnumerable<LoanPastDueViewModel> ProcessUnauthorisedOverdraftInterestRepaymentPostingPastDue(DateTime applicationDate);
        IEnumerable<LoanOperationTypeViewModel> GetOperationTypeByLoanId(LoanProductTypeEnum productTypeId, LoanScheduleTypeEnum scheduleTypeId);
        IEnumerable<LoanOperationTypeViewModel> GetReviewApprovalOperationTypeByLoanId(LoanProductTypeEnum productTypeId, LoanScheduleTypeEnum scheduleTypeId);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetLoanOperationAwaitingApproval(int staffId, int companyId);
        IEnumerable<LoanReviewOperationApprovalViewModel> GetApprovedLoanOperationReview();
        Task<IEnumerable<ApprovalTrailDetailsViewModel>> GetApprovalDetails(int loanId, int OperationId);
        IEnumerable<LoanCovenantDetailViewModel> ProcessIDFExpiryAndlocking(DateTime applicationDate);
        IEnumerable<LoanCovenantDetailViewModel> ProcessOverdraftBalanceSuspensionBaseOnCovenant(DateTime applicationDate);
        IEnumerable<LoanCovenantDetailViewModel> ProcessOverdraftBalanceSuspensionBaseOnCleanUp(DateTime applicationDate);
        IEnumerable<LoanViewModel> ProcessIntervalFeeandCommissionPosting(DateTime applicationDate);
        IEnumerable<LoanCovenantDetailViewModel> ProcessCFFExpiryAndlocking(DateTime applicationDate);
        IEnumerable<LoanCovenantDetailViewModel> ProcessLPOExpiryAndlocking(DateTime applicationDate);
        IEnumerable<LoanPastDueViewModel> ProcessUnauthorisedOverdraftPrincipalRepaymentPostingPastDue(DateTime applicationDate);
        bool LoanCancellation(int loanId, DateTime applicationDate, int staffId);
        bool OverdraftTopUp(TwoFactorAutheticationViewModel twoFactorAuth, int loanId, int loanReviewOperationsId, decimal amount);
        //bool OverdraftTopUp(int loanId, decimal amount);
        IEnumerable<LimitSuspensionViewModel> ProcessNPLByBranchSuspension();
        IEnumerable<LoanRepaymentViewModel> ProcessLoanRepaymentPostingForceDebit(DateTime applicationDate, int companyId, int staffId);
        IEnumerable<LoanRepaymentViewModel> ProcessLoanRepaymentPostingPastDue(DateTime applicationDate, int companyId, int staffId);
        IEnumerable<DailyInterestAccrualViewModel> InterestSuspension(int loanId, DateTime applicationDate, int staffId);
        LoanViewModel ArchiveLoan(int loanId, int operationId, string archiveBatchCode, string changeReason);
        IEnumerable<LoanViewModel> BulkArchiveLoan();
        IEnumerable<LoanPaymentSchedulePeriodicViewModel> ArchivePeriodicSchedule(int loanId, string archiveBatchCode);
        IEnumerable<LoanPaymentScheduleDailyViewModel> ArchiveDailySchedule(int loanId, string archiveBatchCode);
        IEnumerable<LoanPaymentSchedulePeriodicViewModel> MergePeriodicSchedule(int loanId, DateTime applicationDate);
        bool LoanRephasementProcess(TwoFactorAutheticationViewModel twoFactorAuth, int loanReviewOperationsId, int loanId, int staffId, LoanSystemTypeEnum facilityType, [Optional] string approvalComment);
        IEnumerable<LoanRepaymentViewModel> ProcessLoanRepaymentPostingPastDueForInterestReview(DateTime applicationDate, int loanId);
        IEnumerable<LoanRepaymentViewModel> ProcessLoanRepaymentPostingPastDueForBulkInterestReview(DateTime applicationDate);
        IEnumerable<LoanRepaymentViewModel> ProcessAuthorisedOverdraftRepaymentPostingForceDebit(DateTime applicationDate);
        bool BulkRateReview(short priceindexId, double newRate, DateTime applicationDate, int staffId, int operationId);
        Task<IEnumerable<LoanViewModel>> GetLoanRateCustomerExcemptions(int companyId);
        Task<bool> addBulkRateLoanExcemptions(LoanViewModel model);
        Task<bool> addInterestRateChange(LoanBulkInterestReviewViewModel model);
        Task<IEnumerable<LoanBulkInterestReviewViewModel>> GetNewInterestRateReviews(int companyId);
        IEnumerable<LoanClassificationViewModel> CalculateLoanClassification(DateTime applicationDate);
        Task<LoanViewModel> GetRunningLoans(int companyId, string refNo);
        Task<LoanViewModel> GetRunningFXLoans(int companyId, string refNo);
        Task<IEnumerable<LoanOperationTypeViewModel>> GetOperationTypeByOD();
        Task<IEnumerable<LoanOperationTypeViewModel>> GetRemedialOperationType();
        Task<IEnumerable<LoanFeeOperationViewModel>> GetLoanChargeFeeByLoanId(int loanId);

        IEnumerable<LoanClassificationViewModel> CalculateOverdraftClassification(DateTime applicationDate);
        IEnumerable<LoanViewModel> LoanHistory();
        IEnumerable<RevolvingLoanViewModel> OverDraftHistory();

        Task<IEnumerable<LoanReviewIrregularScheduleViewModel>> GetLoanReviewOperationIrregularSchedule(int loanReviewOperationId);

        void ProcessGlobalInterestRepricing(DateTime effectiveDate, int productPriceIndexID, short staffId, bool isMarketInduced, int productPriceIndexGlobalId);

        bool AddOperationReviewContingent(LoanReviewOperationViewModel model);
        bool AddOperationReviewContingentWithImage(LoanReviewOperationViewModel model, byte[] buffer);
        bool SaveMainDocument(LoanReviewOperationViewModel model, int loanId, byte[] file, int loanreviewoperationId);
        Task<bool> SaveDocument(LoanReviewOperationViewModel model, byte[] file);
        bool DeleteLoanExistingOnDailyAndPeriodicSchedule(int loanId);

        Task<bool> SendEmailToRecoveryAgent(int companyId, int staffId, short branchId, int accreditedConsultantId);

        Task<LoanViewModel> GetWriteOffLoans(int companyId, string refNo);



        #region COMMERCIAL PAPER LOANS
        Task<bool> SubAllocateCommercialLoanPrincipal(subAllocationViewModel models);
        Task<IEnumerable<MaturityIntructionViewModel>> GetMaturityInstructionType();
        Task<bool> ApproveMaturityInstructionRequest(MaturityIntructionViewModel model);
        bool addMaturityInstruction(MaturityIntructionViewModel model);
        Task<IEnumerable<MaturityIntructionViewModel>> GetLoanMaturityInstructions();
        Task<bool> ApproveCommercialPaperManualRollOverRequest(MaturityIntructionViewModel model, string refNo);
        bool RolloverCommercialLoanByManualProcess(MaturityIntructionViewModel model, string refNo);
        //void CommercialPaperManualRollOver(DateTime applicationDate);        
        Task<int> addApplicationGoForApproval(ApprovalViewModel userModel);

        Task<bool> ApproveNonTermLoanTenorReviewRequest(LoanReviewViewModel userModel);
        bool ReviewNonTermLoanTenor(LoanReviewViewModel userModel);
        Task<List<LoanReviewOperationParentChildViewModel>> GetRunningCommercialLoanLines(int companyId);
        Task<bool> AproveApplicationLineRateChangeRequest(LoanReviewViewModel userModel);
        bool ReviewApplicationLineRate(LoanReviewViewModel userModel);
        Task<List<LoanReviewOperationParentChildViewModel>> GetCommercialLoansLines(int companyId);
        Task<List<LoanReviewOperationApprovalViewModel>> GetDueCommercialLoans(int companyId);
        Task<List<LoanReviewOperationApprovalViewModel>> GetDueCommercialLoansByApplicationDetailId(int companyId, int loanApplicationDetailID);
        //IEnumerable<DailyInterestAccrualViewModel> ProcessDailyCommercialPaperInterestAccrual(DateTime applicationDate);
        void CommercialPaperChangeOperativeAccount(int casaPayAccountId, int newCasaPayAccountId);
        bool CommercialPaperDetailsCancellation(string refNo, DateTime applicationDate, int staffId);
        //loanPrepaymentViewModel addCommercialLoanPrepayment(string refNo, loanPrepaymentViewModel model);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetRunningCommercialLoans(int companyId, string loanReferenceNumber);
        Task<int> LineOperationGoForApproval(ApprovalViewModel userModel);
        Task<bool> AproveApplicationLineTenorChangeRequest(LoanReviewViewModel userModel);
        Task<IEnumerable<CamProcessedLoanViewModel>> GetApplicationLineTenorChangeAwaitingApproval(int staffId, int companyId);
        Task<bool> ApproveNonTermLoanLoanRateChangeRequest(LoanReviewViewModel userModel);

        bool ReviewNonTermLoanLoanRate(LoanReviewViewModel userModel);
        Task<WorkflowResponse> ApproveApplicationLineAmountChangeRequest(LoanReviewViewModel userModel);

        bool changeApplicationLineAmount(LoanReviewViewModel userModel);
        bool GetRepaymentFromStaging();

        IEnumerable<LoanOperationTypeViewModel> GetOperationTypeByContingent();

        IEnumerable<LoanRepaymentViewModel> ProcessLoanDisbursmentRollOver(DateTime applicationDate, int companyId);

        void ProcessAutomaticCommercialLoanRollover(DateTime applicationDate, int companyId, int staffId);

        #endregion

        #region
        List<ItemValue> FlowTypes();
        #endregion
    }
}