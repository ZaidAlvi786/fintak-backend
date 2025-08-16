using FintrakBanking.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface ICashFlowLendingRepository
    {
        Task<APIResponse> AddCustomer(IncomingCustomerViewModels model);
        Task<APIResponse> submitRequest(CflLoanApplication model);
        int SaveCashflowRequestToApiLog2(CflLoanApplication request);
        APIResponse SaveLoanDocuments(CflLoanApplication model);
    }
}
