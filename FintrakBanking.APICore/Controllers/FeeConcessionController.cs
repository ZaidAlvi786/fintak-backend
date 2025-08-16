using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Common.Enum;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common;
using System.Threading.Tasks;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/fees")]
    public class FeeConcessionController : ApiControllerBase
    {
         private IFeeConcessionRepository repo;
       

        TokenDecryptionHelper token = new TokenDecryptionHelper();

        public FeeConcessionController(IFeeConcessionRepository _repo)
        {
            repo = _repo;
        }
      [HttpGet] [ClaimsAuthorization]  
        [Route("fee-concession-type")]
        public async Task<HttpResponseMessage> GetFeeConcessionType()
        {
            try
            {
                var data = await repo.GetConcessionFeeType();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("fee-concession-charges")]
        public async Task<HttpResponseMessage> GetFeeConcessionCharges(int loanApplicationDetailId)
        {
            try
            {
                var data = await repo.GetAllLoanFeeChargeByDetailId(loanApplicationDetailId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("fee-concession")]
        public async Task<HttpResponseMessage> GetFeeConcessionByLoanApplicationDetailId(int loanDetailId)
        {
            try
            {
                var data = await repo.GetAllConcessionFee(loanDetailId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }
      [HttpGet] [ClaimsAuthorization]  
        [Route("fee-concession-awaiting-approval")]
        public async Task<HttpResponseMessage> GetAllConcessionFeeAwaitingApproval()
        {
            try
            {
                var data = await repo.GetAllConcessionFeeAwaitingApproval(token.GetStaffId, token.GetCompanyId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }
         [HttpPost] [ClaimsAuthorization]
        [Route("fee-concession")]
        public async Task<HttpResponseMessage> AddUpdateFeeConcession([FromBody] FeeConcessionViewModel entity)
        {
            try
            {
                string createUpdate = "";
                if (entity.concessionId != 0 || entity.concessionId < 0)
                {
                    createUpdate = "updated";
                    if(await repo.ValidateApprovedFeeConcession(entity.concessionId))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                                                 new { success = false, message = TranslateHelper.get("Approved Record cannot be modified") });
                    }
                }
                else
                {
                    createUpdate = "created";
                    if (await repo.ValidateFeeConcession(entity.loanApplicationDetailId, entity.loanChargeFeeId))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                           new { success = false, message = TranslateHelper.get("Concession Record of the same type is already undergoing approval.") });
                    }
                }
              
              
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = (short)token.GetCompanyId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;

                var data = await repo.AddUpdateFeeConcession(entity);
                if (data.Item1 > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = $"{TranslateHelper.get("The record has been")} {createUpdate} {TranslateHelper.get("successfully")}" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = data.Item2});
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }
         [HttpPost] [ClaimsAuthorization]
        [Route("fee-concession-approval")]
        public HttpResponseMessage GoForApproval([FromBody]ApprovalViewModel entity)
        {
            try
            {
                entity.BranchId = token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.staffId = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userIPAddress = Request.RequestUri.Host;

                var data = repo.GoForApproval(entity);

                if (data == 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = TranslateHelper.get("Record has been approved successfully") });
                } else if (data == 2)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, message = TranslateHelper.get("Record has been disapproved successfully") });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = true, message = TranslateHelper.get("Operation successful, request has been routed to the next approving office") });
                }
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error submitting this record") });
            }
        }
    }
}
