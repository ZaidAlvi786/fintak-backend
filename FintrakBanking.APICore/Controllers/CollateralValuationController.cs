using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.ViewModels;
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
    [RoutePrefix("api/v1/valuation")]
    public class CollateralValuationController : ApiControllerBase
    {
        private ICollateralValuationRepository _colValuationRepo;
        private IValuationReportRepository _valuationRepo;
        private IValuationRequestTypeRepository _valuationRequestRepo;
        TokenDecryptionHelper token = new TokenDecryptionHelper();

        public CollateralValuationController(ICollateralValuationRepository colValuationRepo, IValuationReportRepository valuationRepo, IValuationRequestTypeRepository valuationRequestRepo)
        {
            _colValuationRepo = colValuationRepo;
            _valuationRepo = valuationRepo;
            _valuationRequestRepo = valuationRequestRepo;
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-all-valuation-reports")]
        public async Task<HttpResponseMessage> GetAllValuationReports()
        {
            try {
                var reports = await _valuationRepo.GetAllValuationReports();
                int totalItems = reports.Count();

                reports = reports.OrderBy(x => x.dateTimeCreated).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = reports, count = totalItems });
            }
            catch (SecureException ex) {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error fetchcing the records") + " " + TranslateHelper.get("Error") + " " + ex.Message });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("add-valuation-report")]
        public async Task<HttpResponseMessage> AddValuationReport([FromBody] ValuationReportViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;

            var response = await _valuationRepo.AddValuationReport(model);

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("add-collateral-valuation")]
        public async Task<HttpResponseMessage> AddCollateralValuation([FromBody] CollateralValuationViewModel model)
        {
            model.userBranchId = (short) token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;

            var response = await _colValuationRepo.AddCollateralValuation(model);

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("add-valuation-prerequisite")]
        public async Task<HttpResponseMessage> AddValuationPrerequisite([FromBody] ValuationPrerequisiteViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;

            var response =await _colValuationRepo.AddValuationPrerequisite(model);

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpPut]
        [Route("edit-valuation-prerequisite/{valuationPrerequisiteId}/valuationPrerequisiteId")]
        public async Task<HttpResponseMessage> UpdateAppraisalMemorandum(int valuationPrerequisiteId, [FromBody] ValuationPrerequisiteViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = await _colValuationRepo.UpdateValuationPrerequisite(valuationPrerequisiteId, entity);
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

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-collateral-valuation/{collteralValuationId}/collteralValuationId")]
        public async Task<HttpResponseMessage> GetCollateralValuation(int collteralValuationId)
        {
            try
            {
                var valuation = await _colValuationRepo.GetCollateralValuation(collteralValuationId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = valuation });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error fetchcing the record")}. {TranslateHelper.get("Error")} - {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-all-collateral-valuations/{collateralId}/collateralId")]
        public async Task<HttpResponseMessage> GetAllCollateralValuations(int collateralId)
        {
            try
            {
                var valuations = await _colValuationRepo.GetAllCollateralValuations(collateralId);
                int totalItems = valuations.Count();

                valuations = valuations.OrderBy(x => x.dateTimeCreated).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = valuations, count = totalItems });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error fetchcing the records. Error")} - {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-all-collateral-valuations-request-list")]
        public async Task<HttpResponseMessage> GetAllValuationRequestList()
        {
            
                var valuations = await _colValuationRepo.GetAllValuationRequestList();
                int totalItems = valuations.Count();
                valuations = valuations.OrderBy(x => x.dateTimeCreated).ToList();
            if (valuations != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = valuations, count = totalItems });
            }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Record(s) not found") });
            
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-valuation-Prerequisite/{valuationPrerequisiteId}/valuationPrerequisiteId")]
        public async Task<HttpResponseMessage> GetCollateralValuationPrerequisiteById(int valuationPrerequisiteId)
        {
            try
            {
                var Prerequisite = await _colValuationRepo.GetCollateralValuationPrerequisiteById(token.GetStaffId, valuationPrerequisiteId);
                //int totalItems = Prerequisites.Count();

                //Prerequisites = Prerequisites.OrderBy(x => x.dateTimeCreated).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = Prerequisite });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error fetchcing the records. Error -") + ex.Message });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-all-valuation-Prerequisites/{collateralValuationId}/collateralValuationId")]
        public async Task<HttpResponseMessage> GetAllValuationPrerequisites(int collateralValuationId)
        {
            try
            {
                var Prerequisites = await _colValuationRepo.GetAllValuationPrerequisitesById(token.GetStaffId, collateralValuationId);
                int totalItems = Prerequisites.Count();

                Prerequisites = Prerequisites.OrderBy(x => x.dateTimeCreated).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = Prerequisites, count = totalItems });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error fetchcing the records. Error") + " - " + ex.Message });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-all-valuation-Prerequisites-list/{collateralValuationId}/collateralValuationId")]
        public async Task<HttpResponseMessage> GetAllValuationPrerequisitesList(int collateralValuationId)
        {
            try
            {
                var Prerequisites = await _colValuationRepo.GetAllValuationPrerequisitesListById(token.GetStaffId, collateralValuationId);
                int totalItems = Prerequisites.Count();

                Prerequisites = Prerequisites.OrderBy(x => x.dateTimeCreated).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = Prerequisites, count = totalItems });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error fetchcing the records. Error") +  " - "+ ex.Message });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-valuation-request-types")]
        public async Task<HttpResponseMessage> GetAllValuationRequestTypes()
        {
            try {
                var requestTypes = await  _valuationRequestRepo.GetAllValuationRequestTypes();
                int totalItems = requestTypes.Count();

                requestTypes = requestTypes.OrderBy(x => x.dateTimeCreated).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = requestTypes, count = totalItems });
            }
            catch (SecureException ex) {
                var message = TranslateHelper.get("There was an error fetchcing the records. Error");
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{message} - {ex.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("go-for-collateral-valuation-approval")]
        public async Task<HttpResponseMessage> GoForApproval([FromBody] ValuationPrerequisiteViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                var response = await _colValuationRepo.GoForCollateralValuationApproval(model);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = _colValuationRepo.ResponseMessage(response, $"COLLATERAL VALUATION ({response.responseMessage})") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error saving this record. Error")} - {ex.Message}" });
            }
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("add-valuer")]
        public async Task<HttpResponseMessage> AddCollateralValuerInfo([FromBody] ValuationPrerequisiteViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var res =await _colValuationRepo.AddCollateralValurerInfo(model);
                //int totalItems = requestTypes.Count();
                //requestTypes = requestTypes.OrderBy(x => x.dateTimeCreated).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = res });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error saving this record. Error")} - {ex.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("update-valuer")]
        public async Task<HttpResponseMessage> UpdateCollateralValuerInfo([FromBody] ValuationPrerequisiteViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var res =await _colValuationRepo.UpdateCollateralValurerInfo(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = res, message = TranslateHelper.get("Success") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record. Error")} - {ex.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("update-valuer-narration")]
        public async Task<HttpResponseMessage> UpdateCollateralNarration([FromBody] ValuationPrerequisiteViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var res =await _colValuationRepo.UpdateCollateralNarration(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = res, message = TranslateHelper.get("Success") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record. Error")} - {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-valuer-info")]
        public async Task<HttpResponseMessage> GetAllCollateralValuerIformation()
        {
            try
            {
                var response = await _colValuationRepo.GetAllCollateralValuerIformation();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error fetchcing the records. Error")} - {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-single-valuer-info/{id}")]
        public async Task<HttpResponseMessage> GetCollateralValuerIformations(int id)
        {
            try
            {
                var response = await _colValuationRepo.GetCollateralValuerIformations(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error fetchcing the records. Error")} - {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-valuer-info/{id}")]
        public async Task<HttpResponseMessage> GetCollateralValuerIformation(int id)
        {
            try
            {
                var response = await _colValuationRepo.GetCollateralValuerIformation(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error fetchcing the records. Error")} - {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-valuer-info-detail/{id}")]
        public async Task<HttpResponseMessage> GetAllCollateralValuerIformationById(int id)
        {
            try
            {
                var response =await _colValuationRepo.GetAllCollateralValuerIformationById(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error fetchcing the records. Error")} - {ex.Message}" });
            }
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-valuation-waiting-for-approval")]
        public async Task<HttpResponseMessage> GetCollateralValuationRequestWaitingForApproval()
        {
            try
            {
                var response = await _colValuationRepo.GetCollateralValuationRequestWaitingForApproval(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response});
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error fetchcing the records. Error")} - {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("search-for-collateral-valuation/{searchString}")]
        public async Task<HttpResponseMessage> SearchForCollateralValuation(string searchString)
        {
            try
            {
                List<ValuationPrerequisiteViewModel> response = await _colValuationRepo.SearchForCollateralValuation(searchString);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-valuation-waiting-for-approval/{collateralId}/collateralId")]
        public async Task<HttpResponseMessage> GetAllValuationRequestWaitingForApproval(int collateralId)
        {
            try
            {
                var response = await _colValuationRepo.GetAllValuationRequest(collateralId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error fetchcing the records. Error")} - {ex.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("submit-collateral-valuation-for-approval")]
        public HttpResponseMessage SubmitApproval([FromBody] ValuationPrerequisiteViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                var response = _colValuationRepo.SubmitApproval(model);
              
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = _colValuationRepo.ResponseMessage(response, $"{TranslateHelper.get("COLLATERAL VALUATION")} ({response.responseMessage})") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error saving this record. Error")} - {ex.Message}" });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("delete-valuation-prerequisite/{valuationPrerequisiteId}/valuationPrerequisiteId")]
        public async Task<HttpResponseMessage> DeleteValuationPrerequisite(int valuationPrerequisiteId)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    //userIPAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()
                };

                var data = await _colValuationRepo.DeleteValuationPrerequisite(valuationPrerequisiteId, user);

                if (data) {
                    return Request.CreateResponse(HttpStatusCode.OK,
                          new { success = true, message = TranslateHelper.get("The valuation prerequisite has been deleted successfully") });
                }
                else {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("There was an error deleting this valuation prerequisite") });
                }
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("There was an error deleting this valuation prerequisite")  + " "+ e.Message });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("update-valuation-prerequisite-status/{valuationPrerequisiteId}/valuationPrerequisiteId")]
        public async Task<HttpResponseMessage> UpdateValuationPrerequisiteStatus(int valuationPrerequisiteId, [FromBody] ValuationPrerequisiteViewModel model)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    //userIPAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()
                };

                var data = await _colValuationRepo.UpdateValuationPrerequisiteStatus(valuationPrerequisiteId, user);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                          new { success = true, message = TranslateHelper.get("The valuation prerequisite has been deleted successfully") });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("There was an error updating this valuation prerequisite") });
                }
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("There was an error updating this valuation prerequisite") + " " +  e.Message });
            }
        }
    }
}