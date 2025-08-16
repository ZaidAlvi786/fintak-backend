using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Finance;
using FintrakBanking.ViewModels.Setups;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace FintrakBanking.Interfaces.Customer
{
    public interface ILoanCovenantRepository
    {
        #region Loan Covenant Detail
        Task<int> AddMultipleLoanCovenantDetail(List<LoanCovenantDetailViewModel> covenantModel);
        Task<bool> AddLoanCovenantDetail(LoanCovenantDetailViewModel entity);
        Task<bool> DeleteLoanCovenantDetail(int loanCovenantDetailId, UserInfo user );
        Task<bool> UpdateLoanCovenantDetail(int loanCovenantDetailId, LoanCovenantDetailViewModel entity);
        Task<IEnumerable<LoanCovenantDetailViewModel>> GetLoanCovenantDetailByCovenantType(int covenantTypeId, int companyId);
        Task<IEnumerable<LoanCovenantDetailViewModel>> GetLoanCovenantDetailByloanId(int loanId, int companyId);
        IEnumerable<LoanCovenantTypeViewModel> GetLoanCovenantDetailById(int covenantDetailId, int companyId);

        Task<bool> DeleteLoanApplicationCovenant(int covenantId, UserInfo user);
        Task<bool> AddLoanApplicationCovenant(LoanCovenantDetailViewModel entity);
        Task<IEnumerable<LoanCovenantDetailViewModel>> GetLoanApplicationCovenant(int applicationId);
        Task<IEnumerable<LoanCovenantDetailViewModel>> GetLoanApplicationDetailCovenant(int applicationDetailId);
        //bool UpdateLoanApplicationCovenant(DateTime date);
        bool UpdateLoanApplicationCovenant(DateTime date, int companyId, int staffId, out string transactionReferenceNo);
        DateTime GetFrequencyDate(int frequencyTypeId, DateTime date);

        #endregion Loan Covenant Detail

        #region Loan Covenant Type
        Task<bool> AddLoanCovenantType(LoanCovenantTypeViewModel entity);         
        Task<bool> UpdateLoanCovenantType(short loanCovenantTypeId, LoanCovenantTypeViewModel entity);
        IEnumerable<LoanCovenantTypeViewModel> GetLoanCovenantType(int companyId);

        Task<IEnumerable<LoanCovenantDetailViewModel>> GetLoanApplicationCovenantLms(int id);
        Task<bool> AddLoanApplicationCovenantLms(LoanCovenantDetailViewModel entity);
        Task<bool> UpdateLoanApplicationCovenantLms(int covenantId, LoanCovenantDetailViewModel entity);
        Task<bool> DeleteLoanApplicationCovenantLms(int id, UserInfo user);


        #endregion Loan Covenant Detail
    }
}
