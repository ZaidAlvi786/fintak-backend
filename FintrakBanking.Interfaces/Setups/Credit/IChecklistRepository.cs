using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.Credit;
using FintrakBanking.ViewModels.WorkFlow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups
{
    public interface IChecklistRepository
    {
        #region Loan Checklist Definition
        Task<IEnumerable<CheckListResponseTypeViewModel>> GetAllChecklistResponseType();
        Task<IEnumerable<CheckListTargetTypeViewModel>> GetAllChecklistType();
        IEnumerable<ChecklistDefinitionViewModel> GetAllChecklistDefinition();
        Task<bool> DeleteChecklistTypeMapping(int checklistTypeMappingId, UserInfo user);
        IEnumerable<CheckListTargetTypeViewModel> GetChecklistTypeByApprovalLevel(int staffId, int companyId, int operationId, int productClassProcessId);
        Task<IEnumerable<ChecklistDefinitionViewModel>> GetAllMappedChecklistDefinitionByProductId(int productId);
        Task<List<ChecklistDefinitionViewModel>> GetAllChecklistDefinitionById(int CheckListDefinitionId);
        Task<bool> AddChecklistDefinition(ChecklistDefinitionViewModel model);
        Task<bool> AddMultipleChecklistDefinition(List<ChecklistDefinitionViewModel> models);
        Task<bool> AddMultipleChecklistDefinitionWithMultipleItems(ChecklistDefinitionViewModel model);
        Task<bool> UpdateChecklistDefinition(int CheckListDefinitionId, ChecklistDefinitionViewModel model);
        Task<bool> DeleteChecklistDefinition(int CheckListDefinitionId, UserInfo user);
        Task<bool> ValidateChecklistDetail(List<ValidateChecklistDetailViewModel> entity);
        Task<IEnumerable<ChecklistDefinitionViewModel>> GetAllMappedChecklistDefinitionByApprovalLevelAndProduct(int approvalLevelId, int productId);
        Task<IEnumerable<ChecklistItemViewModel>> GetAllUnmappedChecklistItemsToApprovalLevelAndProduct(int approvalLevelId, int productId);
        Task<IEnumerable<ChecklistDefinitionViewModel>> GetUnmappedChecklistDefintionToApprovalLevel(int approvalLevelId);
        // IEnumerable<ChecklistDefinitionViewModel> GetChecklistDefinitionByApprovalLevelCheckListType(int staffId, int? productId, int loanTargetId, int operationId, int checkListTypeId);
        IEnumerable<ChecklistDefinitionAndDetailViewModel> GetChecklistDefinitionByApprovalLevelCheckListType(int staffId, int? productId, int loanTargetId, int operationId, int checkListTypeId , int? customerId=null);
        #endregion
        #region Loan Checklist Detail
        Task<IEnumerable<ChecklistDetailViewModel>> GetAllChecklistDetail();
        Task<IEnumerable<ChecklistDetailViewModel>> GetChecklistByTargetId(int targetId);
        Task<IEnumerable<ChecklistDetailViewModel>> GetChecklistByCheckListTypeAndTargetId(int targetId, int checkListtypeId, bool isCamChecklist,int? customerId=null);
        // IEnumerable<ChecklistDetailViewModel> GetChecklistByCheckListTypeAndTargetId(int targetId, int checkListtypeId);
        Task<List<ChecklistDetailViewModel>> GetAllChecklistDetailById(int ChecklistId);
        Task<List<ChecklistDetailViewModel>> GetAllChecklistDetailByProductAndTargetId(int targetTypeId, int productId);
        Task<List<ChecklistDetailViewModel>> GetAllChecklistDetailByProductId(int targetId);
        Task<List<ChecklistDetailViewModel>> GetAllChecklistDetailByChecklistDefinitionId(int checklistDefinitionId);
        Task<bool> AddChecklistDetail(ChecklistDetailViewModel model);
        Task<bool> UpdateChecklistDetail(int ChecklistId, ChecklistDetailViewModel model);
        Task<bool> DeleteChecklistDetail(int ChecklistId, UserInfo user);
        Task<bool> AddMultipleChecklistDetails(List<ChecklistDetailViewModel> models, int staffId, short BranchId);
        #endregion
        #region CheckList Items
        Task<IEnumerable<ChecklistItemViewModel>> GetAllChecklistItem();
        Task<IEnumerable<ChecklistItemViewModel>> GetAllChecklistItemBycheckListTypeId(int checkListTypeId);
        Task<List<ChecklistItemViewModel>> GetAllChecklistItemById(int CheckListItemId);
        Task<bool> AddChecklistItem(ChecklistItemViewModel model);
        Task<bool> AddMultipleChecklistItem(List<ChecklistItemViewModel> model);
        Task<bool> UpdateChecklistItem(int CheckListItemId, ChecklistItemViewModel model);
        Task<bool> DeleteChecklistItem(int CheckListItemId, UserInfo user);
        #endregion
        #region CheckList Select Lists
        Task<IEnumerable<CheckListStatusViewModel>> GetAllChecklistStatus();
        Task<IEnumerable<CheckListTargetTypeViewModel>> GetAllChecklistTargetType();
        #endregion
        #region Checklist Validation
        Task<bool> ValidateChecklistDetailEntry(int checklistDefinitionId, int targetId);
        Task<bool> ValidateConditionPrecedentDetail(ConditionPrecedentViewModel entity);
        Task<bool> ValidateChecklistForDefferalOrWaival(ConditionPrecedentViewModel entity);
        #endregion
        // IEnumerable<ConditionPrecedentViewModel> GetConditionPrecedenceChecklist(int loanApplicationId);
        Task<IEnumerable<ConditionPrecedentViewModel>> GetConditionPrecedenceChecklist(int loanApplicationId, bool isAvailment);
        Task<IEnumerable<ConditionPrecedentViewModel>> GetConditionPrecedenceChecklistStatus(int loanApplicationId, bool isAvailment, int staffId = 0);
        Task<bool> UpdateLoanConditionPrecedenceStatus(ConditionPrecedentViewModel model);
        Task<bool> ForwardChecklistForApproval(List<ConditionPrecedentViewModel> model);
        WorkflowResponse GoForApproval(ApprovalViewModel entity);
        Task<bool> ExtendChecklistDeferralDate(ConditionPrecedentViewModel model);
        Task<bool> UpdateProvidedChecklist(ConditionPrecedentViewModel model);
        Task<bool> ValidateDeferralDateExpiration(int conditionId);
        Task<IEnumerable<ChecklistApprovalViewModel>> GetDeferralDocumentsAwaitingApproval(int staffId, int companyId);
        Task<IEnumerable<ChecklistApprovalViewModel>> GetDeferralExtensionsAwaitingApproval(int staffId, int companyId);
        String ResponseMessage(WorkflowResponse response, string itemHeading);
        WorkflowResponse SubmitDeferralDocumentForApproval(ConditionPrecedentViewModel model);
        Task<bool> SubmitDeferralExtensionForApproval(ConditionPrecedentViewModel model);
        Task<IEnumerable<ChecklistApprovalViewModel>> GetChecklistAwaitingApproval(int staffId, int companyId);
        Task<IEnumerable<DeferredChecklistViewModel>> GetAllDeferralChecklist();
        Task<IEnumerable<DeferredChecklistViewModel>> GetDeferralChecklistByConditionId(int conditionId);
        Task<bool> ValidateChecklist(int applicationId);
        #region Checklist Type Mapping
        Task<IEnumerable<CheckListTypeMappingViewModel>> GetAllChecklistTypeMapping();
        Task<bool> AddChecklistTypeMapping(CheckListTypeMappingViewModel model);
        Task<bool> ValidateChecklistTypeMapping(short checklistTypeId, int approvallevelId);
        #endregion
        #region ESG Checklist
        Task<IEnumerable<ESGClassViewModel>> GetESGClass();
        Task<IEnumerable<ESGTypeViewModel>> GetESGType();
        Task<IEnumerable<ESGCategoryViewModel>> GetESGCategory();
        Task<IEnumerable<ESGSubCategoryViewModel>> GetESGSubCategory(int categoryId);
        Task< IEnumerable<ESGChecklistDefinitionViewModel>> GetESGChecklistDefinition();
        Task<IEnumerable<ESGChecklistDetailViewModel>> GetESGChecklistDetail(int loanApplicationDetailId);
        Task<IEnumerable<ESGChecklistDefinitionAndDetailViewModel>> GetESGChecklistStatus(int loanApplicationDetailId);
        Task<IEnumerable<CheckListScores>> GetCheckListScores(int checkListTypeId);
        Task<IEnumerable<ESGChecklistDefinitionViewModel>> GetGreenRatingDefinition();
        Task<IEnumerable<ESGChecklistDetailViewModel>> GetGreenRatingDetail(int loanApplicationDetailId);
        IEnumerable<ESGChecklistDefinitionAndDetailViewModel> GetGreenRatingStatus(int loanApplicationDetailId);
        Task<ESGChecklistSummaryViewModel> CalculateGreenRatingSummary(List<ESGChecklistDetailViewModel> models);
        Task<bool> AddGreenRatingDetail(List<ESGChecklistDetailViewModel> models);
        Task<bool> AddGreenRatingSummary(ESGChecklistSummaryViewModel models);
        Task<bool> AddGreenRatingDefinition(List<ESGChecklistDefinitionViewModel> models);
        Task<bool> DeleteGreenRatingDefinition(int esgChecklistDefinitionId, int staffId);
        Task<ESGChecklistSummaryViewModel> CalculateESGChecklistSummary(List<ESGChecklistDetailViewModel> models);
        Task<IEnumerable<LoanApplicationDetailViewModel>> GetAllFacilityDetails(int loanApplicationId, int companyId);
        Task<bool> AddESGCategory(ESGChecklistDefinitionViewModel model);
        Task<bool> AddESGSubCategory(ESGChecklistDefinitionViewModel model);
        Task<bool> UpdateESGCategory(ESGChecklistDefinitionViewModel model);
        Task<bool> UpdateESGSubCategory(ESGChecklistDefinitionViewModel model);
        Task<bool> DeleteESGCategory(int ESGCategoryId, UserInfo user);
        Task<bool> DeleteESGSubcategory(int ESGSubcategoryId, UserInfo user);
        Task<bool> AddESGChecklistDefinition(List<ESGChecklistDefinitionViewModel> models);
        Task<bool> AddESGChecklistDetail(List<ESGChecklistDetailViewModel> models);
        Task<bool> AddESGChecklistSummary(ESGChecklistSummaryViewModel models);
        Task<bool> DeleteESGChecklistDefinition(int esgChecklistDefinitionId, int staffId);
        #endregion
        Task<bool> RegulatoryChecklistAutomapping(int customerId, ChecklistDetailViewModel model);
        Task<bool> DeleteLoanConditionPrecedenceStatus(int conditionId, bool isLMSChecklist, UserInfo user);
        Task<bool> ValidatePrecedenceChecklistCompleted(int loanApplicationId);
        Task<bool> LMSValidatePrecedenceChecklistCompleted(int applicationId);
        #region Condition Precedence Checklist
        Task<IEnumerable<ConditionPrecedentViewModel>> GetLMSConditionPrecedenceChecklist(int loanReviewApplicationId);
        Task<IEnumerable<ConditionPrecedentViewModel>> GetLMSConditionPrecedenceChecklistStatus(int loanReviewApplicationId);
        #endregion
        Task<IEnumerable<ChecklistDefinitionAndDetailViewModel>> GetChecklistItemSimulationDetails(int productId);
        Task<bool> PopulateLoanApplicationChecklist(int loanApplicationId, int staffId, int companyId, int productClassProcessId);
        Task<IEnumerable<ApprovalTrailViewModel>> GetDeferralApprovalTrail(int targetId, int operationId);
    }
}
