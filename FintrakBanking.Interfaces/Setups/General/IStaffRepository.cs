using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.WorkFlow;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.General
{
    public interface IStaffRepository
    {
        Task<staffBulkFeedbackViewModel> UploadBulkPrepaymentData(StaffDocumentViewModel model, byte[] file);
        Task<bool> DeleteBulkPrepayment(int bulkPrepaymentId, UserInfo user);
        Task<bool> UpdatePrepayment(int staffid, StaffInfoViewModel staffModel);
        Task<bool> AddBulkPrepaymentData(StaffInfoViewModel staffModel, int batchCode, DateTime applicationDate);
        bool UpdateStaff(int staffid, StaffInfoViewModel staffModel);
        Task<IEnumerable<BatchPrepaymentViewModel>> GetAllUnprocessedBulkPrepayment();
        bool AddTempStaff(StaffInfoViewModel staffModel);
        Task<int> GoForApproval(ApprovalViewModel entity);
        Task<int> GoForStaffDeleteApproval(ApprovalViewModel entity);
        IEnumerable<StaffInfoViewModel> GetAllStaff();
        List<StaffSensitivityLevelViewModel> GetStaffSensitivityLevel();
        Task<IEnumerable<StaffInfoViewModel>> GetStaffAwaitingApprovals(int staffId, int companyId);
        Task<IEnumerable<StaffInfoViewModel>> GetStaffDeleteRequestAwaitingApprovals(int staffId, int companyId);
        simpleStaffModel StaffReportingTo(int staffId, string staffCode, int companyId);
        IEnumerable<StaffViewModel> GetStaffName();
        Task<IEnumerable<simpleStaffModel>> GetStaffRelationshipManagerByStaffId(int staffId);
        IEnumerable<simpleStaffModel> GetStaffBusinessManagerByStaffId(int staffId);
        IEnumerable<simpleStaffModel> GetStaffByUnitId(int companyId, short departmentUnitId);
        Task<bool> LogDeleteRequestStaff(int staffId, UserInfo user);
        bool IsStaffCodeAlreadyExist(string staffCode);
        bool IsTempStaffExist(string staffCode);
        Task<StaffInfoViewModel> GetStaffById(int staffId);
        Task<StaffDetailsModel> GetTempStaffDetail(int staffId);
        IEnumerable<StaffDetailsModel> GetStaffDetails(int companyId);
        StaffDetailsModel GetStaffDetail(string staffCode, int companyId);
        IEnumerable<simpleStaffModel> GetStaffNames(int companyId);
        IEnumerable<simpleStaffModel> GetStaffRoles(int companyId);
        Task<IEnumerable<ApprovalStatusViewModel>> GetApprovalStatus();
        IQueryable<simpleStaffModel> SearchStaff(string searchString, int companyId); 
        IQueryable<simpleStaffModel> SearchStaffbyDepartmentId(string searchString, int companyId, int departmentId);
        bool AddStaffSignature(StaffDocumentViewModel model, byte[] file);
        Task<bool> UpdateStaffSignature(StaffDocumentViewModel model, int documentId);
        Task<staffBulkFeedbackViewModel> UploadStaffData(StaffDocumentViewModel model, byte[] file);
        IEnumerable<StaffDocumentViewModel> GetAllStaffSignatures(int companyId);
        StaffDocumentViewModel GetStaffSignatureByStaffCode(string staffCode, int companyId);
        Task<bool> UpdateSupervisor(SupervisorViewModel entity);
        Task<bool> GoForBulkApproval(List<ApprovalViewModel> model, UserInfo userInfo);
        Task<List<simpleStaffModel>> StaffReportingLine(int staffId, string staffCode, int companyId);
        Task<simpleStaffModel> StaffInformation(int staffId, string staffCode, int companyId);
        StaffMISDetailsModel StaffMIS(int staffId, string staffCode);
        Task<IEnumerable<simpleStaffModel>> GetSearchedStaff(string search);
        Task<IEnumerable<simpleStaffModel>> SearchApprovers(int levelId, string queryString, int getCompanyId);
        Task<IEnumerable<BatchPrepaymentViewModel>> GetAllUnprocessedBulkPrepaymentBatch(int staffId);
        Task<bool> SubmitPrepaymentBatchForApproval(ApprovalViewModel model);
        Task<IEnumerable<BatchPrepaymentViewModel>> GetBulkPrepaymentsAwaitingApprovalBatch(int staffId, int companyId);
        Task<bool> SubmitPrepaymentBatchForWorkflowApproval(ApprovalViewModel model);
        IEnumerable<BatchPrepaymentViewModel> GetProcessingBulkPrepaymentByBatchId(int batchId);
    }
}