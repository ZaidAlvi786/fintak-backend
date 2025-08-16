using FintrakBanking.ViewModels.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface ILoanPrincipalRepository
    {
        Task<IEnumerable<LoanPrincipalViewModel>> GetLoanPrincipal(int companyId);

        Task<LoanPrincipalViewModel> GetLoanPrincipal(int principalId, int companyId);

        Task<string> AddLoanPrincipal(LoanPrincipalViewModel loanPrincipal);

        Task<string> UpdateLoanPrincipal(LoanPrincipalViewModel loanPrincipal);

        Task<string> DeleteLoanPrincipal(LoanPrincipalViewModel loanPrincipal);
    }
}
