using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels.Setups.Credit;
using FintrakBanking.ViewModels;

namespace FintrakBanking.Interfaces.Setups.Credit
{
   public  interface IPrudentialGuidelineSetupRepository
    {
        Task<IEnumerable<PrudentialGuidelineViewModel>> GetAllGuidelines(int companyId);

        Task<PrudentialGuidelineViewModel> getGuideline(int prudentialGuidelineId);

        Task<string> UpdateGuideline(PrudentialGuidelineViewModel guideline,int prudentialGuidelineId);

        Task<string> DeleteGuideline(int prudentialGuidelineId);

        string AddGuideline(PrudentialGuidelineViewModel guideline);
        IEnumerable<PrudentialGuidelineViewModel> GetAllGuidelineTypes(int getCompanyId);
    }
}

<!-- Auto-push timestamp: 2026-04-28 20:56:21 -->