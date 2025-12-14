using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.ViewModels;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.ViewModels.Credit;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.Common;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/credit")]
    public class AppraisalMemorandumController : ApiControllerBase
    {
        TokenDecryptionHelper token = new TokenDecryptionHelper();
        private IAppraisalMemorandumRepository repo;

        public AppraisalMemorandumController(IAppraisalMemorandumRepository repo_)
        {
            this.repo = repo_;
        }

        [HttpGet]
        [Route("appraisal-memorandum/loan-application/{loanApplicationId}")]
        public HttpResponseMessage GetAppraisalMemorandumByLoanApplicationId(int loanApplicationId)
        {
            var data = repo .GetAppraisalMemorandum(loanApplicationId, token.GetStaffId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [Route("appraisal-memorandum/loan-application/{loanApplicationId}/documentation")]
        public async Task<HttpResponseMessage> GetAppraisalMemorandumDocumentation(int loanApplicationId)
        {
            var data = await repo.GetAllDocumentation(loanApplicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost]
        [Route("appraisal-memorandum")]
        public async Task<HttpResponseMessage> AddAppraisalMemorandum([FromBody] AppraisalMemorandumViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = await repo .AddAppraisalMemorandum(entity);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        [HttpPut]
        [Route("appraisal-memorandum/{appraisalMemorandumId}")]
        public HttpResponseMessage UpdateAppraisalMemorandum([FromBody] AppraisalMemorandumViewModel entity, int appraisalMemorandumId)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data =repo .UpdateAppraisalMemorandum(entity, appraisalMemorandumId);
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

        [HttpPost]
        [Route("appraisal-memorandum/forward")]
        public HttpResponseMessage ForwardAppraisalMemorandum([FromBody] ForwardViewModel entity)
        {
            var isRemoteCall = HttpContext.Current.Request.Headers["isRemote"];
            if (!string.IsNullOrEmpty(isRemoteCall)) { entity.isRemoteCall = true; }
            else { entity.isRemoteCall = false; };
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.staffRoleCode = token.GetStaffRoleCode;
            
            try
            {

                WorkflowResponse response = repo .ForwardAppraisalMemorandum(entity);
                if (response != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The loan application has been acted on successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error acting on this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error acting on this record")} {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("adhoc-appraisal/forward")]
        public async Task<HttpResponseMessage> AdhocAppraisalMemorandum([FromBody] ForwardViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = await repo.AdhocAppraisalMemorandum(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, TranslateHelper.get("ADHOC APPLICATION")) });
        }

        [HttpPost]
        [Route("lc-approval/forward")]
        public async Task<HttpResponseMessage> LcAppraisalMemorandum([FromBody] LcForwardViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = await repo.LcAppraisalMemorandum(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, TranslateHelper.get("LC ISSUANCE")) });
        }

        [HttpPost]
        [Route("lc-release/forward")]
        public async Task<HttpResponseMessage> LcReleaseMemorandum([FromBody] LcForwardViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = await repo .LcReleaseMemorandum(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, TranslateHelper.get("LC RELEASE")) });
        }

        [HttpPost]
        [Route("lc-cancelation/forward")]
        public async Task<HttpResponseMessage> LcCancelationMemorandum([FromBody] LcForwardViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = await repo.LcCancelationMemorandum(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, TranslateHelper.get("LC CANCELATION")) });
        }

        [HttpPost]
        [Route("lc/enhancement-forward")]
        public async Task<HttpResponseMessage> LcEnhancementMemorandum([FromBody] LcForwardViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = await repo.LcEnhancementMemorandum(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, TranslateHelper.get("LC ENHANCEMENT")) });
        }

        [HttpPost]
        [Route("lc/extension-forward")]
        public async Task<HttpResponseMessage> LcExtensionMemorandum([FromBody] LcForwardViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = await repo .LcIssuanceExtensionMemorandum(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, TranslateHelper.get("LC EXTENSION")) });
        }

        [HttpPost]
        [Route("lc/ussance-extension-forward")]
        public async Task<HttpResponseMessage> LcUsanceExtensionMemorandum([FromBody] LcForwardViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = await repo.LcUsanceExtensionMemorandum(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, TranslateHelper.get("LC USSANCE EXTENSION")) });
        }

        [HttpPost]
        [Route("lc/ussance-forward")]
        public async Task<HttpResponseMessage> LcUssanceMemorandum([FromBody] LcForwardViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = await repo.LcUssanceMemorandum(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, "LC USSANCE") });
        }

        [HttpPost]
        [Route("letter-gen-request/forward")]
        public HttpResponseMessage LetterGenerationRequestMemorandum([FromBody] LetterGenerationRequestViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = repo.LetterGenerationRequestMemorandum(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, "LETTER GENERATION") });
        }

        //[HttpPost]
        //[Route("collateral-swap/forward-for-approval")]
        //public HttpResponseMessage CollateralSwapMemorandum([FromBody] CollateralSwapViewModel entity)
        //{
        //    entity.userBranchId = (short)token.GetBranchId;
        //    entity.companyId = token.GetCompanyId;
        //    entity.createdBy = token.GetStaffId;
        //    entity.staffId = token.GetStaffId;
        //    entity.applicationUrl = HttpContext.Current.Request.Path;

        //    WorkflowResponse response = repo.CollateralSwapMemorandum(entity);

        //    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, "COLLATERAL SWAP") });
        //}


        [HttpGet]
        [Route("appraisal-memorandum/trail/{applicationId}/operation/{operationId}/all/{all}")]
        public async Task<HttpResponseMessage> GetAppraisalMemorandumTrail(int applicationId, int operationId, bool all)
        {
            var data = await repo.GetAppraisalMemorandumTrail(applicationId, operationId, all);
            if (data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("No Record Found") });
        }

        [HttpGet]
        [Route("appraisal-memorandum-lms/trail/{applicationId}/operation/{operationId}")]
        public HttpResponseMessage GetAppraisalMemorandumTrailLms(int applicationId, int operationId)
        {
            var data = repo.GetCallmemoApprovalTrail(applicationId, operationId);
            if (data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("No Record Found") });
        }

        [HttpGet]
        [Route("call-memo/trail/{applicationId}/operation/{operationId}")]
        public HttpResponseMessage GetCallMemoApprovalTrail(int applicationId, int operationId)
        {
            var data = repo .GetCallmemoApprovalTrail(applicationId, operationId);
            if (data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("No Record Found") });
        }

        [HttpGet]
        [Route("get-group-office-failed-transactions")]
        public async Task<HttpResponseMessage> GetFailedGroupOfficeTransactions()
        {
            var data = await repo.GetFailedGroupOfficeTransactions();
            if (data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "No record Found" });
        }

        [HttpGet]
        [Route("appraisal-memorandum/trail/{applicationId}/operation/{operationId}/currentLevel/{currentLevelId}/all/{all}/isClassified/{isClassified}")]
        public HttpResponseMessage GetTrailForReferBack(int applicationId, int operationId, int currentLevelId, bool all, bool isClassified)
        {
            var data = repo.GetTrailForReferBack(applicationId, operationId, currentLevelId, all, isClassified);
            if (data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("No Record Found") });
        }

        [HttpGet]
        [Route("appraisal-memorandum/trail/{applicationId}/operation/{operationId}/currentLevel/{currentLevelId}/all/{all}/isClassified/{isClassified}/isLMSCrossWorkflow/{isLMSCrossWorkflow}")]
        public HttpResponseMessage GetTrailForReferBack(int applicationId, int operationId, int currentLevelId, bool all, bool isClassified, bool isLMSCrossWorkflow = false)
        {
            var data = repo.GetTrailForReferBack(applicationId, operationId, currentLevelId, all, isClassified, isLMSCrossWorkflow);
            if (data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("No Record Found") });
        }


        [HttpGet]
        [Route("appraisal-memorandum/trail/{operationId}")]
        public async Task<HttpResponseMessage> GetAppraisalMemorandumTrailCallMemo(int operationId)
        {
            //var data = repo.GetAppraisalMemorandumTrailCallMemo(operationId);
            //if (data.Any())
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            //}
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("No Record Found") });
        }

        [HttpPost]
        [Route("appraisal-memorandum/privilege")]
        public HttpResponseMessage GetUserPrivilege([FromBody] AuthoritySignatureViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffRoleCode = token.GetStaffRoleCode;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            var data = repo.GetUserPrivilege(entity);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });

        }

        [HttpPost]
        [Route("appraisal-memorandum/privilege-by-code")]
        public HttpResponseMessage GetUserPrivilegeByCode([FromBody] AuthoritySignatureViewModel entity)
        {
         
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.staffRoleCode = token.GetStaffRoleCode;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                
                var data = repo.GetUserPrivilegeByCode(entity);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
 
        }

        [HttpGet]
        [Route("appraisal-memorandum/loan-detail/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetApprovedLoanDetail(int loanApplicationId)
        {
            LoanApplicationDetailsViewModel data = await repo.GetLoanApplicationDetail(loanApplicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [Route("appraisal-memorandum/tranche-detail/{bookingRequestId}")]
        public async Task<HttpResponseMessage> GetApprovedTrancheDetail(int bookingRequestId)
        {
            LoanApplicationDetailsViewModel data = await repo.GetApprovedTrancheDetail(bookingRequestId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [Route("appraisal-memorandum/loan-detail-refnumber/{applicationReferenceNumber}")]
        public async Task<HttpResponseMessage> GetApprovedLoanDetailByReferenceNumber(string applicationReferenceNumber)
        {
            LoanApplicationDetailsViewModel data = await repo.GetLoanApplicationDetailByRefNo(applicationReferenceNumber);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [Route("appraisal-memorandum/single-detail/{detailId}")]
        public async Task<HttpResponseMessage> GetSingleApprovedLoanDetail(int detailId)
        {
            LoanApplicationDetailsViewModel data = await repo.GetSingleLoanApplicationDetail(detailId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }



        [HttpGet]
        [ClaimsAuthorization]
        [Route("crms-secured-collateral-type")]
        public async Task<HttpResponseMessage> GetAllCRMSSecuredCollateralType()
        {
            var data = await repo.GetAllCRMSSecuredCollateralType(token.GetCompanyId);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("crms-all-collateral-type")]
        public async Task<HttpResponseMessage> GetAllCRMSCollateralType()
        {
            var data = await repo.GetAllCRMSAllCollateralType(token.GetCompanyId);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("crms-unsecured-collateral-type")]
        public async Task<HttpResponseMessage> GetAllCRMSUnsecuredCollateralType()
        {
            var data = await repo .GetAllCRMSUnsecuredCollateralType(token.GetCompanyId);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [Route("appraisal-memorandum/loan-detail-fees/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetLoanDetailsFee(int loanApplicationId)
        {
            var data = await  repo.GetLoanDetailsFee(loanApplicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [Route("appraisal-memorandum/loan-detail-change-log/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetLoanDetailChangeLog(int loanApplicationId)
        {
            var data = await repo .GetLoanDetailChangeLog(loanApplicationId);
            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result =  TranslateHelper.get("No records found") });
        }

        [HttpGet, Route("loan-application-approval-process")]
        public HttpResponseMessage GetPendingLoanApplications([FromUri] int operationId, [FromUri] int page, [FromUri] int itemsPerPage, [FromUri] int? classId, [FromUri] string searchString, [FromUri] bool isSpecific)
        {
            IQueryable<LoanApplicationViewModel> items;
            items =  repo.GetPendingLoanApplications(operationId, token.GetCountryId, token.GetBranchId, token.GetStaffId, classId, isSpecific);


            if (!String.IsNullOrEmpty(searchString))
            {

                searchString = searchString.Trim().ToLower();
                items = (from x in items
                         where x.applicationReferenceNumber.ToLower().StartsWith(searchString)
                         || x.applicantName.ToLower().StartsWith(searchString)
                         || x.applicationAmount.ToString() == searchString
                         //|| x.customerGroupName.ToLower().StartsWith(searchString)
                         select x);

                items = items.Take(itemsPerPage);

                //items = items.Where(x =>
                //    (searchString.StartsWith(x.applicationReferenceNumber))
                //    || (searchString.StartsWith(x.customerName.ToLower()))
                //    || (searchString.StartsWith(x.customerGroupName.ToLower()))
                //    ).Take(itemsPerPage);
            }

            var data = items
                .OrderByDescending(x => x.timeIn) // OrderBy() must be called for Skip() to work!
                //.OrderByDescending(x => x.applicationReferenceNumber) // OrderBy() must be called for Skip() to work!
                .Skip(page)
                .Take(itemsPerPage)
                .ToList();
            data = repo .CalculateSLA(data);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = items.Count() });
        }

        [HttpGet, Route("loan-cashflow-document-review")]
        public HttpResponseMessage GetPendingCashFlowDocumentApplication([FromUri] int operationId, [FromUri] int page, [FromUri] int itemsPerPage, [FromUri] int? classId, [FromUri] string searchString, [FromUri] bool isSpecific)
        {
            IQueryable<LoanApplicationViewModel> items;
            items = repo.GetPendingCashFlowDocumentApplication(operationId, token.GetCountryId, token.GetBranchId, token.GetStaffId, classId, isSpecific);


            if (!String.IsNullOrEmpty(searchString))
            {

                searchString = searchString.Trim().ToLower();
                items = (from x in items
                         where x.applicationReferenceNumber.ToLower().StartsWith(searchString)
                         || x.applicantName.ToLower().StartsWith(searchString)
                         || x.applicationAmount.ToString() == searchString
                         //|| x.customerGroupName.ToLower().StartsWith(searchString)
                         select x);

                items = items.Take(itemsPerPage);

                
            }

            var data = items
                .OrderByDescending(x => x.timeIn) // OrderBy() must be called for Skip() to work!
                                                  //.OrderByDescending(x => x.applicationReferenceNumber) // OrderBy() must be called for Skip() to work!
                .Skip(page)
                .Take(itemsPerPage)
                .ToList();
            data = repo.CalculateSLA(data);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = items.Count() });
        }


        [HttpGet, Route("subsidiaries-loan-applications")]
        public async Task<HttpResponseMessage> GetSubsidiaryPendingLoanApplications()
        {
            var data = await repo.GetSubsidiaryPendingLoanApplications();
            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count()});
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }       
        }

        [HttpGet, Route("subsidiaries")]
        public async Task<HttpResponseMessage> GetSubsidiaries()
        {
            var data = await repo.GetSubsidiaries();
            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
        }

        [HttpGet, 
            ClaimsAuthorization,
        Route("pool-application-approval-process")]
        public HttpResponseMessage GetPoolApplications([FromUri] int operationId, [FromUri] int? classId, [FromUri] string searchString)
        {
            IQueryable<LoanApplicationViewModel> items;
            items =  repo.GetPoolApplications(operationId, token.GetCountryId, token.GetBranchId, token.GetStaffId, classId);

            if (!String.IsNullOrEmpty(searchString))
            {

                searchString = searchString.Trim().ToLower();
                items = (from x in items
                         where x.applicationReferenceNumber.ToLower().StartsWith(searchString)
                         || x.applicantName.ToLower().StartsWith(searchString)
                         || x.applicationAmount.ToString() == searchString
                         //|| x.customerGroupName.ToLower().StartsWith(searchString)
                         select x);
            }

            var data = items.OrderByDescending(x => x.timeIn).ToList();
            data =  repo .CalculateSLA(data);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = items.Count() });
        }

        [HttpPut, Route("reassign-application/owner/{staffId}")]
        public async Task<HttpResponseMessage> ChangeApplicationOwner([FromBody] int loanApplicationId, int staffId)
        {
            var entity = new GeneralEntity
            {
                userBranchId = (short)token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path
            };

            var reassigned = await repo.ChangeApplicationOwner(loanApplicationId, staffId, entity);
            if (reassigned)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("Ownership was reassigned successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An error occured when trying to reassign") });
        }

        [HttpPut, Route("reassign-application/{staffId}")]
        public async Task<HttpResponseMessage> ReassignApplication([FromBody] int approvalTrailId, int staffId)
        {
            var entity = new GeneralEntity
            {
                userBranchId = (short)token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path
            };

            var reassigned = await repo.AssignApplication(approvalTrailId, staffId, entity);
            if (reassigned)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("Request was reassigned successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An error occured when trying to reassign") });
        }

        [HttpPut, Route("reassign-multiple-requests/{staffId}")]
        public async Task<HttpResponseMessage> ReassignMultipleRequests([FromBody] List<int> model, int staffId)
        {
            var entity = new GeneralEntity
            {
                userBranchId = (short)token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path
            };

            var reassigned = await repo .ReassignMultipleRequests(model, entity, staffId);
            if (reassigned)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("Request(s) were assigned successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An error occured when trying to reassign") });
        }

        [HttpPut, Route("self-assign-multiple-approval-item")]
        public async Task<HttpResponseMessage> SelfAssignmultipleApprovalItem([FromBody] List<ForwardViewModel> model)
        {
            var entity = new GeneralEntity
            {
                userBranchId = (short)token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path
            };

            var reassigned = await repo.SelfAssignMultpleApplication(model, entity);
            if (reassigned)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("Request assigned successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An error occured when trying to reassign") });
        }

        [HttpPut, Route("revert-transaction-to-general-pool/{trailId}")]
        public async Task<HttpResponseMessage> ReturnAssignApplicationToPool([FromBody] List<ForwardViewModel> model, int trailId)
        {
            var entity = new GeneralEntity
            {
                userBranchId = (short)token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path
            };

            var reassigned = await repo.ReturnAssignApplicationToPool(trailId, entity);
            if (reassigned)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("Request has been returned to general pool successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An error occured when trying to reassign") });
        }

        [HttpPut, Route("selfAssign-application")]
        public async Task<HttpResponseMessage> AssignApplication([FromBody] int approvalTrailId)
        {
            var entity = new GeneralEntity
            {
                userBranchId = (short)token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path
            };

            var reassigned = await repo.AssignApplication(approvalTrailId, token.GetStaffId, entity);
            if (reassigned)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("Request was assigned successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An error occured") });
        }

        [HttpGet, Route("adhoc-approval/{operationId}/class/{classId}")]
        public async Task<HttpResponseMessage> getApplicationsToBeAdhocApprovedForInitiateBooking(int operationId, int? classId)
        {
            IQueryable<LoanApplicationViewModel> items;
            items = await repo.GetPendingAdhocApplications(operationId, token.GetCountryId, token.GetBranchId, token.GetStaffId, classId);


            //if (!String.IsNullOrEmpty(searchString))
            //{

            //    searchString = searchString.Trim().ToLower();
            //    items = (from x in items
            //             where x.applicationReferenceNumber.ToLower().StartsWith(searchString)
            //             || x.applicantName.ToLower().StartsWith(searchString)
            //             || x.applicationAmount.ToString() == searchString
            //             //|| x.customerGroupName.ToLower().StartsWith(searchString)
            //             select x);

            //    items = items.Take(itemsPerPage);

            //    //items = items.Where(x =>
            //    //    (searchString.StartsWith(x.applicationReferenceNumber))
            //    //    || (searchString.StartsWith(x.customerName.ToLower()))
            //    //    || (searchString.StartsWith(x.customerGroupName.ToLower()))
            //    //    ).Take(itemsPerPage);
            //}

            var data = items.OrderByDescending(x => x.applicationReferenceNumber).ToList(); // OrderBy() must be called for Skip() to work!
                                                                                //.Skip(page)
                                                                                //.Take(itemsPerPage)
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = items.Count() });
        }

        [HttpGet]
        [Route("current-committee/application/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetCurrentCommitteeByLoanApplicationId(int loanApplicationId)
        {
            var data = await repo.GetCurrentCommittee(loanApplicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("appraisal-memorandum/forward-secretariat")]
        public async Task<HttpResponseMessage> SecretariatForwardAppraisalMemorandum([FromBody] ForwardCommitteeCamViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            var response = await repo.SecretariatForwardAppraisalMemorandum(entity);

            if (response == true)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpGet, Route("regional-loan-application")]
        public HttpResponseMessage GetRegionalLoanApplications([FromUri] int page, [FromUri] int itemsPerPage, [FromUri] string searchString)
        {
            var items =  repo.GetRegionalLoanApplications(token.GetStaffId);

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim().ToLower();
                items = items.Where(x =>
                    x.applicationReferenceNumber.Contains(searchString)
                    || x.customerName.ToLower().Contains(searchString)
                    || x.customerGroupName.ToLower().Contains(searchString)
                    ).Take(itemsPerPage);
            }

            var data = items
                .OrderByDescending(x => x.timeIn) // OrderBy() must be called for Skip() to work!
                .ThenByDescending(x => x.loanApplicationId)
                .Skip(page)
                .Take(itemsPerPage)
                .ToList();

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = items.Count() });
        }

        [HttpGet]
        [Route("appraisal-memorandum/pending-product-program")]
        public HttpResponseMessage GetPendingProductProgram()
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                staffId = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };

            var data =  repo.GetPendingProductProgram(user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [Route("untenored-status/application/{applicationId}")]
        public HttpResponseMessage GetUntenoredStatus(int applicationId)
        {
            bool status =  repo.GetUntenoredStatus(applicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = status });
        }

        #region MONITORING TRIGGERS

        [HttpGet]
        [Route("application-monitoring-triggers/{applicationId}")]
        public  async Task<HttpResponseMessage> GetApplicationMonitoringTriggers(int applicationId)
        {
            var response = await repo.GetApplicationMonitoringTriggers(applicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpGet]
        [Route("application-monitoring-triggers-aps/{operationId}/applicationDetailId/{applicationDetailId}")]
        public async Task<HttpResponseMessage> GetASP_MonitoringTriggers(int operationId,int applicationDetailId)
        {
            var response = await repo.GetApplicationMonitoringTriggersByOperationId(operationId, applicationDetailId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("application-monitoring-triggers/{applicationId}")]
        public async Task<HttpResponseMessage> SaveApplicationMonitoringTriggers(int applicationId, [FromBody] List<MonitoringTriggersViewModel> entity)
        {
            var response = await repo.SaveApplicationMonitoringTriggers(applicationId, entity, token.GetStaffId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        #endregion MONITORING TRIGGERS

        [HttpGet]
        //[ClaimsAuthorization]
        [Route("repayment-schedule-terms")]
        public HttpResponseMessage GetAllSetupRepaymentTerms()
        {
            //try
            //{
            var response =  repo.GetAllSetupRepaymentTerms();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            //}
            //catch (SecureException ex)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            //}
        }


        [HttpGet]
        //[ClaimsAuthorization]
        [Route("repayment-schedule-terms/{applicationId}")]
        public HttpResponseMessage GetRepaymentScheduleAndTerms(int applicationId)
        {
            try
            {
                var response = repo.GetRepaymentScheduleAndTerms(applicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [Route("get-old-application-reference/{data}")]
        public async Task<HttpResponseMessage> GetAllOldApplicationReference(string data)
        {
            var response = await repo.GetAllOldApplicationReference(data);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpPost]
        //[ClaimsAuthorization]
        [Route("repayment-schedule-terms")]
        public async Task<HttpResponseMessage> SaveRepaymentScheduleAndTerms([FromBody] RepaymentScheduleTermsViewModel entity)
        {
            //try
            //{
                IEnumerable<RepaymentScheduleTermsViewModel> response = await repo.SaveRepaymentScheduleAndTerms(entity);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            //}
            //catch (SecureException ex)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            //}
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("product-limit-validation")]
        public async Task<HttpResponseMessage> SaveProductLimitValidation([FromBody] ProductLimitValidationViewModel entity)
        {
            List<ProductLimitValidationViewModel> response = await repo.SaveProductLimitValidation(entity);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpGet]
        [Route("product-limit-validation/{applicationId}/class/{classId}")]
        public async Task<HttpResponseMessage> GetProductLimitValidation(int applicationId, int classId)
        {
            List<ProductLimitValidationViewModel> response = await repo.GetProductLimitValidation(applicationId, classId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }


        [HttpGet]
        [Route("appraisal-memorandum/workflow-test")]
        public async Task<HttpResponseMessage> WorkflowTest()
        {
            bool data = await repo.WorkflowTest();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [Route("approval-trail/{approvalTrailId}")]
        public async Task<HttpResponseMessage> GetapprovalTrailByTrailId(int approvalTrailId)
        {
            var data = await repo .GetapprovalTrailByTrailId(approvalTrailId);
            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data });
        }

        #region recommended collateral

        [HttpGet]
        [Route("recommended-collateral/{applicationId}")]
        public async Task<HttpResponseMessage> GetRecommendedCollateral(int applicationId)
        {
            List<RecommendedCollateralViewModel> response = await repo.GetRecommendedCollateral(applicationId, token.GetStaffId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpGet]
        [Route("recommended-collateral-history/{applicationId}")]
        public async Task<HttpResponseMessage> GetRecommendedCollateralHistory(int applicationId)
        {
            List<RecommendedCollateralViewModel> response = await repo.GetRecommendedCollateralHistory(applicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                success = true,
                result = response
           });
        } 

        [HttpPost]
        [ClaimsAuthorization]
        [Route("recommended-collateral")]
        public async Task<HttpResponseMessage> AddRecommendedCollateral([FromBody] RecommendedCollateralViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.createdBy = (short)token.GetStaffId;
            List<RecommendedCollateralViewModel> response = await repo.AddRecommendedCollateral(entity);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("recommended-collateral")]
        public async Task<HttpResponseMessage> UpdateRecommendedCollateral([FromBody] RecommendedCollateralViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.createdBy = (short)token.GetStaffId;
            List<RecommendedCollateralViewModel> response = await repo.UpdateRecommendedCollateral(entity);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        #endregion recommended collateral



        #region LMS APPROVAL

        [HttpGet]
        [Route("lms-application-monitoring-triggers/{applicationId}")]
        public async Task<HttpResponseMessage> GetApplicationMonitoringTriggersLms(int applicationId)
        {
            IEnumerable<MonitoringTriggersViewModel> response = await repo.GetApplicationMonitoringTriggersLms(applicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                success = true,
                result = response
            });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lms-application-monitoring-triggers/{applicationId}")]
        public async Task<HttpResponseMessage> SaveApplicationMonitoringTriggersLms(int applicationId, [FromBody] List<MonitoringTriggersViewModel> entity)
        {
            IEnumerable<MonitoringTriggersViewModel> response = await repo.SaveApplicationMonitoringTriggersLms(applicationId, entity, token.GetStaffId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lms-repayment-schedule-terms")]
        public async Task<HttpResponseMessage> SaveRepaymentScheduleAndTermsLms([FromBody] RepaymentScheduleTermsViewModel entity)
        {
            List<RepaymentScheduleTermsViewModel> response = await repo .SaveRepaymentScheduleAndTermsLms(entity);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpGet]
        [Route("lms-recommended-collateral/{applicationId}")]
        public async Task<HttpResponseMessage> GetRecommendedCollateralLms(int applicationId)
        {
            List<RecommendedCollateralViewModel> response = await repo.GetRecommendedCollateralLms(applicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpGet]
        [Route("lms-recommended-collateral-history/{applicationId}")]
        public async Task<HttpResponseMessage> GetRecommendedCollateralHistoryLms(int applicationId)
        {
            List<RecommendedCollateralViewModel> response = await repo .GetRecommendedCollateralHistoryLms(applicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lms-recommended-collateral")]
        public async Task<HttpResponseMessage> AddRecommendedCollateralLms([FromBody] RecommendedCollateralViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            List<RecommendedCollateralViewModel> response = await repo .AddRecommendedCollateralLms(entity);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("lms-recommended-collateral")]
        public async Task<HttpResponseMessage> UpdateRecommendedCollateralLms([FromBody] RecommendedCollateralViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            List<RecommendedCollateralViewModel> response = await repo.UpdateRecommendedCollateralLms(entity);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        #endregion LMS APPROVAL
        
        [HttpPost]
        [ClaimsAuthorization]
        [Route("tranch-disbursment-approval-level")]
        public async Task<HttpResponseMessage> saveTranchDisbursmentApprovalLevel([FromBody] TranchDisbursmentViewModel entity)
        {
            bool response = await repo.saveTranchDisbursmentApprovalLevel(entity);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpGet]
        [Route("appraisal-memorandum/lms-loan-detail/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetApprovedLMSLoanDetail(int loanApplicationId)
        {
            LoanApplicationDetailsViewModel data = await repo.GetLMSLoanApplicationDetail(loanApplicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost]
        [Route("appraisal-memorandum/forward-status")]
        public async Task<HttpResponseMessage> GetWorkflowNextStatus([FromBody] ForwardViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = await repo .GetWorkflowNextStatus(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The loan application has been acted on successfully") });
        }

        [HttpPost]
        [Route("appraisal-memorandum/forward-status-lms")]
        public async Task<HttpResponseMessage> GetWorkflowNextStatusLms([FromBody] ForwardReviewViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = await repo .GetWorkflowNextStatusLms(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The loan application has been acted on successfully") });
        }

        [HttpPost]
        [Route("contractor-tiering")]
        public async Task<HttpResponseMessage> PostContractorTiering([FromBody] ContractorTieringViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            bool response = await repo.AddContractorTiering(entity);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The Contractor criteria has been added successfully") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = response, message = TranslateHelper.get("Error saving Contractor criteria") });
        }

        [HttpGet]
        [Route("global-interest-rate-change-comments/trail/{applicationId}/operation/{operationId}")]
        public async Task<HttpResponseMessage> GetGlobalInterestRateChangeTrail(int applicationId, int operationId)
        {
            var data = await repo .GetGlobalInterestRateChangeTrail(applicationId, operationId);
            if (data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("No Record Found") });
        }

        [HttpPost]
        [Route("add-project-risk-rating")]
        public async Task<HttpResponseMessage> PostProjectRiskRating([FromBody] ProjectRiskRatingViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            var response = await repo .AddProjectRiskRating(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The Contractor criteria has been added successfully") });
        }

    }
}

<!-- Auto-push timestamp: 2025-12-14 14:08:42 -->