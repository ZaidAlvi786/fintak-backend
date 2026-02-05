using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.credit;

namespace FintrakBanking.Interfaces.credit
{
    public interface ILcCashBuildUpPlanRepository
    {
        LcCashBuildUpPlanViewModel GetLcCashBuildUpPlan(int id);

        Task<IEnumerable<LcCashBuildUpPlanViewModel>> GetLcCashBuildUpPlansByLcIssuanceId(int id);
        Task<IEnumerable<LcCashBuildUpPlanViewModel>> GetLcCashBuildUpReferenceTypes();

        Task<bool> AddLcCashBuildUpPlan(LcCashBuildUpPlanViewModel model);

        Task<bool> UpdateLcCashBuildUpPlan(LcCashBuildUpPlanViewModel model, int id, UserInfo user);

        Task<bool> DeleteLcCashBuildUpPlan(int id, UserInfo user);
    }
}

<!-- Auto-push timestamp: 2026-02-05 20:19:46 -->