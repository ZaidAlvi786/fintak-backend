using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface IFeeConcessionRepository
    {
        Task<IEnumerable<FeeConcessionTypeViewModel>> GetConcessionFeeType();
        Task<IEnumerable<LoanFeeChargesViewModel>> GetAllLoanFeeChargeByDetailId(int loanApplicationDetailId);
        Task<IEnumerable<FeeConcessionViewModel>> GetAllConcessionFee(int loanApplicationDetailId);
        Task<Tuple<int, string>> AddUpdateFeeConcession(FeeConcessionViewModel model);
        int GoForApproval(ApprovalViewModel entity);
        Task<bool> ValidateFeeConcession(int loanApplicationDetailId, int? loanChargeFeeId);
        Task<bool> ValidateApprovedFeeConcession(int concessionId);
        Task<IEnumerable<FeeConcessionViewModel>> GetAllConcessionFeeAwaitingApproval(int staffId, int companyId);
    }
}
