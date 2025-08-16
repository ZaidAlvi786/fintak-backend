using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Customer;
using System.Collections.Generic;
using System.Threading.Tasks;
using FintrakBanking.ViewModels.CreditLimitValidations;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.General;

namespace FintrakBanking.Interfaces.CreditLimitValidations
{
    public interface ICreditLimitValidationsRepository
    {
        Task<IEnumerable<ContractorCriteriaViewModel>> getContractorTieringForEdit(int contractorTieringId);
        Task<IEnumerable<ContractorCriteriaOptionViewModel>> getAllContractorCriteriaOption();
        Task<bool> UpdateContractorCriteriaOption(ContractorCriteriaOptionViewModel entity);
        Task<bool> AddContractorCriteriaOption(ContractorCriteriaOptionViewModel entity);
        Task<IEnumerable<ContractorCriteriaViewModel>> getAllCriteriaList();
        Task<IEnumerable<ProjectRiskRatingViewModel>> getProjectRiskRatingByApplicationDetailId(int loanApplicationId, int loanApplicationDetailId, int loanBookingRequestId);
        Task<IEnumerable<ProjectRiskRatingViewModel>> getProjectRiskRatingByApplicationAndApplicationDetailId(int loanApplicationId, int loanApplicationDetailId);
        Task<IEnumerable<ProjectRiskRatingCategoryViewModel>> getAllProjectRiskRatingByCategories();
        Task<IEnumerable<ContractorTieringViewModel>> getContractorTieringByApplicationAndCustomer(int loanApplicationId, int customerId);
        Task<IEnumerable<ContractorTieringViewModel>> getContractorTieringByApplication(int loanApplicationId, int customerId);
        IEnumerable<ContractorCriteriaViewModel> getAllContractorCriteria();
        Task<IEnumerable<ProjectRiskRatingCriteriaViewModel>> getAllProjectRiskRatingCriteria();
        Task<bool> UpdateProjectRiskCategory(ProjectRiskRatingCategoryViewModel entity);
        Task<bool> AddProjectRiskCategory(ProjectRiskRatingCategoryViewModel entity);
        Task<bool> UpdateProjectRiskCriteria(ProjectRiskRatingCriteriaViewModel entity);
        Task<bool> AddProjectRiskCriteria(ProjectRiskRatingCriteriaViewModel entity);
        Task<bool> UpdateContractorCriteria(ContractorCriteriaViewModel entity);
        Task<IEnumerable<ProjectRiskRatingCategoryViewModel>> getAllProjectRiskRatingCategories();
        Task<bool> AddContractorCriteria(ContractorCriteriaViewModel entity);
        CreditLimitValidationsModel ValidateAmountFacilityBySector(int sectorId);
        CreditLimitValidationsModel ValidateNPLByGroupFirstTwenty(LoanApplicationViewModel application);
        CreditLimitValidationsModel ValidateNPLByGroupFirstHundred(LoanApplicationViewModel application);
        CreditLimitValidationsModel ValidateNPLByCurrency(LoanApplicationViewModel application);
        Task<IEnumerable<CurrencyLimitViewModel>> GetAllCurrencyLimit();
        Task<bool> AddCurrencyLimits(CurrencyLimitViewModel entity);
        Task<bool> UpdateCurrencyLimits(CurrencyLimitViewModel entity);
        Task<bool> DeleteCurrencyLimit(int id, UserInfo user);
        Task<IEnumerable<GroupLimitViewModel>> GetAllGroupLimit();
        Task<bool> AddGroupLimits(GroupLimitViewModel entity);
        Task<bool> UpdateGroupLimits(GroupLimitViewModel entity);
        Task<bool> DeleteGroupLimit(int id, UserInfo user);
        Task<int> ValidateBlackList(string customerCode);
        // int ValidateBlackList(int customerId);
        
        Task<int> ValidateWatchList(int customerId);
        bool IsDirectorRelatedGroup(int? customerGroupId);
        bool CustomerIsDirector(int? customerId);
        CreditLimitValidationsModel ValidateNPLByDirectors(LoanApplicationViewModel application);
        //int ValidateCamsol(int customerId);
      
        //IEnumerable<CustomerEligibilityViewModel> ValidateCamsol(string customerCode);
        CreditLimitValidationsModel ValidateNPLByInsiderCustomer();
        Task<IEnumerable<CustomerEligibilityViewModel>> ValidateCustomerEligibility(string customerCode);
        bool ValidateIsInsiderCustomer(int customerId);
        Task<CreditLimitValidationsModel> ValidateAmountByBranch(short branchId);
        CreditLimitValidationsModel ValidateNPLByBranch(short branchId);
        Task<CreditLimitValidationsModel> ValidateAmountBySector(int customerId);
        CreditLimitValidationsModel ValidateNPLBySector(int subSectorId);
        Task<CreditLimitValidationsModel> ValidateAmountByCustomer(int subSectorId);
        Task<CreditLimitValidationsModel> ValidateNPLByCustomer(int customerId);
        Task<CreditLimitValidationsModel> ValidateAmountByCustomerGroup(int customergroupId);
        Task<CreditLimitValidationsModel> ValidateNPLByCustomerGroup(int customergroupId);
        Task<CreditLimitValidationsModel> ValidateCreditLimitNPLByRMBM(short relationshipofficerId);
        Task<CreditLimitValidationsModel> ValidateAmountBySegment(short segmentId);
        Task<CreditLimitValidationsModel> ValidateNPLBySegment(short segmentId);
        CreditLimitValidationsModel ValidateSingleObligorLimit(LoanApplicationViewModel application);
        Task<IEnumerable<ObligorLimitViewModel>> GetAllObligorLimit();
        bool ValidateRiskRating(string riskRating);
        Task<bool> AddUpdateRiskRating(ObligorLimitViewModel entity);
        Task<bool> DeleteRiskRating(int id, UserInfo user);
        CreditLimitValidationsModel ValidateCreditLimitByRMBM(short relationshipofficerId);
        Task<bool> UpdateCustomerRating(ObligorLimitViewModel entity);
        Task<bool> UpdateApplicationCustomerRating(ObligorLimitViewModel entity);
        CreditLimitValidationsModel ValidateApplicationCustomerRating(ObligorLimitViewModel entity);
        Task<CustomerEligibility> GetCustomerEligibility(string customerCode);
        bool BranchLimitExceeded(int branchId, decimal applicationAmount);
        bool SectorLimitExceeded(int sectorId, decimal applicationAmount);
        bool ProductLimitExceeded(int productId, decimal applicationAmount);
        Task<TotalExposureLimit> GetTotalExposureLimit(ExposureLimitRequestModel model);
        Task<TotalExposureLimit> GetTotalExposureLimitReference(string reference, int getCompanyId);
    }
}