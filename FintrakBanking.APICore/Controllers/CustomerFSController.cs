using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.Customer;
using FintrakBanking.Interfaces.ErrorLogger;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common;
using System.Threading.Tasks;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/customers")]
    public class CustomerFsController : ApiControllerBase
    {
        private ICustomerFSCaptionGroupRepository _fsGroupRepo;
        private ICustomerFSCaptionRepository _fsCaptionRepo;
        private ICustomerFSCaptionDetailRepository _fsDetailRepo;
        private ICustomerFSRatioRepository _fsRepo;
        private TokenDecryptionHelper token = new TokenDecryptionHelper();
        private IErrorLogRepository _errorLog;

        public CustomerFsController(ICustomerFSCaptionGroupRepository fsGroupRepo,
                                    ICustomerFSCaptionRepository fsCaptionRepo,
                                    ICustomerFSCaptionDetailRepository fsDetailRepo,
                                    ICustomerFSRatioRepository fsRepo,
            IErrorLogRepository errorLog
                                    )
        {
            _fsGroupRepo = fsGroupRepo;
            _fsCaptionRepo = fsCaptionRepo;
            _fsDetailRepo = fsDetailRepo;
            _fsRepo = fsRepo;
            _errorLog = errorLog;
        }

        #region Customer FS Caption Group

         [HttpPost] [ClaimsAuthorization]
        [Route("customer-fs-caption-group")]
        public async Task<HttpResponseMessage> AddCustomerFsCaptionGroup([FromBody] CustomerFSCaptionGroupViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data =await _fsGroupRepo.AddCustomerFSCaptionGroup(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-caption-group")]
        public async Task<HttpResponseMessage> GetCustomerFsCaptionGroup()
        {
            try
            {
                var data =await _fsGroupRepo.GetCustomerFSCaptionGroup();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }
    
                [HttpGet][ClaimsAuthorization]
        [Route("customer-fs-caption-group-without-ratio")]
        public async Task<HttpResponseMessage> GetCustomerFSCaptionGroupWithoutRatio()
        {
            try
            {
                var data =await _fsGroupRepo.GetCustomerFSCaptionGroupWithoutRatio();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-caption-group/{fsCaptionGroupId}")]
        public HttpResponseMessage GetCustomerFsCaptionGroupById(short fsCaptionGroupId)
        {
            try
            {
                var data = _fsGroupRepo.GetCustomerFSCaptionGroupById(fsCaptionGroupId);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = 1 });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("customer-fs-caption-group/{fsCaptionGroupId}")]
        public async Task<HttpResponseMessage> UpdateCustomerFsCaptionGroup(short fsCaptionGroupId, [FromBody] CustomerFSCaptionGroupViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data =await _fsGroupRepo.UpdateCustomerFSCaptionGroup(fsCaptionGroupId, entity);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("customer-fs-caption-group/{fsCaptionGroupId}")]
        public async Task<HttpResponseMessage> DeleteCustomerFsCaptionGroup(int fsCaptionGroupId)
        {
            try
            {
                var user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = Request.RequestUri.Host,
                    createdBy = token.GetStaffId
                };

                await _fsGroupRepo.DeleteCustomerFSCaptionGroup(fsCaptionGroupId, user);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = fsCaptionGroupId, message = TranslateHelper.get("Record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        #endregion Customer FS Caption Group

        #region Customer FS Caption

        [HttpPost] [ClaimsAuthorization]
        [Route("customer-fs-caption")]
        public async Task<HttpResponseMessage> AddCustomerFsCaption([FromBody] CustomerFSCaptionViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

               if (_fsCaptionRepo.ValidateFSCaption(entity.fsCaptionName))
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("FS Caption with the same name already exist") });
                }

                var data =await _fsCaptionRepo.AddCustomerFSCaption(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-caption/group/{fsCaptionGroupId}")]
        public async Task<HttpResponseMessage> GetCustomerFsCaption(short fsCaptionGroupId)
        {
            try
            {
                var data =await _fsCaptionRepo.GetCustomerFSCaptionByGroupId(fsCaptionGroupId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("customer-fs-caption/group")]
        public async Task<HttpResponseMessage> GetCustomerFsCaptions()
        {
            try
            {
                var data = await _fsCaptionRepo.GetCustomerFSCaptions();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-caption/{fsCaptionId}")]
        public HttpResponseMessage GetCustomerFsCaptionById(short fsCaptionId)
        {
            try
            {
                var data = _fsCaptionRepo.GetCustomerFSCaptionById(fsCaptionId);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = 1 });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

        //[HttpGet("customer-fs-caption/unmapped/{fsCaptionGroupId}/customer/{customerId}/date/{fsDate}")]
      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-caption/customer/unmapped")]
        public async Task<HttpResponseMessage> GetUnmappedCustomerFsCaption(short fsCaptionGroupId, int customerId, DateTime fsDate)
        {
            try
            {
           
               //     var fsDateConverted = DateTime.Parse(fsDate.ToString()); //DateTime.ParseExact(fsDate, "yy/mm/dd", System.Globalization.CultureInfo.InvariantCulture); //Convert.ToDateTime(fsDate); //DateTime.ParseExact(fsDate, "yy/mm/dd", System.Globalization.CultureInfo.InvariantCulture); // DateTime.Parse(fsDate); // 
                var data =await _fsCaptionRepo.GetUnmappedCustomerFSCaption(fsCaptionGroupId, customerId, fsDate);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-caption/customer-group/unmapped")]
        public async Task<HttpResponseMessage> GetUnmappedCustomerGroupFsCaption(short fsCaptionGroupId, int customerGroupId, DateTime fsDate)
        {
            try
            {
                var data = await _fsCaptionRepo.GetUnmappedCustomerGroupFSCaption(fsCaptionGroupId, customerGroupId, fsDate);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("customer-fs-caption/{fsCaptionId}")]
        public async Task<HttpResponseMessage> UpdateCustomerFsCaption(int fsCaptionId, [FromBody] CustomerFSCaptionViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data =await _fsCaptionRepo.UpdateCustomerFSCaption(fsCaptionId, entity);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("customer-fs-caption/{fsCaptionId}")]
        public async Task<HttpResponseMessage> DeleteCustomerFsCaption(int fsCaptionId)
        {
            try
            {
                var user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = Request.RequestUri.Host,
                    createdBy = token.GetStaffId
                };

                await _fsCaptionRepo.DeleteCustomerFSCaption(fsCaptionId, user);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = fsCaptionId, message = TranslateHelper.get("Record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }




        #endregion Customer FS Caption

        #region Customer FS Caption Detail

        [HttpPost]
        [ClaimsAuthorization]
        [Route("calculate-fs-ratio-value-derived")]
        public HttpResponseMessage CalculateFSRatioValueForDerived([FromBody] CustomerFSCaptionDetailViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data = _fsRepo.CalculateFSRatioValueForDerived(entity);

                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The ratio value was calculated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = "The ratio value was calculated successfully" });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error calculating the ratio value") + " " + TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost] [ClaimsAuthorization]
        [Route("customer-fs-caption-detail")]
        public async Task<HttpResponseMessage> AddCustomerFsCaptionDetail([FromBody] CustomerFSCaptionDetailViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data = await _fsDetailRepo.AddCustomerFSCaptionDetail(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}" });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("customer-group-fs-caption-detail")]
        public async Task<HttpResponseMessage> AddCustomerGroupFsCaptionDetail([FromBody] CustomerGroupFSCaptionDetailViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data =await  _fsDetailRepo.AddCustomerGroupFSCaptionDetail(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}" });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("customer-fs-caption-detail/multiple")]
        public async Task<HttpResponseMessage> AddMultipleCustomerFsCaptionDetail([FromBody] List<CustomerFSCaptionDetailViewModel> entities)
        {
            try
            {
                var userBranch = (short)token.GetBranchId;
                var userIpAddress = Request.RequestUri.Host;
                var applicationUrl = HttpContext.Current.Request.Path;
                var createdBy = token.GetStaffId;

                foreach (CustomerFSCaptionDetailViewModel entity in entities)
                {
                    entity.userBranchId = userBranch;
                    entity.userIPAddress = userIpAddress;
                    entity.applicationUrl = applicationUrl;
                    entity.createdBy = createdBy;
                }

                var data = await _fsDetailRepo.AddMultipleCustomerFSCaptionDetail(entities);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record(s) has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating these record(s)") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating these record(s)") + " " + TranslateHelper.get(ex.Message) });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("customer-group-fs-caption-detail/multiple")]
        public HttpResponseMessage AddMultipleCustomerGroupFsCaptionDetail([FromBody] List<CustomerGroupFSCaptionDetailViewModel> entities)
        {
            try
            {
                var userBranch = (short)token.GetBranchId;
                var userIpAddress = Request.RequestUri.Host;
                var applicationUrl = HttpContext.Current.Request.Path;
                var createdBy = token.GetStaffId;

                foreach (CustomerGroupFSCaptionDetailViewModel entity in entities)
                {
                    entity.userBranchId = userBranch;
                    entity.userIPAddress = userIpAddress;
                    entity.applicationUrl = applicationUrl;
                    entity.createdBy = createdBy;
                }

                var data = _fsDetailRepo.AddMultipleCustomerGroupFSCaptionDetail(entities);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record(s) has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating these record(s)") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating these record(s)") + " " + TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-caption-detail/customer/")]
        public async Task<HttpResponseMessage> GetMappedCustomerFsCaptionDetail(short fsCaptionGroupId, int customerId, DateTime fsDate)
        {
            try
            {
           //     var fsDateConverted = Convert.ToDateTime(fsDate); //DateTime.ParseExact(fsDate, "yy/mm/dd", System.Globalization.CultureInfo.InvariantCulture);  //DateTime.Parse(fsDate); //
                var data =await  _fsDetailRepo.GetMappedCustomerFsCaptionDetail(customerId, fsCaptionGroupId, fsDate);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("all-customer-fs-caption-detail/customer/")]
        public async Task<HttpResponseMessage> GetAllMappedCustomerFsCaptionDetail(int customerId, DateTime fsDate)
        {
            try
            {
                var data =await _fsDetailRepo.GetAllMappedCustomerFsCaptionDetail(customerId, fsDate);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-caption-detail/customer/{customerId}")]
        public async Task<HttpResponseMessage> GetMappedCustomerFsCaptions(int customerId)
        {
            try
            {
                var data = await _fsDetailRepo.GetMappedCustomerFsCaptions(customerId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group-fs-caption-detail/customer-group/")]
        public async Task<HttpResponseMessage> GetMappedCustomerGroupFsCaptionDetail(short fsCaptionGroupId, int customerGroupId, DateTime fsDate)
        {
            try
            {
                var data = await _fsDetailRepo.GetMappedCustomerGroupFsCaptionDetail(customerGroupId, fsCaptionGroupId, fsDate);

                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group-fs-caption-detail/customer-group/{customerGroupId}")]
        public async Task<HttpResponseMessage> GetMappedCustomerGroupFsCaptions(int customerGroupId)
        {
            try
            {
                var data =await _fsDetailRepo.GetMappedCustomerGroupFsCaptions(customerGroupId);

                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-ratio-values/{customerId}")]
        public async Task<HttpResponseMessage> GetCustomerFSRatioValues(int customerId)
        {
            try
            {
                var data =await _fsRepo.GetCustomerFSRatioValues(customerId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-caption-detail/{fsDetailId}")]
        public HttpResponseMessage GetCustomerFsCaptionById(int fsDetailId)
        {
            try
            {
                var data = _fsDetailRepo.GetCustomerFSCaptionDetailById(fsDetailId);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = 1 });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("customer-fs-caption-detail/{fsDetailId}")]
        public async Task<HttpResponseMessage> UpdateCustomerFsCaptionDetail(int fsDetailId, [FromBody] CustomerFSCaptionDetailViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data =await _fsDetailRepo.UpdateCustomerFSCaptionDetail(fsDetailId, entity);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("customer-group-fs-caption-detail/{fsDetailId}")]
        public async Task<HttpResponseMessage> UpdateCustomerGroupFsCaptionDetail(int fsDetailId, [FromBody] CustomerGroupFSCaptionDetailViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data =await _fsDetailRepo.UpdateCustomerGroupFSCaptionDetail(fsDetailId, entity);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("customer-fs-caption-detail/{fsdetailId}")]
        public HttpResponseMessage DeleteCustomerFsCaptionDetail(int fsdetailId)
        {
            try
            {
                var user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = Request.RequestUri.Host,
                    createdBy = token.GetStaffId
                };

                _fsDetailRepo.DeleteCustomerFSCaptionDetail(fsdetailId, user);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = fsdetailId, message = TranslateHelper.get("Record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("customer-group-fs-caption-detail/{fsdetailId}")]
        public HttpResponseMessage DeleteCustomerGroupFsCaptionDetail(int fsdetailId)
        {
            try
            {
                var user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = Request.RequestUri.Host,
                    createdBy = token.GetStaffId
                };

                _fsDetailRepo.DeleteCustomerGroupFSCaptionDetail(fsdetailId, user);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = fsdetailId, message = TranslateHelper.get("Record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("customer-fs-caption-detail/multiple/{fsdetailIds}")]
        public HttpResponseMessage DeleteMultileCustomerFsCaptionDetail(List<int> fsdetailIds)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = Request.RequestUri.Host
                };

                _fsDetailRepo.DeleteMultipleCustomerFSCaptionDetail(fsdetailIds, user);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = 1, message = TranslateHelper.get("record(s) has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        #endregion Customer FS Caption Detail

        #region Customer FS Ratio Caption

         [HttpPost] [ClaimsAuthorization]
        [Route("customer-fs-ratio-caption")]
        public async Task<HttpResponseMessage> AddFsRatioCaption([FromBody] CustomerFSRatioCaptionViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = Request.RequestUri.Host;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data =await _fsRepo.AddFSRatioCaption(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}" });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-ratio-caption")]
        public async Task<HttpResponseMessage> GetFsRatioCaption()
        {
            try
            {
                var data =await _fsRepo.GetFSRatioCaption(token.GetCompanyId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("customer-fs-ratio-caption-by-group/{fSCaptionGroupId}")]
        public async Task<HttpResponseMessage> GetFSRatioCaptionByFSCaptionGroupId(int fSCaptionGroupId)
        {
            try
            {
                var data = await _fsRepo.GetFSRatioCaptionByFSCaptionGroupId(token.GetCompanyId, fSCaptionGroupId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-ratio-caption/{RatioCaptionId}")]
        public HttpResponseMessage GetFsRatioCaptionById(short ratioCaptionId)
        {
            try
            {
                var data = _fsRepo.GetFSRatioCaptionById(ratioCaptionId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("customer-fs-ratio-caption/{RatioCaptionId}")]
        public async Task<HttpResponseMessage> UpdateFsRatioCaption(short ratioCaptionId, [FromBody] CustomerFSRatioCaptionViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = Request.RequestUri.Host;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data =await _fsRepo.UpdateFSRatioCaption(ratioCaptionId, model);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("customer-fs-ratio-caption/{RatioCaptionId}")]
        public HttpResponseMessage DeleteFsRatioCaption(short ratioCaptionId)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = Request.RequestUri.Host
                };

                _fsRepo.DeleteFSRatioCaption(ratioCaptionId, user);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = ratioCaptionId, message = TranslateHelper.get("Record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        #endregion Customer FS Ratio Caption

        #region FS Ratio Detail

         [HttpPost] [ClaimsAuthorization]
        [Route("customer-fs-ratio-detail")]
        public async Task<HttpResponseMessage> AddFsRatioDetail([FromBody] CustomerFSRatioDetailViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = Request.RequestUri.Host;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data =await _fsRepo.AddFSRatioDetail(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}" });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("customer-fs-ratio-detail/multiple")]
        public async Task<HttpResponseMessage> AddMultipleFsRatioDetail([FromBody] List<CustomerFSRatioDetailViewModel> models)
        {
            try
            {
                var userBranch = (short)token.GetBranchId;
                var userIpAddress = Request.RequestUri.Host;
                var applicationUrl = HttpContext.Current.Request.Path;
                var createdBy = token.GetStaffId;

                foreach (CustomerFSRatioDetailViewModel model in models)
                {
                    model.userBranchId = userBranch;
                    model.userIPAddress = userIpAddress;
                    model.applicationUrl = applicationUrl;
                    model.createdBy = createdBy;
                    model.companyId = token.GetCompanyId;
                }

                var data =await _fsRepo.AddMultipleFSRatioDetail(models);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record(s) has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "There was an error creating these record(s)" });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating these record(s") + " "+ TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-ratio-detail/ratio-caption/{ratioCaptionId}/caption-group/{fsCaptionGroupId}")]
        public async Task<HttpResponseMessage> GetFsRatioDetail(short ratioCaptionId, short fsCaptionGroupId)
        {
            try
            {
                var data =await _fsRepo.GetFSRatioDetail(ratioCaptionId, fsCaptionGroupId, token.GetCompanyId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-ratio-detail/{ratioDetailId}")]
        public HttpResponseMessage GetFsRatioDetailById(int ratioDetailId)
        {
            try
            {
                var data = _fsRepo.GetFSRatioDetailById(ratioDetailId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("customer-fs-ratio-detail/{ratioDetailId}")]
        public async Task<HttpResponseMessage> UpdateFsRatioDetail(int ratioDetailId, [FromBody] CustomerFSRatioDetailViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = Request.RequestUri.Host;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await _fsRepo.UpdateFSRatioDetail(ratioDetailId, model);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("customer-fs-ratio-detail/{ratioDetailId}")]
        public async Task<HttpResponseMessage> DeleteFsRatioDetail(int ratioDetailId)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = Request.RequestUri.Host
                };

                await _fsRepo.DeleteFSRatioDetail(ratioDetailId, user);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = ratioDetailId, message = TranslateHelper.get("Record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("customer-fs-ratio-detail/multiple/{ratioDetailId}")]
        public HttpResponseMessage DeleteMultileFsRatioDetail(List<int> ratioDetailIds)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = Request.RequestUri.Host
                };

                _fsRepo.DeleteMultipleFSRatioDetail(ratioDetailIds, user);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = 1, message = "record(s) has been deleted successfully" });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-ratio-detail/divisor-type")]
        public async Task<HttpResponseMessage> GetAllDivisorType()
        {
            try
            {
                var data =await _fsRepo.GetAllDivisorType();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-fs-ratio-detail/value-type")]
        public async Task<HttpResponseMessage> GetAllValueType()
        {
            try
            {
                var data = await _fsRepo.GetAllValueType();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                _errorLog.LogError(ex, HttpContext.Current.Request.Path, token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }

        #endregion FS Ratio Detail
    }
}