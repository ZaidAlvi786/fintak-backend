using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.ErrorLogger;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.WorkFlow;
using FintrakBanking.ViewModels.Setups.General;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using FintrakBanking.Common.CustomException;
using System.Text;
using FintrakBanking.ViewModels.Setups.Credit;
using FintrakBanking.Common;
using System.Threading;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/setup")]
    public class StaffController : ApiControllerBase
    {
        TokenDecryptionHelper token = new TokenDecryptionHelper();
        private IStaffRepository repo;
        private IErrorLogRepository errorLogger;

        public StaffController(IStaffRepository _repo,
                                IErrorLogRepository _errorLogger)
        {
            this.repo = _repo;
            this.errorLogger = _errorLogger;
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("staff")]
        public HttpResponseMessage GetstaffInfo()
        {
            try
            {
                var staffInfo = repo.GetAllStaff().Where(x => x.companyId == token.GetCompanyId).ToList();

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = staffInfo, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("sensitivitylevel")]
        public HttpResponseMessage GetSensitivityLevel()
        {
            try
            {
                var data = repo.GetStaffSensitivityLevel().OrderByDescending(a => a.level);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                //errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }
        [HttpGet] [ClaimsAuthorization]  
        [Route("staff/approvals/temp")]
        public async Task<HttpResponseMessage> GetStaffAwaitingApproval()
        {
            try
            {
                var staffInfo = await repo.GetStaffAwaitingApprovals(token.GetStaffId, token.GetCompanyId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo.ToList() });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("staff-delete/approvals/temp")]
        public async Task<HttpResponseMessage> GetStaffDeleteRequestAwaitingApprovals()
        {
            try
            {
                var staffInfo = await repo.GetStaffDeleteRequestAwaitingApprovals(token.GetStaffId, token.GetCompanyId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo.ToList() });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }



        [HttpDelete]
        [ClaimsAuthorization]
        [Route("bulkrepayment/{staffId}")]
        public async Task<HttpResponseMessage> DeleteBulkRepayment(int staffId)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    createdBy = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = Request.RequestUri.Host
                };
                var staff = await repo.DeleteBulkPrepayment(staffId, user);
                if (staff)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                          new { success = true, result = staff, message = TranslateHelper.get("Record deleted successfully") });
                    //new { success = true, result = staff, message = "Staff delete has been successfully submited for approval " });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("Deleting record failed") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("staff/bulk-prepament-reversal")]
        public async Task<HttpResponseMessage> UploadBulkPrepaymentData()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, TranslateHelper.get("Unsupported media type"));
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
                //    return Request.CreateResponse(HttpStatusCode.BadRequest, "File Type is invalid.");
                //}


                byte[] pass = Convert.FromBase64String(provider.FormData["loginStaffPassCode"]);
                string password = Encoding.UTF8.GetString(pass);

                var entity = new StaffDocumentViewModel
                {
                    staffCode = provider.FormData["staffCode"],
                    documentTitle = provider.FormData["documentTitle"],
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                    loginStaffPassword = password,
                    loginStaffCode = token.GetUsername
                };

                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("No file uploaded"));
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.branchId = (short)token.GetBranchId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;


                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                var data = await repo.UploadBulkPrepaymentData(entity, buffer);

                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Prepayment data was successfully uploaded") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Error uploading prepayment data") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")}. " + ex.Message });
            }
        }



        [HttpPut]
        [ClaimsAuthorization]
        [Route("bulkrepaymentinfo/{staffid}")]
        public async Task<HttpResponseMessage> UpdatePrepaymentInfo(int staffid, [FromBody] StaffInfoViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.companyId = (short)token.GetCompanyId;
                model.userIPAddress = Request.RequestUri.Host;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;

                var staff = await repo.UpdatePrepayment(staffid, model);

                if (staff)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = staff, message = TranslateHelper.get("Prepayment Amount has been updated successfully, now waiting for approval") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Prepayment Amount not updated" });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }




        //[HttpGet]
        //[ClaimsAuthorization]
        //[Route("staff/sample-document")]
        //public HttpResponseMessage GetStaffSampleDocument()
        //{
        //    try
        //    {
        //        var staffDoc = repo.GetStaffSampleDocument();

        //        if (staffDoc == null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffDoc });
        //    }
        //    catch (SecureException ex)
        //    {
        //        errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
        //    }

        //}

        [HttpGet] [ClaimsAuthorization]  
        [Route("staff/approvals/temp/{staffId}")]
        public async Task<HttpResponseMessage> GetTempStaffDetailsById(int staffId)
        {
            try
            {
                var staffInfo = await repo.GetTempStaffDetail(staffId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("staff/approvals/{staffCode}")]
        public HttpResponseMessage GetStaffDetailsById(string staffCode)
        {
            try
            {
                var staffInfo = repo.GetStaffDetail(staffCode, token.GetCompanyId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Request.RequestUri.Host, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("staff/approvals")]
        public HttpResponseMessage GetStaffDetails()
        {
            try
            {
                var staffInfo = repo.GetStaffDetails(token.GetCompanyId);

                if (staffInfo != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo.ToList() });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("staff/names")]
        public HttpResponseMessage GetStaff()
        {
            try
            {
                var staffInfo = repo.GetStaffNames(token.GetCompanyId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-relationship-Manager/{staffId}")]
        public async Task<HttpResponseMessage> GetStaffRelationshipManagerByStaffId(int staffId)
        {
            try
            {
                var staffInfo = await repo.GetStaffRelationshipManagerByStaffId(staffId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-business-Manager/{staffId}")]
        public HttpResponseMessage GetStaffBusinessManagerByStaffId(int staffId)
        {
            try
            {
                var staffInfo = repo.GetStaffBusinessManagerByStaffId(staffId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("staff/unit/{departmentUnitId}")]
        public HttpResponseMessage GetStaff(short departmentUnitId)
        {
            try
            {
                var staffInfo = repo.GetStaffByUnitId(token.GetCompanyId, departmentUnitId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo.ToList() });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("approval-status")]
        public async Task<HttpResponseMessage> GetApprovalStatus()
        {
            try
            {
                var staffInfo = await repo.GetApprovalStatus();

                if (staffInfo != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo.ToList() });

                }
                return Request.CreateResponse(HttpStatusCode.OK,
                           new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("staff/{staffId}")]
        public async Task<HttpResponseMessage> GetStaffInfoById(int staffId)
        {
            try
            {
                var staffInfo = await repo.GetStaffById(staffId);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { error = true, message = TranslateHelper.get(ex.Message) });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("staff")]
        public HttpResponseMessage AddTempStaff([FromBody] StaffInfoViewModel model)
        {
            try
            {
                if (repo.IsStaffCodeAlreadyExist(model.StaffCode))
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("A staff with") + " " + model.StaffCode + " " + TranslateHelper.get("already exist") });
                }
                if ( repo.IsTempStaffExist(model.StaffCode))
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"{TranslateHelper.get("A staff with")} {model.StaffCode} {TranslateHelper.get("already exist waiting for approval")}" });
                }

                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = Request.RequestUri.Host;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var staff =  repo.AddTempStaff(model);

                if (staff)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = staff, message = TranslateHelper.get("Staff has been created successfully, now waiting for approval") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("staff not created") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("staff/{staffid}")]
        public HttpResponseMessage UpdateStaffInfo(int staffid, [FromBody] StaffInfoViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.companyId = (short)token.GetCompanyId;
                model.userIPAddress = Request.RequestUri.Host;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;

                var staff = repo.UpdateStaff(staffid, model);

                if (staff)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = staff, message = TranslateHelper.get("Staff has been updated successfully, now waiting for approval") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("staff not created") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("staff/{staffId}")]
        public async Task<HttpResponseMessage> DeletestaffInfo(int staffId)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    createdBy = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = Request.RequestUri.Host
                };
                var staff = await repo.LogDeleteRequestStaff(staffId, user);
                if (staff)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = staff, message = TranslateHelper.get("Staff delete has been successfully submited for approval") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("Deleting staff record failed") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("staffbybranch/{branchId}")]
        public HttpResponseMessage GetstaffInfoByBranchId(int branchId)
        {
            try
            {
                var staffInfo = repo.GetAllStaff().SingleOrDefault(c => c.BranchId == branchId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("staff/approval")]
        public async Task<HttpResponseMessage> GoForApprovalAsync([FromBody]ApprovalViewModel entity)
        {
            try
            {
                entity.BranchId = token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.staffId = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userIPAddress = Request.RequestUri.Host;

                var data = await repo.GoForApproval(entity);

                if (data ==  1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = TranslateHelper.get("Staff record has been approved successfully") });
                }
                else if (data == 2)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = TranslateHelper.get("Staff details has been disapproved") });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Operation successful, request has been routed to the next approving office") });
                }
                
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
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("staff-delete/approval")]
        public async Task<HttpResponseMessage> GoForStaffDeleteApproval([FromBody]ApprovalViewModel entity)
        {
            try
            {
                entity.BranchId = token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.staffId = token.GetStaffId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userIPAddress = Request.RequestUri.Host;

                var data = await repo.GoForStaffDeleteApproval(entity);

                if (data == 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = TranslateHelper.get("Deleting of staff has been approved successfully and staff has been deleted from the system") });
                }
                else if (data == 2)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = TranslateHelper.get("Staff delete has been disapproved") });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Operation successful, request has been routed to the next approving office") });
                }

            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = ce.Message });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message =ex.Message });
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }
        }

        [HttpPost] [ClaimsAuthorization]
        [Route("staff/bulk-approval")]
        public async Task<HttpResponseMessage> GoForBulkApproval([FromBody]List<ApprovalViewModel> entity)
        {
            try
            {
                var info = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = Request.RequestUri.Host
                };
               
                var data = await repo.GoForBulkApproval(entity, info);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = TranslateHelper.get("Staff record has been approved successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Operation successful, request has been routed to the next approving office") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("staff/search/")]
        public HttpResponseMessage SearchStaff(string queryString = "")
        {
            if (queryString == null) queryString = string.Empty;
            var data = repo.SearchStaff(queryString, token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("UnprocessedBulkPrepaymentReversal")]
        public HttpResponseMessage GetAllUnprocessedBulkPrepayment()
        {
            try
            {
                var staffInfo = repo.GetAllUnprocessedBulkPrepayment();

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = staffInfo, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-unprocessed-bulk-prepayment-batch")]
        public HttpResponseMessage GetAllUnprocessedBulkPrepaymentBatch()
        {
            try
            {
                var batch = repo.GetAllUnprocessedBulkPrepaymentBatch(token.GetStaffId);

                if (batch == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = batch, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = batch });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("submit-prepayment-batch-for-approval")]
        public HttpResponseMessage SubmitBatchPrepaymentForApproval([FromBody] ApprovalViewModel model)
        {
            try
            {
                model.BranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var res = repo.SubmitPrepaymentBatchForApproval(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = res });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error saving this record. Error")} - {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-bulk-prepayments-awaiting-approval-batch")]
        public async Task<HttpResponseMessage> GetBulkPrepaymentsAwaitingApprovalBatch()
        {
            try
            {
                var data = await repo.GetBulkPrepaymentsAwaitingApprovalBatch(token.GetStaffId, token.GetCompanyId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("submit-prepayment-batch-for-workflow-approval")]
        public async Task<HttpResponseMessage> SubmitPrepaymentBatchForWorkflowApproval([FromBody] ApprovalViewModel model)
        {
            try
            {
                model.BranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var res = await repo.SubmitPrepaymentBatchForWorkflowApproval(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = res });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error saving this record. Error")} - {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-processing-bulk-prepayment-by-batch/batchId/{batchId}")]
        public HttpResponseMessage GetProcessingBulkPrepaymentByBatch(int batchId)
        {
            try
            {
                var batch = repo.GetProcessingBulkPrepaymentByBatchId(batchId);

                if (batch == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = batch, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = batch });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("staff/approver-search/levelId/{levelId}")]
        public async Task<HttpResponseMessage> SearchApprovers(int levelId, string queryString = "")
        {
            if (queryString == null) queryString = string.Empty;
            var data = await repo.SearchApprovers(levelId, queryString, token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("staff/{departmentId}/search/")]
        public HttpResponseMessage SearchStaffbyDepartmentId(string queryString, int departmentId)
        {
            try
            {
                var data = repo.SearchStaffbyDepartmentId(queryString, token.GetCompanyId, departmentId);
                return Request.CreateResponse(HttpStatusCode.OK,
                     new { success = true, result = data.ToList() });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new
                    {
                        success = false,
                        message = TranslateHelper.get(ex.Message)
                    });
            }

        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("staff/signature/all")]
        public HttpResponseMessage GetAllStaffSignatures()
        {
            try
            {
                var data = repo.GetAllStaffSignatures(token.GetCompanyId);

                //var staffInfo = repo.GetAllStaff();
                //foreach (var item in data)
                //{
                //    var staffName = staffInfo.FirstOrDefault(x => x.StaffCode == item.staffCode);
                //    if (staffName!=null)
                //    {
                //    item.StaffName = staffName.FirstName + " " + staffName.MiddleName + " " + staffName.LastName;
                //    }
                //}

                if (data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("staff/upload-signature")]
        public async Task<HttpResponseMessage> UploadStaffSignature()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, TranslateHelper.get("Unsupported media type"));
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
                    return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("Upload Type is invalid"));
                }

                var entity = new StaffDocumentViewModel
                {
                    staffCode = provider.FormData["staffCode"],
                    documentTitle = provider.FormData["documentTitle"],
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                };

                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("No file uploaded"));
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                var data = repo.AddStaffSignature(entity, buffer);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Staff signature uploaded successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Error uploading staff signature") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                var innerException = ex.InnerException;
                string innerMessage = "";
                if (innerException != null)
                    innerMessage = innerException.Message;

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")}" });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("staff/signature/{documentId}")]
        public async Task<HttpResponseMessage> UpdateStaffSignature([FromBody] StaffDocumentViewModel entity, int documentId)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = await repo.UpdateStaffSignature(entity, documentId);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = TranslateHelper.get("The record has been updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("staff/signature")]
        public HttpResponseMessage GetStaffSignatureByStaffCode(string staffCode)
        {
            try
            {
                var data = repo.GetStaffSignatureByStaffCode(staffCode, token.GetCompanyId);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("staff/multiple-staff-data")]
        public async Task<HttpResponseMessage> UploadStaffData()
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


                var isFinal = Convert.ToBoolean(provider.FormData["isFinal"]);

               // byte[] pass

                var pass = "" ; // Convert.(provider.FormData["loginStaffPassCode"]);
                string password = pass; // Encoding.UTF8.GetString(pass);

                var entity = new StaffDocumentViewModel
                {
                    staffCode = provider.FormData["staffCode"],
                    documentTitle = provider.FormData["documentTitle"],
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                    loginStaffPassword= password,
                    loginStaffCode = token.GetUsername
                 };

                if (entity.loginStaffPassword == string.Empty) entity.loginStaffPassword = "A031E392FA3FF64D1A5F18F047A23BAA7D41FACB25F6CAD49C7B9FCE945C83E6B210FDE78CCE5CEF63342748D8355DE11222FC52DCE218B090623CCEBF970C88";

                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("No file uploaded"));
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.branchId = (short)token.GetBranchId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                var data = repo.UploadStaffData(entity, buffer);

                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Staff data was successfully uploaded") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Error uploading staff data") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")}. " + ex.Message });
            }
        }


         [HttpPost] [ClaimsAuthorization]
        [Route("staff/update-supervisor")]
        public async Task<HttpResponseMessage> UpdateSupervisor([FromBody]SupervisorViewModel entity)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userIPAddress = Request.RequestUri.Host;
                bool success = await repo.UpdateSupervisor(entity);
                string message = success == true ? TranslateHelper.get("Supervisor Updated Successfully") : TranslateHelper.get("Supervisor Update Failed");
                return Request.CreateResponse(HttpStatusCode.OK, new { success = success, message = message });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("staff-reporting/{staffCode}")]
        public async Task<HttpResponseMessage> GetStaffSupervisorReporting(string staffCode)
        {
            try
            {
                var staffInfo = await repo.StaffReportingLine(token.GetStaffId, staffCode, token.GetCompanyId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo.ToList() });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("staff-reporting-search/{staffCode}")]
        public async Task<HttpResponseMessage> GetStaffSupervisorReportingSearch(string staffCode)
        {
            try
            {
                var staffInfo = await repo.StaffReportingLine(token.GetStaffId, staffCode, token.GetCompanyId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo.ToList() });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("supervisor/{staffCode}")]
        public HttpResponseMessage Supervisor(string staffCode)
        {
            try
            {
                var staffInfo =repo.StaffReportingTo(token.GetStaffId, staffCode, token.GetCompanyId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("supervisor/{staffCode}/{slashedNumber}")]
        public HttpResponseMessage SupervisorSlashed(string staffCode, string slashedNumber)
        {
            try
            {
                staffCode = staffCode + "/" + slashedNumber;
                var staffInfo = repo.StaffReportingTo(token.GetStaffId, staffCode, token.GetCompanyId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("staff/information/{staffCode}")]
        public async Task<HttpResponseMessage> StaffInformation(string staffCode)
        {
            try
            {
                var staffInfo = await repo.StaffInformation(token.GetStaffId, staffCode, token.GetCompanyId);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("staff/mis/{staffCode}")]
        public HttpResponseMessage StaffMIS(string staffCode)
        {
            try
            {
                var staffInfo = repo.StaffMIS(token.GetStaffId, staffCode);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No record found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = ex.Message });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("staff/mis/{staffCode}/{slashedNumber}")]
        public HttpResponseMessage StaffMISSlashed(string staffCode, string slashedNumber)
        {
            try
            {
                staffCode = staffCode + "/" + slashedNumber;
                var staffInfo = repo.StaffMIS(token.GetStaffId, staffCode);

                if (staffInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No record found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = staffInfo });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = ex.Message });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("staff/search/{searchQuery}")]
        public async Task<HttpResponseMessage> SearchForBranch(string searchQuery)
        {
            try
            {
                var data = await repo.GetSearchedStaff(searchQuery);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                      new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("staff/roles")]
        public HttpResponseMessage GetStaffRoles()
        {
            try
            {
                var data = repo.GetStaffRoles(token.GetCompanyId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                      new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
    }

}