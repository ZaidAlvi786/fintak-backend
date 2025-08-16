using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Interfaces.CRMS;
using FintrakBanking.ViewModels.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/crms")]
    public class CRMSController : ApiControllerBase
    {
        private ICRMSRegulatories repo;
        private TokenDecryptionHelper token = new TokenDecryptionHelper();

        public CRMSController(ICRMSRegulatories _repo)
        {
            this.repo = _repo;
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("regulatory/fetch-crms-code")]
        public async Task<HttpResponseMessage> GetCRMSCode([FromBody] CRMSViewModel customer)
        {
            try
            {
                customer.companyId = token.GetCompanyId;
                customer.createdBy = token.GetStaffId;
                customer.userBranchId = (short)token.GetBranchId;
                customer.companyId = token.GetCompanyId;
                customer.lastUpdatedBy = token.GetStaffId;
                customer.applicationUrl = HttpContext.Current.Request.Path;
                var data = await repo.GetCRMSCode(customer);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { data = TranslateHelper.get("no-record"), success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error: an error occured")}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("regulatory/reset-crms-code")]
        public async Task<HttpResponseMessage> ResetCrmsCode([FromBody] CRMSViewModel customer)
        {
            try
            {
                customer.companyId = token.GetCompanyId;
                customer.createdBy = token.GetStaffId;
                customer.userBranchId = (short)token.GetBranchId;
                customer.companyId = token.GetCompanyId;
                customer.lastUpdatedBy = token.GetStaffId;
                customer.applicationUrl = HttpContext.Current.Request.Path;
                var data = await repo .ResetCrmsCode(customer);
                if (!data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No record reset") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("CRMS was reset successfully") });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { data = "no-record", success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("regulatory/crms-code")]
        public async Task<HttpResponseMessage> AddCRMSCode([FromBody] CRMSViewModel customer)
        {
            try
            {
                customer.companyId=  token.GetCompanyId;
                customer.createdBy = token.GetStaffId;
                customer.userBranchId = (short)token.GetBranchId;
                customer.companyId = token.GetCompanyId;
                customer.lastUpdatedBy = token.GetStaffId;
                customer.applicationUrl = HttpContext.Current.Request.Path;
                var data = await repo.AddCRMSCodeAsync(customer);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data});
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { data ="no-record", success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("regulatory/all-loan-with-crms-code")]
        public async Task<HttpResponseMessage> GetAllLoans([FromBody]CRMSViewModel model)
        {
            try
            {
                model.companyId = token.GetCompanyId;
                var data = await repo.GetAllLoansForCRMS(model);
                var dataCount = await repo.LoanCountsByLegalStatus(data);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count= dataCount });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("regulatory/export")]
        public async Task<HttpResponseMessage> ExportScheduleToExcel([FromBody] List<CRMSViewModel> models)
        {
            try
            {
                foreach(var model in models) { model.companyId = token.GetCompanyId; }
                
                var fileBytes = await repo .GenerateCBNReport(models);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = fileBytes });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { data = "no-record",  success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }

            }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("regulatory/export-by-loan-application-id")]
        public async Task<HttpResponseMessage> ExportCRMSToExcel([FromBody] List<CRMSViewModel> models)
        {
            try
            {
                foreach(var model in models) { model.companyId = token.GetCompanyId; }
                var fileBytes = await repo .GenerateCBNReportByLoanAppId(models);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = fileBytes });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { data = "no-record", success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }

        }



        [HttpPost]
        [ClaimsAuthorization]
        [Route("postingdetail/export")]
        public async Task<HttpResponseMessage> ExportPostingDetailToExcel([FromBody] List<CRMSViewModel> models)
        {
            try
            {
                foreach (var model in models) { model.companyId = token.GetCompanyId; }
                var fileBytes = await repo .GenerateCBNReport(models);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = fileBytes });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { data = "no-record", success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }

        }

    }
}
