using System;
using System.Threading.Tasks;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.APICore.JWTAuth;
using System.Web.Http;
using System.Net.Http;
using System.Web;
using FintrakBanking.APICore.core;
using System.Net;
using FintrakBanking.Common.CustomException;
using System.Linq;
using FintrakBanking.Interfaces.Setups.Credit;
using FintrakBanking.ViewModels.WorkFlow;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using FintrakBanking.Interfaces.CASA;
using FintrakBanking.Common.Enum;
using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{

    [RoutePrefix("api/v1/credit")]
    public class CustomerCollateralController : ApiControllerBase
    {
        TokenDecryptionHelper token = new TokenDecryptionHelper();

        private ICustomerCollateralRepository repo;
        private ICollateralDocumentRepository document;
        private ICollateralTypeRepository type;
        private ICasaRepository casa;


        // private IGuaranteeCollateralRepository guaratee;

        public CustomerCollateralController(
            ICustomerCollateralRepository repo,
            ICollateralTypeRepository type,
            ICollateralDocumentRepository document,
            ICasaRepository _casa
            // IGuaranteeCollateralRepository guaratee
            )
        {
            this.repo = repo;
            this.type = type;
            this.document = document;
            this.casa = _casa;
            //  this.guaratee = guaratee;
        }

        #region
        [HttpGet, Route("collateral-document-release/{collateralId}")]
        public async Task<HttpResponseMessage> GetCollateralReleaseDocumentByCollateral(int collateralId)
        {
            try
            {
                var data = await document.GetCustomerCollateralReleaseDocument(collateralId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("customer-collateral/supporting-document-upload")]
        public async Task<HttpResponseMessage> AddCollateralSupportingDocument() // DEPRECATED
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported media type.");
                }

                MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                int uploadType;
                //if (!Int32.TryParse(provider.FormData["documentTypeId"], out uploadType))
                //{
                //    return Request.CreateResponse(HttpStatusCode.BadRequest, "Upload Type is invalid.");
                //}

                var entity = new CollateralViewModel
                {
                    collateralCode = provider.FormData["collateralCode"],
                    collateralReleaseId = Convert.ToInt32(provider.FormData["collateralReleaseId"]),
                    collateralCustomerId = Convert.ToInt32(provider.FormData["collateralCustomerId"]),
                    //SourceId = Convert.ToInt32( provider.FormData["sourceId"]),
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                };

                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No file uploaded.");
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                var data = await repo.AddReleaseDocument(entity, buffer);

                if (data == 2)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.InnerException}" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("customer-collateral/releaseId/{releaseId}")]
        public async Task<HttpResponseMessage> GetCollateralReleaseDocumentByReleaseId(int releaseId)
        {
            try
            {
                var data = await repo.GetCollateralReleaseDocument(releaseId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("customer-collateral/{documentId}")]
        public async Task<HttpResponseMessage> GetReleaseSupportingDocumentDocument(int documentId)
        {
            try
            {
                var data = await repo.GetReleaseSupportingDocument(documentId);

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

        [HttpPost, Route("customer-collateral/release-collateral")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> ReleaseCollateral([FromBody] CollateralViewModel entity)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.companyId = token.GetCompanyId;
                entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;

                var response = await repo.ReleaseCollateral(entity);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Collateral Updated successfully" });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
            }
            catch (ConditionNotMetException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        [HttpGet, Route("customer-collateral/release-collateral-awaiting-approval")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> GetCollateralReleaseAwaitingApproval()
        {
            try
            {
                var response = await repo.GetCollateralReleaseAwaitingApproval(token.GetCompanyId, token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response.ToList() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpPost, Route("customer-collateral/complete-job-request-release-collateral")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> ReleaseCollateralJobRequest([FromBody] CollateralViewModel entity)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.companyId = token.GetCompanyId;
                entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;

                var response = await repo.ReleaseCollateralJobRequest(entity);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Collateral Release Sent For Approval successfully" });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
            }
            catch (ConditionNotMetException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }


        [HttpPost, Route("customer-collateral/go-for-approval-release-collateral")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> ReleaseCollateralGoForApproval([FromBody] ApprovalViewModel entity)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.BranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.companyId = token.GetCompanyId;
                entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;

                var response = await repo.ReleaseCollateralGoForApproval(entity);
                if (response.status == (int)ApprovalStatusEnum.Approved)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Collateral Release Approved successfully" });
                }
                if (response.status == (int)ApprovalStatusEnum.Processing)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Collateral Release Has Been Sent To The Next Approver (" + response.approvalLevel + ")" });
                }
                if (response.status == (int)ApprovalStatusEnum.Disapproved)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Collateral Release Disapproved successfull" });
                }
                if (response.status == (int)ApprovalStatusEnum.Referred)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Collateral Release Has Been Reffered Back successfull (" + response.approvalLevel + ")" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
            }
            catch (ConditionNotMetException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }



        [HttpGet, Route("customer-collateral/release-collateral-awaiting-job-request")]
        [ClaimsAuthorization]
        public HttpResponseMessage GetCollateralReleaseAwaitingJobRequest()
        {
            try
            {
                var response =  repo.GetCollateralReleaseAwaitingJobRequest(token.GetCompanyId, token.GetBranchId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response.ToList() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("customer-collateral/release-collateral-awaiting-job-request/collateralId/{collateralId}")]
        [ClaimsAuthorization]
        public HttpResponseMessage GetCollateralReleaseAwaitingJobRequest(int collateralId)
        {
            try
            {
                var response =  repo.GetCollateralReleaseAwaitingJobRequest(token.GetCompanyId, token.GetBranchId).Where(a => a.collateralCustomerId == collateralId); ;
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response.ToList() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("customer-collateral/get-collateral-information/colateralcustomerId/{colateralcustomerId}")]
        [ClaimsAuthorization]
        public HttpResponseMessage GetCollateralInformation(int colateralcustomerId)
        {
            try
            {
                var response = repo.GetCustomerCollateralInformation(colateralcustomerId, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("customer-property-collateral/customerId/{customerId}")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> GetCustomerPropertyCollaterals(int? customerId)
        {
            try
            {
                var response = await repo.GetCustomerPropertyCollaterals(customerId, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        //[HttpPost, Route("customer-collateral")]
        //public async Task<HttpResponseMessage> AddCollateral()
        //{
        //    CollateralViewModel incomingData = new CollateralViewModel();

        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported media type.");
        //    }

        //    MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
        //    await Request.Content.ReadAsMultipartAsync(provider);

        //    var formData = provider.FormData["formData"];

        //    var errors = new List<string>();
        //    incomingData = JsonConvert.DeserializeObject<CollateralViewModel>(formData,
        //         new JsonSerializerSettings
        //         {
        //             NullValueHandling = NullValueHandling.Include,
        //             Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs earg)
        //             {
        //                 errors.Add(earg.ErrorContext.Member.ToString());
        //                 earg.ErrorContext.Handled = true;
        //             }
        //         });


        //    if (!provider.FileStreams.Any())
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, "No file uploaded.");
        //    }

        //    incomingData.userBranchId = (short)token.GetBranchId;
        //    incomingData.companyId = token.GetCompanyId;
        //    incomingData.createdBy = token.GetStaffId;
        //    incomingData.applicationUrl = HttpContext.Current.Request.Path;

        //    var file = provider.Contents.FirstOrDefault();
        //    var buffer = await file.ReadAsByteArrayAsync();
        //    var data = repo.AddCollateral(incomingData, buffer);

        //    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });


        //}

        [HttpPost, Route("customer-join-collateral")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> AddJoinCollateralInformation()
        {

            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported media type.");
                }
                MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                var formData = provider.FormData["formData"];

                var errors = new List<string>();
                CollateralViewModel incomingData = JsonConvert.DeserializeObject<CollateralViewModel>(formData,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Include,
                        Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs earg)
                        {
                            errors.Add(earg.ErrorContext.Member.ToString());
                            earg.ErrorContext.Handled = true;
                        }
                    });


                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No file uploaded.");
                }

                incomingData.userBranchId = (short)token.GetBranchId;
                incomingData.companyId = token.GetCompanyId;
                incomingData.createdBy = token.GetStaffId;
                incomingData.applicationUrl = HttpContext.Current.Request.Path;

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                var data = await repo.AddGuaranteeJoinCollateral(incomingData, buffer);

                if (data.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "THERE WAS AN ERROR CREATING THIS RECORD, REFERENCE NUMBER EXIST" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}" });
            }
        }

        [HttpPut, Route("customer-collateral/{collateralId}")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> UpdateCollateral([FromBody] CollateralViewModel entity, int collateralId)
        {
            //try
            //{
            entity.lastUpdatedBy = token.GetStaffId;
            entity.createdBy = token.GetStaffId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.companyId = token.GetCompanyId;

            var response = await repo.UpdateCollateral(entity, collateralId);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Collateral Updated successfully" });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
            //}
            //catch (SecureException ex)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            //}
        }

        [HttpGet, Route("customer-collateral/customer/{id}/application/{applicationId}")]
        [ClaimsAuthorization]
        public HttpResponseMessage GetCustomerCollateral(int id, int? applicationId)
        {
            try
            {
                var response = repo.GetCustomerCollateral(id, applicationId, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("customer-facility/customer/{id}")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> GetCustomerFacility(int id)
        {
            try
            {
                var response = await repo.GetCustomerFacility(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("customer-cash-collateral/customer/{id}/application/{applicationId}")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> GetCustomerCashCollateral(int id, int? applicationId)
        {
            try
            {
                var response = await repo.GetCustomerCashCollateral(id, applicationId, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("customer-cash-collateral-applications/collateralCustomerId/{id}")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> GetCustomerCashCollateralApplications(int id)
        {
            try
            {
                var response = await repo.GetCustomerCashCollateralApplications(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpGet, Route("collateral/application/{applicationId}/currencyId/{currencyId}")]
        public HttpResponseMessage GetProposedCustomerCollateral(int? applicationId, int currencyId)
        {
            try
            {
                var response = repo.GetProposedCustomerCollateral(applicationId, currencyId, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("collateral/application/{customerId}/{getAll}")]
        public HttpResponseMessage GetProposedCustomerCollateralByCustomerId(int customerId, bool getAll)
        {
            try
            {
                var response = repo.GetProposedCustomerCollateralByCustomerId(customerId, getAll);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, ClaimsAuthorization, Route("collateral/failities/{collateralId}")]
        public HttpResponseMessage GetProposedFacilitiesToCollateralByCollateralId(int collateralId)
        {
            try
            {
                var response = repo.GetProposedFacilitiesToCollateralByCollateralId(collateralId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpGet, Route("collateral-lms/application/{customerId}/{getAll}")]
        public async Task<HttpResponseMessage> GetProposedCustomerCollateralByCustomerIdLMS(int customerId, bool getAll)
        {
            try
            {
                var response = await repo.GetProposedCustomerCollateralByCustomerIdLMS(customerId, getAll);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("collateral/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetProposedCustomerCollateralByLoanApplicationDetailId(int loanApplicationDetailId)
        {
            try
            {
                var response = await repo.GetProposedCustomerCollateralByLoanApplicationDetailId(loanApplicationDetailId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("customer-collateral/searchParam/{searchParam}")]
        public HttpResponseMessage GetCustomerCollateralReport(string searchParam)
        {
            try
            {
                var response = repo.GetCustomerCollateralReport(searchParam, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("get-customer-fixed-deposit-collateral/searchParam/{searchParam}")]
        public HttpResponseMessage GetCustomerFixedDepositCollateral(string searchParam)
        {
            try
            {
                var response = repo.GetCustomerFixedDepositCollateral(searchParam, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost, Route("customer-collateral/customer")]
        public HttpResponseMessage GetCustomerCollateralRepo([FromBody]NewCollateralViewModel data)
        {
            try
            {
                var response = repo.GetCustomerCollateral(data.customerId, data.applicationId, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }




        [HttpGet, Route("temp-customer-collateral")]
        public async Task<HttpResponseMessage> GetTempCustomerCollateral()
        {
            try
            {
                var response = await repo.GetTempCustomerCollateralForApproval(token.GetCompanyId, token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost, Route("customer-collateral-by-collateralId")]
        public HttpResponseMessage GetCustomerCollateral([FromBody]int collateralId)
        {
            try
            {
                var response = repo.GetCustomerCollateralByCollateralId(token.GetCompanyId, collateralId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("customer-collateral-insurance-tracking")]
        public async Task<HttpResponseMessage> saveCustomerCollateralInsuranceTracking([FromBody] CollateralInsuranceTrackingViewModel data)
        {
            try
            {
                bool response = await repo.AddCollateralInsuranceTrackingForm(token.GetStaffId, data);
                
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = ex.Message });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = ex.Message });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("customer-collateral-insurance-tracking-update/{id}")]
        public async Task<HttpResponseMessage> saveCustomerCollateralInsuranceTrackingUpdate(int id, [FromBody] CollateralInsuranceTrackingViewModel model)
        {
            try
            {
                bool response = await repo.UpdateCollateralInsuranceTrackingForm(token.GetStaffId, id, model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = ex.Message });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = ex.Message });
            }
        }

        [HttpGet, Route("customer-collateral-insurance-details-confirmation/{id}")]
        public async Task<HttpResponseMessage> getCustomerCollateralInsuranceDetailsConfirmation(int id)
        {
            try
            {
                bool response = await repo.GetCustomerCollateralInsuranceDetailsConfirmation(token.GetStaffId, id);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = ex.Message });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = ex.Message });
            }
        }

        [HttpGet, Route("delete-customer-collateral-insurance-details/{id}")]
        public async Task<HttpResponseMessage> deleteCustomerCollateralInsuranceDetails(int id)
        {
            try
            {
                bool response = await repo.DeleteCustomerCollateralInsuranceDetails(token.GetStaffId, id);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = ex.Message });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = ex.Message });
            }
        }




        [HttpGet, Route("temp-item-policy")]
        public async Task<HttpResponseMessage> GetItemPolicyCollateral()
        {
            try
            {
                var response = await repo.GetTempCollateralInsurancePoliciesWaitingForApproval(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("insurance-policy-approval")]
        public async Task<HttpResponseMessage> GetCollateralInsurancePoliciesWaitingForApproval()
        {
            try
            {
                var response = await repo.GetCollateralInsurancePoliciesWaitingForApproval(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("temp-item-policy/{collateralId}")]
        public async Task<HttpResponseMessage> GetItemPolicyCollateralList(int collateralId)
        {
            try
            {
                var response = await repo.GetTempCollateralInsurancePolicy(collateralId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("item-policy/{collateralId}")]
        public async Task<HttpResponseMessage> GetPolicyCollateralList(int collateralId)
        {
            try
            {
                var response = await repo.GetCollateralInsurancePolicy(collateralId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost, Route("collateral-insurance-policy-list")]
        public async Task<HttpResponseMessage> GetCollateralInsurancePolicyList([FromBody]InsurancePolicy model)
        {
            try
            {
                var response = await repo.GetCollateralInsurancePolicyReport(model.startDate, model.expiryDate, model.valueCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("insurance-policy-report/{trackingId}")]
        public HttpResponseMessage GetInsurancePolicyCollateralReport(int trackingId)
        {
            try
            {
                var response = repo.GetInsurancePolicyCollateralReport(trackingId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost, Route("temp/customer-collateral-approval")]
        public async Task<HttpResponseMessage> PostCustomerCollateralApproval([FromBody]ApprovalViewModel model)
        {
            try
            {
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                model.BranchId = token.GetBranchId;

                var response = await repo.GoForApproval(model);

                if (response == (int)ApprovalStatusEnum.Disapproved)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Disapproved Successfully" });
                }
                else if (response == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = response, message = "Approval has failed" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Approved Successfully" });
            }
            catch (ConditionNotMetException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost, Route("temp/policy-approval")]
        public async Task<HttpResponseMessage> PostItemPolicyApproval([FromBody]ApprovalViewModel model)
        {
            try
            {
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                model.BranchId = token.GetBranchId;

                var response = await repo.GoForPolicyApproval(model);
                if (response == (int)ApprovalStatusEnum.Disapproved)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Disapproved Successfully" });
                }
                else if (response == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = response, message = "Approval has failed" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Approved Successfully" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost, Route("insurance-policy-approval")]
        public async Task<HttpResponseMessage> GoForInsurancePolicyApproval([FromBody]ApprovalViewModel model)
        {
            try
            {
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                model.BranchId = token.GetBranchId;

                WorkflowResponse response = await repo.GoForInsurancePolicyApproval(model);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("customer-collateral/customer/{customerId}/collateral-type/{collateralTypeId}/thirdparty/{thirdpartyCustomerId}")]
        public HttpResponseMessage GetCollateralByCollateralTypeIdByCustomerId(int customerId, short collateralTypeId, short thirdpartyCustomerId = 0)
        {
            try
            {
                var response = repo.GetCollateralByCollateralTypeIdByCustomerId(token.GetCompanyId, collateralTypeId, customerId, thirdpartyCustomerId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        //[HttpGet, Route("customer-collateral/type/collateral/{collateralId}/type/{typeId}")]
        //public HttpResponseMessage GetCollateralTypeByCollateralId(int collateralId, int typeId)
        //{
        //    try
        //    {
        //        var response = repo.GetCollateralTypeByCollateralId(collateralId, typeId);

        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
        //    }
        //}

        [HttpGet, Route("customer-collateral/{collateralId}/collateral/{typeId}/type")]
        public async Task<HttpResponseMessage> GetCollateralTypeByCollateral(int collateralId, int typeId)
        {
            try
            {
                var response = await repo.GetCollateralTypeByCollateralId(collateralId, typeId);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpGet, Route("temp-customer-collateral/type/collateral/{collateralId}/type/{typeId}")]
        public async Task<HttpResponseMessage> GetTempCollateralTypeByCollateralId(int collateralId, int typeId)
        {
            try
            {
                var response = await repo.GetTempCollateralTypeByCollateralId(collateralId, typeId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("get-customer-collateral-by-customer-collateralId/{customerCollateralId}")]
        public async Task<HttpResponseMessage> GetCustomerCollateralByCustomerCollateralId(int customerCollateralId)
        {
            try
            {
                var response = await repo.GetCustomerCollateralByCustomerCollateralId(customerCollateralId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("insurance-policies/{collateralId}")]
        public async Task<HttpResponseMessage> GetInsurancePolicies(int collateralId)
        {
            try
            {
                var response = await repo.GetCollateralInsurancePolicies(collateralId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpGet, Route("insurance-policy/{collateralId}")]
        public async Task<HttpResponseMessage> GetInsurancePolicy(int collateralId)
        {
            try
            {
                var response = await repo.GetInsurancePolicy(collateralId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpPost, Route("add-insurance-policy")]
        public async Task<HttpResponseMessage> AddNewInsurancePolicy(InsurancePolicy insurancePolicies)
        {
            try
            {
                insurancePolicies.userBranchId = (short)token.GetBranchId;
                insurancePolicies.companyId = token.GetCompanyId;
                insurancePolicies.createdBy = token.GetStaffId;
                insurancePolicies.applicationUrl = HttpContext.Current.Request.Path;

                var respo = await repo.AddNewItemInsurancePolicy(insurancePolicies);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = repo });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpGet, Route("collateral-document/{collateralId}")]
        public async Task<HttpResponseMessage> GetCollateralDocumentByCollateral(int collateralId)
        {
            try
            {
                var data = await document.GetCustomerCollateralDocument(collateralId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("temp-collateral-document/{collateralId}")]
        public async Task<HttpResponseMessage> GetTempCollateralDocumentByCollateral(int collateralId)
        {
            try
            {
                var data = await document.GetTempAllCollateralDocument(collateralId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-visitation-file/{documentId}")]
        public async Task<HttpResponseMessage> GetVisitationFile(int documentId)
        {
            try
            {
                var data = await document.GetCollateralVisitationDocument(documentId); //CollateralVisitationDocumentViewModel

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
        [Route("temp-collateral-visitation-file/{documentId}")]
        public async Task<HttpResponseMessage> GetTempVisitationFile(int documentId)
        {
            try
            {
                var data = await document.GetTempCollateralVisitationDocument(documentId); //CollateralVisitationDocumentViewModel

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
        [Route("collateral-guarantee/{targetId}")]
        public async Task<HttpResponseMessage> GetCollaterGuaranteeFile(int targetId)
        {
            try
            {
                var data =  await document.GetCollateralGuaranteeDocument(targetId); //CollateralVisitationDocumentViewModel

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
        [Route("loan-visitation/{collateralVisitationId}")]
        public async Task<HttpResponseMessage> GetVisitationDocument(int collateralVisitationId)
        {
            try
            {
                var data = await repo.GetPropertyVistation(collateralVisitationId);

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
        [Route("temp-collateral-visitation/{collateralVisitationId}")]
        public async Task<HttpResponseMessage> GetTempVisitationDocument(int collateralVisitationId)
        {
            try
            {
                var data = await repo.GetTempPropertyVistation(collateralVisitationId);

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

        [HttpPost]
        [ClaimsAuthorization]
        [Route("visitation-document")]
        public async Task<HttpResponseMessage> AddVisitationDocument()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported media type.");
                }

                MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                int collateralCustomerId;
                if (!Int32.TryParse(provider.FormData["collateralCustomerId"], out collateralCustomerId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Upload Type is invalid.");
                }

                var visitationDate = provider.FormData["lastVisitaionDate"];
                var nextVisitation = provider.FormData["nextVisitationDate"];

                var actualDate = visitationDate.Substring(0, 15);
                var nextDate = nextVisitation.Substring(0, 15);
                var dateVisited = DateTime.ParseExact(actualDate, "ddd MMM dd yyyy", CultureInfo.InvariantCulture);
                var nextVisitationDate = DateTime.ParseExact(nextDate, "ddd MMM dd yyyy", CultureInfo.InvariantCulture);

                var entity = new CollateralDocumentViewModel
                {
                    lastVisitaionDate = dateVisited,
                    nextVisitationDate = nextVisitationDate,
                    visitationRemark = provider.FormData["visitationRemark"],
                    collateralCustomerId = Convert.ToInt32(provider.FormData["collateralCustomerId"]),
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                };

                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No file uploaded.");
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.collateralCustomerId = collateralCustomerId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                var data =await document.AddCollateralVisitation(entity, buffer);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.InnerException}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("temp-visitation-document")]
        public async Task<HttpResponseMessage> AddTempVisitationDocument()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported media type.");
                }

                MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                int collateralCustomerId;
                if (!Int32.TryParse(provider.FormData["collateralCustomerId"], out collateralCustomerId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Upload Type is invalid.");
                }

                var visitationDate = provider.FormData["lastVisitaionDate"];

                var actualDate = visitationDate.Substring(0, 15);
                var dateVisited = DateTime.ParseExact(actualDate, "ddd MMM dd yyyy", CultureInfo.InvariantCulture);

                var entity = new CollateralDocumentViewModel
                {
                    lastVisitaionDate = dateVisited,
                    visitationRemark = provider.FormData["visitationRemark"],
                    collateralCustomerId = Convert.ToInt32(provider.FormData["collateralCustomerId"]),
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                };

                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No file uploaded.");
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.collateralCustomerId = collateralCustomerId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                var data =await document.AddTempCollateralVisitation(entity, buffer);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.InnerException}" });
            }
        }

        [HttpPost, Route("collateral-visitation")]
        public HttpResponseMessage AddCollateralVisitation([FromBody] CollateralDocumentViewModel entity)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.companyId = token.GetCompanyId;

                var response = repo.AddPropertyVistation(entity);
                if (response > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Created successfully" });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        [HttpPost, Route("collateral-document")]
        public async Task<HttpResponseMessage> AddCollateralDocument()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported media type.");
                }

                MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                int collateralId;
                if (!Int32.TryParse(provider.FormData["collateralId"], out collateralId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Upload Type is invalid.");
                }
                int documentTypeId;
                if (!Int32.TryParse(provider.FormData["documentTypeId"], out documentTypeId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Upload Type is invalid.");
                }
                var entity = new CollateralDocumentViewModel
                {
                    documentTitle = provider.FormData["documentTitle"], // document code
                    documentTypeId = Convert.ToInt32(provider.FormData["documentTypeId"]),
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                    collateralCode = provider.FormData["collateralCode"],
                };

                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No file uploaded.");
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.collateralId = collateralId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                var data =await document.AddCollateralDocument(entity, buffer);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.InnerException}" });
            }
        }

        [HttpPost, Route("temp-collateral-document")]
        public async Task<HttpResponseMessage> AddTempCollateralDocument()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported media type.");
                }

                MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                int collateralId;
                if (!Int32.TryParse(provider.FormData["collateralId"], out collateralId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Upload Type is invalid.");
                }

                var entity = new CollateralDocumentViewModel
                {
                    documentTitle = provider.FormData["documentTitle"], // document code
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                    collateralCode = provider.FormData["collateralCode"],
                };

                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No file uploaded.");
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.collateralId = collateralId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                var data =await document.AddTempCollateralDocument(entity, buffer);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.InnerException}" });
            }
        }

        [HttpGet, Route("customer-collateral/loan/{loanId}/productTypeId/{productTypeId}")]
        public async Task<HttpResponseMessage> GetLoanCollateral(int loanId, int productTypeId)
        {
            try
            {
                var response = await repo.GetLoanCollateral(loanId, productTypeId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("customer-collateral/active/{customerId}")]
        public HttpResponseMessage GetActiveCustomerCollateral(int customerId)
        {
            try
            {
                var response = repo.GetActiveCustomerCollateral(customerId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("customer-collateral/release/{mappingId}")]
        public async Task<HttpResponseMessage> ReleaseCollateral(int mappingId)
        {
            GeneralEntity userInfo = new GeneralEntity()
            {
                createdBy = token.GetStaffId,
                companyId = token.GetCompanyId,
                userBranchId = (short)token.GetBranchId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress,
            };
            var response = await repo.ReleaseCollateral(mappingId, token.GetStaffId, userInfo);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
        }

        [HttpGet, Route("collateral-release/pending-approval")]
        public async Task<HttpResponseMessage> GetPendingCustomerCollateralRelease()
        {
            try
            {
                var response = await repo.GetPendingCustomerCollateralRelease(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost, Route("customer-collateral/release-approval")]
        public async Task<HttpResponseMessage> ApproveCollateralRelease([FromBody] ApprovalViewModel entity)
        {
            try
            {
                GeneralEntity userInfo = new GeneralEntity()
                {
                    createdBy = token.GetStaffId,
                    companyId = token.GetCompanyId,
                    userBranchId = (short)token.GetBranchId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = HttpContext.Current.Request.UserHostAddress,
                };
                var response = await repo.ApproveCollateralRelease(entity, token.GetStaffId, userInfo);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost, Route("customer-collateral/assignment")]
        public HttpResponseMessage AssignCollateral([FromBody] ActiveCustomerCollateralViewModel entity)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;

                var response = false; // repo.AssignCollateral(entity);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("customer-collateral/search/")]
        public HttpResponseMessage SearchStaff(string queryString)
        {
            try
            {
                var data = repo.SearchCollateral(queryString, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }

        }

        #endregion New

        #region Collateral 

        [HttpPost]
        [ClaimsAuthorization]
        [Route("customer-collateral")]
        public async Task<HttpResponseMessage> AddCollateral([FromBody] CollateralViewModel entity)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();

            entity.createdBy = token.GetStaffId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            entity.companyId = token.GetCompanyId;

            var response = await repo.AddCollateral(entity);
            if (response > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Collateral Created successfully" });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
            //return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
        }

        //[HttpPut]
        //[Route("customer-collateral/{collateralCustomerId}")]
        //public async Task<HttpResponseMessage> UpdateCustomCollateral(int collateralCustomerId, [FromBody] CollateralCustomerViewModel entity)
        //{

        //    try
        //    {
        //        entity.lastUpdatedBy = token.GetStaffId;
        //        entity.applicationUrl = HttpContext.Current.Request.Path;
        //        entity.userBranchId = (short)token.GetBranchId;

        //        var response = await repo.UpdateCollateralCustomer(collateralCustomerId, entity);
        //        if (!response)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
        //    }
        //}


        #endregion

        #region Collatera Types
        [Route("collateral-document-type/{id}")]
        public async Task<HttpResponseMessage> GetCollateralDocumentType(int id)
        {
            try
            {
                var response = await type.GetCollateralDocumentTypes(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("collateral-document-type")]
        public async Task<HttpResponseMessage> AddCollateralDocumentType([FromBody] CollateralDocumentTypeViewModel entity)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.companyId = token.GetCompanyId;

                var response = await type.AddCollateralDocumentType(entity);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Created successfully" });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }



        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-type")]
        public HttpResponseMessage GetCollateralType()
        {
            try
            {
                var response = type.GetCollateralTypes();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-type/loan-application/{id}")]
        public async Task<HttpResponseMessage> GetCollateralTypeByLoanApplicationId(int? id)
        {
            try
            {
                var response =  await type.CollateralTypesByLoanApplication(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-sub-type")]
        public async Task<HttpResponseMessage> GetCollateralSubTypes()
        {
            try
            {
                var response = await type.GetCollateralSubTypes();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-sub-type/{id}")]
        public async Task<HttpResponseMessage> GetCollateralSubTypes(int id)
        {
            try
            {
                var response = await type.CollateralSubType(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            //catch (SecureException ex)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            //}
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-sub-type/collateral-type/{collateralTypeId}")]
        public async Task<HttpResponseMessage> GetCollateralSubTypeByCollateralTypeId(short collateralTypeId)
        {
            try
            {
                var response = await type.GetCollateralSubTypeByCollateralTypeId(collateralTypeId);
                if (response == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("collateral-sub-type")]
        public async Task<HttpResponseMessage> AddCollateralSubType([FromBody] CollateralSubTypeViewModel entity)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.companyId = token.GetCompanyId;

                var response = await type.AddCollateralSubTypes(entity);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Created successfully" });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("collateral-type/{collateralTypeId}")]
        public async Task<HttpResponseMessage> UpdateCollateralType(short collateralTypeId, [FromBody] CollateralTypeViewModel entity)
        {
            try
            {
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userBranchId = (short)token.GetBranchId;
                entity.createdBy = token.GetStaffId;


                var response = await type.UpdateCollateralTypes(collateralTypeId, entity);
                if (!response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Record created sussessfully", result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("collateral-sub-type/{collateralSubTypeId}")]
        public async Task<HttpResponseMessage> UpdateCollateralSubType(short collateralSubTypeId, [FromBody] CollateralSubTypeViewModel entity)
        {
            try
            {
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userBranchId = (short)token.GetBranchId;
                entity.createdBy = token.GetStaffId;

                var response = await type.UpdateCollateralSubTypes(collateralSubTypeId, entity);
                if (!response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Update successful", result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        #endregion  End of Collateral Types

        #region Seniority Of Claims
        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-seniority-of-claims")]
        public HttpResponseMessage GetCollateralSeciorityOfClaims()
        {
            try
            {
                TokenDecryptionHelper token = new TokenDecryptionHelper();

                var response = repo.GetCollateralSeniorityOfClaims();
                if (response == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        #endregion Seniority Of Claims

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-valuer")]
        public async Task<HttpResponseMessage> GetCollateralValuers()
        {
            try
            {
                var response = await repo.GetCollateralValuer(token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-perfection-status")]
        public HttpResponseMessage GetCollateralPerfectionStatus()
        {
            try
            {
                var response = repo.GetCollateralPerfectionStatus();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-valuer-type")]
        public async Task<HttpResponseMessage> GetCollateralValuerType()
        {
            try
            {
                var response = await repo.GetCollateralValuerType();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-value-base-type/{collateralType}")]
        public HttpResponseMessage GetCollateralValueBaseType(short collateralType)
        {
            try
            {
                var response = repo.GetCollateralValueBaseType(collateralType);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost, Route("collateral-valuer")]
        public async Task<HttpResponseMessage> AddCollateralValuer([FromBody] CollateralValuersViewModel entity)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.companyId = token.GetCompanyId;

                var response = await repo.AddCollateralValuer(entity);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("Created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        [HttpPut, Route("collateral-valuer/{id}")]
        public async Task<HttpResponseMessage> UpdateCollateralValuer([FromBody] CollateralValuersViewModel entity, int id)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.companyId = token.GetCompanyId;

                var response = await repo.UpdateCollateralValuer(entity, id);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message =TranslateHelper.get("Updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        //[HttpGet, Route("unmapped-collateral-application/customer/{customerId}/loanapplication/{loanapplicationid}")]
        //public HttpResponseMessage GetAllUnmappedCustomerCollateral(int customerId, int loanApplicationId)
        //{
        //    try
        //    {
        //        var response = repo.GetAllUnmappedCustomerCollateral(customerId, loanApplicationId , token.GetCompanyId );
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
        //    }
        //}

        //[HttpGet, Route("mapped-collateral-application/customer/{customerId}/loanapplication/{loanapplicationid}")]
        //public HttpResponseMessage GetAllMappedCustomerCollateral(int customerId, int loanApplicationId)
        //{
        //    try
        //    {
        //        var response = repo.GetAllMappedCustomerCollateral(customerId, loanApplicationId, token.GetCompanyId);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
        //    }
        //}

        [HttpPost, Route("application-collateral/map")]
        public async Task<HttpResponseMessage> MapApplicationCollateral([FromBody] ApplicationCollateralMapping entity)
        {
            try
            {

                entity.staffId = token.GetStaffId;

                var response = await repo.MapApplicationCollateral(entity);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost, Route("application-collateral/mapped")]
        public async Task<HttpResponseMessage> IsCollateralMapped([FromBody] ApplicationCollateralMapping entity)
        {
            try
            {

                //      entity.staffId = token.GetStaffId;

                var response = await repo.IsCollateralMapped(entity);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost, Route("application-collateral/unmap")]
        public HttpResponseMessage UnmapApplicationCollateral([FromBody] ApplicationCollateralMapping entity)
        {
            try
            {
                var response = repo.UnmapApplicationCollateral(entity);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet, Route("collateral-information-view/{customercollateralId}")]
        public HttpResponseMessage GetCollateralInformationById(int customercollateralId)
        {
            try
            {
                var response = repo.GetCollateralInformationById(customercollateralId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("get-fixeddeposit-lien-amount")]
        public async Task<HttpResponseMessage> GetLienAmountForFD([FromBody]string accountNumber)
        {
            try
            {
                var response = repo.GetAccountLeinAmountForFD(accountNumber);
                var lienData = await repo.GetAccountLienDetail(accountNumber);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, data = lienData });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-casa-lien-amount/{accountNumber}")]
        public async Task<HttpResponseMessage> GetLienAmountForCASA(string accountNumber)
        {
            try
            {
                var response = repo.GetAccountLeinAmountForCASA(accountNumber);
                var lienData = await repo.GetAccountLienDetail(accountNumber);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, data = lienData });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-history/{collateralId}")]
        public HttpResponseMessage GetCollateralHistory(short collateralId)
        {
            var response = repo.getCollateralHistory(collateralId);
            if (response == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Records not found") });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-history-usage/{collateralId}")]
        public HttpResponseMessage GetCollateralHistoryUsage(int collateralId)
        {
            var response = repo.getCollateralHistoryUsage(collateralId);
            if (response == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Records not found") });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("stock-price")]
        public async Task<HttpResponseMessage> GetStockPrice()
        {
            try
            {
                var response = await repo.getStockPrice();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-stamp-to-cover-values/{customerId}")]
        public async Task<HttpResponseMessage> GetCollateralStampToCoverValues(int customerId)
        {
            try
            {
                var response = await repo.GetCollateralStampToCoverValues(customerId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-casa-balance/{accountNumber}")]
        public HttpResponseMessage GetCollateralStampToCoverValues(string accountNumber)
        {
            try
            {
                var response = repo.GetFixedDepositAccountDetail(accountNumber);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("reject-propose/collateral/{collateralCustomerId}/collateralCustomerId")]
        public async Task<HttpResponseMessage> RejectCollateral(int collateralCustomerId)
        {
            try
            {
                var response = await repo.RejectProposedCollateralForUsage(collateralCustomerId);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("propose-collateral")]
        public async Task<HttpResponseMessage> ProposeCollateral(CollateralCoverageViewModel model)
        {
            try
            {
                model.createdBy = token.GetStaffId;
                model.userBranchId = (short)token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.companyId = token.GetCompanyId;
                var response = await repo.ProposeCollateralForUsage(model);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = ex.Message });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = ex.Message });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("propose-collaterals-lms")]
        public async Task<HttpResponseMessage> ProposeCollateralLms(CollateralCoverageViewModel model)
        {
            try
            {
                model.createdBy = token.GetStaffId;
                model.userBranchId = (short)token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.companyId = token.GetCompanyId;
                var response = await repo.ProposeCollateralForUsageLMS(model);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = ex.Message });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = ex.Message });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-usage")]
        public HttpResponseMessage CollateralUsage()
        {
            try
            {
                var response = repo.GetCollateralUsageStatus();

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("insurance-type")]
        public HttpResponseMessage GetInsuranceType()
        {
            try
            {
                var response = repo.GetInsuranceType();

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("insurance-company")]
        public HttpResponseMessage GetInsuranceCompany()
        {
            try
            {
                var response = repo.GetInsuranceCompany();

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("check-insurance-policy")]
        public async Task<HttpResponseMessage> checkInsurancePolicy([FromBody] InsurancePolicy model)
        {
            try
            {
                var response = await repo.checkInsurancePolicy(model);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });

            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("insurance-policy")]
        public async Task<HttpResponseMessage> AddInsurancePolicy([FromBody] CollateralInsurancePolicyViewModel model)
        {
            try
            {
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                model.userBranchId = (short)token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;

                var response = await repo.AddInsurancePolicy(model);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }

            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });

            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("policy-insurance-doc/{id}")]
        public async Task<HttpResponseMessage> UpdateInsurancePolicy([FromUri] int id, [FromBody] CollateralInsurancePolicyViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                bool response = await repo.UpdateInsurancePolicy(id, model);

                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });

            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("insurance-request-go-for-approval")]
        public async Task<HttpResponseMessage> AddInsurancePolicy([FromBody] CollateralViewModel model)
        {
            try
            {
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                model.userBranchId = (short)token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;

                var response = await repo.InsuranceRequestGoForApproval(model);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }

            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });

            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("update-insurance-policy-request/{id}")]
        public async Task<HttpResponseMessage> UpdateInsurancePolicyRequest([FromBody] CollateralInsuranceRequestViewModel model, int id)
        {
            try
            {
                model.createdBy = token.GetStaffId;
                var response = await repo.UpdateInsurancePolicyRequest(model, id);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("insurance-policy-request/{id}")]
        public async Task<HttpResponseMessage> AddInsurancePolicyRequest([FromBody] CollateralInsuranceRequestViewModel model, int? id)
        {
            try
            {
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                model.userBranchId = (short)token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;

                var response = await repo.AddInsurancePolicyRequest(model, id);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this request, Contact the System Administrator") });
            }

            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });

            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("insurance-request-referenceNumber")]
        public HttpResponseMessage GetReferenceNumber()
        {
            try
            {
                var response = repo.GetReferenceNumber();

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-insurance-requests")]
        public async Task<HttpResponseMessage> GetInsuranceRequests()
        {
            try
            {
                var response = await repo.GetInsuranceRequests(token.GetStaffId);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-insurance-last-comment/{operationId}/{targetId}")]
        public async Task<HttpResponseMessage> GetLastComment(int operationId, int targetId)
        {
            try
            {
                var response = await repo.GetLastComment(targetId, operationId);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("delete-insurance-request/{insuranceRequestId}")]
        public async Task<HttpResponseMessage> DeleteInsuranceRequest(int insuranceRequestId)
        {
            var response = await repo.DeleteInsuranceRequest(insuranceRequestId);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been removed successfully") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error removing this record") });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-coverage/{collateralSubTypeId}/collateralSubTypeId")]
        public async Task<HttpResponseMessage> GetCollateralCoverage(int collateralSubTypeId)
        {
            try
            {
                var response = await repo.GetCollateralCoverage(collateralSubTypeId);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpPost, Route("collateral-coverage")]
        public HttpResponseMessage AddCollateralCoverage([FromBody] CollateralCoverageViewModel entity)
        {
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;

            var response = repo.AddCollateralCoverage(entity);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpDelete, Route("delete-collateral-coverage/{collateralCoverageId}/collateralCoverageId")]
        public async Task<HttpResponseMessage> DeleteCollateralCoverage(int collateralCoverageId)
        {
            var response = await repo.DeleteCollateralCoverage(collateralCoverageId, token.GetStaffId);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been deleted successfully") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpDelete, Route("delete-valuation-valuer/{valuerId}")]
        public async Task<HttpResponseMessage> DeleteAddedValuer(int valuerId)
        {
            var response = await repo.DeleteAddedValuer(valuerId, token.GetStaffId);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been deleted successfully") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpPost, Route("delete-proposed-collateral-coverage")]
        public async Task<HttpResponseMessage> DeleteProposedCollateral(CollateralCoverageViewModel model)
        {
            model.createdBy = token.GetStaffId;
            var response = await repo.DeleteProposedCollateral(model);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The collateral has been unproposed successfully") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error unproposing this collateral") });
        }

        [HttpPost, Route("delete-duplicate-collateral")]
        public async Task<HttpResponseMessage> DeleteDuplicatedCollateral(CollateralViewModel model)
        {
            model.createdBy = token.GetStaffId;
            model.deletedBy = token.GetStaffId;

            var response = await repo.DeleteDuplicatedCollateral(model);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The collateral has been deleted successfully") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("You can not delete a collateral that is not created by you") });
        }

        [HttpPost, Route("calculate-collateral-coverage")]
        public async Task<HttpResponseMessage> CalculateCoverateOfCollateral([FromBody] CollateralCoverageViewModel entity)
        {
            try
            {
                var response = await repo.CalculateCoverateOfCollateral(entity);
                if (response != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });

        }

        [HttpPost, Route("calculate-collateral-coverage-lms")]
        public async Task<HttpResponseMessage> CalculateCoverateOfCollateralLms([FromBody] CollateralCoverageViewModel entity)
        {
            try
            {
                var response = await repo.CalculateCoverateOfCollateralLMS(entity);
                if (response != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });

        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("insurance")]
        public async Task<HttpResponseMessage> GetInsuranceCompanies()
        {
            IEnumerable<InsuranceCompanyViewModel> response = await repo.GetInsuranceCompanies();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("insurance/{id}")]
        public async Task<HttpResponseMessage> GetInsuranceCompany(int id)
        {
            InsuranceCompanyViewModel response = await repo.GetInsuranceCompany(id);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("insurance")]
        public HttpResponseMessage AddInsuranceCompany([FromBody] InsuranceCompanyViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            var response = repo.AddInsuranceCompany(model);
            if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }


        [HttpPut]
        [ClaimsAuthorization]
        [Route("insurance/{id}")]
        public async Task<HttpResponseMessage> UpdateInsuranceCompany([FromBody] InsuranceCompanyViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = await repo.UpdateInsuranceCompany(model, id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, message = TranslateHelper.get("The record has been updated successfully"), count = 1 });
        }


        [HttpDelete]
        [ClaimsAuthorization]
        [Route("insurance/{id}")]
        public async Task<HttpResponseMessage> DeleteInsuranceCompany(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = await repo.DeleteInsuranceCompany(id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("insurance-type-all")]
        public async Task<HttpResponseMessage> GetInsuranceTypes()
        {
            IEnumerable<InsuranceTypeViewModel> response = await repo.GetInsuranceTypes();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-type-all")]
        public async Task<HttpResponseMessage> GetCollateralTypes()
        {
            IEnumerable<CollateralTypeViewModel> response = await repo.GetCollateralTypes();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-sub-type-all/{collateralTypeId}")]
        public async Task<HttpResponseMessage> GetCollateralSubType(int collateralTypeId)
        {
            IEnumerable<CollateralSubTypeViewModel> response = await repo.GetCollateralSubTypes(collateralTypeId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("insurance-status-all")]
        public async Task<HttpResponseMessage> GetInsuranceStatus()
        {
            IEnumerable<InsuranceStatusViewModel> response = await repo.GetInsuranceStatus();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("insurance-type-view-all")]
        public async Task<HttpResponseMessage> GetInsuranceTypesViewAll()
        {
            IEnumerable<InsuranceTypeViewModel> response = await repo.GetInsuranceTypes();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }



        [HttpGet]
        [ClaimsAuthorization]
        [Route("insurance-policy-type-all")]
        public async Task<HttpResponseMessage> GetInsurancePolicyTypes()
        {
            IEnumerable<InsurancePolicyTypeViewModel> response = await repo.GetInsurancePolicyTypes();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("insurance-type/{id}")]
        public async Task<HttpResponseMessage> GetInsuranceType(int id)
        {
            InsuranceTypeViewModel response = await repo.GetInsuranceType(id);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("insurance-type")]
        public HttpResponseMessage AddInsuranceType([FromBody] InsuranceTypeViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            var response = repo.AddInsuranceType(model);
            if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("insurance-policy-type")]
        public HttpResponseMessage AddInsurancePolicyType([FromBody] InsurancePolicyTypeViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            var response = repo.AddInsurancePolicyType(model);
            if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }


        [HttpPut]
        [ClaimsAuthorization]
        [Route("insurance-type/{id}")]
        public async Task<HttpResponseMessage> UpdateInsuranceType([FromBody] InsuranceTypeViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = await repo.UpdateInsuranceType(model, id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, message = TranslateHelper.get("The record has been updated successfully"), count = 1 });
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("insurance-policy-type/{id}")]
        public async Task<HttpResponseMessage> UpdateInsurancePolicyType([FromBody] InsurancePolicyTypeViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = await repo.UpdateInsurancePolicyType(model, id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, message = TranslateHelper.get("The record has been updated successfully"), count = 1 });
        }


        [HttpDelete]
        [ClaimsAuthorization]
        [Route("insurance-type/{id}")]
        public async Task<HttpResponseMessage> DeleteInsuranceType(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = await repo.DeleteInsuranceType(id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("insurance-policy-type/{id}")]
        public async Task<HttpResponseMessage> DeleteInsurancePolicyType(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = await repo.DeleteInsurancePolicyType(id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("insurance-search/{searchString}")]
        public async Task<HttpResponseMessage> GetInsuranceSearch(string searchString)
        {
            IEnumerable<InsurancePolicy> response = await repo.Explore(searchString);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("policy-insurance-doc")]
        public HttpResponseMessage SaveInsurancePolicy([FromBody]   InsurancePolicy model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            var response = repo.AddInsurancePolicyFile(model);
            if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }


        [HttpDelete]
        [ClaimsAuthorization]
        [Route("policy-insurance-doc/{id}")]
        public async Task<HttpResponseMessage> DeleteInsurancePolicy([FromUri] int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = await repo.DeleteInsurancePolicy(id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }

        #region collateral-swap
        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-swap-search/{searchString}")]
        public async Task<HttpResponseMessage> SearchCollateralSwap(string searchString)
        {
            IEnumerable<CollateralSwapViewModel> response = await repo.SearchCollateralSwap(searchString);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-swap")]
        public async Task<HttpResponseMessage> GetAllCollateralSwaps()
        {
            IEnumerable<CollateralSwapViewModel> response = await repo.GetAllCollateralSwaps(token.GetStaffId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-swap-approval")]
        public async Task<HttpResponseMessage> GetCollateralSwapsForApproval()
        {
            IEnumerable<CollateralSwapViewModel> response = await repo.GetCollateralSwapsForApproval(token.GetStaffId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-swap/{id}")]
        public async Task<HttpResponseMessage> GetCollateralSwap(int id)
        {
            CollateralSwapViewModel response = await repo.GetCollateralSwap(id);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("collateral-swap")]
        public HttpResponseMessage AddCollateralSwap([FromBody] CollateralSwapViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            try
            {
                var response = repo.AddCollateralSwap(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                //return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [Route("collateral-swap/forward-for-approval")]
        public HttpResponseMessage CollateralSwapMemorandum([FromBody] CollateralSwapViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = repo.CollateralSwapMemorandum(entity);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, "COLLATERAL SWAP") });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-next-level-for-collateral-swap/{collateralSwapId}")]
        public async Task<HttpResponseMessage> GetNextLevelForCollateralSwap(int collateralSwapId)
        {
            var data = await repo.GetNextLevelForCollateralSwapAsync(collateralSwapId, token.GetStaffId, token.GetCompanyId);

            if (data > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data, message = "NextLevelId fetching was successfull!" });
            }
            return Request.CreateResponse(HttpStatusCode.OK,
                new { success = false, message = "NextLevelId fetching was unsuccessful!" });

        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("collateral-swap/{id}")]
        public async Task<HttpResponseMessage> UpdateCollateralSwap([FromBody] CollateralSwapViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = await repo.UpdateCollateralSwap(model, id, user);

            if (!response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, message = TranslateHelper.get("An error occurred while updating the record"), count = 0 });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, message = TranslateHelper.get("The record has been updated successfully"), count = 1 });
        }


        [HttpDelete]
        [ClaimsAuthorization]
        [Route("collateral-swap/{id}")]
        public async Task<HttpResponseMessage> DeleteCollateralSwap(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = await repo.DeleteCollateralSwap(id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collateral-mapping/{id}")]
        public async Task<HttpResponseMessage> GetCollateralMappingDetails(int id)
        {
            IEnumerable<LoanApplicationDetailViewModel> response = await repo.GetCollateralMappingDetails(id);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-insurance/{CollateralId}")]
        public async Task<HttpResponseMessage> GetAddedInsuranceById(int CollateralId)
        {
            var response = await repo.GetAddedInsuranceById(CollateralId);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }
        #endregion collateral-swap
    }

}

