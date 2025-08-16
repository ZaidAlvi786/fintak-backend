using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using System.Threading.Tasks;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/credit")]
    public class CallMemoController : ApiControllerBase
    {
        private readonly ICallMemoRepository repo;
        private readonly TokenDecryptionHelper token = new TokenDecryptionHelper();
        public CallMemoController(ICallMemoRepository _repo)
        {
            repo = _repo;
        }
      [HttpGet] [ClaimsAuthorization]  
        [Route("loan-search/")]
        public async Task<HttpResponseMessage> SearchForCallMemoLoan(string searchQuery)
        {
            try
            {
                var data = await repo.SearchForCallMemoLoan(token.GetStaffId, searchQuery);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, result = data });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $" {ce.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                      new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            

        }
        #region "Call Limit"
      [HttpGet] [ClaimsAuthorization]  
        [Route("call-limit-type")]
        public async Task<HttpResponseMessage> GetCallLimitType()
        {
            try
            {

                var response = await repo.GetCallLimitType();
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
      [HttpGet] [ClaimsAuthorization]  
        [Route("call-limit")]
        public async Task<HttpResponseMessage> GetAllCallLimit()
        {
            try
            {

                var response = await repo.GetAllCallLimit(token.GetCompanyId);
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
      [HttpGet] [ClaimsAuthorization]  
        [Route("call-limit-type/{limitId}")]
        public async Task<HttpResponseMessage> GetCallLimitByTypeId(int limitId)
        {
            try
            {

                var response = await repo.GetCallLimitByTypeId(limitId);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("call-limit")]
        public async Task<HttpResponseMessage> AddCallLimit([FromBody] CallLimitViewModel model)
        {
            try
            {

                model.userBranchId = (short)token.GetBranchId;
                //model.userIPAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                if (repo.isLimitExist(model))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Call Limit setup already exist for the selected role") });
                }
                var response = await repo.AddCallLimit(model);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("call-limit/{limitId}")]
        public async Task<HttpResponseMessage> UpdateCallLimit(int LimitId, [FromBody] CallLimitViewModel model)
        {
            try
            {
                ;
                model.userBranchId = (short)token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var response = await repo.UpdateCallLimit(LimitId, model);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been updated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {e.Message}" });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("call-limit/{limitId}")]
        public async Task<HttpResponseMessage> DeleteCallLimit(int LimitId)
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
                await repo.DeleteCallLimit(LimitId, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = LimitId, message = TranslateHelper.get("Record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        #endregion
        #region "Call Memo"

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-customer-call-memo/{customerId}/customerId")]
        public async Task<HttpResponseMessage> GetCustomerCallMemo(int customerId)
        {
            try
            {
                var response = await repo.GetCustomerCallMemo(token.GetStaffId, customerId);
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-customer-approved-call-memo/{customerId}/customerId")]
        public async Task<HttpResponseMessage> GetCustomerApprovedCallMemo(int customerId)
        {
            try
            {
                var response = await repo.GetCustomerApprovedCallMemo(token.GetStaffId, customerId);
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-call-memo-waiting-for-approval")]
        public async Task<HttpResponseMessage> GetCallMemoWaitingForApproval()
        {
            try
            {
                var response = await repo.GetCallMemoWaitingForApproval(token.GetStaffId);
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("call-getMemo/{callMemoId}/callMemoId")]
        public async Task<HttpResponseMessage> GetCallMemoById(int callMemoId)
        {
            try
            {
                var response = await repo.GetCallMemoById(callMemoId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("search-call-memo")]
        public async Task<HttpResponseMessage> SearchCallMemo([FromBody] CallMemoViewModel model)
        {
            try
            {
                var response = await repo.SearchCallMemo(token.GetStaffId, model);
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("call-getMemo")]
        public async Task<HttpResponseMessage> GetAllCallMemo()
        {
            try
            {

                IEnumerable<CallMemoViewModel> response = await repo.GetAllCallMemo(token.GetStaffId);
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
         [HttpPost] [ClaimsAuthorization]
        [Route("call-memo")]
        public async Task<HttpResponseMessage> AddCallMemo([FromBody] CallMemoViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.staffId = token.GetStaffId;
                model.companyId = token.GetCompanyId;
        
                var response = await repo.AddCallMemo(model);
                if (response != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("call-memo/{memoId}")]
        public async Task<HttpResponseMessage> UpdateCallMemo(int memoId, [FromBody] CallMemoViewModel model)
        {
            try
            {
                //;
                model.userBranchId = (short)token.GetBranchId;
                //model.userIPAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var response = await repo.UpdateCallMemo(memoId, model);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been updated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {e.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("go-for-approval")]
        public async Task<HttpResponseMessage> GoForApproval([FromBody] CallMemoViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var res = await repo.GoForCallMemoApproval(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = res });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error saving this record. Error")} - {ex.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("submit-approval")]
        public async Task<HttpResponseMessage> SubmitApproval([FromBody] CallMemoViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var res = await repo.SubmitApproval(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = res });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error saving this record. Error")} - {ex.Message}" });
            }
        }
        #endregion
    }
}
