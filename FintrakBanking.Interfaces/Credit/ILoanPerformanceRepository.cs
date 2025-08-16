using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
   public interface ILoanPerformanceRepository
    {
        Task<IEnumerable<PrudGuildlineTypeViewModel>> GetPrudGuildlineType();
        IQueryable<LoanViewModel> GetAllLoan();
        IEnumerable<LoanViewModel> GetAllLoans();
        Task<IEnumerable<PrudentialGuidelineViewModel>> GetPrudGuildlineStatus();
        Task<bool> LoanPerformanceStatusChange(PrudGuidelineStatusChangeViewModel entity);
    }
}
