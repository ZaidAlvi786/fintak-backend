using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.ViewModels;
using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels.WorkFlow;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FintrakBanking.ViewModels.Credit;
using System.Threading.Tasks;
using System.Linq;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common;
using System.Threading;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/workflow")]
    public class JobRequestController : ApiControllerBase
    {
        
        private IJobRequestRepository repo;

        public JobRequestController(IJobRequestRepository repo)
        {
            this.repo = repo;
        }
        TokenDecryptionHelper token = new TokenDecryptionHelper();

        //[HttpGet] [ClaimsAuthorization]  
        //[Route("job-request")]
        //public HttpResponseMessage GetJobRequest()
        //{
        //    var data = repo.GetAllJobRequest();
        //    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        //}       

        [HttpGet] [ClaimsAuthorization]  
        [Route("job-request-detail/legal-details")]
        public async Task<HttpResponseMessage> GetLegalJobRequestDetails()
        {
            var data = await repo.GetLegalJobRequestDetails();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("legal-job-request-detail/{jobRequestId}")]
        public async Task<HttpResponseMessage> GetJobRequestDetailsById(int jobRequestId)
        {
            var data = await repo.GetJobRequestDetailsById(jobRequestId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }


        [HttpGet] [ClaimsAuthorization]  
        [Route("job-request/loan-application-details/{applicationId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationJobsById(int applicationId)
        {
            var data =await repo.GetLoanApplicationJobsById(applicationId, token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("job-request-status-feedback/{statusId}/{jobTypeId}")]
        public async Task<HttpResponseMessage> GetJobRequestStatusFeedback(short statusId, short jobTypeId)
        {
            var data = await repo.GetJobRequestStatusFeedback(statusId, jobTypeId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("reasigned-job-by-staff/{staffId}")]
        public async Task<HttpResponseMessage> GetJobReasignmentStaffById(int staffId)
        {
            var data =await repo.GetJobReasignmentStaffById(staffId, token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        
        [HttpGet] [ClaimsAuthorization]  
        [Route("job-request/staff")]
        public async Task<HttpResponseMessage> getJobRequestByStaffId()
        {
            var data = await repo.GetJobRequestByStaffId(token.GetStaffId, token.GetBranchId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-request/search/{searchString}")]
        public async Task<HttpResponseMessage> GetJobRequestBySearchString(string searchString)
        {
            var data = await repo.GetJobRequestBySearchString(token.GetStaffId, searchString);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("facility-job-request/{facilityReferenceNumber}")]
        public async Task<HttpResponseMessage> GetAllGlobalJobRequestByFacilityRef(string facilityReferenceNumber)
        {
            var data =await repo.GetAllGlobalJobRequestByFacilityRef(facilityReferenceNumber);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-type-admin-staff")]
        public async Task<HttpResponseMessage> GetJobTypeReasignmentAdminStaff()
        {
            var data =await repo.GetJobTypeReasignmentAdmin(token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-type-hub-staff")]
        public async Task<HttpResponseMessage> GetJobTypeHubStaff()
        {
            var data = await repo.GetJobTypeHubStaff();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("job-request/{jobRequestId}")]
        public async Task<HttpResponseMessage> GetJobRequest(int jobRequestId)
        {
            var data = await repo.GetJobRequest(jobRequestId);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("filter-job-request-by-status/{filter}/{startNumber}")]
        public async Task<HttpResponseMessage> GetJobRequestByFilter(string filter,int? startNumber)
        {
            var data = await repo.GetJobRequestByFilter(token.GetStaffId, token.GetBranchId, filter, startNumber);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("count-job-request-by-status")]
        public async Task<HttpResponseMessage> GetJobRequestStatusCount()
        {
            var data =await repo.GetJobRequestStatusCount(token.GetStaffId, token.GetBranchId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("application-detail-job-request/{targetId}/Operation/{operationId}/source/{jobSourceId}")]
        public async Task<HttpResponseMessage> GetApplicationJobRequest(int targetId, int operationId,short jobSourceId)
        {
            var data =await repo.GetApplicationJobRequest(targetId, operationId, jobSourceId);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-request-approval-status")]
        public async Task<HttpResponseMessage> GetJobRequestApprovaStatus()
        {
            var data =await repo.GetJobRequestApprovaStatus();

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("job-request/comments/{jobRequestId}")]
        public async Task<HttpResponseMessage> GetJobComments(int jobRequestId)
        {
            var data = await repo.GetJobComments(jobRequestId);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost] [ClaimsAuthorization]
        [Route("global-job-request")]
        public async Task<HttpResponseMessage> AddGlobalJobRequest([FromBody] JobRequestViewModel entity)
         {
            entity.companyId = (int)token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.branchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.branchId = (short)token.GetBranchId;

            var code = await repo.AddGlobalJobRequest(entity);
            if (code != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = code, message = TranslateHelper.get("Request logged successfully. The Request Code is") + " " + code });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error logging this request") });
        }

        [HttpPost] [ClaimsAuthorization]
        [Route("job-request/comment")]
        public async Task<HttpResponseMessage> AddJobComment([FromBody] JobRequestMessageViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            var data =await repo.AddJobComment(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Comment added successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error commenting on this job") });
        }


        [HttpPut] [ClaimsAuthorization]
        [Route("job-request/reply/{jobRequestId}")]
        public async Task<HttpResponseMessage> ReplyJobRequest([FromBody] JobRequestViewModel entity, int jobRequestId)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.lastUpdatedBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;

            var data =await repo.ReplyJobRequest(entity, jobRequestId);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = TranslateHelper.get("Job response was successfully saved") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("re-route-job")]
        public async Task<HttpResponseMessage> ReRouteJobRequest([FromBody] JobRequestViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.lastUpdatedBy = token.GetStaffId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.staffId = token.GetStaffId;

            var data =await  repo.ReRouteJobRequest(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = TranslateHelper.get("The job re-route was successful") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
        }

        [HttpPut, Route("job-request/reassign/{jobRequestId}")]
        public async Task<HttpResponseMessage> ReassignJobRequest([FromBody] JobRequestViewModel entity, int jobRequestId)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.lastUpdatedBy = token.GetStaffId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.staffId = token.GetStaffId;

            var data = await repo.ReassignJobRequest(entity, jobRequestId);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = TranslateHelper.get("The job been assigned successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
        }

        public async Task<HttpResponseMessage> AcknowledgeJob([FromBody] JobRequestViewModel entity, int jobRequestId)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.lastUpdatedBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            var data = await repo.AcknowledgeJob(entity, jobRequestId);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = TranslateHelper.get("Job Acknowledged") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
        }
        #region Middle Office Request
        [HttpPut, Route("job-request/invoice-status")]
        public async Task<HttpResponseMessage> UpdateInvoiceStatus([FromBody] JobRequestInvoiceViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = await repo.UpdateInvoiceStatus(entity);
                if (data)
                {
                    if (entity.status)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = "Invoice successfully approved." });
                    }
                    else return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = "Invoice successfully disapproved." });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"There was an error updating this record." });
            }
        }
        #endregion Middle Office Request

        #region Collateral Job Search Charges
        [HttpPost]
        [ClaimsAuthorization]
        [Route("job-request/legal-collateral-job")]
        public async Task<HttpResponseMessage> EffectLegaCollateralJobs([FromBody] JobRequestCollateralSearchViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            var data = await repo.saveCollateralJobsChargesSpecifiedByLegal(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = "Search charge instruction sent Successfully" });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Failure! Search charge Instructions failed to save " });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("job-request/place-legal-job-charges")]
        public async Task<HttpResponseMessage> PlaceChargeOnCustomerForCollateralSearch([FromBody] JobRequestCollateralSearchViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            var data = await repo.ChargeCustomerForOnSearchJobs(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = "Operation Performed Successfully" });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Failure! failed to Perform Operation " });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("job-request/reverse-legal-job-charges")]
        public async Task<HttpResponseMessage> ReverseChargeOnCustomerForCollateralSearch([FromBody] JobRequestCollateralSearchViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            var data = await repo.ReverseChargeOnCustomerForCollateralSearch(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = "Operation Performed Successfully" });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Failure! failed to Perform Operation " });
        }

        #endregion End of Collateral Job Search Charges


        #region Job-Documents

        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-request-documents/{jobRequestCode}")]
        public async Task<HttpResponseMessage> GetJobRequestDocuments(string jobRequestCode)
        {
            var data = await repo.GetJobRequestDocuments(jobRequestCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-request-document/{documentId}")]
        public async Task<HttpResponseMessage> GetJobRequestDocumentById(int documentId)
        {
            var data = await repo.GetJobRequestDocumentById(documentId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("job-document")]
        public async Task<HttpResponseMessage> AddJobDocument()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported media type.");
                }

                MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
                Task.Factory
                    .StartNew(() => provider = Request.Content.ReadAsMultipartAsync(provider).Result,
                        CancellationToken.None,
                        TaskCreationOptions.LongRunning, // guarantees separate thread
                        TaskScheduler.Default)
                    .Wait();
                //if(documentList)

                int uploadType;
                if (!Int32.TryParse(provider.FormData["documentTypeId"], out uploadType))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Upload Type is invalid.");
                }

                var entity = new RequestDocumentViewModel
                {
                    //targetId = Convert.ToInt32(provider.FormData["targetId"]),
                    //targetReferenceNumber = provider.FormData["targetReferenceNumber"],
                    jobRequestCode = provider.FormData["jobRequestCode"],
                    documentTitle = provider.FormData["documentTitle"],
                    documentTypeId = (short)uploadType,
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                    physicalFileNumber = provider.FormData["physicalFileNumber"],
                    physicalLocation = provider.FormData["physicalLocation"],

                };

                var receiverStaffId = provider.FormData["receiverStaffId"];

                var requestModel = new JobRequestViewModel();
                requestModel.departmentId = (short)Convert.ToInt32(provider.FormData["departmentId"]);
                requestModel.departmentUnitId = (short)Convert.ToInt32(provider.FormData["departmentUnitId"]);
                requestModel.requestTitle = provider.FormData["requestSubject"];
                requestModel.senderComment = provider.FormData["senderComment"];
                requestModel.targetId = Convert.ToInt32(provider.FormData["targetId"]);
                requestModel.operationsId = Convert.ToInt32(provider.FormData["operationId"]);
                requestModel.jobTypeId = (short)Convert.ToInt32(provider.FormData["jobTypeId"]);
                requestModel.jobSubTypeId = (short)Convert.ToInt32(provider.FormData["jobSubTypeId"]);
                requestModel.isReassigned = provider.FormData["isReassigned"].ToLower() != "undefined" ? Convert.ToBoolean(provider.FormData["isReassigned"]) : false;
                requestModel.isAcknowledged = provider.FormData["isAcknowledged"] != "undefined" ? Convert.ToBoolean(provider.FormData["isAcknowledged"]) : false;

                if (receiverStaffId != null && receiverStaffId != string.Empty && receiverStaffId != "" && receiverStaffId != "undefined" && receiverStaffId != "null")
                    requestModel.receiverStaffId = Convert.ToInt32(receiverStaffId);

                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No file uploaded.");
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                requestModel.userBranchId = (short)token.GetBranchId;
                requestModel.branchId = (short)token.GetBranchId;
                requestModel.companyId = token.GetCompanyId;
                requestModel.createdBy = token.GetStaffId;
                requestModel.applicationUrl = HttpContext.Current.Request.Path;

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                var code = await repo.AddJobDocument(entity, requestModel, buffer);

                if (code != string.Empty)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = code, message = "Request logged successfully. The Request Code is " + code });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")}." + ex.Message + ". " + ex.InnerException });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("job-reply-and-job-document")]
        public async Task<HttpResponseMessage> AddJobReplyAndDocument()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported media type.");
                }

                MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
                Task.Factory
                    .StartNew(() => provider = Request.Content.ReadAsMultipartAsync(provider).Result,
                        CancellationToken.None,
                        TaskCreationOptions.LongRunning, // guarantees separate thread
                        TaskScheduler.Default)
                    .Wait();

                int uploadType;
                if (!Int32.TryParse(provider.FormData["documentTypeId"], out uploadType))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Upload Type is invalid.");
                }

                var entity = new RequestDocumentViewModel();
                entity.jobRequestCode = provider.FormData["jobRequestCode"];
                entity.documentTitle = provider.FormData["documentTitle"];
                entity.documentTypeId = (short)uploadType;
                entity.fileName = provider.FormData["fileName"];
                entity.fileExtension = provider.FormData["fileExtension"];
                entity.physicalFileNumber = provider.FormData["physicalFileNumber"];
                entity.physicalLocation = provider.FormData["physicalLocation"];
                entity.comment = provider.FormData["responseComment"];
                entity.statusId = (short)Convert.ToInt32(provider.FormData["statusId"]);

                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No file uploaded.");
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                var data = await repo.AddJobReplyAndDocument(entity, buffer);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")}." });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("job-document-only")]
        public async Task<HttpResponseMessage> AddJobDocumentOnly()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported media type.");
                }

                MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
                Task.Factory
                    .StartNew(() => provider = Request.Content.ReadAsMultipartAsync(provider).Result,
                        CancellationToken.None,
                        TaskCreationOptions.LongRunning, // guarantees separate thread
                        TaskScheduler.Default)
                    .Wait();

                //int uploadType;
                //if (!Int32.TryParse(provider.FormData["documentTypeId"], out uploadType))
                //{
                //    return Request.CreateResponse(HttpStatusCode.BadRequest, "Upload Type is invalid.");
                //}

                var entity = new RequestDocumentViewModel();
                entity.jobRequestCode = provider.FormData["jobRequestCode"];
                entity.documentTitle = provider.FormData["documentTitle"];
                entity.documentTypeId = 1; // (short)uploadType;
                entity.fileName = provider.FormData["fileName"];
                entity.fileExtension = provider.FormData["fileExtension"];
                entity.physicalFileNumber = provider.FormData["physicalFileNumber"];
                entity.physicalLocation = provider.FormData["physicalLocation"];
                // entity.comment = provider.FormData["responseComment"];

                if (entity.jobRequestCode == "undefined") return Request.CreateResponse(HttpStatusCode.BadRequest, "upload failed");

                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No file uploaded.");
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                var data = await repo.AddJobDocumentOnly(entity, buffer);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = "Document uploaded successfully" });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Error occured while uploaded document" });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")}." });
            }
        }
        [HttpDelete, Route("delete-job-request-document/{documentId}")]
        public async Task<HttpResponseMessage> deleteJobDocument(int documentId)
        {
            try
            {
                if (await repo.deleteJobDocument(documentId, token.GetStaffId))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Document Successfully deleted" });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "There was an error deleting this record" });
            }

            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"There was an error updating this record" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("lmsr-application-data/{targetId}")]
        public async Task<HttpResponseMessage> getLMSRDetail(int targetId)
        {
            var data = await repo.getLMSRApplicationDetail(targetId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("lmsr-operation-data/{targetId}")]
        public async Task<HttpResponseMessage> getLMSROperation(int targetId)
        {
            var data = await repo.getLMSROperation(targetId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-data/{loanId}/operation/{operationId}")]
        public async Task<HttpResponseMessage> getLMSROperation(int loanId, int operationId)
        {
            var data = await repo.getLOSOperationLoanData(loanId, operationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        #endregion Job-Documents


        #region job-type

        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-type")]
        public async Task<HttpResponseMessage> GetJobType()
        {
            var data = await repo.GetAllJobType();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-sub-type/{jobId}")]
        public async Task<HttpResponseMessage> GetJobSubType(short jobId)
        {
            var data = await repo.GetJobSubType(jobId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-sub-type-class/{jobSubTypeId}")]
        public async Task<HttpResponseMessage> GetJobSubTypeClass(short jobSubTypeId)
        {
            var data = await repo.GetJobSubTypeClass(jobSubTypeId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost, Route("map-job-type-hub-staff")]
        public async Task<HttpResponseMessage> mapJobTypeHubStaff([FromBody] JobTypeHubViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;


            var data = await repo.mapJobTypeHubStaff(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = "The staff - hub mapping was successful" });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
        }

        [HttpPut, Route("update-map-job-type-hub-staff")]
        public async Task<HttpResponseMessage> UpdatemappedJobTypeHubStaff([FromBody] JobTypeHubViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;

            if (await repo.UpdatemappedJobTypeHubStaff(entity)) { return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = "Update was successful" }); }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
        }


        [HttpDelete, Route("delete-mapped-job-type-hub-staff/{hubStaffId}")]
        public async Task<HttpResponseMessage> DeleteMappedJobTypeHubStaff(int hubStaffId)
        {
            try
            {

                if (await repo.DeleteMappedJobTypeHubStaff(hubStaffId, token.GetStaffId)) { return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Delete was successful." }); }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "There was an error deleting this record" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"There was an error deleting this record" });
            }
        }

        [HttpPost, Route("assign-job-type")]
        public async Task<HttpResponseMessage> AssignJobTypeToStaff([FromBody] jobReasignment entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;


            var data = await repo.AssignJobTypeToStaff(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = "The job type been assigned successfully" });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
        }

        [HttpPut, Route("update-assigned-job-type")]
        public async Task<HttpResponseMessage> UpdateAssignJobTypeToStaff([FromBody] jobReasignment entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;


            var data = await repo.UpdateAsignedJobTypeToStaff(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = "The job type been assigned successfully" });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
        }



        [HttpPost, Route("delete-assigned-job-type")]
        public async Task<HttpResponseMessage> DeleteAssignedJobTypeToStaff([FromBody] jobReasignment entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            //entity.dateTimeCreated = 


            var data = await repo.DeleteJobTypeForAStaff(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = "The job type been assigned successfully" });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-type-hub/{jobTypeId}")]
        public async Task<HttpResponseMessage> GetAllJobTypeHub(short jobTypeId)
        {
            var data =await repo.GetAllJobTypeHub(jobTypeId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        } 

        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-type-unit/{jobTypeId}")]
        public async Task<HttpResponseMessage> GetAllJobTypeUnit(short jobTypeId)
        {
            var data =await repo.GetAllJobTypeUnit(jobTypeId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]      [ClaimsAuthorization]
        [Route("job-hub-staff/{hubId}")]
        public async Task<HttpResponseMessage> GetHubStaffByHubId(short hubId)
        {
            var data =await repo.GetHubStaffByHubId(hubId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-unit-hub-staff/{unitId}")]
        public async Task<HttpResponseMessage> GetHubStaffByHubTypeUnitId(short unitId)
        {
            var data = await repo.GetHubStaffByHubTypeUnitId(unitId);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                success = true,
                result = data
            });
         }


        [HttpPost] [ClaimsAuthorization][Route("job-type")]
        public async Task<HttpResponseMessage> AddJobType([FromBody] JobTypeViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            var data =await repo.AddJobType(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

       [HttpPut] [ClaimsAuthorization][Route("job-type/{jobTypeId}")]
        public async Task<HttpResponseMessage> UpdateJobType([FromBody] JobTypeViewModel entity, short jobTypeId)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.lastUpdatedBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            var data = await repo.UpdateJobType(entity, jobTypeId);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = TranslateHelper.get("The record has been updated successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
        }

       
        [HttpGet]
        [ClaimsAuthorization]
        [Route("reasigned-job-type")]
        public async Task<HttpResponseMessage> GetJobTypeReasignment(int staffId)
        {
            var data = await repo.GetJobTypeReasignmentAdmin(token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }
        #endregion job-type


        #region Job Request Feedback
        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-request-status")]
        public async Task<HttpResponseMessage> GetJobRequestStatus()
        {
            try
            {
                var data =await repo.GetJobRequestStatus();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("job-request-feedback")]
        public async Task<HttpResponseMessage> GetAllJobRequestStatusFeedback()
        {
            try
            {
                var data = await repo.GetAllJobRequestStatusFeedback();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("job-request-feedback")]
        public async Task<HttpResponseMessage> AddUpdateCompanyDirector([FromBody]JobRequestStatusFeedbackViewModel entity)
        {
            try
            {
                string createUpdate = "";
                if (entity.jobStatusFeedbackId != 0 || entity.jobStatusFeedbackId > 0)
                {
                    createUpdate = "updated";
                }
                else
                {
                    createUpdate = "created";
                    if (await repo.ValidateJobRequestFeedBack(entity.jobStatusFeedbackName))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                            new { success = false, message = $"Job request feedback {entity.jobStatusFeedbackName} already exist." });
                    }
              
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;

                var data =await repo.AddUpdateJobRequestFeedBack(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = $"{TranslateHelper.get("The record has been")} {createUpdate} {TranslateHelper.get("successfully")}" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"There was an error {createUpdate} this record" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }
        #endregion

    }
}
