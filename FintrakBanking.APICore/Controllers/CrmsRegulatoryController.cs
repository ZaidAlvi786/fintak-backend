using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.ViewModels.Setups.Credit;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Interfaces.Setups.Credit;
using System.Threading.Tasks;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/credit/regulatory")]
    public class CrmsRegulatoryController : ApiControllerBase
    {
        TokenDecryptionHelper token = new TokenDecryptionHelper();
        private ICrmsRegulatoryRepository repo;

        public CrmsRegulatoryController(ICrmsRegulatoryRepository repo)
        {
            this.repo = repo;
        }

        #region CRMS CREDIT TYPE PRODUCT
        [HttpGet]
        [ClaimsAuthorization]
        [Route("regulatory-setup")]
        public async Task<HttpResponseMessage> GetAllRegulatorySetup()
        {
            try
            {
                var data = await repo.GetAllRegulatorySetup();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("regulatory-type/regtype/{crmsTypeId}")]
        public async Task<HttpResponseMessage> GetRegulatoryByTypeId(int crmsTypeId)
        {
            try
            {
                var data = await repo.GetRegulatoryByTypeId(crmsTypeId, token.GetCompanyId);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("regulatory-type")]
        public async Task<HttpResponseMessage> GetAllRegulatoryType()
        {
            try
            {
                var data = await repo.GetAllRegulatoryType();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("regulatory-setup")]
        public async Task<HttpResponseMessage> AddRegulatory([FromBody] CrmsRegulatoryViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userIPAddress = Request.RequestUri.Host;

                var data = await repo.AddRegulatory(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("regulatory-setup/{regulatoryId}")]
        public async Task<HttpResponseMessage> UpdateRegulatory([FromBody] CrmsRegulatoryViewModel entity, int regulatoryId)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userIPAddress = Request.RequestUri.Host;

                var data = await repo.UpdateRegulatory(entity, regulatoryId);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = TranslateHelper.get("The record has been updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }
        [HttpDelete]
        [ClaimsAuthorization]
        [Route("regulatory-setup/")]
        public async Task<HttpResponseMessage> DeleteRegulatory(int regulatoryId)
        {
            try
            {
                var userBranchId = (short)token.GetBranchId;
                var companyId = token.GetCompanyId;
                var lastUpdatedBy = token.GetStaffId;
                var applicationUrl = HttpContext.Current.Request.Path;
                var userIPAddress = Request.RequestUri.Host;

                var data = await repo.DeleteRegulatory(regulatoryId, userBranchId, companyId, lastUpdatedBy, applicationUrl, userIPAddress);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("The record has been deleted successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error deleting this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }
        #endregion


    }
}
