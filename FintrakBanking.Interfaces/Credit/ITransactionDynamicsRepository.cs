using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface ITransactionDynamicsRepository
    {
        IEnumerable<TransactionDynamicsViewModel> GetTransactionDynamicsByDetailId(int detailId);
        IEnumerable<TransactionDynamicsViewModel> GetAllTransactionDynamics();
        Task<bool> AddTransactionDynamics(TransactionDynamicsViewModel model);
        Task<IEnumerable<TransactionDynamicsViewModel>> GetTransactionDynamicsTemplate();
        Task<bool> AddTransactionDynamicsTemplate(TransactionDynamicsViewModel model);
        Task<bool> UpdateTransactionDynamicsTemplate(TransactionDynamicsViewModel model, int conditionPrecedentId);
        Task<bool> RemoveLoanTransactionDynamics(int id, UserInfo user);
        Task<bool> EditLoanTransactionDynamics(int id, TransactionDynamicsViewModel entity);
        List<TransactionDynamicsViewModel> GetTransactionDynamicsDefaultByDetailId(int detailId);
        Task<List<TransactionDynamicsViewModel>> AddSelectedTransactionDynamics(SelectedIdsViewModel entity);
        Task<List<TransactionDynamicsViewModel>> AddSelectedTransactionDynamicsLms(SelectedIdsViewModel entity);
        bool RemoveLoanTransactionDynamicsLms(int id, UserInfo user);
        Task<bool> EditLoanTransactionDynamicsLms(int id, TransactionDynamicsViewModel entity);
        Task<bool> AddTransactionDynamicsLms(TransactionDynamicsViewModel entity);
        Task<IEnumerable<TransactionDynamicsViewModel>> GetTransactionDynamicsByDetailIdLms(int detailId);
        Task<List<TransactionDynamicsViewModel>> GetTransactionDynamicsDefaultByDetailIdLms(int detailId);
        Task<List<TransactionDynamicsViewModel>> GetTransactionDynamicsDefaultByApplicationIdAndOperationLms(int detailId, int? operationId);
        Task<bool> AddSuggestedConditions(SuggestedConditionsViewModel entity);
        Task<List<SuggestedConditionsViewModel>> GetSuggestedConditions(int applicationId);
        Task<List<SuggestedConditionsViewModel>> GetSuggestedConditionsByApplicationId(int applicationId);
        Task<bool> UpdateSuggestedConditions(int id, SuggestedConditionsViewModel entity);
        Task<bool> RemoveSuggestedConditions(int id, UserInfo user);

    }
}