using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.Setups.Credit;
using FintrakBanking.ViewModels.Setups.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using FintrakBanking.APICore.core;
using FintrakBanking.Common;
using System.Threading.Tasks;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/setup")]
    public class PrudentialGuidelineController : ApiControllerBase
    {


        private IPrudentialGuidelineSetupRepository repo;
        private TokenDecryptionHelper token = new TokenDecryptionHelper();

        public PrudentialGuidelineController(IPrudentialGuidelineSetupRepository _repo)
        {
            repo = _repo;
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("get-prudential-guidelines-type")]
        public HttpResponseMessage getAllPrudentialGuidelinesTypes()
        {
            try
            {
                var data = repo.GetAllGuidelineTypes(token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-prudential-guidelines")]
        public async Task<HttpResponseMessage> getAllPrudentialGuidelines()
        {
            try
            {
                var data = await repo.GetAllGuidelines(token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("get-prudential-guideline/{Id}")]
        public async Task<HttpResponseMessage> getprudentialGuideline(int prudentialGuidelineId)
        {
            try
            {
                var data = await repo.getGuideline(prudentialGuidelineId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("add-prudential-guideline")]
        public HttpResponseMessage addPrudentialGuideline(PrudentialGuidelineViewModel guideline)
        {
            try
            {
                guideline.companyId = token.GetCompanyId;
                guideline.staffId = token.GetStaffId;
                guideline.userBranchId = (short)token.GetBranchId;

                var data = repo.AddGuideline(guideline);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
       [HttpPut] [ClaimsAuthorization]
        [Route("update-prudential-guideline/{prudentialGuidelineId}")]
        public async Task<HttpResponseMessage> updatePrudentialGuideline(int prudentialGuidelineId, PrudentialGuidelineViewModel guideline)
        {
            try
            {
                guideline.companyId = token.GetCompanyId;
                guideline.staffId = token.GetStaffId;
                guideline.userBranchId = (short)token.GetBranchId;


                var data = await repo.UpdateGuideline(guideline, prudentialGuidelineId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpDelete] [ClaimsAuthorization]
        [Route("delete-prudential-guideline/{prudentialGuidelineId}")]
        public async Task<HttpResponseMessage> deletePrudentialGuideline(int prudentialGuidelineId, PrudentialGuidelineViewModel guideline)
        {
            try
            {
                var data = repo.DeleteGuideline(prudentialGuidelineId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }



    }
}

