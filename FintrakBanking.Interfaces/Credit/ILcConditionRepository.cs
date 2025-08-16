using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FintrakBanking.ViewModels.credit;
using FintrakBanking.ViewModels;

namespace FintrakBanking.Interfaces.credit
{
    public interface ILcConditionRepository
    {
        Task<LcConditionViewModel> GetLcCondition(int id);

        Task<IEnumerable<LcConditionViewModel>> GetLcConditions();

        Task<IEnumerable<LcConditionViewModel>> GetLcConditionsBylcIssuanceId(int lcIssuanceId);

        Task<bool> AddLcCondition(LcConditionViewModel model);

        Task<bool> UpdateLcCondition(LcConditionViewModel model, int id, UserInfo user);

        Task<bool> DeleteLcCondition(int id, UserInfo user);
    }
}
