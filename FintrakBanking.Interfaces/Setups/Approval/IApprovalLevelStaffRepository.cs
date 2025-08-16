using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Reports;
using FintrakBanking.ViewModels.WorkFlow;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.Approval
{
    public interface IApprovalLevelStaffRepository
    {
        IEnumerable<WorkflowTrackerViewModel> GetApprovalTrailBySiteTargetId(int targetId, int companyId);
        Task<bool> AddApprovalLevelStaff(ApprovalLevelStaffViewModel model);

        Task<bool> UpdateApprovalLevelStaff(int staffLevelId, ApprovalLevelStaffViewModel model);

        Task<bool> DeleteApprovalLevelStaff(int staffLevelId, UserInfo user);

        //IEnumerable<ApprovalLevelStaffViewModel> GetAllAssignedApprovalLevelStaff(int companyId);

        IEnumerable<ApprovalLevelStaffViewModel> GetAllApprovalLevelStaffByOperationId(int operationId, int companyId);

        ApprovalLevelStaffViewModel GetAllApprovalLevelStaffByStaffId(int staffId, int companyId, int operationId);

        IEnumerable<ApprovalLevelStaffViewModel> GetAllApprovalLevelStaff(int companyId);

        IEnumerable<ApprovalLevelStaffViewModel> GetApprovalLevelStaffById(int staffLevelId, int companyId);

        //Task<IEnumerable<WorkflowTrackerViewModel>> GetApprovalTrailByOperationIdAndTargetId(int operationId,
        //    int targetId, int companyId);

        IEnumerable<WorkflowTrackerViewModel> GetApprovalTrailByOperationIdAndTargetId(int operationId,
            int targetId, int companyId);

        IQueryable<WorkflowTrackerViewModel> GetAllRecordsOnApprovalTrail(int companyId);

        IEnumerable<ApprovalLevelStaffViewModel> GetAllAssignedApprovalLevelStaff(int companyId);

        ApprovalLevelStaffViewModel GetAllApprovalLevelStaffByStaffId(int staffId, int companyId);
        Task<List<WorkflowTrackerViewModel>> GetAllApprovalStatus();
        Task<List<WorkflowTrackerViewModel>> GetAllApprovalOperations();
        int GoForApproval(ApprovalLevelStaffViewModel model);
        Task<IEnumerable<ApprovalLevelStaffViewModel>> GetTempApprovalLevelStaff(int staffId);
        List<WorkflowTrackerViewModel> GetApprovalMointoring(DateRange param);
        List<WorkflowTrackerViewModel> GetTurnAroundMointoring(DateRange param);
        List<WorkflowTrackerViewModel> GetBookingMointoring(DateRange param);
        List<WorkflowTrackerViewModel> GetBookingTATMointoring(DateRange param);
        Task<List<WorkflowTrackerViewModel>> GetContractReviewMointoring(DateRange param);
        Task<List<WorkflowTrackerViewModel>> GetBookingApprovalTrailByTargetId(int targetId, int companyId);
        Task<List<WorkflowTrackerViewModel>> GetApprovalTrailByTargetId(int targetId, int companyId);
        Task<WorkflowTrackerViewModel> GenerateApprovalMonitoringReport(DateRange param);
        WorkflowTrackerViewModel ExportApprovalComments(List<ApprovalTrailViewModel> commentsData, bool requireAll);
        Task<bool> AddTATSetup(TurnAroundTimeViewModel entity);
        IEnumerable<TurnAroundTimeViewModel> GetTATSetup();
        Task<bool> UpdateTATSetup(int tatId, TurnAroundTimeViewModel entity);
        Task<bool> DeleteTATSetup(int tatId, UserInfo user);

    }
}