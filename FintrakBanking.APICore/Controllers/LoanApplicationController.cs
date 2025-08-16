using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common.Enum;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.Interfaces.CreditLimitValidations;
using FintrakBanking.Interfaces.ErrorLogger;
using FintrakBanking.Interfaces.Setups.Credit;
using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.Credit;
using FintrakBanking.ViewModels.ThridPartyIntegration;
using FintrakBanking.ViewModels.WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common;
using System.Threading;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/credit")]
    public class LoanApplicationController : ApiControllerBase
    {
        private ILoanApplicationRepository repo;
        //private ILoanRepository loanRepository;
        //private ICreditLimitValidationsRepository creditLimitValidationsRepository;
        private ILoanPreliminaryEvaluationRepository repoLoanPEN;
        private TokenDecryptionHelper token = new TokenDecryptionHelper();
        //private IErrorLogRepository errorLogger;
        //private IRepaymentTermsRepository repaymentRepo;

        public LoanApplicationController(
            ILoanApplicationRepository _repo,
            // ILoanRepository _loanRepository,
            // ICreditLimitValidationsRepository _creditLimitValidationsRepository,
            ILoanPreliminaryEvaluationRepository _repoLoanPEN
            //IErrorLogRepository _errorLogger
            // IRepaymentTermsRepository _repaymentRepo
            )
        {
            this.repo = _repo;
            //  this.loanRepository = _loanRepository;
            // this.creditLimitValidationsRepository = _creditLimitValidationsRepository;
            repoLoanPEN = _repoLoanPEN;

            // repaymentRepo = _repaymentRepo;
        }

        #region Loan Application

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application")]
        public async Task<HttpResponseMessage> GetAllLoanApplications()
        {
            try
            {
                var response =await repo.GetAllLoanApplications(token.GetCompanyId);
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet, Route("loan-application/operation/{operationId}/class/{classId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationsByOperation(int operationId, int? classId)
        {
            try
            {
                IQueryable<LoanApplicationViewModel> items;

                items =await repo.GetLoanApplicationsByOperation(operationId, classId, token.GetBranchId, token.GetStaffId);

                var data = items.ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = items.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application/customer/{id}")]
        public async Task<HttpResponseMessage> ExistingLoanApplication(int id)
        {
            try
            {
                var response =await repo.ExistingLoanApplication(id, token.GetCompanyId);
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("check-exiting-certificate-of-ownership/{certificateofownership}")]
        public async Task<HttpResponseMessage> CheckExitingCertificateOfOwnership(string certificateofownership)
        {
            try
            {
                var response = await repo.CheckExistingCertificateOfOwnership(certificateofownership, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationById([FromUri] int loanApplicationId)
        {
            try
            {
                var response = await repo.GetLoanApplicationById(loanApplicationId, token.GetCompanyId);
                if (response != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("single-loan-application/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetSingleLoanApplicationById([FromUri] int loanApplicationId)
        {
            try
            {
                var response = await repo.GetSingleLoanApplicationById(loanApplicationId, token.GetCompanyId);
                if (response == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-detail/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationDetailById([FromUri] int loanApplicationDetailId)
        {
            try
            {
                var response = await repo.GetLoanApplicationDetailById(loanApplicationDetailId, token.GetCompanyId);
                if (response == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }



        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-detail/application/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationDetailByLoanApplicationId([FromUri] int loanApplicationId)
        {
            try
            {
                var response = await repo.GetLoanApplicationDetailByLoanApplicationId(loanApplicationId, token.GetCompanyId);
                if (response == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-list")]
        public async Task<HttpResponseMessage> GetLoanApplicationByRelationshipOfficerId()
        {
            var data = await repo.GetLoanApplicationByRelationshipOfficerId(token.GetStaffId, token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("send-to-edit/{loanApplicationId}/{operationId}")]
        public HttpResponseMessage SendApplicationToEdit(int loanApplicationId, int operationId)
        {
            //try
            //{
            var data = repo.SendApplicationToEdit(loanApplicationId, operationId, token.GetStaffId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = "Sent To Applications List For Modifications" });
            //}

            //catch (SecureException e)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            //}
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-info/application/{id}")]
        public async Task<HttpResponseMessage> GetLoanApplicationInfo(int id)
        {
            try
            {
                var data = await repo.GetLoanApplicationById(id, token.GetCompanyId);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        //[HttpGet] [ClaimsAuthorization]  
        //  [Route("loan-application/{id}")]
        //  public HttpResponseMessage GetLoanAppById(int id)
        //  {
        //      try
        //      { 
        //          var data = repo.GetLoanAppById(id, token.GetCompanyId);

        //          if (data == null)
        //          {
        //              return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
        //          }

        //          return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        //      }
        //      catch (SecureException e)
        //      {
        //          return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
        //      }
        //  }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-eligibility/loanApplicationId/{id}")]
        public async Task<HttpResponseMessage> GetLoanApplicationsDetails(int id)
        {
            try
            {
                var data =await repo.GetLoanApplicationsDetails(id, token.GetCompanyId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-eligibility/loanApplicationDetailId/{id}")]
        public async Task<HttpResponseMessage> GetSingleLoanApplicationsDetails(int id)
        {
            try
            {
                var data = await repo.GetSingleLoanApplicationsDetails(id, token.GetCompanyId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-details/loanApplicationId/{id}")]
        public async Task<HttpResponseMessage> GetAllLoanApplicationsDetails(int id)
        {
            try
            {
                var data =await repo.GetAllLoanApplicationsDetailsById(id, token.GetCompanyId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-applications-details")]
        public async Task<HttpResponseMessage> GetLoanApplicationByRelationshipOfficerId([FromUri] int page, [FromUri] int itemsPerPage)
        {
            try
            {
                var response =await repo.GetLoanApplicationByRelationshipOfficerId(token.GetStaffId, token.GetCompanyId);

                var data = response.OrderByDescending(c => c.loanApplicationId)
                      .Take(itemsPerPage)
                      .Skip(page)
                      .ToList();
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-product-class")]
        public async Task<HttpResponseMessage> GetProductClass()
        {
            try
            {
                var response =await repo.GetProductClass();
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        //[HttpGet] [ClaimsAuthorization]  
        //  [Route("loan-application/search/{searchCriteria}")]
        //  public HttpResponseMessage FindLoan(string searchCriteria)
        //  {
        //      try
        //      {
        //          var response = repo.FindLoanApplication(searchCriteria, token.GetCompanyId);

        //          return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        //      }
        //      catch (SecureException e)
        //      {
        //          return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
        //      }
        //  }

        [HttpGet]
        [Route("customer-by-application/{applicationId}/{processtype}")]
        public async Task<HttpResponseMessage> GetCustomerByApplicationId(int applicationId, string processtype)
        {
            try
            {
                var status = await repo.GetCustomerByApplicationId(applicationId, processtype);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = status });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [Route("customer-transactions/{customerId}/{applicationId}")]
        public async Task<HttpResponseMessage> GetCustomerTransactions(int customerId, int applicationId)
        {
            try
            {
                var status = await repo.GetCustomerTransactions(customerId, applicationId, false);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = status });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [Route("customer-transactions-accounts/{customerId}/{applicationId}")]
        public async Task<HttpResponseMessage> GetCustomerTransactionsAccounts(int customerId, int applicationId)
        {
            try
            {
                var status = await repo.GetCustomerTransactionsAccounts(customerId, applicationId, false);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = status });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpGet]
        [Route("lms-customer-transactions-filtered/{customerId}/{applicationId}/{accountnumber}/{fromYear}/{fromMonth}/{toYear}/{toMonth}")]
        public async Task<HttpResponseMessage> GetLmsCustomerTransactions(int customerId, int applicationId, string accountnumber, int? fromYear, int fromMonth, int? toYear, int toMonth)
        {
            try
            {
                var status = await repo.GetCustomerTransactionsFiltered(customerId, applicationId, accountnumber, fromYear, fromMonth, toYear, toMonth, true);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = status });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [Route("customer-transactions-filter/{customerId}/{applicationId}/{froma}/{to}/{fYear}/{tYear}")]
        public async Task<HttpResponseMessage> GetCustomerTransactionsByFilter(int customerId, int applicationId, int froma, int to, int fYear, int tYear)
        {
            try
            {
                var status = await repo.GetCustomerTransactionsByFilterLogic(customerId, applicationId, froma, to, fYear, tYear, false);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = status });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpGet]
        [Route("retail-recovery-report/{startDate}/{endDate}/{accreditedConsultantId}/{customer}")]
        public async Task<HttpResponseMessage> GetRetailRecoveryReporting([FromUri] DateTime startDate, [FromUri] DateTime endDate, [FromUri] string customer, [FromUri] int accreditedConsultantId)
        {
                var records = await repo.GetRetailRecoveryReporting(startDate, endDate, accreditedConsultantId, customer);
            if (records != null) {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = records });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No record(s) found" });
            }
            
        }




        [HttpGet]
        [Route("customer-ratios/{customerId}/{applicationId}")]
        public async Task<HttpResponseMessage> GetCustomerRatios(int customerId, int applicationId)
        {
            try
            {
                var status = await repo.GetCustomerRatios(customerId, applicationId, false);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = status });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [Route("lms-customer-transactions/{customerId}/{applicationId}")]
        public async Task<HttpResponseMessage> GetLmsCustomerTransactions(int customerId, int applicationId)
        {
            try
            {
                var status = await repo.GetCustomerTransactions(customerId, applicationId, true);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = status });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application")]
        public async Task<HttpResponseMessage> UpdateApprovalStatusForApplication([FromBody] int id)
        {
            var responseMessage = string.Empty;

            //model.applicationUrl = HttpContext.Current.Request.Path;
            //model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            //model.userBranchId = (short)token.GetBranchId;
            //model.createdBy = token.GetStaffId;
            //model.companyId = token.GetCompanyId;
            //model.branchId = (short)token.GetBranchId;

            var response = await repo.UpdateApprovalStatusForApplication(id, token.GetStaffId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("load-customer-turnover/{loanApplicationId}")]
        public HttpResponseMessage LoadCustomerTurnover(int loanApplicationId, [FromBody] CustomerTurnoverViewModel model)
        {
            try
            {
                repo.LoadCustomerTurnover(loanApplicationId, token.GetStaffId, model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = "Successful!" });
            }
            catch (APIErrorException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), errorCode = "99" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Core Banking API error, Failed to Load Customer Turnover!" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-customer-ratios-basel/{loanApplicationId}")]
        public HttpResponseMessage GetCustomerRatiosFromBasel(int loanApplicationId)
        {
            try
            {
                repo.GetCustomerRatiosFromBasel(loanApplicationId, token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = "Successful!" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Core Banking API error, Failed to Load Customer Ratios!" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-customer-group-ratios-basel/{loanApplicationId}")]
        public HttpResponseMessage GetCustomerGroupRatiosFromBasel(int loanApplicationId)
        {
            try
            {
                repo.GetCustomerGroupRatiosFromBasel(loanApplicationId, token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = "Successful!" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Core Banking API error, Failed to Load Customer Group Ratios!" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-corporate-customer-rating-basel/{loanApplicationId}")]
        public HttpResponseMessage GetCorporateCustomerRatingFromBasel(int loanApplicationId)
        {
            try
            {
                repo.GetCorporateCustomerRatingFromBasel(loanApplicationId, token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = "Successful!" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Core Banking API error, Failed to Load Corporate Customer Rating!" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-facility-rating-basel/{loanApplicationId}")]
        public HttpResponseMessage GetFacilityRatingFromBasel(int loanApplicationId)
        {
            try
            {
                repo.GetFacilityRatingFromBasel(loanApplicationId, token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = "Successful!" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Core Banking API error, Failed to Load Facility Rating!" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("load-customer-turnover-lms/{loanApplicationId}")]
        public HttpResponseMessage LoadCustomerTurnoverLms(int loanApplicationId, CustomerTurnoverViewModel model)
        {
            try
            {
                repo.LoadCustomerTurnoverLms(loanApplicationId, token.GetStaffId, model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = "Successful!" });
            }
            catch (APIErrorException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), errorCode = "99" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Core Banking API error, Failed to Load Customer Turnover!" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-facility-rating/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetFacilityRating(int loanApplicationDetailId)
        {
            try
            {
                var facility = await repo.GetFacilityRating(loanApplicationDetailId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = facility });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("loan-application-for-cam")]
        public async Task<HttpResponseMessage> SubmitLoanApplicationForCam([FromBody] LoanApplicationUpdateViewModel loan)
        {
            try
            {
                var responseMessage = string.Empty;
                var feed =await repo.SubmitLoanApplicationForCam(loan.applicationId, token.GetStaffId, loan.checkListIndex);
                var response = (feed == 1 || feed == 2) ? true : false;
                var jumptoDrawdown = feed == 2;
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, jumptoDrawdown = jumptoDrawdown, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("update-loan-application")]
        public async Task<HttpResponseMessage> UpdateLoanApplicationDetails([FromBody]LoanApplicationDatailViewModel entity)
        {
            try
            {

                var user = new UserInfo
                {
                    BranchId = (short)token.GetBranchId,
                    createdBy = token.GetStaffId,
                    companyId = token.GetCompanyId,
                };


                var response =await repo.UpdateLoanApplicationDetails(entity, user);
                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "The loan application completed successfully" });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)

            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"Error Occured =>  {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-product-fees/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationProductFees(int loanApplicationDetailId)
        {
            try
            {
                var response = await repo.GetLoanApplicationProductFees(loanApplicationDetailId);
                if (response == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan/collateralrequirement/{applicationId}/{collateralCurrencyId}")]
        public HttpResponseMessage GetCollateralRequirements(int applicationId, int? collateralCurrencyId)
        {
            try
            {
                var response = repo.GetCollateralRequirements(applicationId, collateralCurrencyId, token.GetCompanyId);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan/validate-application")]
        public async Task<HttpResponseMessage> ValidateDuplicateLoanApplication([FromBody] LoanApplicationViewModel entity)
        {
            var isDuplicate =await repo.ValidateDuplicateLoanApplication(entity);
            if (isDuplicate)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = isDuplicate, result = isDuplicate });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = isDuplicate });
        }

        [HttpPost]
        // [ClaimsAuthorization]
        [Route("loan/application")]
        public HttpResponseMessage AddLoanApplication([FromBody] LoanApplicationViewModel entity)
        {
            try
            {
                var loanDetail = entity.LoanApplicationDetail;
                string msg = "";
                if (entity.productClassId == (short)ProductClassEnum.BondAndGuarantees)
                {
                    foreach (var item in loanDetail)
                    {
                        var bond = item.bondDetails;
                        if (bond == null)
                        {
                            msg = "Kindly Enter Records Into Compulsary Fields";
                            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{msg}" });
                        }
                    }

                }
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;  //FinTrakBankingContext
                entity.branchId = (short)token.GetBranchId;

                entity.misCode = "001";
                entity.teamMisCode = "004";
                //if( entity.LoanApplicationDetail.Count == 0)
                //     return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No facility detail is provided" });

                var response = repo.AddLoanApplication(entity);
                if (response != null)
                {
                    if (response.jumpedDestination) { return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "The loan application completed successfully. proceeds to drawdown." }); }

                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "The loan application completed successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });

            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-reference-number")]
        public HttpResponseMessage GetRefrenceNumber()
        {
            try
            {
                var response = repo.GetRefrenceNumber();
                if (response != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "The loan application completed successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{e.Message}" });
            }
        }

        //[HttpGet]
        //[Route("loan-application/job")]
        //public HttpResponseMessage GetLoanApplicationJobs(int page, int itemsPerPage, int level, int scope)
        //{
        //    try
        //    {
        //        var response = repo.GetLoanApplicationJobs(token.GetCompanyId, level, scope);

        //        int totalItems = response.Count();

        //        response = response
        //            .Skip(page).Take(itemsPerPage)
        //            .ToList();

        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, totalItems = totalItems, message = "Empty result" });
        //    }
        //    catch (SecureException e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
        //    }
        //}

        #endregion Loan Application

        #region Loan Preliminary Evaluation

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan/preliminary-evaluation")]
        public async Task<HttpResponseMessage> AddPreliminaryEvaluation(LoanPreliminaryEvaluationViewModel model)
        {
            try
            {
                var responseMessage = string.Empty;

                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.userBranchId = (short)token.GetBranchId;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                model.branchId = (short)token.GetBranchId;

                if (model.sendForEvaluation)
                {
                    model.isCurrent = true;
                }
                else
                {
                    model.isCurrent = false;
                }

                var response = await repoLoanPEN.AddPreliminaryEvaluation(model);

                if (response != null)
                {
                    if (response.sendForEvaluation)
                    {
                        responseMessage = $"Preliminary evaluation note ({response.preliminaryEvaluationCode}) created successfully, now awaiting approval";
                        return Request.CreateResponse(HttpStatusCode.OK,
                            new { success = true, message = $"{responseMessage}" });
                    }
                    else
                    {
                        responseMessage = $"Preliminary evaluation note ({response.preliminaryEvaluationCode}) saved successfully";
                        return Request.CreateResponse(HttpStatusCode.OK,
                            new { success = true, message = $"{responseMessage}" });
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = "Preliminary evaluation note not created" });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: Preliminary Evaluation Note failed to save." });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan/preliminary-evaluation/approval")]
        public async Task<HttpResponseMessage> ApprovePreliminaryEvaluation(ApprovalViewModel model)
        {
            try
            {
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                model.BranchId = (short)token.GetBranchId;
                model.staffId = token.GetStaffId;

                var data = await repoLoanPEN.GoForApproval(model);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                                            new { success = true, message = "Preliminary evaluation note has been approved successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = "Operation successful, request has been routed to the next approving office" });
                }
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-preliminary-evaluation/loan-type/{loanTypeId}")]
        public async Task<HttpResponseMessage> GetAllLoanPreliminaryEvaluationsByLoanType(int loanTypeId)
        {
            try
            {
                var data = await repoLoanPEN.GetLoanPreliminaryEvaluationsByLoanTypeId(loanTypeId);

                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, count = data.Count(), result = data.ToList() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-preliminary-evaluation/application/{applicationId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationPreliminaryEvaluations(int applicationId)
        {
            var data = await repoLoanPEN.GetLoanApplicationPreliminaryEvaluations(applicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, count = data.Count(), result = data });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-preliminary-evaluation-mapped-to-application")]
        public async Task<HttpResponseMessage> GetLoanPreliminaryEvaluationMappedToApplication()
        {
            var data = await repoLoanPEN.GetLoanPreliminaryEvaluationMappedToApplication();

            if (!data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, count = data.Count(), result = data.ToList() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        //[Route("customer-pen-code")]
        [Route("customer-pen-code/{customerId}/{loanTypeId}/{customerGroupId}")]

        public async Task<HttpResponseMessage> GetCustomerLoanPreliminaryEvaluations(int customerId, int loanTypeId, int customerGroupId = 0)
        {
            try
            {
                var data = await repoLoanPEN.GetCustomerLoanPreliminaryEvaluations(customerId, loanTypeId, customerGroupId);

                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, count = data.Count(), result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan/preliminary-evaluation/awaiting-approval/loan-type/{loanTypeId}")]
        public async Task<HttpResponseMessage> GetLoanPreliminaryEvaluationsForAppprovalByLoanType(int loanTypeId)
        {
            try
            {
                var data = await repoLoanPEN.GetLoanPreliminaryEvaluationsAwaitingApprovalByLoanTypeId(token.GetStaffId, token.GetCompanyId, loanTypeId);

                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("loan/preliminary-evaluation/{loanPenId}")]
        public async Task<HttpResponseMessage> UpdateLoanPreliminaryEvaluation(int loanPenId, LoanPreliminaryEvaluationViewModel model)
        {
            try
            {
                var responseMessage = string.Empty;

                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.userBranchId = (short)token.GetBranchId;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                model.branchId = (short)token.GetBranchId;

                responseMessage = "Preliminary evaluation note updated successfully";

                if (model.sendForEvaluation)
                {
                    model.isCurrent = true;
                    responseMessage = "Preliminary evaluation note updated successfully, now awaiting approval";
                }
                else
                {
                    model.isCurrent = false;
                }

                var response = await repoLoanPEN.UpdatePreliminaryEvaluation(loanPenId, model);

                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = $"{responseMessage}" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = "Preliminary evaluation note not updated" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("loan/preliminary-evaluation/{loanPenId}/loan-application")]
        public async Task<HttpResponseMessage> SendPreliminaryEvaluationForLoanApplication(int loanPenId, LoanPreliminaryEvaluationViewModel model)
        {
            try
            {
                var responseMessage = string.Empty;

                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.userBranchId = (short)token.GetBranchId;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
                model.branchId = (short)token.GetBranchId;

                responseMessage = "Preliminary evaluation note updated successfully";

                var response = await repoLoanPEN.SendPreliminaryEvaluationForLoanApplication(loanPenId, model);

                if (response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = $"{responseMessage}" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = "Preliminary evaluation note not updated" });
                }
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        #endregion Loan Preliminary Evaluation

        #region Loan Collateral

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application/collateral")]
        public async Task<HttpResponseMessage> SaveLoanApplicationCollateral([FromBody] List<LoanApplicationCollateralViewModel> entity)
        {
            try
            {
                foreach (var item in entity)
                {
                    item.userBranchId = (short)token.GetBranchId;
                    item.companyId = token.GetCompanyId;
                    item.createdBy = token.GetStaffId;
                    item.applicationUrl = HttpContext.Current.Request.Path;
                    item.userIPAddress = Request.RequestUri.Host;
                    item.createdBy = token.GetStaffId;
                }


                if (entity != null)
                {
                    var response =await repo.AddLoanApplicationCollateral(entity);

                    if (response)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Collateral saved successfully" });
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Collateral not successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Collateral not successfully" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-collateral/loan-application/{Id}")]
        public HttpResponseMessage GetLoanApplicationCollateral(int id)
        {
            try
            {
                var response = repo.GetLoanApplicationCollateral(id);

                if (response != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = response, message = "No record found!" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        #endregion Loan Collateral

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-details-product/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationDetailsProductProgram([FromUri] int loanApplicationDetailId)
        {
            var response = await repo.GetLoanApplicationDetailsProductProgram(loanApplicationDetailId);
            if (response == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-dedube-check/{customerId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationDedubeCheck([FromUri] int customerId)
        {
            var response = await repo.GetLoanApplicationDedubeCheck(customerId, token.GetCompanyId);
            if (response == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-validate-document-date")]
        public async Task<HttpResponseMessage> ValidateDocumentDate([FromBody] ValidateDataViewModel data)
        {


            var response = await repo.ValidateDocumentDate(data);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });

        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-validate-document-number")]
        public async Task<HttpResponseMessage> ValidateDocumentNumber([FromBody] ValidateNumberViewModel data)
        {


            var response = await repo.ValidateDocumentNumber(data);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });

            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });


        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("validate-invoice-details")]
        public async Task<HttpResponseMessage> ValidateInvoiceDetails([FromBody] ValidateNumberViewModel data)
        {


            var response =await repo.ValidateInvoiceDetails(data);
            if (response == true)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
        }

        //[HttpPost]
        //[ClaimsAuthorization]
        //[Route("validate-bulk-invoice-details")]
        //public HttpResponseMessage ValidateBulkLoanInvoice([FromBody] byte[] data)
        //{
        //    try
        //    {
        //        var response = repo.ValidateBulkLoanInvoice(data);
        //        if (response.Count > 0)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        //        }

        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"Upload file was not found!" });
        //        }      

        //    }
        //    catch (SecureException e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
        //    }
        //}

        [HttpGet, Route("loan-application-and-offer/rejected")]
        public async Task<HttpResponseMessage> GetRejectedLoanApplications()
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                staffId = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
            };

            IQueryable<LoanApplicationViewModel> items;

            items = await repo.GetRejectedLoanApplications(user);

            var data = items.ToList();

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = items.Count() });

        }

        [HttpGet, Route("loan-review-application-and-offer/rejected")]
        public async Task<HttpResponseMessage> GetRejectedReviewLoanApplications()
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                staffId = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
            };

            IQueryable<LoanReviewApplicationViewModel> items;

            items =await repo.GetRejectedReviewLoanApplications(user);

            var data = items.ToList();

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = items.Count() });

        }

        [HttpGet, Route("loan-application-and-offer/rejected/arch")]
        public async Task<HttpResponseMessage> GetRejectedLoanApplicationsArch()
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                staffId = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
            };

            IQueryable<LoanApplicationViewModel> items;

            items =await repo.GetRejectedLoanApplicationsArch(user);

            var data = items.ToList();

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = items.Count() });

        }

        //[HttpPut]
        //[Route("loan-application-for-cam")]
        //public HttpResponseMessage SubmitLoanApplicationForCam([FromBody] dynamic model)
        //{
        //    try
        //    {
        //        var response = repo.SubmitLoanApplicationForCam(model.id, token.GetStaffId, model.checkListIndex);

        //        bool ok = !response.isdone  ? false : true;

        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = ok, result = response });
        //    }
        //    catch (SecureException e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: { e.InnerException }" });
        //    }
        //}


        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application/review-request")]
        public async Task<HttpResponseMessage> ReviewRequest([FromBody] ForwardViewModel model)
        {


            model.userBranchId = (short)token.GetBranchId;
            model.companyId = token.GetCompanyId;
            model.createdBy = token.GetStaffId;
            model.applicationUrl = HttpContext.Current.Request.Path;

            string response =await repo.ReviewRequest(model);

            bool ok = response == string.Empty ? false : true;

            return Request.CreateResponse(HttpStatusCode.OK, new { success = ok, result = response });

            //return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-fees/{loanDetailId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationFees(int loanDetailId)
        {
            var response = await repo.GetLoanApplicationFees(loanDetailId);
            if (response == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("fee-concession-request")]
        public async Task<HttpResponseMessage> ProductFeesConcession([FromBody]ProductFeesViewModel entity)
        {
            var user = new UserInfo
            {
                BranchId = (short)token.GetBranchId,
                createdBy = token.GetStaffId,
                companyId = token.GetCompanyId,
            };
            var response =await repo.ProductFeesConcession(entity, user);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "The Fee concession request completed successfully" });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application-detail-search")]
        public async Task<HttpResponseMessage> LoanApplicationSearch([FromBody] SearchViewModel model)
        {
            var response = await repo.Search(model.searchString);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Search result for " + model.searchString, result = response });

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("drawdown-application-detail-search")]
        public async Task<HttpResponseMessage> DrawDownApplicationSearch([FromBody] SearchViewModel model)
        {
            var response = await repo.SearchDrawDown(model.searchString);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Search result for " + model.searchString, result = response });

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("search-booked-loans/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> SearchBookedLoans(int loanApplicationDetailId)
        {
            var response = await repo.SearchBookedLoans(loanApplicationDetailId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Search result for " + loanApplicationDetailId, result = response });

        }

        // search loan application by either reference number or name =======by benjamin
        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-search/search/{searchString}")]
        public async Task<HttpResponseMessage> GetLoanApplicationSearch([FromUri] string searchString)
        {
            var response = await repo.LoanSearch(searchString);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Search result for " + searchString, result = response });
            
            
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application-details/search")]
        public async Task<HttpResponseMessage> SearchLoanApplicationDetails([FromBody] SearchViewModel model)
        {
            var response = await repo.GetLoanApplicationDetailsByReference(model.searchString, token.GetCompanyId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Search result for " + model.searchString, result = response });
           
            
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("approved-loan-application-details/search")]
        public async Task<HttpResponseMessage> SearchApprovedLoanApplicationDetails([FromBody] SearchViewModel model)
        {
            var response = await repo.SearchApprovedLoanApplicationDetails(model.searchString, token.GetCompanyId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Search result for " + model.searchString, result = response });
           
           
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-total-bank-exposure-and-limit")]
        public HttpResponseMessage GetBankTotalExposure()
        {
            var response = repo.GetTotalBankExposure();

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
           
            
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("modify-facility/{loanApplicationDetailId}")]
        public HttpResponseMessage ModifyFacility([FromBody] FacilityModificationViewModel model, int loanApplicationDetailId)
        {
            var response = repo.ModifyFacility(model, loanApplicationDetailId);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Facility Has Been Modified Successfully" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = response, message = "Facility Modification was not Successful" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application/cancellation")]
        public async Task<HttpResponseMessage> LoanApplicationCancellation()
        {
            var response = await repo.GetAllRequestsForLoanCancellation(token.GetStaffId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
           
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lms-loan-application/cancellation")]
        public async Task<HttpResponseMessage> LmsLoanApplicationCancellation()
        {
            var response = await repo.GetAllLmsRequestsForLoanCancellation(token.GetStaffId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application/loan-cancellation")]
        public async Task<HttpResponseMessage> LoanApplicationCancellationRequest([FromBody] LoanApplicationViewModel data)
        {
            data.createdBy = token.GetStaffId;
            data.companyId = token.GetCompanyId;
            data.userBranchId = (short)token.GetBranchId;
            var response =await repo.SaveCancelledApplcation(data);
            if (response == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Loan Application Has been Cancelled Successfully" });
            }
            else if (response == 2)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Loan Application Has been Cancelled Successfully" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, result = response, message = "An error Occured while cancelling this loan Application" });
            }
            
            //if()
            //return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            //try
            //{
            //    data.createdBy = token.GetStaffId;
            //    data.companyId = token.GetCompanyId;
            //    var response = repo.SaveCancelledApplcation(data);
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            //}
            //catch (ConditionNotMetException e)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, warning = true, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            //}
            //catch (SecureException e)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            //}
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application/lms-loan-cancellation")]
        public async Task<HttpResponseMessage> LmsLoanApplicationCancellationRequest([FromBody] LoanReviewApplicationViewModel data)
        {
            data.createdBy = token.GetStaffId;
            data.companyId = token.GetCompanyId;
            data.userBranchId = (short)token.GetBranchId;
            var response =await repo.SaveLMSCancelledApplcation(data);
            if (response == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Loan Application Has been Cancelled Successfully" });
            }
            else if (response == 2)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Loan Application Has been saved Successfully and sent for approval" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, result = response, message = "An error Occured while cancelling this loan Application" });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application/search")]
        public async Task<HttpResponseMessage> SearchLoanApplication(string searchString)
        {
            var response = await repo.SearchForLoan(searchString);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Search result for " + searchString, result = response });
            
           
        }

        //[HttpGet]
        //[ClaimsAuthorization]
        //[Route("loan-detail-by-application-reference/{searchString}")]
        //public HttpResponseMessage LoanApplicationDetailByApplicationRef(string searchString)
        //{
        //    var response = repo.SearchLoanApplicationDetails(token.GetCompanyId, searchString);

        //    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        //}

        [HttpGet]
        [ClaimsAuthorization]
        [Route("committee-credit-application/{applicationType}")]
        public async Task<HttpResponseMessage> CommitteeCreditApplications(int applicationType)
        {
            var response = await repo.CommitteeCreditApplications(applicationType, token.GetStaffId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            
           
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("tranche-facility/{searchValue}")]
        public async Task<HttpResponseMessage> TrancheLoanDetails(string searchValue)
        {
            var response = await repo.GetLoanApplication(searchValue);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            
           
        }


        [HttpDelete]
        [ClaimsAuthorization]
        [Route("loanApplication/{id}")]
        public async Task<HttpResponseMessage> DeleteLoanApplication(int id)
        {
            var token = new TokenDecryptionHelper();

            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                staffId = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            var result = await repo.DeleteLoanApplication(id);
            // if(result)
            return Request.CreateResponse(HttpStatusCode.OK,
                new { success = true, result = result, message = "loan Application was removed successfully" });
        }




        [HttpDelete]
        [ClaimsAuthorization]
        [Route("loanApplicationDetail/{id}")]
        public async Task<HttpResponseMessage> DeleteLoanApplicationDetail(int id)
        {
            var token = new TokenDecryptionHelper();

            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                staffId = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            var result = await repo.DeleteLoanApplicationDetail(id);
            // if(result)
            return Request.CreateResponse(HttpStatusCode.OK,
                new { success = true, result = result, message = "loan Application was removed successfully" });
        }

        [HttpPost]
        [Route("reroute-workflow-target")]
        public HttpResponseMessage RerouteWorkflowTarget([FromBody] ForwardViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = repo.RerouteWorkflowTarget(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Reroute done." });
        }

        [HttpPost]
        [Route("route-workflow-target")]
        public HttpResponseMessage RouteWorkflowTarget([FromBody] ForwardViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = repo.RouteWorkflowTarget(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Reroute done." });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application-cancellation")]
        public async Task<HttpResponseMessage> ViewLaonApplicationCancellationDetails([FromBody] LoanApplicationViewModel data)
        {
            var response = await repo.ViewLaonApplicationCancellationDetails(data);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            
            
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lms-loan-application-cancellation")]
        public async Task<HttpResponseMessage> ViewLmsLaonApplicationCancellationDetails([FromBody] LoanReviewApplicationViewModel data)
        {
            var response = await repo.ViewLmsLaonApplicationCancellationDetails(data);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });


        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application-cancellation-approval")]
        public async Task<HttpResponseMessage> GoForLoanApplicationCancellationApproval([FromBody] LoanApplicationViewModel data)
        {
            data.userBranchId = (short)token.GetBranchId;
            data.companyId = token.GetCompanyId;
            data.createdBy = token.GetStaffId;
            var response = await repo.GoForLoanApplicationCancellationApproval(data);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            
            
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("lms-loan-application-cancellation-approval")]
        public async Task<HttpResponseMessage> GoForLmsLoanApplicationCancellationApproval([FromBody] LoanReviewApplicationViewModel data)
        {
            data.userBranchId = (short)token.GetBranchId;
            data.companyId = token.GetCompanyId;
            data.createdBy = token.GetStaffId;
            var response = await repo.GoForLmsLoanApplicationCancellationApproval(data);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });


        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("transaction/dynamics/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetTransactionDynamics(int loanApplicationId)
        {
            var response = await repo.GetTrnasactionDynamics(loanApplicationId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            
           
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("transaction/lms-dynamics/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetLMSTransactionDynamics(int loanApplicationId)
        {
            var response = await repo.GetTrnasactionDynamics(loanApplicationId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
           
           
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan/condition-precident/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetConditionPrecidents(int loanApplicationId)
        {
            var response = await repo.GetConditionPrecidents(loanApplicationId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            
            
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan/lms-condition-precident/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetLMSConditionPrecidents(int loanApplicationId)
        {
            var response = await repo.GetLMSConditionPrecidents(loanApplicationId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            
           
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application-detail-suggestion")]
        public async Task<HttpResponseMessage> updateSuggestionsLoanApplicationdetail([FromBody] LoanApplicationDetailViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;  //FinTrakBankingContext
            entity.userBranchId = (short)token.GetBranchId;

            var response =await repo.updateSuggestionsLoanApplicationdetail(entity);
            if (response == true)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Suggestions updated successfully" });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("crms-funding-source")]
        public async Task<HttpResponseMessage> GetAllCRMSFundingSource()
        {
            var data = await repo.GetAllCRMSFundingSource();

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("crms-repayment-source")]
        public async Task<HttpResponseMessage> GetAllCRMSRepaymentSource()
        {
            var data = await repo.GetAllCRMSRepaymentSource();

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("crms-repayment-agreement/type")]
        public async Task<HttpResponseMessage> GetAllCRMSRepaymentAgreementType()
        {
            var response = await repo.GetAllCRMSRepaymentAgreementType();

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
           
            
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-syndication-type")]
        public async Task<HttpResponseMessage> GetAllSyndicationType()
        {
            var response = await repo.GetAllSyndicationType();

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            
            
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-details/reference/{reference}")]
        public async Task<HttpResponseMessage> GetLoanApplicationDetailsByReference(string reference)
        {
            var data =await repo.GetLoanApplicationDetailsByReference(reference, token.GetCompanyId);

            var id = 0;
            foreach (var d in data)
            {
                id = d.loanApplicationId;
                break;
            }
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                success = true,
                result = data,
                count = data.Count(),
                loanApplicationId = id
            });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-exceptional-loans-for-approval")]
        public HttpResponseMessage GetExceptionalLoansForApproval()
        {
            var data = repo.GetExceptionalLoansForApproval(token.GetStaffId);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost]
        [Route("exceptional-loan/forward-for-approval")]
        public async Task<HttpResponseMessage> GoForApprovalExceptionalLoan([FromBody] ExceptionalLoanViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.createdBy = token.GetStaffId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;

            WorkflowResponse response = await repo.GoForApprovalExceptionalLoan(entity);

            if (response.responseMessage != "") {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, $"{TranslateHelper.get("EXCEPTIONAL LOAN")} - {response.responseMessage}") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, TranslateHelper.get("EXCEPTIONAL LOAN")) });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("application-detail-fields/{id}")]
        public async Task<HttpResponseMessage> GetLoanApplicationDetailFields(int id)
        {
            var data = await repo.GetLoanApplicationDetailFields(id);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("application-detail/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationDetailsById(int loanApplicationId)
        {
            var data = await repo.GetLoanApplicationDetailsById(loanApplicationId, token.GetCompanyId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }
        //[HttpGet]
        //[ClaimsAuthorization]
        //[Route("application-detail-lms/{loanApplicationId}")]
        //public HttpResponseMessage GetLmsLoanApplicationDetailsById(int loanApplicationId)
        //{
        //    var data = repo.GetLmsLoanApplicationDetailsById(loanApplicationId, token.GetCompanyId);

        //    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        //}


        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-tags/{id}")]
        public HttpResponseMessage GetLoanApplicationTags(int id)
        {
            LoanApplicationTagsViewModel response = repo.GetLoanApplicationTags(id);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-tags-lms/{id}")]
        public async Task<HttpResponseMessage> GetLoanApplicationTagsLMS(int id)
        {
            LoanApplicationTagsLMSViewModel response =await repo.GetLoanApplicationTagsLMS(id);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("revised-process-flow-by-product-class/{productClassId}/{productId}/{productTypeId}")]
        public async Task<HttpResponseMessage> getFacilityApplicationRevisedProcessFlowByProductClassId(short productClassId, short productId, short productTypeId)
        {
            var response = await repo.getFacilityApplicationRevisedProcessFlowByProductClassId(productClassId, productId, productTypeId);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("cash-collaterized-process-flow")]
        public async Task<HttpResponseMessage> getCashCollaterizedProcessFlowBy()
        {
            var response = await repo.getCashCollaterizedProcessFlowBy();
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("revised-process-flow")]
        public async Task<HttpResponseMessage> getFacilityApplicationRevisedProcessFlow()
        {
            var response = await repo.getFacilityApplicationRevisedProcessFlow();
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }

        //RevisedProcessFlowModel
        [HttpPut]
        [ClaimsAuthorization]
        [Route("loan-application-tags/{id}")]
        public HttpResponseMessage UpdateLoanApplicationTags([FromBody] LoanApplicationTagsViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response = repo.UpdateLoanApplicationTags(model, id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("loan-application-tags-lms/{id}")]
        public async Task<HttpResponseMessage> UpdateLoanApplicationTagsLMS([FromBody] LoanApplicationTagsLMSViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response =await repo.UpdateLoanApplicationTagsLMS(model, id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("facility-by-loanApplicationId/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetFacilityByApplicationId(int loanApplicationId)
        {
            var response = await repo.GetFacilityByApplicationId(loanApplicationId);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("delete-failedrac-loan-application/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> DeleteLoanApplicationThatFailedRAC(int loanApplicationDetailId)
        {
            var response =await repo.DeleteLoanApplicationThatFailedRAC(loanApplicationDetailId, token.GetStaffId);
            if (!response) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-flowchange/{loanApplicationId}")]
        public async Task<HttpResponseMessage> LoanApplicationFlowChange(int loanApplicationId)
        {
            var response =await repo.LoanApplicationFlowChange(loanApplicationId);
            if (!response) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-flow-change")]
        public async Task<HttpResponseMessage> LoanApplicationFlowChange()
        {          
                 var response =await repo.GetLoanApplicationFlowChange();

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-flow-change/{id}")]
        public async Task<HttpResponseMessage> GetLoanApplicationFlowChange(int id)
        {
            LoanApplicationFlowChangeViewModel response =await repo.GetLoanAppicationFlowChange(id);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }

       


        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-application-flow-change")]
        public async Task<HttpResponseMessage> AddLoanApplicationFlowChange([FromBody] LoanApplicationFlowChangeViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress; 
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId; 
            model.companyId = token.GetCompanyId;
            var response =await repo.AddLoanApplicationFlowChange(model);
            if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("loan-application-flow-change/{id}")]
        public async Task<HttpResponseMessage> UpdateLoanApplicationFlowChange([FromUri] int id, [FromBody] LoanApplicationFlowChangeViewModel model)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response =await repo.UpdateLoanApplicationFlowChange(model, id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("loan-application-flow-change/{id}")]
        public async Task<HttpResponseMessage> DeleteLoanApplicationFlowChange(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response =await repo.DeleteLoanApplicationFlowChange(id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("multiple-invoice")]
        public async Task<HttpResponseMessage> GetBulkLoanInvoice()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, TranslateHelper.get("Unsupported media type"));
            }

            MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
            Task.Factory
                .StartNew(() => provider = Request.Content.ReadAsMultipartAsync(provider).Result,
                    CancellationToken.None,
                    TaskCreationOptions.LongRunning, // guarantees separate thread
                    TaskScheduler.Default)
                .Wait();

            //int uploadType;
            //if (!Int32.TryParse(provider.FormData["documentTypeId"], out uploadType))
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest, "File Type is invalid.");
            //}


            byte[] pass = Convert.FromBase64String(provider.FormData["loginStaffPassCode"]);
            string password = Encoding.UTF8.GetString(pass);

            var entity = new UserInfo
            {
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                BranchId = (short)token.GetBranchId,
            };

            if (!provider.FileStreams.Any())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("No file uploaded"));
            }




            var file = provider.Contents.FirstOrDefault();
            var buffer = await file.ReadAsByteArrayAsync();

            var data = repo.GetBulkLoanInvoice(buffer, entity);

            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("bulk invoice data was successfully uploaded") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Error uploading bulk invoice data") });
        }

        #region LIEN

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lien")]
        public async Task<HttpResponseMessage> GetApplicationDetailLien()
        {
            IEnumerable<LoanApplicationLienViewModel> response =await repo.GetApplicationDetailLien();
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lien-id/{id}")]
        public async Task<HttpResponseMessage> GetApplicationDetailLien(int id)
        {
            LoanApplicationLienViewModel response =await repo.GetApplicationDetailLien(id);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lien-applicationDetailId/{applicationDetailId}")]
        public async Task<HttpResponseMessage> GetLienByApplicationDetailId(int applicationDetailId)
        {
            IEnumerable<LoanApplicationLienViewModel> response =await repo.GetLienByApplicationDetailId(applicationDetailId);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lien-collateralId/{collateralId}")]
        public async Task<HttpResponseMessage> GetLienByCollateralId(int collateralId)
        {
            IEnumerable<LoanApplicationLienViewModel> response =await repo.GetLienByCollateralId(collateralId);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lien-accountNo/{accountNo}")]
        public async Task<HttpResponseMessage> GetApplicationDetailLienByAccountNo(string accountNo)
        {
            IEnumerable<LoanApplicationLienViewModel> response =await repo.GetApplicationDetailLienByAccountNo(accountNo);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lien")]
        public async Task<HttpResponseMessage> AddLoanApplicationDetailLien([FromBody] LoanApplicationLienViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            var response =await repo.AddLoanApplicationDetailLien(model);
            if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Lien has been proposed successfully" });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("lien/{id}")]
        public async Task<HttpResponseMessage> UpdateLoanApplicationDetailLien([FromUri] int id, [FromBody] LoanApplicationLienViewModel model)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response =await repo.UpdateLoanApplicationDetailLien(model, id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1 });
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("lien/{id}")]
        public async Task<HttpResponseMessage> DeleteLoanApplicationDetailLien(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            bool response =await repo.DeleteLoanApplicationDetailLien(id, user);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = "Lien has been unproposed successfully" });
        }
        #endregion LIEN

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-review-type")]
        public async Task<HttpResponseMessage> GetAllLoanDetailReviewTypes()
        {
            IEnumerable<LoanDetailReviewTypeViewModel> response =await repo.GetAllLoanDetailReviewTypes();
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("approved-trade-cycle")]
        public async Task<HttpResponseMessage> GetAllApprovedTradeCycles()
        {
            IEnumerable<ApprovedTradeCycleViewModel> response =await repo.GetAllApprovedTradeCycles();
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }
    }
}