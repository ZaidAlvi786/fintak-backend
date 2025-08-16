using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.CASA;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.ThridPartyIntegration;
using FintrakBanking.ViewModels.WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface ICustomerCollateralRepository
    {
        Task<IEnumerable<CollateralCashReleaseViewModel>> GetCustomerCashCollateralApplications(int id);
        Task<IEnumerable<CollateralViewModel>> GetCustomerCashCollateral(int customerId, int? applicationId, int companyId, bool isLMS = false);
        Task<bool> DeleteAddedValuer(int valuerId, int createdById);
        CollateralHistory getCollateralHistoryUsage(int collateralId);
        Task<IEnumerable<CollateralCoverageViewModel>> GetProposedCustomerCollateralByCustomerIdLMS(int customerId, bool getAll = false);
        Task<bool> ProposeCollateralForUsageLMS(CollateralCoverageViewModel model);
        #region Collateral

        //int AddCollateral(CollateralViewModel entity, byte[] file);
        Task<IEnumerable<CollateralCoverageViewModel>> CalculateCoverateOfCollateralLMS(CollateralCoverageViewModel model);
        Task<bool> DeleteDuplicatedCollateral(CollateralViewModel model);
        Task<int> AddCollateral(CollateralViewModel entity);
        Task<int> AddReleaseDocument(CollateralViewModel model, byte[] file);
        Task<IEnumerable<CollateralViewModel>> GetCollateralReleaseDocument(int releaseId);
        Task<CollateralViewModel> GetReleaseSupportingDocument(int documentId);

        Task<bool> ReleaseCollateral(CollateralViewModel entity);
        Task<bool> ReleaseCollateralJobRequest(CollateralViewModel entity);
        Task<ApprovalResponse> ReleaseCollateralGoForApproval(ApprovalViewModel entity);

        Task<IQueryable<CollateralViewModel>> GetCollateralReleaseAwaitingApproval(int companyId, int staffId);

        IQueryable<CollateralViewModel> GetCollateralReleaseAwaitingJobRequest(int companyId, int branchId);

        Task<bool> UpdateCollateral(CollateralViewModel entity, int collateralId);
        Task<IEnumerable<OriginalDocumentSubmissionByFacilityViewModel>> GetCustomerFacility(int customerId);
        IEnumerable<CollateralViewModel> GetCustomerCollateral(int customerId, int? applicationId, int companyId, bool isLMS = false);
        IEnumerable<CollateralCoverageViewModel> GetProposedCustomerCollateral( int? applicationDetailId, int currencyId, int companyId);
        IEnumerable<CollateralCoverageViewModel> GetProposedCustomerCollateralByCustomerId(int customerId, bool getAll);
        IEnumerable<CollateralCoverageViewModel> GetProposedFacilitiesToCollateralByCollateralId(int collateralId);
        Task<IEnumerable<CollateralCoverageViewModel>> GetProposedCustomerCollateralByLoanApplicationDetailId(int loanApplicationDetailId);
        Task<CollateralViewModel> GetCustomerCollateralInformation(int collateralCustomerId, int companyId);
        Task<List<CollateralViewModel>> GetCustomerPropertyCollaterals(int? customerId, int companyId);

        Task<CollateralViewModel> GetCustomerCollateralByCustomerCollateralId(int customerCollateralId);

        Task<IEnumerable<CollateralViewModel>> GetTempCustomerCollateralForApproval(int companyId, int staffId);
        IEnumerable<CollateralViewModel> GetCustomerCollateral(int companyId);
        Task<CollateralViewModel> GetCollateralTypeByCollateralId(int collateralId, int typeId);
        Task<CollateralViewModel> GetTempCollateralTypeByCollateralId(int collateralId, int typeId);
        IEnumerable<CollateralViewModel> GetCollateralByCollateralTypeIdByCustomerId(int companyId, short collateralTypeId, int customerId, int thirdpartyCustomerId);
        IEnumerable<ActiveCustomerCollateralViewModel> GetActiveCustomerCollateral(int customerId);
        Task<IEnumerable<ActiveCustomerCollateralViewModel>> GetLoanCollateral(int loanId, int productTypeId);

        Task<bool> AddCollateralValuer(CollateralValuersViewModel entity);
        Task<bool> UpdateCollateralValuer(CollateralValuersViewModel entity, int id);

        Task<bool> ReleaseCollateral(int collateralMappingId, int staffId, GeneralEntity model);
        Task<bool> ApproveCollateralRelease(ApprovalViewModel entity, int staffId, GeneralEntity model);
        Task<IEnumerable<ActiveCustomerCollateralViewModel>> GetPendingCustomerCollateralRelease(int staffId);
        Task<List<InsurancePolicy>> GetCollateralInsurancePolicy(int collateralId);
        string GetInsurancePolicyCollateralReport(int trackingId);
        Task<List<InsurancePolicy>> GetCollateralInsurancePolicyReport(DateTime? startDate, DateTime? endDate, string searchString);
        IQueryable<CollateralSearchViewModel> SearchCollateral(string searchString, int companyId);
        //bool AssignCollateral(ActiveCustomerCollateralViewModel entity);

        decimal GetAccountLeinAmountForFD(string accountNumber);

        decimal GetAccountLeinAmountForCASA(string accountNumber);

        CollateralHistory getCollateralHistory(int collateralId);


        #endregion Collateral

        #region Collateral Type
        IEnumerable<CollateralTypeViewModel> GetCollateralType();
        #endregion End of Collateral Type 

        #region Seniority Of Claims
        Task<bool> AddCollateralSeniorityOfClaims(CollateralSeniorityOfClaimsViewModel entity);
        Task<bool> DeleteCollateralSeniorityOfClaims(int seniorityOfClaimId, UserInfo user);
        Task<bool> UpdateCollateralSeniorityOfClaims(int seniorityOfClaimId, CollateralSeniorityOfClaimsViewModel entity);
        IEnumerable<CollateralSeniorityOfClaimsViewModel> GetCollateralSeniorityOfClaims();
        #endregion Seniority Of Claims

        #region Listing Functions
        IEnumerable<CollateralValueBaseTypeViewModel> GetCollateralValueBaseType(short collateralType);
        Task<IEnumerable<CollateralValuersViewModel>> GetCollateralValuer(int companyId);
        Task<List<CollateralValuerTypeViewModel>> GetCollateralValuerType();
        IEnumerable<CollateralPerfectionStatusViewModel> GetCollateralPerfectionStatus();
        #endregion End Of Listing Functions

        Task<IEnumerable<LoanApplicationCollateralViewModel>> MapApplicationCollateral(ApplicationCollateralMapping entity);
        Task<bool> IsCollateralMapped(ApplicationCollateralMapping entity);
        IEnumerable<LoanApplicationCollateralViewModel> UnmapApplicationCollateral(ApplicationCollateralMapping entity);

        //  IEnumerable<CollateralLoanApplication> GetAllUnmappedCustomerCollateral(int customerId, int loanApplicationId, int companyId);
        //  IEnumerable<CollateralLoanApplication> GetAllMappedCustomerCollateral(int customerId, int loanApplicationId, int companyId);
        //  bool DeleteCollateralApplicationMapped(IEnumerable<CollateralLoanApplication> mappings, int companyId);

        #region Collateral Information View
        IEnumerable<AllCollateralViewModel> GetCollateralInformationById(int customercollateralId);


        #endregion


        int AddPropertyVistation(CollateralDocumentViewModel entity);

        Task<IEnumerable<StockCompanyViewModel>> getStockPrice();

        bool CheckForExpiredItemPolicies(DateTime currentDate);

        Task<List<CollateralViewModel>> AddGuaranteeJoinCollateral(CollateralViewModel entity, byte[] bufer);

        Task<List<InsurancePolicy>> GetCollateralInsurancePolicies(int collateralId);

        void AddTempItemInsurancePolicy(int collateralId, CollateralViewModel entity);

        Task<bool> AddNewItemInsurancePolicy(InsurancePolicy entity);

        Task<int> GoForApproval(ApprovalViewModel model);

        Task<List<InsurancePolicy>> GetTempCollateralInsurancePoliciesWaitingForApproval(int staffId);

        Task<int> GoForPolicyApproval(ApprovalViewModel model);

        Task<List<CollateralDocumentViewModel>> GetPropertyVistation(int collateralId);
        Task<List<CollateralDocumentViewModel>> GetTempPropertyVistation(int collateralId);
        Task<List<InsurancePolicy>> GetTempCollateralInsurancePolicy(int collateralId);

        Task<CasaLienViewModel> GetAccountLienDetail(string AccountNumber);
        //CasaLienViewModel GetAccountLienDetailForFD(string AccountNumber);//not implemented
        Task<bool> AddCollateralInsuranceTrackingForm(int accountOfficer, CollateralInsuranceTrackingViewModel model);
        IEnumerable<CollateralViewModel> GetCustomerCollateralByCollateralId(int companyId, int collaterId);

        Task<IEnumerable<CollateralViewModel>> GetCollateralStampToCoverValues(int customerId);

        TDAccountRecordViewModel GetFixedDepositAccountDetail(string AccpuntNumber);
        IEnumerable<CollateralViewModel> GetCustomerCollateralReport(string searchParam, int companyId);
        IEnumerable<CollateralViewModel> GetCustomerFixedDepositCollateral(string searchParam, int companyId);

        Task<bool> ProposeCollateralForUsage(CollateralCoverageViewModel model);
        Task<bool> RejectProposedCollateralForUsage(int collateralCustomerId);

        IEnumerable<CollateralUsageStatus> GetCollateralUsageStatus();

        IEnumerable<InsurancePolicy> GetInsuranceCompany();

        IEnumerable<InsurancePolicy> GetInsuranceType();

        Task<bool> AddInsurancePolicy(CollateralInsurancePolicyViewModel entity);

        Task<List<InsurancePolicy>> GetCollateralInsurancePoliciesWaitingForApproval(int staffId);
        Task<IEnumerable<InsurancePolicy>> Explore(string searchString);

        Task<WorkflowResponse> GoForInsurancePolicyApproval(ApprovalViewModel model);
        Task<IEnumerable<CollateralCoverageViewModel>> GetCollateralCoverage(int collateralSubTypeId);
        bool AddCollateralCoverage(CollateralCoverageViewModel model);
        Task<bool> DeleteCollateralCoverage(int collateralCoverageId, int createdById);
        Task<IEnumerable<CollateralCoverageViewModel>> CalculateCoverateOfCollateral(CollateralCoverageViewModel model);

        Task<bool> DeleteProposedCollateral(CollateralCoverageViewModel model);

        Task<InsuranceCompanyViewModel> GetInsuranceCompany(int id);
        Task<IEnumerable<InsuranceCompanyViewModel>> GetInsuranceCompanies();
        bool AddInsuranceCompany(InsuranceCompanyViewModel model);
        Task<bool> DeleteInsuranceCompany(int id, UserInfo user);
        Task<bool> UpdateInsuranceCompany(InsuranceCompanyViewModel model, int id, UserInfo user);
        Task<bool> DeleteInsurancePolicyType(int id, UserInfo user);
        bool AddInsurancePolicyType(InsurancePolicyTypeViewModel model);
        Task<InsuranceTypeViewModel> GetInsuranceType(int id);
        Task<IEnumerable<InsuranceTypeViewModel>> GetInsuranceTypes();
        Task<IEnumerable<CollateralTypeViewModel>> GetCollateralTypes();
        Task<IEnumerable<CollateralSubTypeViewModel>> GetCollateralSubTypes(int collateralTypeId);
        Task<IEnumerable<InsuranceStatusViewModel>> GetInsuranceStatus();
        Task<IEnumerable<InsurancePolicyTypeViewModel>> GetInsurancePolicyTypes();
        
        bool AddInsuranceType(InsuranceTypeViewModel model);
        Task<bool> DeleteInsuranceType(int id, UserInfo user);
        Task<bool> UpdateInsuranceType(InsuranceTypeViewModel model, int id, UserInfo user);
        Task<bool> UpdateInsurancePolicyType(InsurancePolicyTypeViewModel model, int id, UserInfo user);

        bool AddInsurancePolicyFile(InsurancePolicy model);
        Task<bool> DeleteInsurancePolicy(int id, UserInfo user);
        Task<bool> UpdateInsurancePolicy(int id, CollateralInsurancePolicyViewModel model);

        Task<IEnumerable<CollateralSwapViewModel>> GetAllCollateralSwaps(int staffId);
        Task<IEnumerable<CollateralSwapViewModel>> GetCollateralSwapsForApproval(int staffId);
        Task<IEnumerable<CollateralSwapViewModel>> SearchCollateralSwap(string searchString);
        Task<CollateralSwapViewModel> GetCollateralSwap(int collateralSwapId);
        CollateralSwapViewModel AddCollateralSwap(CollateralSwapViewModel model);
        Task<bool> UpdateCollateralSwap(CollateralSwapViewModel model, int id, UserInfo user);
        Task<bool> DeleteCollateralSwap(int collateralSwapId, UserInfo user);
        Task<IEnumerable<LoanApplicationDetailViewModel>> GetCollateralMappingDetails(int id);
        Task<CollateralInsurancePolicyViewModel> GetAddedInsuranceById(int id);
        WorkflowResponse CollateralSwapMemorandum(CollateralSwapViewModel model);

        String ResponseMessage(WorkflowResponse response, string itemHeading);
        Task<int> GetNextLevelForCollateralSwapAsync(int collateralSwapId, int createdBy, int companyId);


        #region collateralInsuranceRequest

        Task<bool> AddInsurancePolicyRequest(CollateralInsuranceRequestViewModel model, int? id);
        string GetReferenceNumber();
        Task<IEnumerable<CollateralViewModel>> GetInsuranceRequests(int staffId);
        Task<bool> InsuranceRequestGoForApproval(CollateralViewModel model);
        Task<InsurancePolicy> GetInsurancePolicy(int collateralId);
        Task<bool> DeleteInsuranceRequest(int insuranceRequestId);
        Task<bool> checkInsurancePolicy(InsurancePolicy model);
        Task<bool> UpdateInsurancePolicyRequest(CollateralInsuranceRequestViewModel model, int id);
        Task<string> GetLastComment(int targetId, int operationId);
        Task<bool> UpdateCollateralInsuranceTrackingForm(int getStaffId, int id, CollateralInsuranceTrackingViewModel model);
        Task<bool> GetCustomerCollateralInsuranceDetailsConfirmation(int getStaffId, int id);
        Task<bool> DeleteCustomerCollateralInsuranceDetails(int getStaffId, int id);
        #endregion
    }
}
