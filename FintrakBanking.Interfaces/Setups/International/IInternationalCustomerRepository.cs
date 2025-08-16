using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups;
using FintrakBanking.ViewModels.Setups.International;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.International
{
    public interface IInternationalCustomerRepository
    {
        IEnumerable<InternationalCustomerViewModel> GetInternationalCustomerSearch(SearchInternationalCustomerViewModel model);
    }
}
