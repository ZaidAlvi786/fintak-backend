//using FintrakBanking.Interfaces.CreditLimitValidations;
using System;
using FintrakBanking.APICore.JWTAuth;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using FintrakBanking.APICore.core;
using FintrakBanking.Interfaces.CreditLimitValidations;
using System.Web;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.CreditLimitValidations;
using FintrakBanking.Common.CustomException;
using FintrakBanking.ViewModels.Setups.General;
using FintrakBanking.ViewModels;
using System.Threading.Tasks;
using FintrakBanking.Common;
using FintrakBanking.AccessSubsediary;

namespace FintrakBanking.APICore.Controllers
{

    [RoutePrefix("api/v1/credit/limitvalidations")]
    public class CreditLimitValidationsController : ApiControllerBase
    {
        private ICreditLimitValidationsRepository repo;

        TokenDecryptionHelper token = new TokenDecryptionHelper();

        public CreditLimitValidationsController(ICreditLimitValidationsRepository _repo)
        {
            this.repo = _repo;
        }



        //[HttpGet]
        //[Route("blacklist/{customerId}")]
        //public HttpResponseMessage ValidateBlackList(int customerId)
        //{ 
        //        try
        //        {
        //            var data = repo.ValidateBlackList(customerId);
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        //        }
        //        catch (SecureException ex)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK,new { success = false, message = ex.Message });
        //        }

