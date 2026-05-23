using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface IConditionPrecedentRepository

    {
        IEnumerable<ConditionPrecedentViewModel> GetConditionPrecedentByDetailId(int detailId);

        IEnumerable<ConditionPrecedentViewModel> GetAllConditionPrecedent();

        Task<bool> AddConditionPrecedent(ConditionPrecedentViewModel model);

        IEnumerable<ConditionPrecedentViewModel> GetConditionPrecedentTemplate();

        Task<bool> AddConditionPrecedentTemplate(ConditionPrecedentViewModel model);

        Task<bool> UpdateConditionPrecedentTemplate(ConditionPrecedentViewModel model, int conditionPrecedentId);

        Task<bool> RemoveLoanConditionPrecedent(int id, UserInfo user);

        bool EditLoanConditionPrecedent(int id, ConditionPrecedentViewModel entity);

        IEnumerable<ComplianceTimelineViewModel> GetComplianceTimelineTemplate();

        bool AddComplianceTimelineTemplate(ComplianceTimelineViewModel model);

        bool UpdateComplianceTimelineTemplate(ComplianceTimelineViewModel model, int timelineId);
        Task<bool> RemoveComplianceTimelineTemplate(UserInfo user, int id);

        Task<List<ConditionPrecedentViewModel>> GetConditionPrecedentDefaultByDetailId(int detailId);

        List<ConditionPrecedentViewModel> AddSelectedConditionPrecedent(SelectedIdsViewModel entity);

        // LMS approval
        Task<bool> RemoveLoanConditionPrecedentLms(int id, UserInfo user);
        Task<bool> EditLoanConditionPrecedentLms(int id, ConditionPrecedentViewModel entity);
        Task<List<ConditionPrecedentViewModel>> AddSelectedConditionPrecedentLms(SelectedIdsViewModel entity);
        Task<bool> AddConditionPrecedentLms(ConditionPrecedentViewModel entity);
        IEnumerable<ConditionPrecedentViewModel> GetConditionPrecedentByDetailIdLms(int detailid);
        Task<List<ConditionPrecedentViewModel>> GetConditionPrecedentDefaultByDetailIdLms(int detailId);
        Task<bool> DeleteConditionPrecedentTemplate(UserInfo user, int id);

        // additional comment condition
        Task<List<AdditionalCommentViewModel>> GetAdditionalComment(int applicationId, int callerId, int userId);
        Task<bool> AddAdditionalComment(AdditionalCommentViewModel entity);
        Task<bool> EditAdditionalComment(int id, AdditionalCommentViewModel entity);
        Task<bool> RemoveAdditionalComment(int id, UserInfo user);

        Task<List<ConditionPrecedentViewModel>> GetConditionPrecedentDefaultByApplicationIdAndOperationLms(int detailId, int? operationId);
    }
}

<!-- Auto-push timestamp: 2026-01-01 16:12:40 -->