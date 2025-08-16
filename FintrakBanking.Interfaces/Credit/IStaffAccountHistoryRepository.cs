using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface IStaffAccountHistoryRepository
    {
        bool AddStaffAccountHistory(StaffAccountHistoryViewModel entity);
        Task<IEnumerable<StaffAccountHistoryViewModel>> GetStaffAccountHistory(int staffId);
        Task<bool> ApproveStaffAccountHistory(ReasignedAccountApprovalViewModel entity);
        Task<StaffMISHistoryViewModel> GetSelectedLoanDetails(int companyId, int loanId, int productTypeId);
        Task<IEnumerable<StaffAccountHistoryViewModel>> GetAllStaffAccountHistory();
        Task<StaffMISHistoryViewModel> GetSelectedApprovalLoanDetails(ReasignedAccountApprovalViewModel entity);
     }
}
