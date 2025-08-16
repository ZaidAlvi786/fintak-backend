using System.Linq;
using System.Threading.Tasks;
using FintrakBanking.Common.CustomException;
using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.ViewModels;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.ViewModels.Credit;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/credit")]
    public class ConditionPrecedentController : ApiControllerBase
    {
        TokenDecryptionHelper token = new TokenDecryptionHelper();
        private IConditionPrecedentRepository repo;

        public ConditionPrecedentController(IConditionPrecedentRepository repo)
        {
            this.repo = repo;
        }

        #region DEFAULT conditions

        [HttpGet]
        [ClaimsAuthorization]
        [Route("condition-precedent-template/application-detail/{detailId}")]
        public async  Task<HttpResponseMessage> GetConditionPrecedentDefaultByApplicationId(int detailId)
        {
            try
            {
                List<ConditionPrecedentViewModel> data = await repo.GetConditionPrecedentDefaultByDetailId(detailId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lms-condition-precedent-template/application-detail/{detailId}")]
        public async Task<HttpResponseMessage> GetConditionPrecedentDefaultByApplicationIdLms(int detailId)
        {
            try
            {
                List<ConditionPrecedentViewModel> data = await repo.GetConditionPrecedentDefaultByDetailIdLms(detailId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("condition-precedent-template")]
        public HttpResponseMessage GetConditionPrecedentTemplate()
        {
            try
            {
                var data = repo.GetConditionPrecedentTemplate();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("condition-precedent-template")]
        public async Task <HttpResponseMessage> AddConditionPrecedentTemplate([FromBody] ConditionPrecedentViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = await repo.AddConditionPrecedentTemplate(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("condition-precedent-template/{conditionId}")]
        public async Task<HttpResponseMessage> UpdateConditionPrecedentTemplate([FromBody] ConditionPrecedentViewModel entity, int conditionId)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = await repo.UpdateConditionPrecedentTemplate(entity, conditionId);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = TranslateHelper.get("The record has been updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}", error = ex.InnerException });
            }
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("condition-precedent-template/delete/{id}")]
        public async Task <HttpResponseMessage> DeleteConditionPrecedentTemplate(int id)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                };
                bool data = await repo.DeleteConditionPrecedentTemplate(user, id);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lms-condition-precedent-template/application-detail/{detailId}/operation/{operationId}")]
        public async Task<HttpResponseMessage> GetConditionPrecedentDefaultByApplicationIdAndOperationLms(int detailId, int? operationId)
        {
            try
            {
                List<ConditionPrecedentViewModel> data = await repo.GetConditionPrecedentDefaultByApplicationIdAndOperationLms(detailId, operationId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("condition-precedent-template/application-detail/{detailId}/operation/{operationId}")]
        public async Task<HttpResponseMessage> GetConditionPrecedentDefaultByApplicationIdAndOperation(int detailId, int? operationId)
        {
            try
            {

                List<ConditionPrecedentViewModel> data = await repo.GetConditionPrecedentDefaultByDetailId(detailId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        #endregion DEFAULT conditions

        #region LOS conditions

        [HttpGet]
        [ClaimsAuthorization]
        [Route("condition-precedent/application-detail/{detailId}")]     
        public HttpResponseMessage GetConditionPrecedentByApplicationId(int detailid)
        {
            try
            {
                var data = repo.GetConditionPrecedentByDetailId(detailid);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("condition-precedent")]
        public async Task<HttpResponseMessage> AddConditionPrecedent([FromBody] ConditionPrecedentViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = await repo.AddConditionPrecedent(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("condition-precedent/selected")]
        public HttpResponseMessage AddSelectedConditionPrecedent([FromBody] SelectedIdsViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                List<ConditionPrecedentViewModel> data = repo.AddSelectedConditionPrecedent(entity);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("condition-precedent-edit/{id}")]
        public HttpResponseMessage EditLoanCditionPrecedent([FromBody] ConditionPrecedentViewModel entity, int id)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = repo.EditLoanConditionPrecedent(id, entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been modified successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("condition-precedent-remove/{id}")]
        public async Task<HttpResponseMessage> RemoveLoanConditionPrecedent(int id)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                };
                var data = await repo.RemoveLoanConditionPrecedent(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been removed successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        #endregion LOS conditions

        #region LMS conditions

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lms-condition-precedent/application-detail/{detailId}")]
        public HttpResponseMessage GetConditionPrecedentByApplicationIdLms(int detailid)
        {
            try
            {
                var data = repo.GetConditionPrecedentByDetailIdLms(detailid);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lms-condition-precedent")]
        public async Task <HttpResponseMessage> AddConditionPrecedentLms([FromBody] ConditionPrecedentViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = await repo.AddConditionPrecedentLms(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lms-condition-precedent/selected")]
        public async Task<HttpResponseMessage> AddSelectedConditionPrecedentLms([FromBody] SelectedIdsViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                List<ConditionPrecedentViewModel> data = await repo.AddSelectedConditionPrecedentLms(entity);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("lms-condition-precedent-edit/{id}")]
        public async Task<HttpResponseMessage> EditLoanCditionPrecedentLms([FromBody] ConditionPrecedentViewModel entity, int id)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = await repo.EditLoanConditionPrecedentLms(id, entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been modified successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("lms-condition-precedent-remove/{id}")]
        public HttpResponseMessage RemoveLoanConditionPrecedentLms(int id)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                };
                var data = repo.RemoveLoanConditionPrecedentLms(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been removed successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        #endregion LMS conditions

        #region Compliance Timeline template

        [HttpGet]
        [ClaimsAuthorization]
        [Route("compliance-timeline-template")]
        public HttpResponseMessage GetComplianceTimelineTemplate()
        {
            try
            {
                var data = repo.GetComplianceTimelineTemplate();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("compliance-timeline-template")]
        public HttpResponseMessage AddComplianceTimelineTemplate([FromBody] ComplianceTimelineViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = repo.AddComplianceTimelineTemplate(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("compliance-timeline-template/{conditionId}")]
        public HttpResponseMessage UpdateComplianceTimelineTemplate([FromBody] ComplianceTimelineViewModel entity, int conditionId)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = repo.UpdateComplianceTimelineTemplate(entity, conditionId);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = TranslateHelper.get("The record has been updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}", error = ex.InnerException });
            }
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("compliance-timeline/remove/{id}")]
        public async Task<HttpResponseMessage> RemoveComplianceTimelineTemplate(int id)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                };
                bool data = await repo.RemoveComplianceTimelineTemplate(user, id);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        #endregion Compliance Timeline template

        #region Additional Comments

        [HttpGet]
        [Route("additional-comment/application/{applicationId}/caller/{callerId}")]
        public async  Task<HttpResponseMessage> GetAdditionalComment(int applicationId, int callerId)
        {
            try
            {
                List<AdditionalCommentViewModel> response = await repo.GetAdditionalComment(applicationId, callerId, token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [Route("additional-comment")]
        public async Task<HttpResponseMessage> AddAdditionalComment([FromBody] AdditionalCommentViewModel entity)
        {
            try
            {
                var isRemoteCall = HttpContext.Current.Request.Headers["isRemote"];
                if (!string.IsNullOrEmpty(isRemoteCall)) { entity.isRemoteCall = true; }
                else { entity.isRemoteCall = false; };
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                bool response = await repo.AddAdditionalComment(entity);
                if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpPut]
        [Route("additional-comment/{id}")]
        public async Task<HttpResponseMessage> EditAdditionalComment([FromBody] AdditionalCommentViewModel entity, int id)
        {
            try
            {
                var isRemoteCall = HttpContext.Current.Request.Headers["isRemote"];
                if (!string.IsNullOrEmpty(isRemoteCall)) { entity.isRemoteCall = true; }
                else { entity.isRemoteCall = false; };
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
   

                bool response = await repo.EditAdditionalComment(id, entity);
                if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been modified successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpDelete]
        [Route("additional-comment/{id}")]
        public async Task<HttpResponseMessage> RemoveAdditionalComment(int id)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    createdBy = token.GetStaffId,
                    userIPAddress= Request.RequestUri.Host,
                };
                var isRemoteCall = HttpContext.Current.Request.Headers["isRemote"];
                if (!string.IsNullOrEmpty(isRemoteCall)) { user.isRemoteCall = true; }
                else { user.isRemoteCall = false; };
                bool response = await repo.RemoveAdditionalComment(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been removed successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        #endregion Additional Comments




    }
}
