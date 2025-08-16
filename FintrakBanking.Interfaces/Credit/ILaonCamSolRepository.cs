using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
   public interface ILaonCamSolRepository
    {
        Task<List<LoanCAMSOLViewModel>> GetCamSol();
        Task<List<LoanCAMSOLViewModel>> GetCamSolByType(int customerName);
        Task<LoanCAMSOLViewModel> ViewCamSolByType(int id);
        List<LoanCAMSOLViewModel> GetCamSol(string loancamsolid);
        Task<List<LoanCAMSOLViewModel>> GetCamSolType();
        Task<List<LoanCAMSOLViewModel>> GetCamSolByCustomerCode(string customerCode);
        Task<string> ApproveCamsol(LoanCAMSOLViewModel option);
        Task<List<LoanCAMSOLViewModel>> CamSolAwaitingApproval(int companyId, int staffId);
        Task<LoanCAMSOLViewModel> CamSolAwaitingApprovalById(int id);
        Task<string> goForApproval(LoanCAMSOLViewModel data);
        Task<bool> GoForBulkApproval(LoanCAMSOLViewModel data);
        camsolBulkFeedbackViewModel UploadCamsolData(CamsolDocumentViewModel model, byte[] file);
        //bool GoForBulkApproval(List<ApprovalViewModel> model, UserInfo userInfo);



    }
}
