using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.ViewModels.Admin;
using FintrakBanking.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FintrakBanking.Common;
using System.Threading.Tasks;

namespace FintrakBanking.APICore.Controllers
{
    [Authorize]
    [RoutePrefix("api/admin")]
    public class APILogController : ApiControllerBase
    {
        private IAPIErrorLog _api;
        TokenDecryptionHelper token = new TokenDecryptionHelper();
        public APILogController(IAPIErrorLog api)
        {
            _api = api;
        }


        [HttpPost]
        [Route("api-log")]
        public async Task<HttpResponseMessage> APILog(DateRange dateRange)
        {
            try
            {
               // dateRange.companyId = token.GetCompanyId;

                var data =  await _api.GetAPILog(dateRange.startDate, dateRange.endDate, dateRange.searchInfo);
                if (data!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }

                return Request.CreateResponse(HttpStatusCode.NoContent, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }

        }

        [HttpPost]
        [Route("error-log")]
        public async Task<HttpResponseMessage> ErrorLog(DateRange dateRange)
        {
            try
            {
                // dateRange.companyId = token.GetCompanyId;

                var data = await _api.GetErrorLog(dateRange.startDate, dateRange.endDate);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }

                return Request.CreateResponse(HttpStatusCode.NoContent, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }

        }
    }
}
