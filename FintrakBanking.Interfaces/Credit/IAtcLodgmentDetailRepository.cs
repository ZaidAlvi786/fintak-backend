using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.credit;

namespace FintrakBanking.Interfaces.credit
{
    public interface IAtcLodgmentDetailRepository
    {
        Task<IEnumerable<AtcLodgmentDetailViewModel>> GetAtcLodgmentDetail(int id);

        Task<bool> AddAtcLodgmentDetail(AtcLodgmentDetailViewModel model);

        Task<bool> DeleteAtcLodgmentDetail(int id, UserInfo user);
    }
}
