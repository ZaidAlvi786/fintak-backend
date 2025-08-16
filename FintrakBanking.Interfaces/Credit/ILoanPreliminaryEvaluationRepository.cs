using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.WorkFlow;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface ILoanPreliminaryEvaluationRepository
    {
        Task<IEnumerable<LoanPreliminaryEvaluationViewModel>> GetLoanPreliminaryEvaluationMappedToApplication();
        Task<LoanPreliminaryEvaluationViewModel> AddPreliminaryEvaluation(LoanPreliminaryEvaluationViewModel model);

        Task<LoanPreliminaryEvaluationViewModel> AddMultiplePreliminaryEvaluation(List<LoanPreliminaryEvaluationViewModel> model);

        Task<IEnumerable<LoanPreliminaryEvaluationViewModel>> GetSingleCustomerPreliminaryEvaluationsAwaitingApproval(int staffId, int companyId);

        Task<IEnumerable<LoanPreliminaryEvaluationViewModel>> GetGroupCustomerPreliminaryEvaluationsAwaitingApproval(
            int staffId, int companyId);

        Task<bool> GoForApproval(ApprovalViewModel entity);

        Task<IEnumerable<LoanPreliminaryEvaluationViewModel>> GetAllSingleCustomerLoanPreliminaryEvaluations();
        
        Task<IEnumerable<LoanPreliminaryEvaluationViewModel>> GetAllGroupCustomerLoanPreliminaryEvaluations();

        Task<bool> UpdatePreliminaryEvaluation(int loanPenId, LoanPreliminaryEvaluationViewModel model);

        Task<bool> SendPreliminaryEvaluationForLoanApplication(int loanPenId, LoanPreliminaryEvaluationViewModel model);

        Task<IEnumerable<LoanPreliminaryEvaluationViewModel>> GetLoanPreliminaryEvaluationsByLoanTypeId(int loanTypeId);

        Task<IEnumerable<LoanPreliminaryEvaluationViewModel>> GetLoanApplicationPreliminaryEvaluations(int applicationId);

        Task<IEnumerable<LoanPreliminaryEvaluationViewModel>> GetLoanPreliminaryEvaluationsAwaitingApprovalByLoanTypeId(
            int staffId, int companyId, int loanTypeId);
        Task<IEnumerable<LookupViewModel>> GetCustomerLoanPreliminaryEvaluations(int customerId, int loanTypeId, int customerGroupId = 0);
    }
}
