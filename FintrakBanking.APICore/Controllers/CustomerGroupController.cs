using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.Customer;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.WorkFlow;
using FintrakBanking.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;
using FintrakBanking.Interfaces.ErrorLogger;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    // [EnableCors("AllDomain")]
    [RoutePrefix("api/v1/customers")] 
    public class CustomerGroupController : ApiControllerBase
    {
        private ICustomerGroupRepository repo;
        private IErrorLogRepository errorLogger;

        TokenDecryptionHelper token = new TokenDecryptionHelper();

        public CustomerGroupController(ICustomerGroupRepository _repo, IErrorLogRepository _errorLogger)
        {
            this.repo = _repo;
            errorLogger = _errorLogger;
        }

        #region Customer Group
         [HttpPost] [ClaimsAuthorization]
        [Route("customer-group")]
        public HttpResponseMessage AddCustomerGroup([FromBody] CustomerGroupViewModel entity)
        {

            try
            {

                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;
                if (repo.DoesGroupNameExist(entity.groupName, entity.groupCode))
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("The Group Code or Group Name you entered already exists") });
                }
                var data = repo.AddTempCustomerGroup(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.Created,
                        new
                        {
                            success = true,
                            result = data,
                            message = TranslateHelper.get("The record has been created successfully, now awaiting approval")
                        });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                //errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("customer-group/{groupId}")]
        public async Task<HttpResponseMessage> DeleteCustomerGroup(short groupId)

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

                var data =await repo.DeleteCustomerGroup(groupId, user);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been deleted successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error deleting this record") });
            }
            catch (SecureException e)
            {
               // errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error deleting this record")  });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group")]
        public HttpResponseMessage GetCustomerGroup()
        {

            try
            {
                var data = repo.GetCustomerGroup();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
               // errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-temp-customer-groups")]
        public async Task<HttpResponseMessage> GetTempCustomerGroups()
        {

            try
            {
                var data = await repo.GetAllTempCustomerGroups();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                // errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group/awaiting-approval")]
        public async Task<HttpResponseMessage> GetCustomerGroupAwaitingApproval()
        {
            try 
            {

                var data =await repo.GetCustomerGroupsAwaitingApprovals(token.GetStaffId, token.GetCompanyId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList() });
            }
            catch (SecureException e)
            {
                //errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group/{customerGroupId}")]
        public HttpResponseMessage GetCustomerGroupByCustomerId(int customerGroupId)
        {
            try
            {
                var data = repo.GetCustomerGroupByCustomerId(customerGroupId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
            }
            catch (SecureException ex)
            {
                //errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }


       [HttpPut] [ClaimsAuthorization]
        [Route("customer-group/{customerGroupId}")]
        public async Task<HttpResponseMessage> UpdateCustomerGroup(int customerGroupId, CustomerGroupViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data =await repo.UpdateCustomerGroupForApproval(customerGroupId, entity);

                if (data)
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully, now awaiting approval") });

                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException e)
            {
               // errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {e.Message}" });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("customer-group/approval")]
        public async Task<HttpResponseMessage> GoForApprovalAsync([FromBody]ApprovalViewModel entity)
        {
            try
            {
                entity.BranchId = token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.staffId = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userIPAddress = Request.RequestUri.Host;

                var data =await repo.GoForApproval(entity);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = TranslateHelper.get("Customer Group has been approved successfully") });
                }
                else if(entity.approvalStatusId == 3)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Operation Rejected") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Operation successful, request has been routed to the next approving office") });
            }
            catch (SecureException ex)
            {
               // errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An error occured") + " " + TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("customer-group-mapping/approval")]
        public async Task<HttpResponseMessage> GoForGroupMappingApproval([FromBody]ApprovalViewModel entity)
        {
            try
            {
                entity.BranchId = token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.staffId = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userIPAddress = Request.RequestUri.Host;

                var data =await repo.GoForGroupMappingApproval(entity);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = TranslateHelper.get("Customer Group Mapping has been approved successfully") });
                }
                else if (entity.approvalStatusId == 3)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Operation Rejected") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message = TranslateHelper.get("Operation successful, request has been routed to the next approving office") });
            }
            catch (SecureException ex)
            {
                // errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An error occured") + ":" + TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group/search/")]
        public async Task<HttpResponseMessage> SearchForCustomerGroupRealtime(string searchQuery)
        {
            try
            {
                var data =await repo.SearchForCustomerGroup(token.GetCompanyId, searchQuery);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data?.ToList() });
                //var data = repo.SearchForCustomerGroupRealtime(token.GetCompanyId, searchQuery);
                //return Request.CreateResponse(HttpStatusCode.OK,
                //    new { success = true, result = data.ToList() });
            }
            catch (SecureException e)
            {
               // errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {e}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("customer-group-mapping/awaiting-approval")]
        public async Task<HttpResponseMessage> GetCustomerGroupMapsAwaitingApprovals()
        {
            var data =await repo.GetCustomerGroupMapsAwaitingApprovals(token.GetStaffId, token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK,
                new { success = true, result = data.ToList() });
        }

        //   [HttpGet] [ClaimsAuthorization]  
        //[Route("all-customer-group-mapping")]
        //public HttpResponseMessage GetAllCustomerGroupMappingByGroupId(int customerGroupId)
        //{
        //    try
        //    {
        //        var data = repo.GetAllCustomerGroupMappingByGroupId(customerGroupId);
        //        return Request.CreateResponse(HttpStatusCode.OK,
        //            new { success = true, result = data });
        //    }
        //    catch (SecureException e)
        //    {
        //        //errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);

        //        return Request.CreateResponse(HttpStatusCode.OK,
        //           new { success = false, message = $"{TranslateHelper.get("Error")}: {e}" });
        //    }
        //}
        [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group/")]
        public HttpResponseMessage CustomerGroupSearch(string searchQuery)
        {
            try
            {
                var data = repo.CustomerGroupSearch(searchQuery);
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data.ToList() });
            }
            catch (SecureException e)
            {
               // errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {e}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group/{customerGroupId}/mapping-details")]
        public async Task<HttpResponseMessage> GetCustomerGroupDetailedMapping(int customerGroupId)
        {
            try
            {
                var data = await repo.GetCustomerGroupDetailsByGroupId(customerGroupId);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data });
            }
            catch (SecureException e)
            {
                //errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {e}" });
            }
        }

        #endregion

        #region Customer Group Mapping
         [HttpPost] [ClaimsAuthorization]
        [Route("customer-group-mapping")]
        public HttpResponseMessage AddCustomerGroupMapping([FromBody] CustomerGroupMappingViewModel entity)
        {
          
                var token = new TokenDecryptionHelper();

                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data = repo.AddTempCustomerGroupMapping(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully, now awaiting approval") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
          
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("customer-group-mapping/multiple")]
        public async Task<HttpResponseMessage> AddMultipleCustomerGroupMapping([FromBody] List<CustomerGroupMappingViewModel> customerGroups)
        {
            
                var token = new TokenDecryptionHelper();
                var data =await repo.AddMultipleCustomerGroupMapping(customerGroups, token.GetStaffId, (short)token.GetBranchId, token.GetCompanyId);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully, now awaiting approval") });

                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });

        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("customer-group-mapping/{groupMapId}")]
        public async Task<HttpResponseMessage> DeleteCustomerGroupMapping(int groupMapId)

        {
            var token = new TokenDecryptionHelper();
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

                var data =await repo.DeleteCustomerGroupMapping(groupMapId, user);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been deleted successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error deleting this record") });
            }
            catch (SecureException e)
            {
                errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error deleting this record")  });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group-mapping")]
        public async Task<HttpResponseMessage> GetCustomerGroupMapping()
        {

            try
            {
                var data =await repo.GetCustomerGroupMapping();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ex.Message}" });
            }
        }
      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group-members/{groupid}")]
        public async Task<HttpResponseMessage> GetGroupMembersByGroupId(int groupid)
        {
            try
            {
                var data = await repo.GetGroupMembersByGroupId(groupid, token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group-mapping/{groupMapId}")]
        public HttpResponseMessage GetCustomerGroupMappingByGroupMapId(int groupMapId)
        {
            try
            {
                var data = repo.GetCustomerGroupMappingByGroupMapId(groupMapId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }


      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group-mapping/customers/{customerGroupId}")]
        public async Task<HttpResponseMessage> GetCustomerGroupMappingByGroupId(int customerGroupId)
        {
            try
            {
                var data =await repo.GetCustomerGroupMappingByGroupId(customerGroupId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-temp-customer-group-mapping/{customerGroupId}")]
        public async Task<HttpResponseMessage> GetTempCustomerGroupMappingByGroupId(int customerGroupId)
        {
            try
            {
                var data =await repo.GetTempCustomerGroupMappingByGroupId(customerGroupId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("customer-group-mapping/relationship-types")]
        public async Task<HttpResponseMessage> GetCustomerGroupRelationshipTypes()
        {
            try
            {
                var data =await repo.GetCustomerGroupRelationshipTypes();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }
         [HttpPost] [ClaimsAuthorization]
        [Route("customer-relationship-type")]
        public async Task<HttpResponseMessage> AddCustomerGroupRelationshipTypes([FromBody] LookupViewModel entity)
        {

            try
            {
                var data =await repo.AddCustomerGroupRelationshipTypes(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.Created,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("customer-group-mapping/{groupMapId}")]
        public async Task<HttpResponseMessage> UpdateCustomerGroupMaping(int groupMapId, [FromBody] CustomerGroupMappingViewModel entity)
        {
            try
            {
                var token = new TokenDecryptionHelper();
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data =await repo.UpdateCustomerGroupMapping(groupMapId, entity);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }
        #endregion
        #region KYC Item Setup
      [HttpGet] [ClaimsAuthorization]  
        [Route("Kycitem")]
        public async Task<HttpResponseMessage> GetKYCItem()
        {
            try
            {

                var data =await repo.GetKYCItems(token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }
         [HttpPost] [ClaimsAuthorization]
        [Route("Kycitem")]
        public async Task<HttpResponseMessage> AddKYCItem([FromBody] KYCItemViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data =await repo.AddKycItem(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.Created,
                        new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                errorLogger.LogError(e, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }
       [HttpPut] [ClaimsAuthorization]
        [Route("Kycitem/{kYCItemId}")]
        public async Task<HttpResponseMessage> UpdateKYCItem(int kYCItemId, [FromBody] KYCItemViewModel entity)
        {
            try
            {
              
                entity.userBranchId = (short)token.GetBranchId;
                entity.userIPAddress = Request.RequestUri.Host;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;
                entity.companyId = token.GetCompanyId;

                var data =await repo.UpdatedKycItem(kYCItemId, entity);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                errorLogger.LogError(ex, Common.CommonHelpers.GetUserIP(), token.GetUsername);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }
        #endregion
    }
}