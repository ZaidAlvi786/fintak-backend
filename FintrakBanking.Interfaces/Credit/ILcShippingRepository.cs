using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FintrakBanking.ViewModels.credit;
using FintrakBanking.ViewModels;

namespace FintrakBanking.Interfaces.credit
{
    public interface ILcShippingRepository
    {
        Task<LcShippingViewModel> GetLcShipping(int id);

        Task<IEnumerable<LcShippingViewModel>> GetLcShippings();

        Task<IEnumerable<LcShippingViewModel>> GetLcShippingsByIssuanceId(int lcIssuanceId);

       Task<bool> AddLcShipping(LcShippingViewModel model);

        Task<bool> UpdateLcShipping(LcShippingViewModel model, int id, UserInfo user);

        Task<bool> DeleteLcShipping(int id, UserInfo user);
    }
}
