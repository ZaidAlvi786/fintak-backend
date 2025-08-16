using FintrakBanking.APICore.JWTAuth;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FintrakBanking.APICore.core;
using System.Web;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.Common.Enum;
using FintrakBanking.ViewModels.WorkFlow;
using FintrakBanking.Interfaces.Customer;
using System.Collections.Generic;
using System.Threading.Tasks;
using FintrakBanking.ViewModels.Report;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common.Extensions;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels;
using System.Globalization;
using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels.Reports;
using FintrakBanking.Common;
using System.Threading;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/loan")]
    public class LoanController : ApiControllerBase
    {
        private ILoanRepository repo;
        private ICustomerCollateralRepository repoCollateral;
        private ICustomerRepository repoCustomer;
        private ILoanScheduleRepository scheduleRepo;
        private ILoanOperationsRepository loanoperations;
        private IProductRepository productRepo;
        private TokenDecryptionHelper token = new TokenDecryptionHelper();
        private ExportDataTableToExcel export = new ExportDataTableToExcel();


        //private IHostingEnvironment _hostingEnvironment;
        //private IHostingEnvironment _hostingEnvironment;
        //TokenDecryptionHelper token = new TokenDecryptionHelper();
        public LoanController(ILoanRepository _repo,
                              ICustomerCollateralRepository _repoCollateral,
                              ICustomerRepository _repoCustomer,
                               ILoanScheduleRepository _scheduleRepo,
                               IProductRepository _productRepo, ILoanOperationsRepository _loanoperations)
        {
            this.repo = _repo;
            this.repoCollateral = _repoCollateral;
            this.repoCustomer = _repoCustomer;
            this.scheduleRepo = _scheduleRepo;
            this.productRepo = _productRepo;
            this.loanoperations = _loanoperations;

            //this._hostingEnvironment = hostingEnvironment;
        }

        #region Loan


        [HttpPost]
        [ClaimsAuthorization]
        [Route("current-exposure/customer/{loanTypeId}")]
        public HttpResponseMessage GetCurrentCustomerExposure([FromBody] List<CustomerExposure> customer, int loanTypeId)
        {
            var data = repo.GetCurrentCustomerExposure(customer, loanTypeId, token.GetCompanyId);
            //if (!data.Any())
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            //}

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("current-camsol/customer/{loanTypeId}")]
        public async Task<HttpResponseMessage> GetCurrentCamsolByCustomer([FromBody] List<CustomerExposure> customer, int loanTypeId)
        {
            var data = await repo.GetCurrentCamsolByCustomer(customer, loanTypeId, token.GetCompanyId);
            //if (!data.Any())
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            //}
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("facility-summary/application/{applicationId}")]
        public async Task<HttpResponseMessage> GetApplicationFacilitySummary(int applicationId)
        {
            List<CurrentCustomerExposure> data = await repo.GetApplicationFacilitySummary(applicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }


        [HttpGet]
        [Route("running-loan/customer/{id}")]
        public HttpResponseMessage GetAllLoanTypes(int id)
        {
            var data = repo.RunningLoans(id, token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpGet]
        [Route("exchange-rate/{fromCode}/{toCode}/{rateCode}")]
        public async Task<HttpResponseMessage> GetExchangeRate(string fromCode ,string toCode,string rateCode)
        {
            var data = await repo.GetExchangeRate(fromCode, toCode, rateCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("customer-accounts/balance/{casaAccountId}")]
        public async Task<HttpResponseMessage> GetCASABalanceById(int casaAccountId)
        {
            var data = await repo.GetCASABalanceById(casaAccountId,token.GetCompanyId);
            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Account Number do not exist") });
            }
                
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("revolving-types")]
        public async Task<HttpResponseMessage> GetRevolvingLoanTypes()
        {
            var data = await repo.GetRevolvingLoanTypes();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-transaction-dynamics/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetLoanTransactionDynamics(int loanApplicationDetailId)
        {
            var data = await repo.GetLoanTransactionDynamics(loanApplicationDetailId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("temporary-overdraft-revolving-types")]
        public async Task<HttpResponseMessage> GetTemporaryOverdrafts()
        {
            var data = await repo.GetTemporaryOverdrafts();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-types")]
        public async Task<HttpResponseMessage> GetLoanApplicationTypes()
        {
            var data = await repo.GetLoanApplicationTypes();
            if (!data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-detail-covenant/{applicationDetailId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationDetailCovenantById(int applicationDetailId)
        {
            var data = await repo.GetLoanApplicationDetailCovenantById(applicationDetailId);
            if (!data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-product-fees/booking-request/{loanBookingRequestId}")]
        public async Task<HttpResponseMessage> GetLoanProductFees(int loanBookingRequestId)
        {
            var response = await repo.GetLoanProductFees(loanBookingRequestId);
            if (response == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

       

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-schedule-category")]
        public async Task<HttpResponseMessage> GetAllLoanScheduleCategory()
        {
            var data = await scheduleRepo.GetAllLoanScheduleCategory();
            //if (!data.Any())
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            //}

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-schedule-types")]
        public async Task<HttpResponseMessage> GetAllLoanScheduleType()
        {
            var data =await  scheduleRepo.GetAllLoanScheduleType();
            if (!data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-schedule-types/{productTypeId}")]
        public async Task<HttpResponseMessage> GetAllLoanScheduleType(short? productTypeId)
        {
            var data = await scheduleRepo.GetAllLoanScheduleType(productTypeId);
            if (!data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-schedule-types/category/{categoryId}")]
        public async Task<HttpResponseMessage> GetLoanScheduleTypeByCategory(short categoryId)
        {
            var data =await scheduleRepo.GetLoanScheduleTypeByCategory(categoryId);
            if (!data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-booking")]
        public async Task<HttpResponseMessage> AddLoanBooking([FromBody] LoanViewModel entity)
        {
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;



            var data =await repo.AddLoanBooking(entity);
            if (data != "")
            {
                if (entity.productTypeId == (short)LoanProductTypeEnum.CommercialLoan
                    || entity.productTypeId == (short)LoanProductTypeEnum.TermLoan
                    || entity.productTypeId == (short)LoanProductTypeEnum.SelfLiquidating
                    || entity.productTypeId == (short)LoanProductTypeEnum.ForeignXRevolving
                    || entity.productTypeId == (short)LoanProductTypeEnum.SyndicatedTermLoan)
                {
                    if (entity.isInEditMode)
                        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = $"{TranslateHelper.get("Loan Loan with Account Number")}: '{ data}' {TranslateHelper.get("was successfully modified")}" });
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Loan booking was successfully initiated and is waiting authorization") + "\r\n" + TranslateHelper.get("Loan Account Number") +"  "+ data });
                }

                if (entity.productTypeId == (short)LoanProductTypeEnum.RevolvingLoan)
                {
                    if (entity.isInEditMode)
                        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Overdaft with Account Number: ") +  data +  " " + TranslateHelper.get("was successfully modified") });
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Revolving facility booking was successfully initiated and is awaiting authorization") + " .\r\n "+ TranslateHelper.get("Facility Account Number") + ": " + data });
                }

                if (entity.productTypeId == (short)LoanProductTypeEnum.ContingentLiability)
                {
                    if (entity.isInEditMode)
                        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = $"{TranslateHelper.get("contigent facility with Account Number")}: { data} {TranslateHelper.get("was successfully modified")}" });
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Contingent facility booking was successfully initiated and is awaiting authorization") + ".\r\n" + TranslateHelper.get("Facility Account Number") +": " + data });
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("add-existing-loan")]
        public async Task<HttpResponseMessage> AddExistingLoan([FromBody] LoanViewModel entity)
        {
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.customerSensitivityLevelId = 1;

            var data =await repo.AddExistingLoan(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = entity.productTypeName + " " + TranslateHelper.get("successfully imported") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error importing this record") });

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("maintain-facility-line")]
        public HttpResponseMessage UpdateFacilityLineStatus([FromBody] LoanViewModel entity)
        {
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;



            var data = repo.UpdateFacilityLineStatus(entity);
            if (data)
            {
              return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Customer's line facility successfully maintained") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Line could not be maintained. An error occured") });
            
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-customer-accounts/{customerId}/application-detail/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetLoanCustomerAccounts(int customerId, int loanApplicationDetailId)
        {
            var data =await repo.GetLoanCustomerAccounts(customerId, loanApplicationDetailId);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-tranches/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetLoanByApplicationDetailId(int loanApplicationDetailId)
        {
            var data =await repo.GetLoanByApplicationDetailId(loanApplicationDetailId);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("customer-lines/{customerId}")]
        public async Task<HttpResponseMessage> GetCustomerLines(int customerId)
        {
            var data =await repo.GetCustomerLines(customerId);
            if (!data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
        }

        [HttpGet]
        [Route("loan-tranche-history/{loanReferenceNumber}")]
        public async Task<HttpResponseMessage> GetLoanHistoryByLoanAccountNumber(string loanReferenceNumber)
        {
            var data =await repo.GetLoanHistoryByLoanAccountNumber(loanReferenceNumber);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpGet]
        [Route("loan-request/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetLoanRequestsByApplicationDetailId(int loanApplicationDetailId)
        {
            var data =await repo.GetLoanRequestsByApplicationDetailId(loanApplicationDetailId);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }
        
        [HttpPost]
        [ClaimsAuthorization]
        [Route("gaurantor/product-type/{productTypeId}/application/{applicationReferenceNumber}")]
        public HttpResponseMessage AddLoanGuarantor([FromBody] LoanGuarantorViewModel entity, short productTypeId, int applicationReferenceNumber)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();

            entity.userBranchId = (short)token.GetBranchId;
            // entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;

            var data = false; //repo.AddLoanGuarantor(entity, productTypeId, applicationReferenceNumber);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The Loan Gaurantor successful added") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error saving gaurantor") });
        }


        [HttpGet]
        [Route("appraisal-loan-details-updates/{appraisalMemorandumId}")]
        public async Task<HttpResponseMessage> GetAppraisalMemorandumLoanUpdates(int appraisalMemorandumId)
        {
            var data =await repo.GetAppraisalMemorandumLoanUpdates(appraisalMemorandumId);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }



        [HttpGet]
        [Route("monitoring-trigger")]
        public async Task<HttpResponseMessage> GetLoanMonitoringTrigger()
        {
            var data =await repo.GetLoanMonitoringTrigger();

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpGet]
        [Route("loan-status")]
        public async Task<HttpResponseMessage> GetLoanStatus()
        {
            var data =await repo.GetLoanStatus(token.GetCompanyId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpGet]
        [Route("monitoring-trigger/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetLoanMonitoringTriggerByLoanApplicationDetailId(int loanApplicationDetailId)
        {
            var data =await repo.GetLoanMonitoringTriggerByLoanApplicationDetailId(loanApplicationDetailId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpGet]
        [Route("loan-application-collateral/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationCollateralsByApplicationId(int loanApplicationId)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data =await repo.GetLoanApplicationCollateralsByApplicationId(loanApplicationId);

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        

        [HttpGet]
        [Route("number-of-installments/tenor-mode/{tenorModeId}/frequency-type/{frequencyTypeId}/tenor/{tenor}")]
        public async Task<HttpResponseMessage> GetNumberOfInstallments(short tenorModeId, short frequencyTypeId, int tenor)
        {
            var data =await scheduleRepo.CalculateNumberOfInstallments((TenorModeEnum)tenorModeId, frequencyTypeId, tenor);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpGet]
        [Route("{loanId}")]
        public async Task<HttpResponseMessage> GetLoan(int loanId)
        {
            var data =await repo.GetLoan(loanId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }


        [HttpGet]
        [Route("loan-booking/approvers/{operationId}")]
        public async Task<HttpResponseMessage> GetLoanOperationApprovers(int operationId)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data =await repo.GetLoanOperationApprovers(operationId, token.GetCompanyId);

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }


        [HttpGet]
        [Route("loan-booking/term/awaiting-approval")]
        public async Task<HttpResponseMessage> GetLoanBookingAwaitingApproval()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data =await repo.GetLoanFacilityBookingAwaitingApproval(token.GetStaffId, token.GetCompanyId);

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpGet]
        [Route("loan-booking/verification/awaiting-approval")]
        public async Task<HttpResponseMessage> GetBookedLoanApplicationForBookingVerification()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data =await repo.GetBookedLoanApplicationForBookingVerification(token.GetStaffId, token.GetCompanyId);

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpGet]
        [Route("loan-booking/verification/awaiting-approval-param/{searchString}")]
        public async Task<HttpResponseMessage> getBookedLoanApplicationsForVerificationAwaitingApprovalParam(string searchString)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data =await repo.GetBookedLoanApplicationForBookingVerificationParam(token.GetStaffId, token.GetCompanyId, searchString);

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpGet]
        [Route("facility-line-maintenance-awaiting-approval")]
        public async Task<HttpResponseMessage> GetFacilityLineAwaitingApproval()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data =await repo.GetFacilityLineAwaitingMaintenanceApproval(token.GetStaffId, token.GetCompanyId);

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpGet]
        [Route("loans-disbursed")]
        public async Task<HttpResponseMessage> GetdisbursedLoansApplicationDetails()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data =await repo.GetdisbursedLoansApplicationDetails(token.GetStaffId, token.GetCompanyId);

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpGet]
        [Route("loan-booking/revolving/awaiting-approval")]
        public async Task<HttpResponseMessage> GetRevolvingFacilityBookingAwaitingApproval()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data =await repo.GetRevolvingFacilityBookingAwaitingApproval(token.GetStaffId, token.GetCompanyId);

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found"), });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpGet]
        [Route("loan-booking/contingent/awaiting-approval")]
        public async Task<HttpResponseMessage> GetContingentFacilityBookingAwaitingApproval()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data =await repo.GetContingentFacilityBookingAwaitingApproval(token.GetStaffId, token.GetCompanyId);

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpGet]
        [Route("commercial-loans")]
        public async Task<HttpResponseMessage> GetLoanCommercialLoans()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data =await repo.GetLoanCommercialLoans(token.GetCompanyId);

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }


        [HttpGet]
        [Route("full-and-final-status")]
        public async Task<HttpResponseMessage> GetFullAndFinalStatus()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var data =await repo.GetFullAndFinalStatus();

            if (data.Any() == false)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-booking/approval/{loanBookingRequestId}/{isManual}")]
        public async Task<HttpResponseMessage> ApproveLoanBooking([FromBody] ApprovalViewModel model, int loanBookingRequestId, bool isManual)
        {
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            model.BranchId = (short)token.GetBranchId;
            model.staffId = token.GetStaffId;

            var responseId =await repo.GoForApproval(model, loanBookingRequestId, isManual);
            var dynamicMessage = string.Empty;
            if (responseId == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Operation successful, sent to Credit Documentation for filling") });
            }
            else if (responseId == 2)
            {
                dynamicMessage = TranslateHelper.get("Loan has been successfully disbursed, sent to Credit Documentation for filling");
                if (model.operationId == (short)OperationsEnum.RevolvingLoanBooking)
                    dynamicMessage = TranslateHelper.get("Overdraft facility grant successfully committed and forward for Filling");
                if (model.operationId == (short)OperationsEnum.ContigentLoanBooking)
                    dynamicMessage = TranslateHelper.get("Contingent Liability has been committed successfully and forward for Filling");
                if (model.operationId == (short)OperationsEnum.TermLoanBooking)
                    dynamicMessage = TranslateHelper.get("Loan has been successfully disbursed and forwarded for Filling");
                return Request.CreateResponse(HttpStatusCode.OK,
                                        new { success = true, message = dynamicMessage });
            }
            else if (responseId == 3)
            {
                dynamicMessage = TranslateHelper.get("Loan disapproval was successful");
                if (model.operationId == (short)OperationsEnum.RevolvingLoanBooking)
                    dynamicMessage = TranslateHelper.get("Overdraft facility grant disapproved");
                if (model.operationId == (short)OperationsEnum.ContigentLoanBooking)
                    dynamicMessage = TranslateHelper.get("Contingent Liability disapproved");
                if (model.operationId == (short)OperationsEnum.TermLoanBooking)
                    dynamicMessage = TranslateHelper.get("Loan disapproval was successful");

                return Request.CreateResponse(HttpStatusCode.OK,
                                        new { success = true, message = dynamicMessage });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("Operation unsuccessful, an error occured while saving changes") });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-booking/verification/{loanBookingRequestId}")]
        public async Task<HttpResponseMessage> ApproveLoanBookingVerification([FromBody] ApprovalViewModel model, int loanBookingRequestId)
        {
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            model.BranchId = (short)token.GetBranchId;
            model.staffId = token.GetStaffId;

            var responseId =await repo.GoForApproval(model, loanBookingRequestId);
            var dynamicMessage = string.Empty;
            if (responseId == 1)
            {
                dynamicMessage = TranslateHelper.get("Facility booking has been successfully authorized");
                if (model.operationId == (short)OperationsEnum.RevolvingLoanBooking)
                    dynamicMessage = TranslateHelper.get("Overdraft booking successfully authorized");
                if (model.operationId == (short)OperationsEnum.ContigentLoanBooking)
                    dynamicMessage = TranslateHelper.get("Contingent Liability booking successfully authorized");
                if (model.operationId == (short)OperationsEnum.TermLoanBooking)
                    dynamicMessage = TranslateHelper.get("Loan booking has been successfully authorized");
                return Request.CreateResponse(HttpStatusCode.OK,
                                        new { success = true, message = dynamicMessage });
            }
            else if (responseId == 2)
            {
                dynamicMessage = TranslateHelper.get("Loan booking has been successfully completed");
                if (model.operationId == (short)OperationsEnum.RevolvingLoanBooking)
                    dynamicMessage = TranslateHelper.get("Overdraft booking successfully completed");
                if (model.operationId == (short)OperationsEnum.ContigentLoanBooking)
                    dynamicMessage = TranslateHelper.get("Contingent Liability successfully released");
                if (model.operationId == (short)OperationsEnum.TermLoanBooking)
                    dynamicMessage = TranslateHelper.get("Loan booking has been successfully completed");
                return Request.CreateResponse(HttpStatusCode.OK,
                                        new { success = true, message = dynamicMessage });
            }
            else if (responseId == 3)
            {
                dynamicMessage = TranslateHelper.get("Loan booking authorization declined");
                if (model.operationId == (short)OperationsEnum.RevolvingLoanBooking)
                    dynamicMessage = TranslateHelper.get("Overdraft facility booking authorization declined");
                if (model.operationId == (short)OperationsEnum.ContigentLoanBooking)
                    dynamicMessage = TranslateHelper.get("Contingent Liability booking authorization declined");
                if (model.operationId == (short)OperationsEnum.TermLoanBooking)
                    dynamicMessage = TranslateHelper.get("Loan booking authorization declined");

                return Request.CreateResponse(HttpStatusCode.OK,
                                        new { success = true, message = dynamicMessage });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("Operation unsuccessful, an error occured while saving changes") });
            }
        }

        [HttpGet]
        [Route("loan-facility-awaiting-booking/{searchString}")]
        public async Task<HttpResponseMessage> getLoanFacilitiesAwaitingApprovalByParam(string searchString)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var response =await repo.getLoanFacilitiesAwaitingApprovalByParam(token.GetCompanyId, token.GetStaffId, searchString);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });

        }

        //[HttpPost]
        //[ClaimsAuthorization]
        //[Route("loan-request/approval/{loanBookingRequestId}")]
        //public HttpResponseMessage ApproveInitiatedLoanBooking([FromBody] ApprovalViewModel model, int loanBookingRequestId)
        //{
        //    model.applicationUrl = HttpContext.Current.Request.Path;
        //    model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
        //    model.createdBy = token.GetStaffId;
        //    model.companyId = token.GetCompanyId;
        //    model.BranchId = (short)token.GetBranchId;
        //    model.staffId = token.GetStaffId;

        //    var responseId = repo.GoForBookingRequestApproval(model, loanBookingRequestId);

        //    //try
        //    //{
        //    //    model.applicationUrl = HttpContext.Current.Request.Path;
        //    //    model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
        //    //    model.createdBy = token.GetStaffId;
        //    //    model.companyId = token.GetCompanyId;
        //    //    model.BranchId = (short) token.GetBranchId;
        //    //    model.staffId = token.GetStaffId;

        //    //    WorkflowResponse response = repo.GoForBookingRequestApproval(model, loanBookingRequestId);

        //    //    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Operation successful, request has been routed to the next approving office" });
        //    //}
        //    //catch (SecureException ex)
        //    //{
        //    //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
        //    //}

        //    if (responseId == 1)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK,
        //            new { success = true, message = "Operation successful, request has been routed to the next approving office" });
        //    }
        //    else if (responseId == 0)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK,
        //                                new { success = true, message = "Loan request has been successfully approved" });
        //    }
        //    else if (responseId == 3)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK,
        //                                new { success = true, message = "Loan request was successfully disapproved" });
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK,
        //            new { success = false, message = "Operation unsuccessful, an error occured while saving changes. " });
        //    }
        //}

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-booking/fee-override/approval")]
        public async Task<HttpResponseMessage> ApproveLoaFeeOverride([FromBody] ApprovalViewModel model)
        {
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            model.BranchId = (short)token.GetBranchId;
            model.staffId = token.GetStaffId;

            var data =await repo.GoForFeeOverrideApproval(model);

            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                                        new { success = true, message = TranslateHelper.get("Loan fee override has been approved successfully") });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Operation successful, request has been routed to the next approving office") });
            }
        }
        
        [HttpGet]
        [Route("customer/{customerId}")]
        public async Task<HttpResponseMessage> GetCustomerLoans(int customerId)
        {
            var data =await repo.GetLoanByCustomer(customerId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }


        [HttpGet]
        [Route("customer-loan-booking-override/{customerCode}")]
        public async Task<HttpResponseMessage> getBookingOverride(string customerCode)
        {
            var data =await repo.getBookingOverride(customerCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }


        [HttpGet]
        [Route("existing-loans/{applicationId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationExistingLoans(int applicationId)
        {
            List<LoanViewModel> data =await repo.GetLoanApplicationExistingLoans(applicationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }

        [HttpGet]
        [Route("customer-group/{customerGroupId}")]
        public async Task<HttpResponseMessage> GetCustomerGroupLoans(int customerGroupId)
        {
            var data =await repo.GetLoanByCustomerGroup(customerGroupId);

            if (!data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = false,
                    result = data.ToList(),
                    message = TranslateHelper.get("No Record Found")
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                success = true,
                result = data.ToList(),
                count = data.Count()
            });
        }

        [HttpGet]
        [Route("find/{searchCriteria}")]
        public async Task<HttpResponseMessage> FindLoan(string searchCriteria)
        {
            var data =await repo.FindLoan(searchCriteria, token.GetCompanyId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-search")]
        public HttpResponseMessage SearchLoan([FromBody] LoanSearchViewModel searchModel)
        {
            var data = repo.LoanSearch(token.GetCompanyId, searchModel);
            //if (!data.Any())
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            //}

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-collaterals/{loanId}/{loanSystemTypeId}")]
        public HttpResponseMessage GetLoanCollateral(int loanId, int loanSystemTypeId)
        {
            var data = repo.GetLoanCollateral(loanId, loanSystemTypeId);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("foreign-loan-naration/{loanId}")]
        public async Task<HttpResponseMessage> GetForeignLoanBeneficiaryNaration(int loanId)
        {
            var data =await repo.GetForeignLoanBeneficiaryNaration(loanId);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-monitoring-triggers/{loanId}/{loanSystemTypeId}")]
        public async Task<HttpResponseMessage> GetLoanMonitoringTriggers(int loanId, int loanSystemTypeId)
        {
            var data =await repo.GetLoanMonitoringTriggers(loanId, loanSystemTypeId);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data });
        }

        [HttpGet]
        [Route("first-pay-date/effective-date/{effectiveDate}/frequency-type/{frequencyTypeId}")]
        public async Task<HttpResponseMessage> GetFirstPayDate(DateTime effectiveDate, short frequencyTypeId)
        {
            var data =await scheduleRepo.CalculateFirstPayDate(effectiveDate, frequencyTypeId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("periodic-schedule")]
        public HttpResponseMessage GeneratePeriodicLoanSchedule([FromBody] LoanPaymentScheduleInputViewModel loanInput)
        {
            var data = scheduleRepo.GeneratePeriodicLoanSchedule(loanInput);

            if (!data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("periodic-prepayment-schedule")]
        public async Task<HttpResponseMessage> GeneratePeriodicPrepaymentLoanSchedule([FromBody] LoanPaymentScheduleInputViewModel loanInput)
        {
            var data = loanoperations.GeneratePrepaymentSchedule(loanInput);

            if (!data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("daily-schedule")]
        public HttpResponseMessage GenerateDailyLoanSchedule([FromBody] LoanPaymentScheduleInputViewModel loanInput)
        {
            var data = scheduleRepo.GenerateDailyLoanSchedule(loanInput);

            if (!data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [Route("customer-collateral/")]
        public async Task<HttpResponseMessage> SearchCustomerCollateral(string searchQuery)
        {
            var data =await repo.SearchCustomerCollateral(token.GetCompanyId, searchQuery);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data });
        }

        [HttpGet]
        [Route("customer-collateral/search")]
        public async Task<HttpResponseMessage> SearchCustomer(string q)
        {
            var data =await repo.SearchCustomerCollateral(token.GetCompanyId, q);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList() });
        }

        [HttpGet]
        [Route("detail/{param}")]
        public async Task<HttpResponseMessage> GetBookedLoanDetails(string param)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();

            var data =await repo.GetBookedLoanDetailsWithParameters(token.GetCompanyId, param);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-schedule")]
        public async Task<HttpResponseMessage> GetBookedLoanDetailsForReport(ReportSearchParamViewModel param)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();

            var data =await repo.GetBookedLoanDetails(token.GetCompanyId, param);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpGet]
        [Route("details/customer/{customerCode}")]
        public async Task<HttpResponseMessage> GetBookedLoanDetail(string customerCode)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();

            var data =await repo.GetBookedLoanDetailsByCustomerCode(customerCode, token.GetCompanyId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }

        [HttpGet]
        [Route("details/reference-number/{loanReferenceNumber}")]
        public async Task<HttpResponseMessage> GetBookedLoanDetailsByLoanReferenceNumber(string loanReferenceNumber)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();

            var data =await repo.GetBookedLoanDetailsByLoanReferenceNumber(loanReferenceNumber, token.GetCompanyId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }



        [HttpPost]
        [ClaimsAuthorization]
        [Route("schedule/export")]
        public async Task<HttpResponseMessage> ExportScheduleToExcel([FromBody] LoanPaymentScheduleInputViewModel model)
        {
            var fileBytes =await scheduleRepo.GenerateLoanScheduleExport(model);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = fileBytes });

        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-booking-modification/refer-back")]
        public async Task<HttpResponseMessage> ReferBackBooking([FromBody] ApprovalViewModel entity)
        {
            entity.BranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.userIPAddress = Request.RequestUri.Host;
            entity.createdBy = token.GetStaffId;

            var response =await repo.ReferBackBooking(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = response.responseMessage });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("referred-booked-facility-record")]
        public async Task<HttpResponseMessage> GetReferedBookingFacilityRecordsById([FromBody] CamProcessedLoanViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = token.GetCompanyId;
            entity.staffId = token.GetStaffId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.userIPAddress = Request.RequestUri.Host;
            entity.createdBy = token.GetStaffId;

            var data =await repo.GetReferedBookingFacilityRecordsById(entity);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
        }


        #endregion Loan

        #region Frequency Type
        [HttpGet]
        [ClaimsAuthorization]
        [Route("limit-frequency-type")]
        public async Task<HttpResponseMessage> GetAllFrequencyType()
        {
            var response =await repo.GetAllFrequencyType();

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }
        #endregion

        #region (Loan Application Date) Pre - Loan booking

        //[HttpGet]
        //[Route("loan-application/adhoc-approval")]
        //public HttpResponseMessage getApplicationsToBeAdhocApprovedForInitiateBooking()
        //{
        //    TokenDecryptionHelper token = new TokenDecryptionHelper();
        //    try
        //    {
        //        var response = repo.getApplicationsToBeAdhocApprovedForInitiateBooking(token.GetCompanyId, token.GetStaffId, token.GetBranchId);
        //        if (!response.Any())
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        //    }
        //    catch (ConditionNotMetException ce)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
        //    }
        //    catch (BadLogicException be)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
        //    }
        //    catch (SecureException e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
        //    }
        //}


        [HttpGet]
        [Route("loan-application-details/{applicationDetailId}")]
        public async Task<HttpResponseMessage> GetLoanApplicationDetails(int applicationDetailId)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var response = await repo.GetLoanApplicationDetails(applicationDetailId, token.GetCompanyId);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }


        [HttpGet]
        [Route("availed-loan-applications/crms-code-ready")]
        public async Task<HttpResponseMessage> GetAvailedLoanApplicationsReadyForCrmsCode()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var response =await repo.GetAvailedLoanApplicationsReadyForCrmsCode(token.GetCompanyId, token.GetStaffId);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });

        }

        [HttpGet]
        [Route("loan-application-detail")]
        public HttpResponseMessage GetApprovedLoanApplicationsDetail()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var response = repo.GetAvailedLoanApplicationsReadyForBooking(token.GetCompanyId, token.GetStaffId);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });

        }


        [HttpGet]
        [Route("availed-loan-applications/booking-ready")]
        public HttpResponseMessage GetAvailedLoanApplicationsReadyForBooking()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var response = repo.GetAvailedLoanApplicationsReadyForBooking(token.GetCompanyId, token.GetStaffId);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });

        }

        [HttpGet]
        [Route("availed-contingent-facility-for-release")]
        public async Task<HttpResponseMessage> GetAvailedContingentFacilityBooking()
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var response =await repo.GetAvailedContingentFacilityBooking(token.GetCompanyId, token.GetStaffId);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });

        }

        [HttpGet]
        [Route("commercial-loans/application-detail/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetAvailedLoanApplicationsDueForInitiateBooking(int loanApplicationDetailId)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var response =await repo.GetCommercialLoanByApplicationDetailId(loanApplicationDetailId);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [Route("requested-loan-booking/{loanBookingRequestId}/application-detail/{applicationDetailId}")]
        public async Task<HttpResponseMessage> GetAvailedLoanApplicationDetailById(int applicationDetailId, int loanBookingRequestId)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var response =await repo.GetAvailedLoanApplicationDetailById(token.GetStaffId, token.GetCompanyId, applicationDetailId, loanBookingRequestId);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [Route("loan-customer-company-information/{customerId}")]
        public async Task<HttpResponseMessage> getLoanCustomerCompanyInformation(int customerId)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var response =await repo.getLoanCustomerCompanyInformation(customerId);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

  


        [HttpGet]
        [Route("loan-application/collateral/customer/{customerId}")]
        public HttpResponseMessage GetCollateralCustomer(int customerId)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            var response = repoCollateral.GetCustomerCollateral(customerId, null, token.GetCompanyId);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [Route("loan-application/charge-fee/{chargeFeeId}/product/{productId}")]
        public async Task<HttpResponseMessage> GetLoanProductChargeFee(int chargeFeeId, int productId)
        {
            var response =await repo.GetLoanProductChargeFee(chargeFeeId, productId);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [Route("loan-product-fees/{productId}")]
        public async Task<HttpResponseMessage> GetProductFees(int productId)
        {
            var response =await repo.GetProductFees(productId);
            if (!response.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        #endregion (Loan Application Date) Pre - Loan booking

        #region Workflow Tracker

        [HttpGet]
        [Route("work-flow-tracker/operation/{operationId}/target/{targetId}")]
        public async Task<HttpResponseMessage> GetApprovalTrailByOperationIdAndTargetId(int operationId, int targetId)
        {
            try
            {
                var data = await repo.GetApprovalTrailByOperationIdAndTargetId(operationId, targetId, token.GetCompanyId, token.GetStaffId);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, count = data.Count() });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }
        }

        #endregion Workflow Tracker

        #region Loan Disbursement 

        //IEnumerable<LoanDisbursementViewModel> GetAllLoanDisbursement(int loanId);
        //bool AddUpdateLoanDisbursement(LoanDisbursementViewModel entity);
        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-disbursement")]
        public HttpResponseMessage GetAllLoanDisbursement(int loanId)
        {
            try
            {
                var data = repo.GetAllLoanDisbursement(loanId);
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

        [HttpPost]
        [ClaimsAuthorization]
        [Route("loan-interest-rate-amount")]
        public HttpResponseMessage getDiscountedCPInterestAmount([FromBody] LoanViewModel entity)
        {
            var loanProductInfo = productRepo.GetProductById(entity.productId);
            var isDicounted = false;
                if(loanProductInfo != null) isDicounted = loanProductInfo.dealTypeId == (short)DealTypeEnum.Upfront ? true : false;

            var data = repo.getLoanInterestRateAmount(entity.principalAmount, entity.interestRate, entity.effectiveDate, entity.maturityDate,(DayCountConventionEnum)entity.scheduleDayCountConventionId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Operation successful" , result = data, isDicounted = isDicounted });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("employer-related-loan-data")]
        public HttpResponseMessage GetEmployerRelatedData(DateRange dateRange)
        {
            var token = new TokenDecryptionHelper();
            var data = repo.GetEmployerRelatedData(token.GetStaffId, token.GetCompanyId, dateRange);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-repricing-mode")]
        public HttpResponseMessage GetLoanRepricingModes()
        {
            try
            {
                var data = repo.GetLoanRepricingModes();
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

        //[HttpPost]
        //[ClaimsAuthorization]
        //[Route("loan-disbursement")]
        //public HttpResponseMessage AddUpdateLoanDisbursement([FromBody]LoanDisbursementViewModel entity)
        //{
        //    try
        //    {
        //        string createUpdate = "";
        //        if (entity.loanDisbursementId != 0 || entity.loanDisbursementId < 0)
        //        {
        //            createUpdate = "updated";
        //        }
        //        else
        //        {
        //            createUpdate = "created";
        //        }
        //        entity.userBranchId = (short)token.GetBranchId;
        //        entity.companyId = (short)token.GetCompanyId;
        //        entity.applicationUrl = HttpContext.Current.Request.Path;
        //        entity.createdBy = token.GetStaffId;
        //        entity.staffId = token.GetStaffId;

        //        var data = repo.AddUpdateLoanDisbursement(entity);
        //        if (data)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK,
        //                new { success = true, result = data, message = $"{TranslateHelper.get("The record has been")} {createUpdate} {TranslateHelper.get("successfully")}" });
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK,
        //           new { success = false, message = $"There was an error {createUpdate} this record" });
        //    }
        //    catch (SecureException e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK,
        //           new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
        //    }
        //}
        #endregion

        #region

        [HttpGet]
        [ClaimsAuthorization]
        [Route("completed-loan")]
        public async Task<HttpResponseMessage> GetCompletedLoan()
        {
            try
            {
                var data =await repo.GetCompletedLoans();
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
        [Route("completed-loan/search/{value}")]
        public async Task<HttpResponseMessage> GetCompletedLoan(string value)
        {
            try
            {
                var data =await repo.GetCompletedLoan(value);
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


        [HttpPost]
        [ClaimsAuthorization]
        [Route("completed-loan-status")]
        public async Task<HttpResponseMessage> GetChangeLoanStatusOfACompletedLoan([FromBody]int loanid)
        {
            try
            {
                var data =await repo.GetChangeLoanStatusOfACompletedLoan(loanid);
                if (data == false)
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
        #endregion




        //    [HttpPost]
        //    [ClaimsAuthorization]
        //    [Route("two-factor-auth-enabled-fee-override")]
        //    public HttpResponseMessage TwoFactorAuthenticationEnabledWithoutFeeOverride([FromBody]LoanViewModel entity)
        //    {
        //        try
        //        {
        //            TokenDecryptionHelper token = new TokenDecryptionHelper();

        //            entity.userBranchId = (short)token.GetBranchId;
        //            entity.applicationUrl = HttpContext.Current.Request.Path;
        //            entity.createdBy = token.GetStaffId;
        //            entity.companyId = token.GetCompanyId;

        //            var data = repo.TwoFactorAuthenticationEnabledWithoutFeeOverride(entity);
        //            if (!data)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK,
        //                   new { success = false, result = data, message = "" });
        //            }
        //            return Request.CreateResponse(HttpStatusCode.OK,
        //                   new { success = true, result = data });
        //        }
        //        catch (System.Exception ex)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK,
        //                  new { success = false, message = TranslateHelper.get(ex.Message) });
        //        }
        //    }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-balance/{loanId}")]
        public async Task<HttpResponseMessage> GetLoanBalances(int loanId)
        {
            var response =await repo.GetLoanBalances(loanId, token.GetCompanyId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("legal-contingent-code-validation/{legalContingentCode}/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> VerifyLegalContingentCode(string legalContingentCode, int loanApplicationDetailId)
        {
            var response =await repo.VerifyLegalContingentCode(legalContingentCode, loanApplicationDetailId);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data = response, message = TranslateHelper.get("Success") });
            }
            else
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Failed") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-loan-entries")]
        public async Task<HttpResponseMessage> saveBulkLoanDisbursementEntries([FromBody] List<multipleDisbursementOutputViewModel> models)
        {
            UserInfo user = new UserInfo();
            user.BranchId = (short)token.GetBranchId;
            user.applicationUrl = HttpContext.Current.Request.Path;
            user.createdBy = token.GetStaffId;
            user.companyId = token.GetCompanyId;

            var data =await repo.saveBulkLoanDisbursementEntries(models,user);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data = data, message = TranslateHelper.get("Bulk loan was successfully submitted for disbursement approval") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving bulk loan for disbursement approval was unsuccessfully") });

        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-insurance-policy-entries")]
        public async Task<HttpResponseMessage> saveBulkInsurancePolicyEntries([FromBody] List<MultipleInsuranceOutputViewModel> models)
        {
            UserInfo user = new UserInfo();
            user.BranchId = (short)token.GetBranchId;
            user.applicationUrl = HttpContext.Current.Request.Path;
            user.createdBy = token.GetStaffId;
            user.companyId = token.GetCompanyId;

            WorkflowResponse response =await repo.saveBulkInsurancePolicyEntries(models, user);
            
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data = response.responseMessage, message = response.responseMessage });
        }



        [HttpPost]
        [ClaimsAuthorization]
        [Route("multiple-disbursement")]
        public async Task<HttpResponseMessage> disburseMultipleLoans([FromBody] List<multipleDisbursementOutputViewModel> models)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            var data =await repo.startBulkLoanDisbursement(models, user);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
           
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-loan-recovery-assignment/{accreditedConsultant}/{expCompletionDate}/{source}/{assignmentType}")]
        public async Task<HttpResponseMessage> saveBulkLoanAssignmentToAgent(int accreditedConsultant, DateTime? expCompletionDate, string source, string assignmentType, [FromBody] List<GlobalExposureApplicationViewModel> models)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            WorkflowResponse data = await repo.saveBulkLoanAssignmentToAgent(models, accreditedConsultant, expCompletionDate, source, assignmentType, user);
            //var data =await repo.saveBulkLoanAssignmentToAgent(models, accreditedConsultant, expCompletionDate, source, assignmentType, user);

            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,

                    new { success = true, data = data, message = data.responseMessage });
                    //new { success = true, data = data, message = TranslateHelper.get("Bulk Recovery Successfully Saved") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving loan recovery assignment unsuccessfully") });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-loan-recovery-assignment-rem/{accreditedConsultant}/{expCompletionDate}/{source}/{assignmentType}")]
        public async Task<HttpResponseMessage> saveBulkLoanAssignmentToAgentRem(int accreditedConsultant, DateTime? expCompletionDate, string source, string assignmentType, [FromBody] List<GlobalExposureApplicationViewModel> models)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            var data =await repo.saveBulkLoanAssignmentToAgentRem(models, accreditedConsultant, expCompletionDate, source, assignmentType, user);

            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data = data, message = TranslateHelper.get("Bulk Recovery Successfully Saved") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving loan recovery assignment unsuccessfully") });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-loan-recovery-re-assignment")]
        public async Task<HttpResponseMessage> saveBulkLoanReAssignmentToAgent([FromBody] GlobalExposureApplicationViewModel model)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            WorkflowResponse data =await repo.saveBulkLoanReAssignmentToAgent(model, user);

            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data = data, message = data.responseMessage });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving loan recovery re-assignment unsuccessfully") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-loan-recovery-re-assignment-remedial")]
        public async Task<HttpResponseMessage> saveBulkLoanReAssignmentToAgentRem([FromBody] LoanRecoveryAssignmentViewModel model)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            bool data =await repo.saveBulkLoanReAssignmentToAgentRem(model, user);

            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data = data, message = TranslateHelper.get("Re-assignment saved successfully") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving loan recovery re-assignment unsuccessfully") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-loan-recovery-un-assignment")]
        public async Task<HttpResponseMessage> saveBulkLoanUnAssignmentToAgent([FromBody] LoanRecoveryAssignmentViewModel model)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            WorkflowResponse data =await repo.saveBulkLoanUnAssignmentToAgent(model, user);

            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = data.responseMessage });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving loan recovery unassignment unsuccessfully") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-retail-loan-recovery-un-assignment")]
        public async Task<HttpResponseMessage> saveRetailBulkLoanUnAssignmentToAgent([FromBody] LoanRecoveryAssignmentViewModel model)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            WorkflowResponse data =await repo.saveRetailBulkLoanUnAssignmentToAgent(model, user);

            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = data.responseMessage });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving loan recovery unassignment unsuccessfully") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("multiple-loan-recovery-re-assignment/{expCompletionDate}/{accreditedConsultant}/{source}")]
        public async Task<HttpResponseMessage> saveMultipleLoanReAssignmentToAgent(DateTime expCompletionDate, int accreditedConsultant, string source,  [FromBody] List<GlobalExposureApplicationViewModel> model)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            WorkflowResponse data =await repo.saveMultipleLoanReAssignmentToAgent(model, user, expCompletionDate, accreditedConsultant, source);

            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data = data, message = data.responseMessage });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving loan recovery re-assignment unsuccessfully") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("multiple-retail-loan-recovery-re-assignment/{expCompletionDate}/{accreditedConsultant}/{source}")]
        public async Task<HttpResponseMessage> saveMultipleRetailLoanReAssignmentToAgent(DateTime expCompletionDate, int accreditedConsultant, string source, [FromBody] List<GlobalExposureApplicationViewModel> model)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            WorkflowResponse data =await repo.saveMultipleRetailLoanReAssignmentToAgent(model, user, expCompletionDate, accreditedConsultant, source);

            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data = data, message = data.responseMessage });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving loan recovery re-assignment unsuccessfully") });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("multiple-loan-recovery-un-assignment")]
        public async Task<HttpResponseMessage> saveMultipleLoanUnAssignmentToAgent([FromBody] List<GlobalExposureApplicationViewModel> model)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            WorkflowResponse data =await repo.saveMultipleLoanUnAssignmentToAgent(model, user);

            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = data.responseMessage });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving loan recovery unassignment unsuccessfully") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("multiple-retail-loan-recovery-un-assignment")]
        public async Task<HttpResponseMessage> saveMultipleRetailLoanUnAssignmentToAgent([FromBody] List<GlobalExposureApplicationViewModel> model)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            WorkflowResponse data =await repo.saveMultipleRetailLoanUnAssignmentToAgent(model, user);

            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = data.responseMessage });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving loan recovery unassignment unsuccessfully") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-loan-recovery-assignment-initiate-approval")]
        public async Task<HttpResponseMessage> bulkLoanAssignmentToAgentGoForApproval([FromBody] GlobalExposureApplicationViewModel models)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;
            WorkflowResponse data =await repo.bulkLoanAssignmentToAgentGoForApproval(models, user);

            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data = data, message = data.responseMessage });
            }
            return Request.CreateResponse(HttpStatusCode.OK,
                new { success = false, message = TranslateHelper.get("Error occur forwarding for approval") });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("collateral-liquidation-recovery-without-file")]
        public async Task<HttpResponseMessage> AddCollateralLiquidationRecovery([FromBody] CollateralLiquidationRecoveryViewModel models)
        {
            try { 
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;
            models.createdBy = token.GetStaffId;
                var response =await repo.AddCollateralLiquidationRecoveryWithoutFile(models);
                if (response == 2) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("Recoveries has been successfully uploaded") });
                if (response == 3) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The recoveries already exist") });
            }
            catch (Exception ex) { return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error uploading recoveries")+ ":"   + TranslateHelper.get(ex.Message) }); }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "There was an error uploading recoveries" });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("collateral-liquidation-recovery")]
        public async Task<HttpResponseMessage> AddCollateralLiquidationRecovery()
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

            if (!provider.FileStreams.Any())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("No file uploaded"));
            }
            try
            {
                var entity = new CollateralLiquidationRecoveryViewModel();
                entity.fileName = provider.FormData["fileName"];
                entity.fileExtension = provider.FormData["fileExtension"];
                entity.fileSize = Convert.ToInt32(provider.FormData["fileSize"]);
                entity.fileSizeUnit = provider.FormData["fileSizeUnit"];
                entity.overwrite = provider.FormData["overwrite"] == "true";
                entity.applicationReferenceNumber = provider.FormData["applicationReferenceNumber"];
                entity.loanId = Convert.ToInt32(provider.FormData["loanId"]);
                entity.customerId = provider.FormData["customerId"];
                entity.accreditedConsultant = Convert.ToInt32(provider.FormData["accreditedConsultant"]);
                entity.loanAssignId = Convert.ToInt32(provider.FormData["loanAssignId"]);
                entity.totalRecoveryAmount = Convert.ToDecimal(provider.FormData["totalRecoveryAmount"]);
                entity.recoveredAmount = Convert.ToDecimal(provider.FormData["recoveredAmount"]);
                entity.collateralCode = provider.FormData["collateralCode"];
                entity.loanReference = provider.FormData["loanReference"];
                entity.collectionMode = provider.FormData["collectionMode"];
                var receiptDate = provider.FormData["receiptDate"];
                var receiptDateSub = receiptDate.Substring(0, 15);
                entity.receiptDate = DateTime.ParseExact(receiptDateSub, "ddd MMM dd yyyy", CultureInfo.InvariantCulture);
                entity.percentageCommission = Convert.ToDecimal(provider.FormData["percentageCommission"]);
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();
                int response =await repo.AddCollateralLiquidationRecovery(entity, buffer);

                if (response == 2) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("Recoveries has been successfully uploaded") });
                if (response == 3) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The recoveries already exist") });
            }
            catch (Exception ex) { return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error uploading recoveries") + ": " + TranslateHelper.get(ex.Message) }); }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "There was an error uploading recoveries" });

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("liquidation-receipt-download/{liquidationRecoveryReceiptId}")]
        public async Task<HttpResponseMessage> GetLiquidationReceipt(int liquidationRecoveryReceiptId)
        {
            CollateralLiquidationRecoveryViewModel data =await repo.GetLiquidationReceipt(liquidationRecoveryReceiptId);
            if (data == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lien-document-download/{lienRemovalId}")]
        public async Task<HttpResponseMessage> GetLienReovalLetter(int lienRemovalId)
        {
            RemoveLienViewModel data =await repo.GetLienRemovalLetter(lienRemovalId);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }else
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("saved-multiple-disbursement")]
        public async Task<HttpResponseMessage> GetpendingMultipleDisbursement()
        {
            var response =await repo.GetpendingMultipleDisbursement();

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
        }
        

        [HttpPost]
        [ClaimsAuthorization]
        [Route("pre-multiple-disbursement")]  
        public async Task<HttpResponseMessage> UploadBulkDisbursementData()
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


            var isFinal = Convert.ToBoolean(provider.FormData["isFinal"]);

            var entity = new UserInfo
            {
                BranchId = (short)token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
            };

            if (!provider.FileStreams.Any())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("No file uploaded"));
            }

            var file = provider.Contents.FirstOrDefault();
            var buffer = await file.ReadAsByteArrayAsync();
            var data =await repo.preBulkLoanDisbursement(buffer, entity, isFinal);

            if (buffer != null)
            {
                bool success = true;
                if (data.Item2 == false && isFinal) { success = false; }
                if (!success) { return Request.CreateResponse(HttpStatusCode.OK, new { success = success, result = data.Item1, message = TranslateHelper.get("Bulk loan disbursement failed to uploaded") }); }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = success, result = data.Item1, message = TranslateHelper.get("Bulk Disbursement data was successfully uploaded") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Error uploading Bulk Disbursement data") });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("pre-multiple-insurance")]
        public async Task<HttpResponseMessage> UploadBulkInsuranceData()
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


            var isFinal = Convert.ToBoolean(provider.FormData["isFinal"]);

            var entity = new UserInfo
            {
                BranchId = (short)token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
            };

            if (!provider.FileStreams.Any())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("No file uploaded"));
            }

            var file = provider.Contents.FirstOrDefault();
            var buffer = await file.ReadAsByteArrayAsync();
            var data =await repo.preBulkInsurance(buffer, entity, isFinal);

            if (buffer != null)
            {
                bool success = true;
                if (data.Item2 == false && isFinal) { success = false; }
                if (!success) { return Request.CreateResponse(HttpStatusCode.OK, new { success = success, result = data.Item1, message = TranslateHelper.get("Pre Bulk insurance failed to upload") }); }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = success, result = data.Item1, message = TranslateHelper.get("Pre Bulk Insurance data was successfully uploaded") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Error uploading Pre Bulk Insurance data") });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("work-flow-tracker-booking/operation/{operationId}/target/{targetId}")]
        public async Task<HttpResponseMessage> GetApprovalTrailByOperationIdAndTargetIdBooking(int operationId, int targetId)
        {
            var data = await repo.GetApprovalTrailByOperationIdAndTargetId(operationId, targetId, token.GetCompanyId);

            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, count = data.Count() });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-loan-recovery-reporting")]
        public async Task<HttpResponseMessage> saveBulkLoanRecoveryReporting([FromBody] List<LoanRecoveryReportBatchViewModel> models)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            var data =await repo.saveBulkLoanRecoveryReporting(models, user);

            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data = data, message = TranslateHelper.get("Bulk Recovery Successfully Saved") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving loan recovery assignment unsuccessfully") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-loan-recovery-reporting-initiate-approval")]
        public async Task<HttpResponseMessage> bulkLoanRecoveryReportingGoForApproval([FromBody] LoanRecoveryReportApprovalViewModel models)
        {
            try
            {
                UserInfo user = new UserInfo();
                user.staffId = token.GetStaffId;
                user.BranchId = (short)token.GetBranchId;
                user.companyId = token.GetCompanyId;
                user.createdBy = token.GetStaffId;

                WorkflowResponse data =await repo.bulkLoanRecoveryReportingGoForApproval(models, user);

                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, data = data, message = data.responseMessage });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Error occur forwarding for approval") });
            }catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = e.Message });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-loan-recovery-commission")]
        public async Task<HttpResponseMessage> saveBulkLoanRecoveryCommission([FromBody] List<LoanRecoveryCommissionBatchViewModel> models)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            var data =await repo.saveBulkLoanRecoveryCommission(models, user);

            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data = data, message = TranslateHelper.get("Recovery Commission Successfully Saved") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("saving loan recovery assignment unsuccessfully") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-loan-recovery-commission-initiate-approval")]
        public async Task<HttpResponseMessage> bulkLoanRecoveryCommissionGoForApproval([FromBody] LoanRecoveryCommissionApprovalViewModel models)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            WorkflowResponse data =await repo.bulkLoanRecoveryCommissionGoForApproval(models, user);

            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, data = data, message = data.responseMessage });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("Error occur forwarding for approval") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("retail-loan-recovery-commission")]
        public async Task<HttpResponseMessage> RetailLoanRecoveryCommission([FromBody] RetailLoanRecoveryCommissionViewModel models)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            bool data =await repo.RetailLoanRecoveryCommission(models, user);

            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Record saved successfully") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = "Error occur saving record" });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("retail-loan-recovery-commission-internal")]
        public async Task<HttpResponseMessage> RetailLoanRecoveryCommissionInternal([FromBody] RetailLoanRecoveryCommissionViewModel models)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            bool data =await repo.RetailLoanRecoveryCommissionInternal(models, user);

            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Record saved successfully") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("Error occur saving record") });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("recovery-report-collection")]
        public async Task<HttpResponseMessage> RetailLoanRecoveryReportCollection([FromBody] RetailLoanRecoveryCommissionViewModel models)
        {
            UserInfo user = new UserInfo();
            user.staffId = token.GetStaffId;
            user.BranchId = (short)token.GetBranchId;
            user.companyId = token.GetCompanyId;
            user.createdBy = token.GetStaffId;

            bool data =await repo.RetailLoanRecoveryReportCollection(models, user);

            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Record saved successfully") });
            }
            return Request.CreateResponse(HttpStatusCode.OK,

                new { success = false, message = TranslateHelper.get("Error occur saving record") });
        }
    }
}