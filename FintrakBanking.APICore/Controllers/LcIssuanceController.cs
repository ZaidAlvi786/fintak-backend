using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.APICore.core;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Interfaces.Setups.Approval;
using FintrakBanking.Interfaces.credit;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.Approval;
using FintrakBanking.ViewModels.credit;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/credit")] // TODO: modify!
    public class LcIssuanceController : ApiControllerBase
    {
        private ILcIssuanceRepository repo;
        private ILcConditionRepository conditionRepo;
        private ILcDocumentRepository documentRepo;
        private ILcShippingRepository shippingRepo;
        private ILcUssanceRepository ussanceRepo;

        TokenDecryptionHelper token = new TokenDecryptionHelper();

        public LcIssuanceController(
            ILcIssuanceRepository _repo,
            ILcConditionRepository _conditionRepo,
            ILcDocumentRepository _documentRepo,
            ILcShippingRepository _shippingRepo,
            ILcUssanceRepository _ussanceRepo
            )
        {
            this.repo = _repo;
            this.conditionRepo = _conditionRepo;
            this.documentRepo = _documentRepo;
            this.shippingRepo = _shippingRepo;
            this.ussanceRepo = _ussanceRepo;
        }

        #region LCISSUANCE
        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-issuance")]
        public async Task<HttpResponseMessage> GetLcIssuances()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await repo.GetLcIssuances(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-enhancement")]
        public async Task<HttpResponseMessage> GetLcIssuancesForEnhancement()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await repo.GetLcIssuancesForEnhancement(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-extension")]
        public async Task<HttpResponseMessage> GetLcIssuancesForExtension()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await repo.GetLcIssuancesForExtension(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-search/{searchString}")]
        public async Task<HttpResponseMessage> SearchLc(string searchString)
        {
            try
            {
                List<LcIssuanceApprovalViewModel> response = await repo.SearchLc(searchString);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-search/lms/{searchString}")]
        public async Task<HttpResponseMessage> SearchLcLMS(string searchString)
        {
            try
            {
                List<LcIssuanceApprovalViewModel> response = await repo.SearchLcLMS(searchString);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-issuance/approval")]
        public async Task<HttpResponseMessage> GetLcIssuancesForApproval()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await repo.GetLcIssuancesForApproval(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-enhancement/approval")]
        public async Task<HttpResponseMessage> GetLcIssuancesForEnhancementApproval()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await repo.GetLcIssuancesForEnhancementApproval(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-extension/approval")]
        public async Task<HttpResponseMessage> GetLcIssuancesForExtensionApproval()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await repo.GetLcIssuancesForExtensionApproval(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-cancelation/approval")]
        public async Task<HttpResponseMessage> GetLcIssuancesForCancelationApproval()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await repo.GetLcIssuancesForCancelationApproval(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-issuance/lines/{customerId}")]
        public async Task<HttpResponseMessage> GetIFFLinesForLCByCustomerId(int customerId)
        {
            try
            {
                IEnumerable<CamProcessedLoanViewModel> response =  await repo.GetIFFLinesForLCByCustomerId(customerId, token.GetCompanyId, token.GetStaffId, token.GetBranchId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-issuance/{lcIssuanceId}")]
        public async Task<HttpResponseMessage> GetLcIssuance(int lcIssuanceId)
        {
            IEnumerable<LcIssuanceViewModel> response = await repo.GetLcIssuance(lcIssuanceId);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-issuance-enhancement/{tempLcIssuanceId}")]
        public async Task<HttpResponseMessage> GetLcIssGetLcEnhancementByLcEnhancementIduance(int tempLcIssuanceId)
        {
            IEnumerable<LcIssuanceApprovalViewModel> response = await repo.GetLcEnhancementByLcEnhancementId(tempLcIssuanceId);
            if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lc-issuance")]
        public async Task<HttpResponseMessage> AddLcIssuance([FromBody] LcIssuanceViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;

            var response = await repo.AddLcIssuance(model);
            if (response.lcIssuanceId > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            //try
            //{

            //}
            //catch (SecureException ex)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            //}
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lc-enhancement")]
        public async Task<HttpResponseMessage> AddLcEnhanceMent([FromBody] LcIssuanceViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            try
            {
                var response = await repo .AddLcEnhancement(model);
                if (response.tempLcIssuanceId > 0) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lc-extension")]
        public async Task<HttpResponseMessage> AddLcExtension([FromBody] LcIssuanceViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            try
            {
                var response = await repo .AddLcExtension(model);
                if (response.tempLcIssuanceId > 0) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("lc-issuance/{id}")]
        public async Task<HttpResponseMessage> UpdateLcIssuance([FromBody] LcIssuanceViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await repo.UpdateLcIssuance(model, id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = TranslateHelper.get("The record has been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("lc-enhancement/{id}")]
        public async Task<HttpResponseMessage> UpdateLcEnhancement([FromBody] LcIssuanceViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await repo.UpdateLcEnhancement(model, id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = TranslateHelper.get("The record has been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("lc-extension/{id}")]
        public async Task<HttpResponseMessage> UpdateLcExtension([FromBody] LcIssuanceViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await repo.UpdateLcExtension(model, id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = TranslateHelper.get("The record has been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("lc-issuance/{id}")]
        public async Task<HttpResponseMessage> DeleteLcIssuance(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await repo.DeleteLcIssuance(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = TranslateHelper.get("The record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("lc-enhancement/{id}")]
        public async Task<HttpResponseMessage> DeleteLcEnhancement(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await repo .DeleteLcEnhancement(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = TranslateHelper.get("The record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("lc-extension/{id}")]
        public async Task<HttpResponseMessage> DeleteLcExtension(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await repo .DeleteLcExtension(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = TranslateHelper.get("The record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        #endregion LCISSUANCE

        #region LCDOCUMENT
        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-document")]
        public async Task<HttpResponseMessage> GetLcDocuments()
        {
            try
            {
                IEnumerable<LcDocumentViewModel> response = await documentRepo.GetLcDocuments();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-documents/{lcIssuanceId}")]
        public async Task<HttpResponseMessage> GetLcDocumentsByIssuanceId(int lcIssuanceId)
        {
            try
            {
                IEnumerable<LcDocumentViewModel> response = await documentRepo.GetLcDocumentsBylcIssuanceId(lcIssuanceId);
                if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-document/{id}")]
        public async Task<HttpResponseMessage> GetLcDocument(int id)
        {
            try
            {
                LcDocumentViewModel response = await documentRepo.GetLcDocument(id);
                if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lc-document")]
        public async Task<HttpResponseMessage> AddLcDocument([FromBody] LcDocumentViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            try
            {
                var response = await documentRepo.AddLcDocument(model);
                if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("lc-document/{id}")]
        public async Task<HttpResponseMessage> UpdateLcDocument([FromBody] LcDocumentViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await documentRepo.UpdateLcDocument(model, id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = TranslateHelper.get("The record has been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("lc-document/{id}")]
        public async Task<HttpResponseMessage> DeleteLcDocument(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await documentRepo.DeleteLcDocument(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = "The record has been deleted successfully" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        #endregion LCDOCUMENT

        #region SHIPPING
        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-shipping")]
        public async Task<HttpResponseMessage> GetLcShippings()
        {
            try
            {
                IEnumerable<LcShippingViewModel> response = await shippingRepo.GetLcShippings();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-shippings/{lcIssuanceId}")]
        public async Task<HttpResponseMessage> GetLcShippingsByLcIssuanceId(int lcIssuanceId)
        {
            try
            {
                IEnumerable<LcShippingViewModel> response = await shippingRepo.GetLcShippingsByIssuanceId(lcIssuanceId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-shipping/{id}")]
        public async Task<HttpResponseMessage> GetLcShipping(int id)
        {
            try
            {
                LcShippingViewModel response = await shippingRepo.GetLcShipping(id);
                if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lc-shipping")]
        public async Task<HttpResponseMessage> AddLcShipping([FromBody] LcShippingViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            try
            {
                var response = await shippingRepo.AddLcShipping(model);
                if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("lc-shipping/{id}")]
        public async Task<HttpResponseMessage> UpdateLcShipping([FromBody] LcShippingViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await shippingRepo.UpdateLcShipping(model, id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = TranslateHelper.get("The record has been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("lc-shipping/{id}")]
        public async Task<HttpResponseMessage> DeleteLcShipping(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await shippingRepo.DeleteLcShipping(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = "The record has been deleted successfully" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            
        }
        #endregion SHIPPING

        #region LCCONDITIONS
        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-condition")]
        public async Task<HttpResponseMessage> GetLcConditions()
        {
            try
            {
                IEnumerable<LcConditionViewModel> response =  await conditionRepo.GetLcConditions();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-conditions/{lcIssuanceId}")]
        public async Task<HttpResponseMessage> GetLcConditionsBylcIssuanceId(int lcIssuanceId)
        {
            try
            {
                IEnumerable<LcConditionViewModel> response = await conditionRepo.GetLcConditionsBylcIssuanceId(lcIssuanceId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-condition/{id}")]
        public async Task<HttpResponseMessage> GetLcCondition(int id)
        {
            try
            {
                LcConditionViewModel response = await conditionRepo.GetLcCondition(id);
                if (response == null) return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lc-condition")]
        public async Task<HttpResponseMessage> AddLcCondition([FromBody] LcConditionViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            try
            {
                var response = await conditionRepo .AddLcCondition(model);
                if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("lc-condition/{id}")]
        public async Task<HttpResponseMessage> UpdateLcCondition([FromBody] LcConditionViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await conditionRepo .UpdateLcCondition(model, id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = TranslateHelper.get("The record has been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("lc-condition/{id}")]
        public async Task<HttpResponseMessage> DeleteLcCondition(int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await conditionRepo .DeleteLcCondition(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = TranslateHelper.get("The record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            
        }
        #endregion LCCONDITIONS

        #region RELEASEOFSHIPPINGDOCUMENTS
        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-issuance/release")]
        public async Task<HttpResponseMessage> GetLcIssuancesForRelease()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await repo.GetLcIssuancesForRelease(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-issuance/releases/{lcIssuanceId}")]
        public async Task<HttpResponseMessage> GetReleasesForLcIssuance(int lcIssuanceId)
        {
            try
            {
                IEnumerable<LcReleaseAmountViewModel> response = await repo.GetReleasesForLcIssuance(lcIssuanceId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-issuance/release-approval")]
        public async Task<HttpResponseMessage> GetLcIssuancesForReleaseApproval()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await repo.GetLcIssuancesForReleaseApproval(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
            
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lc-release")]
        public async Task<HttpResponseMessage> AddLCReleaseAmount([FromBody] LcReleaseAmountViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            try
            {
                var response = await repo.AddLCReleaseAmount(entity);
                if (response.lcReleaseAmountId > 0) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been added successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error adding this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("lc-release")]
        public async Task<HttpResponseMessage> UpdateLCReleaseAmount([FromBody] LcReleaseAmountViewModel entity)
        {
            entity.userBranchId = (short)token.GetBranchId;
            entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;
            try
            {
                var response = await repo.UpdateLCReleaseAmount(entity);
                if (response.lcReleaseAmountId > 0) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been updated successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error adding this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        #endregion RELEASEOFSHIPPINGDOCUMENTS

        #region LCUSSANCE
        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-ussance-lcIssuanceId/{lcIssuanceId}")]
        public async Task<HttpResponseMessage> GetLcUssancesByLCIssuanceId(int lcIssuanceId)
        {
            try
            {
                List<LcUssanceViewModel> response = await ussanceRepo.GetLcUssanceByLCIssuanceId(lcIssuanceId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-ussance-extension/lcUsanceId/{lcUsanceId}")]
        public async Task<HttpResponseMessage> GetLcUssanceExtensionByLcUsanceId(int lcUsanceId)
        {
            try
            {
                List<LcUssanceViewModel> response = await ussanceRepo.GetLcUssanceExtensionsByLcUsanceId(lcUsanceId);
                if (response.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = response, message = TranslateHelper.get("No record was found")});
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-ussance-extension/{tempLcUsanceId}")]
        public async Task<HttpResponseMessage> GetLcUssanceExtensionByTempLcUsanceId(int tempLcUsanceId)
        {
            try
            {
                LcUssanceViewModel response = await ussanceRepo.GetLcUssanceExtensionByTempLcUsanceId(tempLcUsanceId);
                if (response == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = response, message = TranslateHelper.get("No record was found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-ussance-lcUsanceId/{lcUsanceId}")]
        public async Task<HttpResponseMessage> GetLcUssanceByLCUsanceId(int lcUsanceId)
        {
            try
            {
                LcUssanceViewModel response = await ussanceRepo .GetLcUssanceByLCUsanceId(lcUsanceId);
                if (response == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = response, message = TranslateHelper.get("No record was found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lc-ussance-extension")]
        public async Task<HttpResponseMessage> AddLcUssanceExtension([FromBody] LcUssanceViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            try
            {
                var response = await ussanceRepo .AddLcUssanceExtension(model);
                if (response.tempLcUsanceId > 0) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("lc-ussance")]
        public async Task<HttpResponseMessage> AddLcUssance([FromBody] LcUssanceViewModel model)
        {
            model.userBranchId = (short)token.GetBranchId;
            model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.createdBy = token.GetStaffId;
            model.companyId = token.GetCompanyId;
            try
            {
                var response = await ussanceRepo .AddLcUssance(model);
                if (response.lcUssanceId > 0) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("lc-ussance-extension/{id}")]
        public async Task<HttpResponseMessage> UpdateLcUsanceExtension([FromBody] LcUssanceViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await ussanceRepo .UpdateLcUsanceExtension(model, id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = TranslateHelper.get("The record has been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("lc-ussance/{id}")]
        public async Task<HttpResponseMessage> UpdateLcUssance([FromBody] LcUssanceViewModel model, int id)
        {
            UserInfo user = new UserInfo()
            {
                BranchId = token.GetBranchId,
                companyId = token.GetCompanyId,
                createdBy = token.GetStaffId,
                applicationUrl = HttpContext.Current.Request.Path,
                userIPAddress = HttpContext.Current.Request.UserHostAddress
            };
            try
            {
                bool response = await ussanceRepo .UpdateLcUssance(model, id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = response, result = response, count = 1, message = TranslateHelper.get("The record has been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-issuance/ussance-extension")]
        public async Task<HttpResponseMessage> GetLcIssuancesForUssanceExtension()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await ussanceRepo.GetLcIssuancesForUssanceExtension(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-issuance/ussance")]
        public async Task<HttpResponseMessage> GetLcIssuancesForUssance()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await ussanceRepo .GetLcIssuancesForUssance(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-issuance/ussance-extension-approval")]
        public async Task<HttpResponseMessage> GetLcIssuancesForUssanceExtensionApproval()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await ussanceRepo.GetLcIssuancesForUssanceExtensionApproval(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("lc-issuance/ussance-approval")]
        public async Task<HttpResponseMessage> GetLcIssuancesForUssanceApproval()
        {
            try
            {
                IEnumerable<LcIssuanceApprovalViewModel> response = await ussanceRepo .GetLcIssuancesForUssanceApproval(token.GetStaffId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        #endregion LCUSSANCE
    }
}
