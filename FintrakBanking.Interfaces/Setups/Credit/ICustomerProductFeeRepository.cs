

using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.Credit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.Credit
{
    public interface ICustomerProductFeeRepository
    {
        IEnumerable<CustomerProductFeeViewModel> GetAllCustomerProductFees(int companyId);
        IEnumerable<CustomerProductFeeViewModel> GetCustomerProductFeeByCustomerId(int companyId, int customerId);
        IEnumerable<CustomerProductFeeViewModel> GetCustomerProductFeeByProductId(int companyId, int productId);
        Task<bool> AddCustomerProductFee(CustomerProductFeeViewModel model);
        Task<bool> UpdateCustomerProductFee(int customerProductFeeId, CustomerProductFeeViewModel model);
        Task<bool> DeleteCustomerProductFee(int customerProductFeeId, UserInfo user);
    }
}