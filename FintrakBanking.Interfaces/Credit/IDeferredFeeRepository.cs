using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;

namespace FintrakBanking.Interfaces.Credit
{
    public interface IDeferredFeeRepository
    {
        Task<LoanChargeFeeViewModel> GetDeferredFee(int id);

        Task<IEnumerable<LoanChargeFeeViewModel>> GetDeferredFees();

        Task<IEnumerable<LoanChargeFeeViewModel>> GetLoanDetailDeferredFees(int loanDetailId);

        Task<bool> AddDeferredFee(List<LoanChargeFeeViewModel> model, UserInfo user);

        Task<bool> UpdateDeferredFee(LoanChargeFeeViewModel model, int id, UserInfo user);

        Task<bool> DeleteDeferredFee(int id, UserInfo user);
    }
}