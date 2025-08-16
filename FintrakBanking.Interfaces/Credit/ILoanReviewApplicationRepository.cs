using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.CASA;
using FintrakBanking.ViewModels.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface ILoanReviewApplicationRepository
    {
        Task<IEnumerable<LoanApplicationDetailViewModel>> ExceptionalSearch(string searchString);
        Task<IEnumerable<LoanReviewOperationViewModel>> ContingentSearch(string searchString);
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> GetAllLienRemovalApplications(int staffId, int companyId);
        
        Task<IEnumerable<LoanReviewOperationApprovalViewModel>> SearchLien(string searchString);
        Task<IQueryable<LoanReviewApplicationViewModel>> GetApplications(UserInfo user, int operationId, int? productClassId);
        List<applicationDetails> GetApplicationsById(UserInfo user, int lmsApplicationId);
        List<LoanReviewApplicationViewModel> CalculateSLA(List<LoanReviewApplicationViewModel> apps);
        IQueryable<LoanReviewApplicationViewModel> GetLoanReviewAvailmentAwaitingApproval(UserInfo user, int operationId, int? classId);
        Task<IQueryable<LoanReviewApplicationViewModel>> GetLoanReviewForCRMS(UserInfo user, int operationId, int? classId);
        IQueryable<LoanReviewApplicationViewModel> GetLoanReviewDrawdownApproval(UserInfo user, int? classId);
        Task<SelectListViewModel> GetAllSelectList();
        SelectListViewModel GetAllLMSApprovalOperationList();
        Task<SelectListViewModel> GetAllLMSApprovalOperationListByProductTypeId(int productTypeId);
        Task<LoanChargeFeeViewModel> GetChargeFeeDetails(int id);

        Task<bool> ValidateSubAllocationOperation(int loanApplicationDetailId, int customerId);

        Task<string> SubmitLoanReviewApplication(LoanReviewApplicationViewModel entity);

        Task<List<LoanReviewOperationViewModel>> GetMaturityInstruction(int loanId, short loansystemTypeId);

        //List<LoanViewModel> LoanSearch(int getCompanyId, SearchViewModel search);

        //int SaveCam(CamViewModel cam);

        //CamViewModel GetCamDocument(int documentationId);

        //CamViewModel GetCamDocumentByApprovalLevel(int applicationId, int staffId);

        //List<CamViewModel> GetCamDocuments(int applicationId);

        Task<WorkflowResponse> ForwardApplication(ForwardReviewViewModel model);
        Task<WorkflowResponse> ForwardApplicationAppraisal(ForwardReviewViewModel model);

        Task<LoanApplicationDetailViewModel> GetLoanApplicationDetail(int loanId, int loanTypeId);

        Task<IQueryable<LoanReviewApplicationViewModel>> GetRegionalLoanApplications(int getStaffId);

        Task<IEnumerable<LoanApplicationViewModel>> Search(string searchString);
        bool AppraisalReviewReferBack(ForwardViewModel entity);
        Task<bool> UpdateManagementPosition(ManagementPositionViewModel entity);
        Task<ManagementPositionViewModel> GetManagementPosition(int detailId);

        Task<bool> ValidateNewSubAllocationOperation(int loanApplicationDetailId, int customerId, int loanSystemTypeId);

        Task<List<LoanReviewOperationViewModel>> GetLMSOperation(int loanId, short loansystemTypeId);
        Task<decimal?> GetWrittenOffAccrualAmount(int loanId, short loanSystemTypeId);
        Task<decimal> GetMaximumApplicationOutstandingBalance(int applicationId);
        Task<ContingentLoansViewModel> GetContingentTotoalUsed(int contingetLoanId);
    }
}
