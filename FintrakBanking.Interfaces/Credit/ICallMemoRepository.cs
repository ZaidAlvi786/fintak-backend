using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface ICallMemoRepository
    {
        #region "Call Limit"
        bool isLimitExist(CallLimitViewModel model);
        Task<IEnumerable<CallMemoTypeViewModel>> GetCallLimitType();
        Task<IEnumerable<CallLimitViewModel>> GetAllCallLimit(int companyId);
        Task<List<CallLimitViewModel>> GetCallLimitByTypeId(int limitId);
        Task<bool> AddCallLimit(CallLimitViewModel model);
        Task<bool> UpdateCallLimit(int limitId, CallLimitViewModel model);
        Task<bool> DeleteCallLimit(int limitId, UserInfo user);
        #endregion

        #region "Call Memo"
        Task<IQueryable<CallMemoLoanSearchViewModel>> SearchForCallMemoLoan(int staffId, string searchQuery);
        Task<IEnumerable<CallMemoViewModel>> GetCustomerCallMemo(int staffId, int customerId);
        Task<IEnumerable<CallMemoViewModel>> GetAllCallMemo(int staffId);
        Task<bool> GoForCallMemoApproval(CallMemoViewModel entity);
        Task<bool> SubmitApproval(CallMemoViewModel model);
        Task<int> AddCallMemo(CallMemoViewModel model);
        Task<bool> UpdateCallMemo(int limitId, CallMemoViewModel model);
        Task<CallMemoViewModel> GetCallMemoById(int callMemoID);

        Task<IEnumerable<CallMemoViewModel>> GetCallMemoWaitingForApproval(int staffId);
        Task<IEnumerable<CallMemoViewModel>> SearchCallMemo(int staffId, CallMemoViewModel model);
        Task<IEnumerable<CallMemoViewModel>> GetCustomerApprovedCallMemo(int staffId, int customerId);
        #endregion
    }
}
