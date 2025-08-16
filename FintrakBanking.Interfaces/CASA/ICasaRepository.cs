using FintrakBanking.ViewModels.CASA;
using FintrakBanking.ViewModels.Customer;
using FintrakBanking.ViewModels.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.CASA
{
    public interface ICasaRepository
    {
        Task<CasaViewModel> GetAccount(int accountId);
        int GetCasaAccountId(string accountNumber, int companyId);
        Task<string> GetAccountOwnerByAccountNumber(string accountNumber, int companyId);
        Task<IEnumerable<CasaViewModel>> GetAccountByCustomerId(int customerId);
        Task<IEnumerable<CasaViewModel>> FindAccount(string accountNumberOrName, int companyId);
        Task<IEnumerable<CasaViewModel>> GetGroupAccountNumberWithCustomerId(string accountNumberOrName, int customerId, int companyId);
        Task<IEnumerable<CasaViewModel>> GetOverdraftAccountNumberWithCustomerId(string accountNumberOrName, int customerId, int companyId);
        IQueryable<CustomerSearchVM> SearchCustomer(int customerTypeId,int companyId, string searchQuery);
        IQueryable<CasaCustomerSearchViewModel> SearchForCustomerAccount(int companyId, string searchQuery, int customerTypeId);
        Task<IEnumerable<CasaBalanceViewModel>> GetAllCustomerAccountByCustomerId(int customerId, int companyId);
        Task<IEnumerable<CasaBalanceViewModel>> GetAllCustomerAccountByCustomerIdAndCurrency(int customerId, int companyId, int currencyId);
        Task<IEnumerable<CasaBalanceViewModel>> GetBusinessAccounts( int companyId);
        CasaBalanceViewModel GetCASABalance(string casaAccountNumber, int companyId);
        Task<string> GetAllCASAAccount(string casaAccountNumber, int companyId);
        Task<CasaCustomerSearchViewModel> GetCustomerAccountDetailsById(int customerId);
        Task<IEnumerable<CustomerCasaAcountsViewModel>> GetAllCustomerAccount(int customerId, int applicationTypeId, int companyId);
        void AddCustomerAccounts(string customerCode);
        Task<IEnumerable<CasaLienTypeViewModel>> GetAllCasaLienTypes(int companyId);
        Task<bool> AddCasaLien(CasaViewModel model);
        Task<IEnumerable<CasaLoanViewModel>> GetAllCasaLoans(int companyId, int casaAccountId);
        Task<IEnumerable<CasaViewModel>> FindCustomerCasaLien(string accountNumberOrName, int companyId);
        Task<IEnumerable<CasaAccountLienViewModel>> GetAllCasaLiens(string accountNumber);
    }
}
