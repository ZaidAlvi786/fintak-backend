using FintrakBanking.Entities.Models;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.Approval;
using FintrakBanking.ViewModels.WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels.Credit;

namespace FintrakBanking.Interfaces.Setups.Approval
{
    public interface IApprovalLevelRepository
    {
        Task<IEnumerable<ApprovalTrailViewModel>> GenericLMSApprovalTrail(int targetId, int operationId);
        Task<IEnumerable<ApprovalLevelViewModel>> GetAllApprovalLevel(int companyId);
        Task<IEnumerable<ApprovalLevelViewModel>> GetAllApprovalLevelDetails(int companyId);
        Task<IEnumerable<ApprovalLevelViewModel>> GetApprovalLevelById(int ApprovalLevelId, int companyId);
        Task<IEnumerable<ApprovalLevelViewModel>> GetApprovalLevelByGroupId(int groupId, int companyId);
        Task<IEnumerable<ApprovalLevelViewModel>> GetApprovalLevelByOperationId(int operationId, int companyId);

        bool AddApprovalLevel(ApprovalLevelViewModel model);
        
        bool AddMultipleApprovalLevel(List<ApprovalLevelViewModel> models);
        bool UpdateApprovalLevel(int ApprovalLevelId, ApprovalLevelViewModel model);
        Task<bool> DeleteApprovalLevel(int ApprovalLevelId, UserInfo user);
        Task<IEnumerable<TBL_STAFF>> GetStaffOrganogram(int companyId);
        Task<bool> UpdateApprovalTrail(TBL_APPROVAL_TRAIL model);
        Task<bool> AddApprovalTrail(TBL_APPROVAL_TRAIL model);
        IQueryable<WorkflowTrackerViewModel> GetApprovalTrail(int operationId, int companyId); 
        IQueryable<WorkflowTrackerViewModel> GetApprovalTrailByOperationIdAndTargetId(int operationId, int targetId, int companyId);
        IQueryable<TBL_APPROVAL_TRAIL> GetApprovalTrail(int operationId, int targetId, int approvalLevelId, int numberOfApprovals);
        Task<bool> PresetRoute(PresetRouteViewModel entity);
        Task<PresetRouteViewModel> GetPresetRouteCollection(int operationId, int? classId);
        Task<List<FintrakDropDownSelectList>> GetApprovalLevelsByOperationIdAndProductClassId(int operationId, int? classId);
        int GoForApproval(ApprovalLevelViewModel model);
        Task<List<ApprovalLevelViewModel>> GetTempApprovalApprovalLevel(int staffId);
        Task<List<FintrakDropDownSelectList>> GetRoutableOperations(List<int> operationIds);
        Task<List<ApprovalLevelViewModel>> GetRerouteApprovalLevels(int operationId);
        Task<bool> RerouteOperation(ForwardViewModel entity);
        Task<IEnumerable<ApprovalTrailViewModel>> GenericApprovalTrail(ApprovalTrailRequestViewModel entity);
        Task<List<FintrakDropDownSelectList>> GetTranchDisbursmentApprovalLevels();
        Task<IQueryable<WorkflowNotificationViewModel>> GetWorkflowMappingNotifications(int MappingId);
        Task<bool> AddWorkflowMappingNotification(WorkflowNotificationViewModel model);
        Task<bool> UpdateWorkflowMappingNotification(WorkflowNotificationViewModel model, int workflowNotificationId);
        Task<bool> DeleteWorkflowMappingNotification(int MappingId, UserInfo user);

        IEnumerable<DynamicWorkflowViewModel> GetDynamicWorkflowContext();
        IEnumerable<OperatorsViewModel> GetAllOperators();
        IEnumerable<DynamicWorkflowViewModel> GetDynamicWorkflowDataItemDefinition();
        IEnumerable<DynamicWorkflowViewModel> GetDynamicWorkflowDataItemByContextId(int contextId);
        DynamicWorkflowViewModel GetValueTypeByItemId(int dataItemId);
        bool CreateDynamicWorkflowItemExpression(DynamicWorkflowViewModel model);
        IEnumerable<DynamicWorkflowViewModel> GetDynamicWorkflowItemExpression();
        // IEnumerable<DynamicWorkflowViewModel> GetDynamicWorkflowItemExpressionById();
        bool UpdateDynamicWorkflowItemExpression(DynamicWorkflowViewModel model, int expressionId);
        List<DynamicContextListViewModel> GetDynamicBusinessRuleItemValueListByItemId(int dataItemId);
    }
}
