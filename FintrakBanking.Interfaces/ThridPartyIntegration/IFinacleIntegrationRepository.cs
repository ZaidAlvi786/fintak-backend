using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.ThridPartyIntegration
{
  public  interface IFinacleIntegrationRepository
    {
        #region
        Task<List<BatchPostingViewModel>> GetBatchPostingDetail(DateTime startDate, DateTime endDate, string searchInfo);
        Task<List<BatchPostingViewModel>> GetBatchPostingMain(DateTime startDate, DateTime endDate, string searchInfo);
        Task<List<BatchPostingViewModel>> GetBatchPostingDetailSearch(DateTime startDate, DateTime endDate, string status);
        Task<CRMSRecord> GenerateExcell(DateTime date, string loanAcct);
        Task<CRMSRecord> GetEODErrorLogDetail(FinanceEndofdayViewModel model);


        #endregion
    }
}
