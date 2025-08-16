using FintrakBanking.APICore.CFLAuthentication;
using FintrakBanking.Common;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Customer;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FintrakBanking.APICore.Controllers
{
    [MyBasicAuthenticationFilter]
    [RoutePrefix("api/v1/credit")]
    public class HeadOfficeToSubController1 : ApiController
    {

        private IAppraisalMemorandumRepository repo;
        private readonly FinTrakBankingContext context;
        public HeadOfficeToSubController1(IAppraisalMemorandumRepository _repo,
            FinTrakBankingContext _context)
        {
            this.repo = _repo;
            this.context = _context;

        }

        [HttpPost]
        [Route("subsidiary/appraisal-memorandum/forward")]
        public HttpResponseMessage ForwardAppraisalMemorandum([FromBody] ForwardViewModel entity)
        {
            entity.applicationUrl = HttpContext.Current.Request.Path;
            APIResponse resp = new APIResponse();
            try
            {
                WorkflowResponse response = repo.ForwardAppraisalMemorandum(entity);
                if (response != null)
                {
                    resp.responseMessage = TranslateHelper.get("The loan application has been acted on successfully");
                    resp.responseCode = "00";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error acting on this record") });
            }
            catch (SecureException ex)
            {
                resp.responseMessage = $"There was an error creating this record, confirm all requested parameters are captured";
                resp.responseCode = "400";
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"{TranslateHelper.get("There was an error acting on this record")} {ex.Message}");
            }
        }

    }
}
