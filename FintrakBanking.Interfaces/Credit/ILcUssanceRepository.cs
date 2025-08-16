using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FintrakBanking.ViewModels.credit;
using FintrakBanking.ViewModels;

namespace FintrakBanking.Interfaces.credit
{
    public interface ILcUssanceRepository
    {
        Task<List<LcUssanceViewModel>> GetLcUssanceByLCIssuanceId(int lcIssuanceId);

        Task<LcUssanceViewModel> GetLcUssanceByLCUsanceId(int lcUsanceId);
        Task<LcUssanceViewModel> GetLcUssanceExtensionByTempLcUsanceId(int tempLcUsanceId);
        Task<List<LcUssanceViewModel>> GetLcUssanceExtensionsByLcUsanceId(int lcUsanceId);

        Task<IEnumerable<LcUssanceViewModel>> GetLcUssances();
        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuancesForUssanceExtensionApproval(int staffId);
        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuancesForUssanceApproval(int staffId);
        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuancesForUssanceExtension(int staffId);
        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuancesForUssance(int staffId);

        Task<LcUssanceViewModel> AddLcUssanceExtension(LcUssanceViewModel model);
        Task<LcUssanceViewModel> AddLcUssance(LcUssanceViewModel model);

        Task<bool> UpdateLcUsanceExtension(LcUssanceViewModel model, int id, UserInfo user);
        Task<bool >UpdateLcUssance(LcUssanceViewModel model, int id, UserInfo user);

        Task<bool> DeleteLcUssance(int id, UserInfo user);
    }
}
