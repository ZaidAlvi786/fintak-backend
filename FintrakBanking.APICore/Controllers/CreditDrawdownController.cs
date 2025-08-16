using FintrakBanking.APICore.JWTAuth;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FintrakBanking.APICore.core;
using System.Web;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.ViewModels.WorkFlow;
using FintrakBanking.Common.Extensions;
using FintrakBanking.ViewModels.Credit;
using System.Collections.Generic;
using FintrakBanking.Common.CustomException;
using System;
using System.Threading.Tasks;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/loan")]
    public class CreditDrawdownController : ApiControllerBase
    {
        private ICreditDrawdownRepository repo;
        private TokenDecryptionHelper token = new TokenDecryptionHelper();
        private ExportDataTableToExcel export = new ExportDataTableToExcel();


        public CreditDrawdownController(ICreditDrawdownRepository _repo)
        {
            this.repo = _repo;
        }


        [HttpGet]
        [Route("loan-booking/request/approval")]
        public HttpResponseMessage GetInitiatedLoanApplicationAwaitingApproval()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data = repo.GetBookingRequestAwaitingApproval(token.GetStaffId, token.GetCompanyId, false);

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpGet]
        [Route("loan-booking/request/availment")]
        public HttpResponseMessage GetInitiatedLoanApplicationAwaitingAvailment()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data = repo.GetBookingRequestAwaitingAvailment(token.GetStaffId, token.GetCompanyId, false);

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-request/approval/{loanBookingRequestId}")]
        public async Task<HttpResponseMessage> ApproveInitiatedLoanBooking([FromBody] ApprovalViewModel model, int loanBookingRequestId)
        {
            try
            {
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                model.BranchId = (short)token.GetBranchId;
                model.staffId = token.GetStaffId;

                var response = await repo.GoForBookingRequestApproval(model, loanBookingRequestId);
                if (response != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = response.responseMessage });
                }
                //if (responseId == 1)
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK,
                //        new { success = true, message = "Operation successful, request has been routed to the next approving office" });
                //}
                //else if (responseId == 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK,
                //                            new { success = true, message = "Loan request has been successfully approved" });
                //}
                //else if (responseId == 3)
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK,
                //                            new { success = true, message = "Loan request was successfully disapproved" });
                //}
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("Operation unsuccessful, an error occured while saving changes") });
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }
            

        [HttpPut]
        [ClaimsAuthorization]
        [Route("loan-request/legal-document-for-lines/{loanBookingRequestId}/{value}")]
        public async Task<HttpResponseMessage> setLineFacilityLegalDocumentStatus(int loanBookingRequestId,bool value, [FromBody] RecommendedCollateralViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.createdBy = (short)token.GetStaffId;
            entity.createdBy = (short)token.GetCompanyId;
            var response = await repo.setLineFacilityLegalDocumentStatus(entity, loanBookingRequestId, value);


            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response });

        }


        [HttpGet]
        [Route("loan-application/availment-completed")]
        public async Task<HttpResponseMessage> GetAvailedLoanApplicationsDueForInitiateBooking()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var response = await repo .GetAvailedLoanApplicationsDueForInitiateBooking(token.GetCompanyId, token.GetStaffId, token.GetBranchId);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [Route("loan-application/global-completed/{searchString}")]
        public async Task<HttpResponseMessage> GetGlobalEmployerLoansDueForInitiateBooking(string searchString)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var response = await repo .GetGlobalEmployerLoansDueForInitiateBooking(token.GetCompanyId, searchString);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application/request-booking/{applicationId}")]
        public async Task<HttpResponseMessage> AddLoanBookingRequest(int applicationId, [FromBody] List<LoanBookingRequestViewModel> models)
        {
            foreach (var model in models)
            {
                model.userBranchId = (short)token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
            }

            try
            {
                var response = await repo .AddLoanBookingRequest(applicationId, models);
                if (response != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = response.responseMessage });
                }
            }
            catch(SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,  new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            return Request.CreateResponse(HttpStatusCode.OK,
                new { success = false, message = TranslateHelper.get("Initiating Drawdown Request was unsuccessful") });

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application/request-booking/next/{applicationId}")]
        public async Task<HttpResponseMessage> GetNextLevelForBookingRequest(int applicationId, [FromBody] List<LoanBookingRequestViewModel> models)
        {
            foreach (var model in models)
            {
                model.userBranchId = (short)token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
            }

            var data = await repo .GetNextLevelForBookingRequest(applicationId, models);
            if (data > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data, message = TranslateHelper.get("NextLevelId fetching was successfull!") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("NextLevelId fetching was unsuccessful!") });

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("log-approval/workflow")]
        public async Task<HttpResponseMessage> LogApproval(ForwardViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;

            try
            {
                var response = await repo .LogApprovalForMessage(model, false, true);
                if (response != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = response.responseMessage });
                }
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            return Request.CreateResponse(HttpStatusCode.OK,
                new { success = false, message = TranslateHelper.get("Documentation approval was unsuccessful") });

        }

    }
}