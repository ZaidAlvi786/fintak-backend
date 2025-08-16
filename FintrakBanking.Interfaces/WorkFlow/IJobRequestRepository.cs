using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.General;
using FintrakBanking.ViewModels.WorkFlow;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.WorkFlow
{
    public interface IJobRequestRepository
    {
        Task<IEnumerable<JobTypeHubViewModel>> GetJobTypeHubStaff();
        Task<IEnumerable<HubStaffViewModel>> GetHubStaffByHubId(short jobTypeHubId);
        Task<IEnumerable<HubStaffViewModel>> GetHubStaffByHubTypeUnitId(short jobTypeUnitId);
        Task<IEnumerable<JobTypeUnitViewModel>> GetAllJobTypeUnit(short jobTypeId);
        Task<IEnumerable<JobTypeHubViewModel>> GetAllJobTypeHub(short jobTypeId);
        Task<IEnumerable<JobRequestViewModel>> GetJobRequestByFilter(int staffId, int branchId, string filter, int? startNumber);
        Task<List<jobReasignment>> GetJobReasignmentStaffById(int staffId, int companyId);
        Task<IEnumerable<ApprovalStatusViewModel>> GetJobRequestApprovaStatus();
        Task<IEnumerable<JobRequestStatusFeedbackViewModel>> GetJobRequestStatusFeedback(short statusId, short jobTypeId);
        Task<JobRequestViewModel> GetJobRequest(int jobRequestId);

        Task<List<JobRequestViewModel>> GetApplicationJobRequest(int targetId, int operationId, short jobSourceId);

        Task<IEnumerable<ApplicationJobRequest>> GetLoanApplicationJobsById(int loanApplicationId, int companyId);
        Task<IEnumerable<JobRequestViewModel>> GetJobRequestByStaffId(int staffId, int branchId);
        Task<IEnumerable<JobRequestMessageViewModel>> GetJobComments(int jobRequestId);
        Task<string> AddGlobalJobRequest(JobRequestViewModel model);
        Task<bool> AddJobComment(JobRequestMessageViewModel model);
        Task<bool> ReplyJobRequest(JobRequestViewModel model, int jobRequestId);
        Task<bool> ReRouteJobRequest(JobRequestViewModel model);
        Task<bool> ReassignJobRequest(JobRequestViewModel model, int jobRequestId);
        Task<List<JobRequestViewModel>> GetJobRequestLegalJobDetail();
        Task<List<JobRequestDetailViewModel>> GetJobRequestDetailsById(int jobRequestId);
        Task<bool> AcknowledgeJob(JobRequestViewModel entity, int jobRequestId);
        Task<List<JobRequestDetailViewModel>> GetLegalJobRequestDetails();
        Task<IEnumerable<JobRequestViewModel>> GetAllGlobalJobRequestByFacilityRef(string facilityRef);
        Task<IEnumerable<JobRequestViewModel>> GetJobRequestBySearchString(int staffId, string searchString);
        Task<List<jobReasignment>> GetJobTypeReasignmentAdmin(int companyId);
        Task<jobRequestCountViewModel> GetJobRequestStatusCount(int staffId, int branchId);

        #region ...Collateral Search Job Charges...
        Task<bool> ChargeCustomerForOnSearchJobs(JobRequestCollateralSearchViewModel model);
        Task<bool> ReverseChargeOnCustomerForCollateralSearch(JobRequestCollateralSearchViewModel model);
        Task<bool> saveCollateralJobsChargesSpecifiedByLegal(JobRequestCollateralSearchViewModel model);
        #endregion End of Collateral Search Job Charges

        #region Job Type
        Task<bool> AssignJobTypeToStaff(jobReasignment model);
        Task<bool> DeleteJobTypeForAStaff(jobReasignment model);
        Task<bool> UpdateAsignedJobTypeToStaff(jobReasignment model);
        Task<bool> mapJobTypeHubStaff(JobTypeHubViewModel model);
        Task<bool> UpdatemappedJobTypeHubStaff(JobTypeHubViewModel model);
        Task<bool> DeleteMappedJobTypeHubStaff(int hubStaffId, int staffId);
        Task<IEnumerable<JobTypeViewModel>> GetAllJobType();
        Task<IEnumerable<JobTypeViewModel>> GetJobSubType(short jobId);
        Task<IEnumerable<JobSubTypeClassViewModel>> GetJobSubTypeClass(short jobSubTypeId);
        #endregion end of Job Type

        #region ...Middle Office Updates...
        Task<bool> UpdateInvoiceStatus(JobRequestInvoiceViewModel model);
        #endregion ...End of Middle Office Updates...

        #region Job Request Setup
        Task<bool> UpdateJobType(JobTypeViewModel model, short jobTypeId);
        Task<bool> AddJobType(JobTypeViewModel model);
        #endregion

        #region Job Request Documents
        Task<string> AddJobDocument(RequestDocumentViewModel model, JobRequestViewModel requestModel, byte[] file);
        Task<bool> AddJobReplyAndDocument(RequestDocumentViewModel model, byte[] file);
        Task<bool> UpdateJobDocument(RequestDocumentViewModel model, int documentId);
        Task<IEnumerable<RequestDocumentViewModel>> GetAllJobDocument();
        Task<RequestDocumentViewModel> GetJobDocument(int documentId);
        Task<IEnumerable<RequestDocumentViewModel>> GetJobRequestDocuments(string jobRequestCode);
        Task<IEnumerable<RequestDocumentViewModel>> GetJobRequestDocumentById(int documentId);
        Task<bool> AddJobDocumentOnly(RequestDocumentViewModel model, byte[] file);
        Task<IEnumerable<LMSOperationListViewModel>> getLMSRApplicationDetail(int targetId);
        Task<IEnumerable<LMSOperationListViewModel>> getLMSROperation(int targetId);
        Task<IEnumerable<LMSOperationListViewModel>> getLOSOperationLoanData(int loanId, int operationId);
        Task<bool> deleteJobDocument(int documentId, int staffId);
        #endregion

        #region Job Request Feedback
        Task<IEnumerable<LookupViewModel>> GetJobRequestStatus();
        Task<IEnumerable<JobRequestStatusFeedbackViewModel>> GetAllJobRequestStatusFeedback();
        Task<bool> AddUpdateJobRequestFeedBack(JobRequestStatusFeedbackViewModel feedback);
        Task<bool> ValidateJobRequestFeedBack(string feedback);
        #endregion
    }
}