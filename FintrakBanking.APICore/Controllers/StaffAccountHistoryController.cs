using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.Common;
using System.Web;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common;
using System.Threading.Tasks;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/loanoperation")]
    public class StaffAccountHistoryController : ApiControllerBase
    {
        
        IStaffAccountHistoryRepository accountHistory;
        TokenDecryptionHelper token = new TokenDecryptionHelper();
        public StaffAccountHistoryController(IStaffAccountHistoryRepository accountHistory)
        {
            this.accountHistory = accountHistory;
        }



         [HttpPost] [ClaimsAuthorization]
        [Route("approve-reasign-account")]
        public async Task<HttpResponseMessage> ApproveStaffAccountHistory(ReasignedAccountApprovalViewModel entity)
        {

            try
            {
              
                entity.userIPAddress = CommonHelpers.GetUserIP();
                entity.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;
                entity.staffId = token.GetStaffId;

                var data = await accountHistory.ApproveStaffAccountHistory(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("reasign-account")]
        public HttpResponseMessage AddStaffAccountHistory(StaffAccountHistoryViewModel entity)
        {
            
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = CommonHelpers.GetUserIP();
                entity.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;
                entity.staffId = token.GetStaffId;

                var data = accountHistory.AddStaffAccountHistory(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("get-reasigned-account-awaiting-approval")]
        public async Task<HttpResponseMessage> GetStaffAccountHistory(StaffAccountHistoryViewModel entity)
        {

            try
            {
                var response = await accountHistory.GetStaffAccountHistory(this.token.GetStaffId);
                if (response == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }



        

      [HttpGet] [ClaimsAuthorization]  
        [Route("get-all-reasigned-account")]
        public async Task<HttpResponseMessage>   GetAllStaffAccountHistory()
        {
            try
            {
                var response = await accountHistory.GetAllStaffAccountHistory(); ;
                if (response == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("get-all-reasigned-account/loan/{loanId}/productType/{productTypeId}")]
        public async Task<HttpResponseMessage> GetSelectedLoanDetails(int loanId, int productTypeId)
        {              
            try
            {
                var response = await accountHistory.GetSelectedLoanDetails(token.GetCompanyId, loanId, productTypeId);  
                if (response == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }



         [HttpPost] [ClaimsAuthorization]
        [Route("selected-reasigned-account")]
        public async Task<HttpResponseMessage> GetSelectedApprovalLoanDetails(ReasignedAccountApprovalViewModel entity)
        {
            try
            {
                entity.companyId = token.GetCompanyId;
                var response = await accountHistory.GetSelectedApprovalLoanDetails(entity);
                if (response == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
    }
}
