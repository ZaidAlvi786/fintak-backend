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
using FintrakBanking.Repositories.Credit;
using System.Collections.Generic;
using FintrakBanking.ViewModels;
using FintrakBanking.Common.CustomException;
using FintrakBanking.ViewModels.WorkFlow;
using System.Threading.Tasks;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/operations")]
    public class LoanOperationController : ApiControllerBase
    {
        private ILoanOperationsRepository repo;
        private TokenDecryptionHelper token = new TokenDecryptionHelper();
        private ILoanScheduleRepository loanScheduleTest;

        public LoanOperationController(ILoanOperationsRepository _repo, ILoanScheduleRepository _loanScheduleTest)
        {
            this.repo = _repo;
            this.loanScheduleTest = _loanScheduleTest;
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("bulk-rate-customer-excemptions")]
        public async Task<HttpResponseMessage> GetLoanRateCustomerExcemptions()
        {
            var data = await repo.GetLoanRateCustomerExcemptions(token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }


        [HttpGet]
        [Route("getrunningloan/")]
        public async Task<HttpResponseMessage> GetRunningLoans()
        {
            try
            {
                var data = await repo.GetCurrentPrepayment(token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("getrunningloan/{refNo}")]
        public async Task<HttpResponseMessage> GetRunningLoans(string refNo)
        {
            var data = await repo.GetRunningLoans(token.GetCompanyId, refNo);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("getrunningloanreversal/{loanId}")]
        public async Task<HttpResponseMessage> GetRunningLoanCurrentReversal(int loanId)
        {
            try
            {
                var data = await repo.GetRunningPrepaymentLoans(token.GetCompanyId, loanId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("getWiteOffloan/{refNo}")]
        public async Task<HttpResponseMessage> getWiteOffloan(string refNo)
        {
            var data = await repo.GetWriteOffLoans(token.GetCompanyId, refNo);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("getPrincipalAndInterestDate/{refNo}/{effectiveDate}")]
        public async Task<HttpResponseMessage> getPrincipalAndInterestDate(string refNo, DateTime effectiveDate)
        {
            var data = await repo.getPrincipalAndInterestDate(token.GetCompanyId, refNo, effectiveDate);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("getrunningloanopeningbalance/{refNo}/{effectiveDate}")]
        public HttpResponseMessage getRunningLoanOpeningBalance(string refNo, DateTime effectiveDate)
        {
            var data = repo.GetRunningLoanOpeningBalance(token.GetCompanyId, refNo, effectiveDate);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-running-fx-revolving-loan/{refNo}")]
        public async Task<HttpResponseMessage> GetRunningFXLoans(string refNo)
        {
            var data = await repo.GetRunningFXLoans(token.GetCompanyId, refNo);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("commercial-loans-lines")]
        public async Task<HttpResponseMessage> GetCommercialLoansLines()
        {
            var data = await repo.GetCommercialLoansLines(token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("maturity-instruction-type")]
        public async Task<HttpResponseMessage> GetMaturityInstructionType()
        {
            var data = await repo.GetMaturityInstructionType();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("due-commercial-loans")]
        public async Task<HttpResponseMessage> GetDueCommercialLoans()
        {
            var data = await repo.GetDueCommercialLoans(token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("due-commercial-loans/detail/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetDueCommercialLoansByApplicationDetailId(int loanApplicationDetailId)
        {
            var data = await repo.GetDueCommercialLoansByApplicationDetailId(token.GetCompanyId, loanApplicationDetailId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [Route("running-commercial-loans/detail")]
        public async Task<HttpResponseMessage> GetRunningCommercialLoanLines()
        {
            var data = await repo.GetRunningCommercialLoanLines(token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("running-commercial-loans/{refNo}")]
        public async Task<HttpResponseMessage> GetRunningCommercialLoans(string refNo)
        {
            var data = await repo.GetRunningCommercialLoans(token.GetCompanyId, refNo);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList() });
        }



        [HttpGet]
        [Route("loan-maturity-instructions")]
        public async Task<HttpResponseMessage> GetLoanMaturityInstructions()
        {
            var data = await repo.GetLoanMaturityInstructions();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("approve-commercial-loan-maturity-instruction")]
        public async Task<HttpResponseMessage> ApproveMaturityInstructionRequest([FromBody] MaturityIntructionViewModel entity)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();

            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;



            //var data = repo.addMaturityInstruction(entity);

            var data = await repo.ApproveMaturityInstructionRequest(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("New Maturity Instruction Successfully Sent For Approval") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("approve-commercial-loan-roll-over")]
        public async Task<HttpResponseMessage> ApproveCommercialPaperManualRollOverRequest([FromBody] MaturityIntructionViewModel entity)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;

            var data = await repo.ApproveCommercialPaperManualRollOverRequest(entity, null);

            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Loan Rollover process was Sent For Approval") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error processing rollover for this record") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("non-term-loan-interest-rate-change")]
        public async Task<HttpResponseMessage> NonTermLoanInterestRateChange([FromBody] LoanReviewViewModel entity)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();

            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            var data = await repo.ApproveNonTermLoanLoanRateChangeRequest(entity);

            //var data = repo.addNonTermLoanLoanRateChange(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Interest Rate Change was Successful") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error running this update") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("application-line-interest-rate-change")]
        public async Task<HttpResponseMessage> ApplicationLineRateChange([FromBody] LoanReviewViewModel entity)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();

            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;

            //var data = repo.addApplicationLineRateChange(entity);
            var data = await repo.AproveApplicationLineRateChangeRequest(entity);

            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Interest Rate Change was successfully sent for Approval") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error running this update") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("application-line-facility-amount-change")]
        public async Task<HttpResponseMessage> changeApplicationLineAmount([FromBody] LoanReviewViewModel entity)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();

            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            var data = await repo.ApproveApplicationLineAmountChangeRequest(entity);

            //var data = repo.changeApplicationLineAmount(entity);
            if (data.stateId > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = data.responseMessage });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = data.responseMessage });
            
        }

        //[HttpPost]
        //[ClaimsAuthorization]
        //[Route("commercial-loan-prepayment/{loanReferenceNumber}")]
        //public HttpResponseMessage CommercialLoanPrepayment([FromBody] loanPrepaymentViewModel entity, string loanReferenceNumber)
        //{
        //    try
        //    {
        //        TokenDecryptionHelper token = new TokenDecryptionHelper();

        //        entity.userBranchId = (short)token.GetBranchId;
        //        entity.applicationUrl = HttpContext.Current.Request.Path;
        //        entity.createdBy = token.GetStaffId;
        //        entity.companyId = token.GetCompanyId;

        //        var data = repo.addCommercialLoanPrepayment(loanReferenceNumber,entity);
        //        if (data.saveStatus.ToLower() =="saved")
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = "Payment was successful" });
        //        }
        //        else if(data.saveStatus == null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = "New payment result generated" });
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "There was an error running this update" });
        //    }
        //    catch (ConditionNotMetException ce)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
        //    }
        //    catch (APIErrorException e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
        //    }
        //    catch (TwoFactorAuthenticationException e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
        //    }
        //    catch (SecureException e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"There was an error in this transaction. " });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
        //    }
        //}


        [HttpPost]
        [ClaimsAuthorization]
        [Route("application-go-for-approval")]
        public async Task<HttpResponseMessage> CPFXApplicationGoForApproval([FromBody] ApprovalViewModel entity)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            entity.BranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            entity.staffId = token.GetStaffId;

            var data = await repo.addApplicationGoForApproval(entity);

            if (data == 1)
            {
                var message = TranslateHelper.get("Approval was Successfully. The operation has been committed");
                if (entity.operationId == (int)OperationsEnum.CommercialLoanRollOver)
                {
                    if (entity.rollOverType == "autoRollover") message = TranslateHelper.get("Maturity Instruction to rollover was successfully committed");
                    else { message = TranslateHelper.get("Rollover approval was successful"); }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = message });
            }
            else if (data == 2)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Operation details has been disapproved") });
            }
            else if (data == 4)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Operation has been Refered Back") });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                new { success = true, message = TranslateHelper.get("Operation successful, request has been routed to the next approving office") });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("non-term-loan-tenor-extension")]
        public async Task<HttpResponseMessage> addNonTermLoanTenorReview([FromBody] LoanReviewViewModel entity)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;

            //var data = repo.addNonTermLoanTenorReview(entity);
            var data = await repo.ApproveNonTermLoanTenorReviewRequest(entity);

            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Tenor Change successfully sent for Approval") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error processing tenor extension for this record") });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("line-operation-awaiting-approval")]
        public async Task<HttpResponseMessage> GetApplicationLineTenorChangeAwaitingApproval()
        {
            var data = await repo.GetApplicationLineTenorChangeAwaitingApproval(token.GetStaffId, token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("application-line-tenor-extension")]
        public async Task<HttpResponseMessage> addApplicationLineTenorChange([FromBody] LoanReviewViewModel entity)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            entity.userBranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            var data = await repo.AproveApplicationLineTenorChangeRequest(entity);

            //var data = repo.addApplicationLineTenorChange(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Tenor successfully sent for Approval") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Record is Still Being Processed For Approval") });

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("application-line-operation-go-for-approval")]
        public async Task<HttpResponseMessage> LineGoForApproval([FromBody] ApprovalViewModel entity)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            entity.BranchId = (short)token.GetBranchId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            entity.staffId = token.GetStaffId;

            // var data = repo.addApplicationLineTenorChangeApproval(entity);

            var data = await repo.LineOperationGoForApproval(entity);

            if (data == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Line Operation Approved Successfully") });
            }
            else if (data == 2)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Operation has been disapproved") });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                new { success = true, message = TranslateHelper.get("Operation successful, request has been routed to the next approving office.") });
            }

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("commercial-loan-sub-allocation")]
        public async Task<HttpResponseMessage> CommercialPaperSubAllocation([FromBody] subAllocationViewModel model)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();
            model.userBranchId = (short)token.GetBranchId;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;

            var data = await repo.SubAllocateCommercialLoanPrincipal(model);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Sub-Allocation was successfull") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error committing this transaction") });
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("new-interest-rate-review")]
        public async Task<HttpResponseMessage> GetNewInterestRateReviews()
        {
            var data = await repo.GetNewInterestRateReviews(token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-charge-fee-byloanid/loanId/{loanId}")]
        public async Task<HttpResponseMessage> GetLoanChargeFeeByLoanId(int loanId)
        {
            var data = await repo.GetLoanChargeFeeByLoanId(loanId);
            if (data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("No record found") });

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("add-bulk-rate-excemption")]
        public async Task<HttpResponseMessage> addBulkLoanRateExcemptions([FromBody] LoanViewModel entity)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();

            entity.branchId = (short)token.GetBranchId;
            // entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;

            var data = await repo.addBulkRateLoanExcemptions(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("New Bulk Rate Loan Excemption Successfully Added") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("interest-rate-change")]
        public async Task<HttpResponseMessage> addBulkInterestRateChange([FromBody] LoanBulkInterestReviewViewModel entity)
        {
            TokenDecryptionHelper token = new TokenDecryptionHelper();

            entity.userBranchId = (short)token.GetBranchId;
            // entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;

            var data = await repo.addInterestRateChange(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("New Interst Rate Successfully Added") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        //
        [HttpPost]
        [ClaimsAuthorization]
        [Route("bulk-interest-rate-change/application")]
        public HttpResponseMessage AddLoanScheduleByBulkRate([FromBody] LoanBulkInterestReviewViewModel entity)
        {
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;

            var data = repo.BulkRateReview(entity.productPriceIndexId, entity.newInterestRate, entity.effectiveDate, token.GetStaffId, (int)OperationsEnum.TermLoanBooking);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("New Interst Rate Successfully Added") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-review-irregular-schedule/{loanReviewOperationId}")]
        public async Task<HttpResponseMessage> GetLoanReviewOperationIrregularSchedule(int loanReviewOperationId)
        {
            var data = await repo.GetLoanReviewOperationIrregularSchedule(loanReviewOperationId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }


        [HttpGet]
        [Route("loan-schedule-test")]
        public HttpResponseMessage GetLoanScheduleTest()
        {

                //int loanId = 6481;
                //double prepaymentAmount = 8500000;
                //DateTime effectiveDate = DateTime.Parse("09/15/2018");
                //double interestRate = 40;
                //int frequencyId = (int)FrequencyTypeEnum.Quarterly;

                //int loanId = 7904;
                //double prepaymentAmount = 8000000;
                //DateTime effectiveDate = DateTime.Parse("01/30/2019");
                //double interestRate = 60;

                //int loanId = 7949;
                //double prepaymentAmount = 1500000;
                //DateTime effectiveDate = DateTime.Parse("12/31/2018");
                //double interestRate = 10;
                //int frequencyId = (int)FrequencyTypeEnum.Quarterly;

                int loanId = 7967;
                double prepaymentAmount = 2000000;
                DateTime effectiveDate = DateTime.Parse("03/15/2019");
                double interestRate = 20;


                //InterestRateChangeWithNewAnnuityAndUnEqualPayment(int loanID, DateTime effectiveDate, int principalRepaymentFrequency, int interestRepaymentFrequency, double interestRate)
                //var data = loanScheduleTest.InterestRateChangeWithKeepExistingAnnuityAndUnEqualPayment(loanId, effectiveDate, 3, 5, interestRate);
                //var data = loanScheduleTest.InterestRateChangeWithNewAnnuityAndUnEqualPayment(loanId,effectiveDate, 3, 5, interestRate);
                var data = loanScheduleTest.InterestRateChangeWithNewAnnuityAndUnEqualPayment(loanId,effectiveDate, 0,5, interestRate, prepaymentAmount);
                //var data = loanScheduleTest.PrepaymentWithKeepExistingAnnuity(loanId, effectiveDate, prepaymentAmount);
                //var data = loanScheduleTest.PrepaymentWithNewAnnuity(loanId, effectiveDate, prepaymentAmount);
                //var data = loanScheduleTest.InterestRateChangeWithKeepExistingAnnuity(loanId,effectiveDate, interestRate);
                //var data = loanScheduleTest.InterestRateChangeEvenPrincipalPaymentsKeepExistingNewAnnuity(loanId, effectiveDate, interestRate, prepaymentAmount);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });

        }

        [HttpGet]
        [Route("get-repayment-date")]
        public async Task<HttpResponseMessage> GetRepaymentDate(int loanId)
        {
            var data = repo.GetRepaymentDate(loanId);

            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("flag-printed")]
        public async Task<HttpResponseMessage> FlagPrintedCreditDocumentation([FromBody] CreditDocumentationViewModel entity)
        {
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            entity.userBranchId = (short)token.GetBranchId;
            var data = await repo.CreditDocumentationFilling(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Credit Documentation saved Successfully, sent for approval") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error in Credit Documentation") });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("flag-printed-los")]
        public async Task<HttpResponseMessage> FlagPrintedCreditDocumentationLos([FromBody] CreditDocumentationViewModel entity)
        {
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            entity.userBranchId = (short)token.GetBranchId;
            var data = await repo.CreditDocumentationFillingLos(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("Credit Documentation saved Successfully sent for approval") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error in Credit Documentation") });
        }

    }
}