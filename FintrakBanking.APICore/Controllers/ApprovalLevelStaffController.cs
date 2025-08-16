using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.Setups.Approval;
using FintrakBanking.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using FintrakBanking.ViewModels.Reports;
using FintrakBanking.ViewModels.WorkFlow;
using System.Collections.Generic;
using FintrakBanking.Common;
using FintrakBanking.Interfaces.ErrorLogger;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/setups")]
    public class ApprovalLevelStaffController : ApiControllerBase
    {
        private IApprovalLevelStaffRepository repo;
        private IErrorLogRepository _errorLog;
        private TokenDecryptionHelper token = new TokenDecryptionHelper();

        public ApprovalLevelStaffController(IApprovalLevelStaffRepository _repo, IErrorLogRepository errorLog)
        {
            this.repo = _repo;
        }

        #region Approval Level Staff

         [HttpPost] [ClaimsAuthorization]
        [Route("approval-level-staff")]
        public async Task<HttpResponseMessage> AddApprovalLevelStaff([FromBody] ApprovalLevelStaffViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await repo.AddApprovalLevelStaff(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("go-for-level-staff-approval")]
        public HttpResponseMessage GoForWorkflowGroupApproval([FromBody]ApprovalLevelStaffViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                var data = repo.GoForApproval(model);
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = true, result = data, count = 1 });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = $"An error has accoured {ex.Message}" });
            }


        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("approval-level-staff/operations/{operationMappingId}")]
        public HttpResponseMessage GetAllApprovalLevelStaff(int operationMappingId)
        {
            try
            {
                var data = repo.GetAllApprovalLevelStaffByOperationId(operationMappingId, token.GetCompanyId);
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
        [HttpGet]
        [ClaimsAuthorization]
        [Route("temp-approval-level-staff")]
        public async Task<HttpResponseMessage> GetTempAllApprovalLevelStaff()
        {
            try
            {
                var data = await repo.GetTempApprovalLevelStaff( token.GetStaffId);
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
        [HttpGet] [ClaimsAuthorization]  
        [Route("approval-level-staff/staff-level/{id}")]
        public HttpResponseMessage GetApprovalLevelStaffById(int id)
        {
            try
            {
                var data = repo.GetApprovalLevelStaffById(id, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("approval-level-staff/{id}")]
        public async Task<HttpResponseMessage> UpdateApprovalLevelStaff([FromBody] ApprovalLevelStaffViewModel model, int id)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await repo.UpdateApprovalLevelStaff(id, model);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {e.Message}" });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("approval-level-staff/{StaffLevelId}")]
        public async Task<HttpResponseMessage> DeleteApprovalLevelStaffAsync(int StaffLevelId)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
            };

                var saved = await repo.DeleteApprovalLevelStaff(StaffLevelId, user);

                if (saved)
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = StaffLevelId, message = TranslateHelper.get("record has been deleted successfully") });

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = StaffLevelId, message = TranslateHelper.get("Record could not be saved") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException, stack = ex.StackTrace });
            }
        }

        #endregion Approval Level Staff

        #region Workflow Tracker

        [HttpGet] [ClaimsAuthorization]  
        [Route("work-flow-tracker/operation/{operationId}/target/{targetId}")]
        public HttpResponseMessage GetApprovalTrailByOperationIdAndTargetId(int operationId, int targetId)
        {
               var data =  repo.GetApprovalTrailByOperationIdAndTargetId(operationId, targetId, token.GetCompanyId);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, count = data.Count() });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("work-flow-tracker/operation-site-report/{targetId}")]
        public HttpResponseMessage GetApprovalTrailBySiteTargetId(int targetId)
        {
            var data = repo.GetApprovalTrailBySiteTargetId(targetId, token.GetCompanyId);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = TranslateHelper.get("No Record Found")});
            }else

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }


        [HttpGet] [ClaimsAuthorization]  
        [Route("work-flow-tracker/approval-trail/all")]
        public async Task<HttpResponseMessage> GetAllRecordsOnApprovalTrail([FromUri] int page, [FromUri] int itemsPerPage)
        {
                var item = repo.GetAllRecordsOnApprovalTrail(token.GetCompanyId);

                var data = await item.Skip(page).Take(itemsPerPage)
                    .ToListAsync();

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, count = item.Count() });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = item.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("work-flow-tracker/approval-status")]
        public async Task<HttpResponseMessage> GetAllApprovalStatus()
        {
                var data = await repo.GetAllApprovalStatus();

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data  });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("work-flow-tracker/approval-operation")]
        public async Task<HttpResponseMessage> GetAllApprovalOperations()
        {
            
                var data = await repo.GetAllApprovalOperations();

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }
        #endregion Workflow Tracker

        #region Approval Monitoring 
        [HttpPost]
        [ClaimsAuthorization]
        [Route("work-flow-tracker/approval-monitoring")]
        public HttpResponseMessage GetApprovalMointoring(DateRange dateRange)
        {
            var token = new TokenDecryptionHelper();

            dateRange.companyId = token.GetCompanyId;
            var data = repo.GetApprovalMointoring(dateRange);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("work-flow-tracker/turn-around-monitoring")]
        public HttpResponseMessage GetTurnAroundMointoring(DateRange dateRange)
        {
            var token = new TokenDecryptionHelper();

            dateRange.companyId = token.GetCompanyId;
            var data = repo.GetTurnAroundMointoring(dateRange);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });

        }

        [HttpPost]
        [Route("work-flow-tracker/approval-booking-monitoring")]
        public HttpResponseMessage GetBookingMointoring(DateRange dateRange)
        {
            var token = new TokenDecryptionHelper();

            dateRange.companyId = token.GetCompanyId;
            var data = repo.GetBookingMointoring(dateRange);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost]
        [Route("work-flow-tracker/approval-booking-tat-monitoring")]
        public HttpResponseMessage GetBookingTATMointoring(DateRange dateRange)
        {
            var token = new TokenDecryptionHelper();

            dateRange.companyId = token.GetCompanyId;
            var data = repo.GetBookingTATMointoring(dateRange);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost]
        [Route("work-flow-tracker/approval-review-monitoring")]
        public async Task<HttpResponseMessage> GetContractReviewMointoring(DateRange dateRange)
        {
            var token = new TokenDecryptionHelper();

            dateRange.companyId = token.GetCompanyId;
            var data = await repo.GetContractReviewMointoring(dateRange);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("work-flow-tracker/target/{targetId}")]
        public async Task<HttpResponseMessage> GetApprovalTrailByTargetId(int targetId)
        {
            
                var data = await repo.GetApprovalTrailByTargetId(targetId, token.GetCompanyId);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, count = data.Count() });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("work-flow-tracker/booking/target/{targetId}")]
        public async Task<HttpResponseMessage> GetBookingApprovalTrailByTargetId(int targetId)
        {
                var data = await repo.GetBookingApprovalTrailByTargetId(targetId, token.GetCompanyId);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, count = data.Count() });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("approval-monitoring/export")]
        public HttpResponseMessage ExportScheduleToExcel([FromBody] DateRange model)
        {
            try
            {
                model.companyId = token.GetCompanyId;
                var fileBytes = repo.GenerateApprovalMonitoringReport(model);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = fileBytes });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { data = "no-record", success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("approval-comments/export/{requireAll}")]
        public HttpResponseMessage ExportApprovalComments([FromBody] List<ApprovalTrailViewModel> model, bool requireAll)
        {
            try
            {
                //model.companyId = token.GetCompanyId;
                var fileBytes = repo.ExportApprovalComments(model, requireAll);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = fileBytes });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { data = "no-record", success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }

        }
        #endregion Approval Monitoring

        [HttpPost]
        [ClaimsAuthorization]
        [Route("tat-setup")]
        public async Task<HttpResponseMessage> AddTATSetup([FromBody] TurnAroundTimeViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data = await repo.AddTATSetup(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("tat-setup")]
        public HttpResponseMessage GetTATSetup()
        {
            try
            {
                var data = repo.GetTATSetup();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("tat-setup/{tatId}")]
        public async Task<HttpResponseMessage> UpdateTATSetup(short tatId, [FromBody] TurnAroundTimeViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data = await repo.UpdateTATSetup(tatId, entity);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("delete-tat/{tatId}")]
        public async Task<HttpResponseMessage> DeleteTATSetup(int tatId)
        {
            try
            {
                var user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = Request.RequestUri.Host,
                    createdBy = token.GetStaffId
                };

                await repo.DeleteTATSetup(tatId, user);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = tatId, message = TranslateHelper.get("Record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
    }
}
