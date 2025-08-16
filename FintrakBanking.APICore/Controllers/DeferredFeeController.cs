using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.APICore.core;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Interfaces.Setups.Approval;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.Approval;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/credit")] // TODO: modify!
    public class DeferredFeeController : ApiControllerBase
    {
        private IDeferredFeeRepository repo;
        TokenDecryptionHelper token = new TokenDecryptionHelper();

        public DeferredFeeController(IDeferredFeeRepository _repo)
        {
            this.repo = _repo;
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("deferred-fees")]
        public async Task<HttpResponseMessage> GetDeferredFees()
        {
            IEnumerable<LoanChargeFeeViewModel> response = await repo.GetDeferredFees();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("deferred-fees/{loanDetailId}")]
        public async Task<HttpResponseMessage> GetLoanDetailDeferredFees(int loanDetailId)
        {
            var response = await repo.GetLoanDetailDeferredFees(loanDetailId);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("deferred-fee/{id}")]
        public async Task<HttpResponseMessage> GetDeferredFee(int id)
        {
            LoanChargeFeeViewModel response = await repo.GetDeferredFee(id);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("deferred-fee")]
        public async Task<HttpResponseMessage> AddDeferredFee([FromBody] List<LoanChargeFeeViewModel> model)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = (short)token.GetBranchId,
                userIPAddress = HttpContext.Current.Request.UserHostAddress,
                applicationUrl = HttpContext.Current.Request.Path,
                createdBy = token.GetStaffId,
                companyId = token.GetCompanyId,
            };
        
            var response = await repo.AddDeferredFee(model, user);
            if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("deferred-fee/{id}")]
        public async Task<HttpResponseMessage> UpdateDeferredFee([FromBody] LoanChargeFeeViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = await repo.UpdateDeferredFee(model, id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("deferred-fee/{id}")]
        public async Task<HttpResponseMessage> DeleteDeferredFee(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = await repo.DeleteDeferredFee(id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }
    }
}
