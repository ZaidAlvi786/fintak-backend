using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Reports;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.CRMS
{
    public interface ICRMSRegulatories
    {
        Task<string> GetCRMSCode(CRMSViewModel code);
        Task<bool> ResetCrmsCode(CRMSViewModel code);
        Task<string> AddCRMSCodeAsync(CRMSViewModel code);
        Task<List<CRMSRegulatoryViewModel>> GetAllLoansForCRMS(CRMSViewModel data);
        Task<CRMSRecord> GenerateCBNReport(List<CRMSViewModel> paramx);
        Task<CRMSRecord> GenerateCBNReportByLoanAppId(List<CRMSViewModel> paramx);

        Task<List<LoansCount>> LoanCountsByLegalStatus(List<CRMSRegulatoryViewModel> loans);
        Task<CRMSRecord> GenerateBatchPosting(DateRange model);

        Task<bool> GenerateCRMSCodes(List<int> loanBookingRequestIds, UserViewModel model);
        Task<bool> GenerateCRMSCode(int loanBookingRequestId, UserViewModel model);

    }
}
