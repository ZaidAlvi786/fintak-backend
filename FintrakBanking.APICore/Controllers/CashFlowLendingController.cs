using FintrakBanking.APICore.CFLAuthentication;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Common;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common.Enum;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels.Authentication;
using FintrakBanking.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{

    [MyBasicAuthenticationFilter] // Authorization: Basic ZmludHJhayZAIyQ6ZmludHJhayZAIzM0OA==
    [RoutePrefix("api/v1/fintrak")]
    public class CashFlowLendingController : ApiController
    {

        private ICashFlowLendingRepository repo;
        private IGeneralSetupRepository genSetup;
        private readonly FinTrakBankingContext context;
        public CashFlowLendingController(ICashFlowLendingRepository _repo,
            FinTrakBankingContext _context


            )
        {
            this.repo = _repo;
            this.context = _context;

        }

        [HttpPost]
        [Route("customer")]
        public async Task<HttpResponseMessage> AddCustomer([FromBody] IncomingCustomerViewModels entity)
        {
            try
            {
                var data = await repo.AddCustomer(entity);

                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                APIResponse response = new APIResponse();
                response.Message = $"{TranslateHelper.get("There was an error creating this record,confirm all requested parameters are captured")}";
                response.requestId = null;
                response.StatusCode = "99";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [HttpPost]
        [Route("cfl-loan-request")]
        public async Task<HttpResponseMessage> submitRequest([FromBody] CflLoanApplication entity)
        {

            try
            {
                var data1 = repo.SaveCashflowRequestToApiLog2(entity);
                //entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = 1; // token.GetCompanyId;
                entity.isInsurance = false;
                //entity.createdBy = token.GetStaffId;
                entity.applicationUrl = Request.RequestUri.AbsoluteUri;
                var data = await repo.submitRequest(entity);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                APIResponse response = new APIResponse();
                response.Message = ex.Message + $"{TranslateHelper.get(" There was an error creating this record, confirm all requested parameters are captured ")}";
                response.requestId = null;
                response.StatusCode = "99";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }

        }

        [HttpPost]
        [Route("cfl-loan-document")]
        public HttpResponseMessage SaveLoanDocuments([FromBody] CflLoanApplication entity)
        {
            APIResponse response = new APIResponse();
            try
            {
                
                entity.isInsurance = true;
                entity.applicationUrl = Request.RequestUri.AbsoluteUri;
                var data = repo.SaveLoanDocuments(entity);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                
                response.Message = ex.Message + $"{TranslateHelper.get(" There was an error creating this record, confirm all requested parameters are captured ")}";
                response.requestId = null;
                response.StatusCode = "99";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }

        }


    }
}
