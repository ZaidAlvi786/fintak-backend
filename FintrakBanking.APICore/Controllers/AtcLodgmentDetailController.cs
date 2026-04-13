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
using FintrakBanking.Interfaces.credit;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.Approval;
using FintrakBanking.ViewModels.credit;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/credit")] // TODO: modify!
    public class AtcLodgmentDetailController : ApiControllerBase
    {
        private IAtcLodgmentDetailRepository repo;
        TokenDecryptionHelper token = new TokenDecryptionHelper();

        public AtcLodgmentDetailController(IAtcLodgmentDetailRepository _repo)
        {
            this.repo = _repo;
        }

       
        [HttpGet]
        [ClaimsAuthorization]
        [Route("atc-lodgment-detail/{id}")]

        public async Task<HttpResponseMessage> GetAtcLodgmentDetail(int id)
        {
            IEnumerable<AtcLodgmentDetailViewModel> response = await repo.GetAtcLodgmentDetail(id);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("atc-lodgment-detail")]
        public async Task<HttpResponseMessage> AddAtcLodgmentDetail([FromBody] AtcLodgmentDetailViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            var response = await repo.AddAtcLodgmentDetail(model);
            if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

       
        [HttpDelete]
        [ClaimsAuthorization]
        [Route("atc-lodgment-detail/{id}")]
        public async Task<HttpResponseMessage> DeleteAtcLodgmentDetail(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = await repo.DeleteAtcLodgmentDetail(id,user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }
    }
}

<!-- Auto-push timestamp: 2026-04-13 16:51:41 -->