using FintrakBanking.APICore.core;
using FintrakBanking.APICore.Filters;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Common.CustomException;
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
using FintrakBanking.Common;
using System.Threading.Tasks;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/chargefee")]
    [SecureExceptionFilterAttribute]
    public class LoanFeeChargeController : ApiControllerBase
    {
        TokenDecryptionHelper token = new TokenDecryptionHelper();
        private ILoanFeeChargeRepository repo;
        private ILoanRepository loanRepo;
        public LoanFeeChargeController(ILoanFeeChargeRepository _repo, ILoanRepository _loanRepo)
        {
            this.repo = _repo;
            this.loanRepo = _loanRepo;
        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("take-fee/submit")]
        public async Task<HttpResponseMessage> SubmitTakeFee([FromBody] LoanFeeChargesViewModel entity)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;
                entity.userBranchId = (short)token.GetBranchId;
                string response = await repo.SubmitTakeFee(entity);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = response, result = response });
            }
            catch (ConditionNotMetException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = e.Message });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), inner = ex.InnerException });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("take-fee-approval")]
        public async Task<HttpResponseMessage> GetTakeFeeAwaitingApproval()
        {
            try
            {
                var data = await repo.GetTakeFeeAwaitingApproval(token.GetStaffId, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("take-fee-approve")]
        public async Task<HttpResponseMessage> TakeFeeGoForApproval([FromBody] ApprovalViewModel entity)
        {
            try
            {
                TokenDecryptionHelper token = new TokenDecryptionHelper();
                entity.BranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;
                entity.staffId = token.GetStaffId;

                // var data = repo.addApplicationLineTenorChangeApproval(entity);

                var data = await repo.ApproveTakeFee(entity);

                if (data == ApprovalStatusEnum.Approved)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Fee Charge Approved Successfully") });
                }
                else if (data == ApprovalStatusEnum.Processing)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Fee Charge successful, request has been routed to the next approving office") });
                }
                else if (data == ApprovalStatusEnum.Disapproved)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = TranslateHelper.get("Fee Charge details has been disapproved") });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Fee Charge successful, request has been routed to the next approving office") });
                }
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error in this transaction") });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: "+ TranslateHelper.get("an error occured") });
            }

        }
    }
}