        //}
        [HttpGet]
        [ClaimsAuthorization]
        [Route("blacklist/{customerCode}")]
        public async Task<HttpResponseMessage> ValidateBlackList(string customerCode)
        {
            try
            {
                var data = await repo.ValidateBlackList(customerCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = ex.Message });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("customer-eligibility/{customerCode}")]
        public async Task<HttpResponseMessage> ValidateCustomerEligibility(string customerCode)
        {
            try
            {
                var data = await repo.ValidateCustomerEligibility(customerCode);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No record Found" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = ex.Message });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-customer-eligibility/{customerCode}")]
        public async Task<HttpResponseMessage> GetCustomerEligibility(string customerCode)
        {
            var data = await repo.GetCustomerEligibility(customerCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        //public CustomerEligibility GetCustomerEligibility(string customerCode)

        [HttpGet]
        [ClaimsAuthorization]
        [Route("watchlist/{customerId}")]
        public async Task<HttpResponseMessage> ValidateWatchList(int customerId)
        {
            try
            {
                var data = await repo.ValidateWatchList(customerId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = ex.Message });
            }

        }


        //[HttpGet] [ClaimsAuthorization]  
        //  [Route("camsol/{customerId}")]
        //  public HttpResponseMessage ValidateCamsol(int customerId)
        //  {
        //      try
        //      {
        //          var data = repo.ValidateCamsol(customerId);
        //          return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        //      }
        //      catch (SecureException ex)
        //      {
        //          return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = ex.Message });
        //      }

        //  }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("validateamount/branch")]
        public async Task<HttpResponseMessage> ValidateAmountByBranch()
        {
            try
            {

                var data = await repo.ValidateAmountByBranch((short)token.GetBranchId);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, message = "No record found" });
                }
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = ex.Message });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("validatenpl/branch")]
        public HttpResponseMessage ValidateNPLByBranch()
        {
            try
            {

                var data =  repo.ValidateNPLByBranch((short)token.GetBranchId);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, message = "No record found" });
                }
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = ex.Message });
            }

        }



        [HttpGet]
        [ClaimsAuthorization]
        [Route("validateamount/segment/{segmentId}")]
        public async Task<HttpResponseMessage> ValidateAmountBySegment(short segmentId)
        {
            try
            {

                var data = await repo.ValidateAmountBySegment(segmentId);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, message = "No record found" });
                }
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = ex.Message });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("validatenpl/segment/{segmentId}")]
        public async Task<HttpResponseMessage> ValidateNPLBySegment(short segmentId)
        {
            try
            {

                var data = await repo.ValidateNPLBySegment(segmentId);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, message = "No record found" });
                }
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = ex.Message });
            }

        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("validateamount/sector/{subSectorId}")]
        public async Task<HttpResponseMessage> ValidateAmountBySector(int subSectorId)
        {

            var data = await repo.ValidateAmountBySector(subSectorId);
            if (data != null)
            {

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, message = "No record found" });
            }
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("validateamountfacility/sector/{sectorId}")]
        public HttpResponseMessage ValidateAmountFacilityBySector(int sectorId)
        {
            var data = repo.ValidateAmountFacilityBySector(sectorId);
            if (data != null)
            {

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, message = "No record found" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("validatenpl/sector/{subSectorId}")]
        public HttpResponseMessage ValidateNPLBySector(int subSectorId)
        {

            var data = repo.ValidateNPLBySector(subSectorId);
            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, message = "No record found" });
            }
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("validateamount/customer/{customerId}")]
        public async Task<HttpResponseMessage> ValidateAmountByCustomer(int customerId)
        {
            try
            {

                var data = await repo.ValidateAmountByCustomer(customerId);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, message = "No record found" });
                }
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = ex.Message });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("validatenpl/customer/{customerId}")]
        public async Task<HttpResponseMessage> ValidateNPLByCustomer(int customerId)
        {
            try
            {

                var data = await repo.ValidateNPLByCustomer(customerId);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, message = "No record found" });
                }
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = ex.Message });
            }

        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("validateamount/customergroup/{customerId}")]
        public async Task<HttpResponseMessage> ValidateAmountByCustomerGroup(int customergroupId)
        {
            try
            {

                var data = await repo.ValidateAmountByCustomerGroup(customergroupId);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, message = "No record found" });
                }
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = ex.Message });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("validatenpl/customergroup/{customerId}")]
        public async Task<HttpResponseMessage> ValidateNPLByCustomerGroup(int customergroupId)
        {
            try
            {

                var data = await repo.ValidateNPLByCustomerGroup(customergroupId);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, message = "No record found" });
                }
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = ex.Message });
            }

        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("validatecreditlimitnpl/RMBM/{relationshipofficerId}")]
        public HttpResponseMessage ValidateCreditLimitByRMBM(short relationshipofficerId)
        {
            try
            {

                var data = repo.ValidateCreditLimitByRMBM(relationshipofficerId);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, message = "No record found" });
                }
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = ex.Message });
            }

        }
        #region Obligor Limit 
        [HttpGet]
        [ClaimsAuthorization]
        [Route("obligor-limit")]
        public async Task<HttpResponseMessage> GetAllObligorLimit()
        {

            var response = await repo.GetAllObligorLimit();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("obligor-limit")]
        public async Task<HttpResponseMessage> AddUpdateObligorLimit([FromBody] ObligorLimitViewModel entity)
        {
            try
            {
                string createUpdate = "";
                if (entity.riskRatingId != 0 || entity.riskRatingId > 0)
                {
                    createUpdate = "updated";
                }
                else
                {
                    createUpdate = "created";
                    if (repo.ValidateRiskRating(entity.riskRating))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                                               new { success = false, message = "Risk Rating with same Name or Code already exist." });
                    }
                }
                entity.userBranchId = (short)token.GetBranchId;

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = (short)token.GetCompanyId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;

                var data = await repo.AddUpdateRiskRating(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = "Changes Saved Successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"Saved Changes not Successful" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"There was an error saving this record {e.Message}" });
            }
        }

        [HttpDelete]
        [Route("obligor-limit/{riskRatingId}")]
        public async Task<HttpResponseMessage> DeleteObligorLimit(int riskRatingId)
        {
            try
            {
                UserInfo user = new UserInfo();
                user.BranchId = (short)token.GetBranchId;
                user.companyId = (short)token.GetCompanyId;
                user.applicationUrl = HttpContext.Current.Request.Path;
                user.staffId = token.GetStaffId;

                var data = await repo.DeleteRiskRating(riskRatingId, user);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = "Changes deleted Successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"deleted Changes not Successfull" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"There was an error deleting this record {e.Message}" });
            }
        }

        [HttpPost]
        [Route("update-customer-rating")]
        public async Task<HttpResponseMessage> UpdateCustomerRating([FromBody] ObligorLimitViewModel entity)
        {
            try
            {
                bool data = await repo.UpdateCustomerRating(entity);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = ex.Message });
            }
        }

        // cam

        [HttpPost]
        [Route("update-application-customer-rating")]
        public async Task<HttpResponseMessage> UpdateApplicationCustomerRating([FromBody] ObligorLimitViewModel entity)
        {
            try
            {
                bool data = await repo.UpdateApplicationCustomerRating(entity);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("validate-application-customer-rating")]
        public HttpResponseMessage ValidateApplicationCustomerRating([FromBody] ObligorLimitViewModel entity)
        {
            CreditLimitValidationsModel data = repo.ValidateApplicationCustomerRating(entity);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        #endregion

        [HttpPost]
        [ClaimsAuthorization]
        [Route("total-exposure-limit")]
        public async Task<HttpResponseMessage> GetTotalExposureLimit([FromBody] ExposureLimitRequestModel entity)
        {
            entity.companyId = token.GetCompanyId;
            TotalExposureLimit data = await repo.GetTotalExposureLimit(entity);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("total-exposure-limit/reference/{reference}")]
        public async Task<HttpResponseMessage> GetTotalExposureLimitReference(string reference)
        {
            TotalExposureLimit data = await repo.GetTotalExposureLimitReference(reference, token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        }


        #region currency & group limits
        [HttpGet]
        [Route("currency-limit")]
        public async Task<HttpResponseMessage> GetAllCurrencyLimit()
        {
            var response = await repo.GetAllCurrencyLimit();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("currency-limit")]
        public async Task<HttpResponseMessage> AddCurrencyLimit([FromBody] CurrencyLimitViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = (short)token.GetCompanyId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;

            var data = await repo.AddCurrencyLimits(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record Saved Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"Saved Record not Successfull" });
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("currency-limit/{currencyLimitId}")]
        public async Task<HttpResponseMessage> UpdateCurrencyLimit(int currencyLimitId, [FromBody] CurrencyLimitViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = (short)token.GetCompanyId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.currencyLimitId = currencyLimitId;

            var data = await repo.UpdateCurrencyLimits(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record Saved Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"Saved Record not Successfull" });
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("currency-limit/{currencyLimitId}")]
        public async Task<HttpResponseMessage> DeleteCurrencyLimit(int currencyLimitId)
        {
            UserInfo user = new UserInfo();
            user.BranchId = (short)token.GetBranchId;
            user.companyId = (short)token.GetCompanyId;
            user.applicationUrl = HttpContext.Current.Request.Path;
            user.staffId = token.GetStaffId;

            var data = await repo.DeleteCurrencyLimit(currencyLimitId, user);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record deleted Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"deleted Record not Successfull" });
        }

        [HttpGet]
        [Route("group-limit")]
        public async Task<HttpResponseMessage> GetAllGroupLimit()
        {
            var response = await repo.GetAllGroupLimit();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("group-limit")]
        public async Task<HttpResponseMessage> AddGroupLimit([FromBody] GroupLimitViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = (short)token.GetCompanyId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;

            var data = await repo.AddGroupLimits(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record Saved Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"Saved Record not Successfull" });
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("group-limit/{groupLimitId}")]
        public async Task<HttpResponseMessage> UpdateGroupLimit(int groupLimitId, [FromBody] GroupLimitViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = (short)token.GetCompanyId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.groupLimitId = groupLimitId;

            var data = await repo.UpdateGroupLimits(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record Saved Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"Saved Record not Successfull" });
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("group-limit/{groupLimitId}")]
        public async Task<HttpResponseMessage> DeleteGroupLimit(int groupLimitId)
        {
            UserInfo user = new UserInfo();
            user.BranchId = (short)token.GetBranchId;
            user.companyId = (short)token.GetCompanyId;
            user.applicationUrl = HttpContext.Current.Request.Path;
            user.staffId = token.GetStaffId;

            var data = await repo.DeleteGroupLimit(groupLimitId, user);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record deleted Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"deleted Record not Successfull" });
        }
        #endregion


        [HttpGet]
        [Route("project-risk-rating-categories")]
        public async Task<HttpResponseMessage> GetAllProjectRiskRatingCategories()
        {
            var response = await repo.getAllProjectRiskRatingCategories();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }

        [HttpGet]
        [Route("all-criteria-list")]
        public async Task<HttpResponseMessage> GetAllCriteriaList()
        {
            var response = await repo.getAllCriteriaList();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }

        [HttpPost]
        [Route("add-contractor-criteria")]
        public async Task<HttpResponseMessage> AddContractorCriteria([FromBody] ContractorCriteriaViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = (short)token.GetCompanyId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;

            var data = await repo.AddContractorCriteria(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record Saved Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"Saved Record not Successfull" });
        }

        [HttpPost]
        [Route("add-contractor-criteria-option")]
        public async Task<HttpResponseMessage> AddContractorCriteriaOption([FromBody] ContractorCriteriaOptionViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = (short)token.GetCompanyId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;

            var data = await repo.AddContractorCriteriaOption(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record Saved Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"Saved Record not Successfull" });
        }

        [HttpPut]
        [Route("update-contractor-criteria/{criteriaId}")]
        public async Task<HttpResponseMessage> UpdateContractorCriteria(int criteriaId, [FromBody] ContractorCriteriaViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = (short)token.GetCompanyId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.criteriaId = criteriaId;

            var data = await repo.UpdateContractorCriteria(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record Saved Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"Saved Record not Successfull" });
        }

        [HttpPut]
        [Route("update-contractor-criteria-option/{optionId}")]
        public async Task<HttpResponseMessage> UpdateContractorCriteriaOption(int optionId, [FromBody] ContractorCriteriaOptionViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = (short)token.GetCompanyId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.optionId = optionId;

            var data = await repo.UpdateContractorCriteriaOption(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record Saved Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"Saved Record not Successfull" });
        }

        [HttpPost]
        [Route("add-project-risk-criteria")]
        public async Task<HttpResponseMessage> AddProjectRiskCriteria([FromBody] ProjectRiskRatingCriteriaViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = (short)token.GetCompanyId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;

            var data = await repo.AddProjectRiskCriteria(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record Saved Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"Saved Record not Successfull" });
        }

        [HttpPut]
        [Route("update-project-risk-criteria/{projectRiskRatingCriteriaId}")]
        public async Task<HttpResponseMessage> UpdateProjectRiskCriteria(int projectRiskRatingCriteriaId, [FromBody] ProjectRiskRatingCriteriaViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = (short)token.GetCompanyId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.projectRiskRatingCriteriaId = projectRiskRatingCriteriaId;

            var data = await repo.UpdateProjectRiskCriteria(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record Saved Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"Saved Record not Successfull" });
        }

        [HttpPost]
        [Route("add-project-risk-category")]
        public async Task<HttpResponseMessage> AddProjectRiskCategory([FromBody] ProjectRiskRatingCategoryViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = (short)token.GetCompanyId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;

            var data = await repo.AddProjectRiskCategory(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record Saved Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"Saved Record not Successfull" });
        }

        [HttpPut]
        [Route("update-project-risk-category/{categoryId}")]
        public async Task<HttpResponseMessage> UpdateProjectRiskCategory(int categoryId, [FromBody] ProjectRiskRatingCategoryViewModel entity)
        {

            entity.userBranchId = (short)token.GetBranchId;
            entity.userBranchId = (short)token.GetBranchId;
            entity.companyId = (short)token.GetCompanyId;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.categoryId = categoryId;

            var data = await repo.UpdateProjectRiskCategory(entity);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, message = "Record Saved Successfully" });
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"Saved Record not Successfull" });
        }

        [HttpGet]
        [Route("contractor-criteria")]
        public HttpResponseMessage GetAllContractorCriteria()
        {
            var response = repo.getAllContractorCriteria();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }

        [HttpGet]
        [Route("contractor-criteria-option")]
        public async Task<HttpResponseMessage> GetAllContractorCriteriaOption()
        {
            var response = await repo.getAllContractorCriteriaOption();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }

        [HttpGet]
        [Route("project-risk-rating-criteria")]
        public async Task<HttpResponseMessage> GetAllProjectRiskCriteria()
        {
            var response = await repo.getAllProjectRiskRatingCriteria();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }

        [HttpGet]
        [Route("contractor-tiering/{loanApplicationId}/{customerId}")]
        public async Task<HttpResponseMessage> GetContractorTieringByApplication(int loanApplicationId, int customerId)
        {
            var response = await repo.getContractorTieringByApplication(loanApplicationId, customerId);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }

        [HttpGet]
        [Route("contractor-tiering-computation/{loanApplicationId}/{customerId}")]
        public async Task<HttpResponseMessage> GettContractorTieringByApplicationAndCustomer(int loanApplicationId, int customerId)
        {
            var response = await repo.getContractorTieringByApplicationAndCustomer(loanApplicationId, customerId);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }


        [HttpGet]
        [Route("contractor-tiering-update/{contractorTieringId}")]
        public async Task<HttpResponseMessage> GetContractorTieringForEdit(int contractorTieringId)
        {
            var response = await repo.getContractorTieringForEdit(contractorTieringId);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }


        [HttpGet]
        [Route("all-project-risk-rating-criteria")]
        public async Task<HttpResponseMessage> GetAllProjectRiskRatingByCategories()
        {
            var response = await repo.getAllProjectRiskRatingByCategories();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }


        [HttpGet]
        [Route("project-risk-rating/{loanApplicationId}/{loanApplicationDetailId}/{loanBookingRequestId}")]
        public async Task<HttpResponseMessage> GetContractorTieringByApplication(int loanApplicationId, int loanApplicationDetailId, int loanBookingRequestId)
        {
            var response = await repo.getProjectRiskRatingByApplicationDetailId(loanApplicationId, loanApplicationDetailId, loanBookingRequestId);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }

        [HttpGet]
        [Route("project-risk-rating-computation/{loanApplicationId}/{loanApplicationDetailId}")]
        public async Task<HttpResponseMessage> GetProjectRiskRatingByApplicationAndApplicationDetailId(int loanApplicationId, int loanApplicationDetailId)
        {
            var response = await repo.getProjectRiskRatingByApplicationAndApplicationDetailId(loanApplicationId, loanApplicationDetailId);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No records found" });
        }

    }
}