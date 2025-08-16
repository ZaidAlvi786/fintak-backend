using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.General;
using FintrakBanking.ViewModels.WorkFlow;
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
    [RoutePrefix("api/v1/setups")]
    public class AccreditedConsultantsController : ApiControllerBase
    {
        TokenDecryptionHelper token = new TokenDecryptionHelper();

        private IAccreditedConsultantsRepository repo;
        public AccreditedConsultantsController(IAccreditedConsultantsRepository repo)
        {
            this.repo = repo;
        }
        #region Solicitors
        [HttpGet, Route("accreditedConsultantType")]
        public HttpResponseMessage GetAccreditedConsultantType()
        {
            try
            {
                var response = repo.GetAccreditedConsultantType();
                if (response != null)
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response});
                }
                var message = TranslateHelper.get("No record found");
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = message });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        [HttpGet, Route("accredited-solicitors-list/{accreditedConsultantId}")]
        public HttpResponseMessage GetAccreditedSolicitors(int accreditedConsultantId)
        {
            try
            {
                var response = repo.GetAccreditedConsultants(token.GetCompanyId, accreditedConsultantId);
                if (response != null)
                {
                    var message1 = TranslateHelper.get("Created successfully");
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = message1});
                }
                var message = TranslateHelper.get("No record found");
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = message });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        [HttpGet, Route("accredited-solicitors/{stateId}")]
        public async Task<HttpResponseMessage> GetAccreditedStateConsultantsByStateId(int stateId)
        {
            try
            {
                var response = await repo.GetAccreditedStateConsultantsByStateId(token.GetCompanyId,stateId);
                if (response != null)
                {
                    var message = TranslateHelper.get("Created successfully");
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = message });
                }
                else
                {
                    var message = TranslateHelper.get("No record found");
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = message });
                }

               
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        [HttpGet, Route("accredited-solicitors")]
        public async Task<HttpResponseMessage> GetAccreditedStateConsultants()
        {
            var response = await repo.GetAccreditedStateConsultants(token.GetCompanyId);
            var message = TranslateHelper.get("Created successfully");
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = message });
        }

        [HttpGet, Route("accredited-consultant")]
        public async Task<HttpResponseMessage> GetAccreditedConsultants()
        {
            var message = TranslateHelper.get("Created successfully");
            var response = await repo.GetAccreditedConsultants(token.GetCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = message });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("agent-search")]
        public async Task<HttpResponseMessage> SearchForAgent(string searchQuery)
        {

            var data = await repo.GetSearchedAgent(searchQuery);
            if (data == null)
            {
                var message = TranslateHelper.get("No record found");
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = message });
            }else
            return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data });

        }

        [HttpPost, Route("consultant-type-add")]
        public async Task<HttpResponseMessage> AddConsultantType([FromBody] AccreditedConsultantTypeViewModel entity)
        {
            try
            {
                //entity.createdBy = token.GetStaffId;
                //entity.userBranchId = (short)token.GetBranchId;
                //entity.applicationUrl = HttpContext.Current.Request.Path;
                //entity.companyId = token.GetCompanyId;
                //entity.countryId = (short)token.GetCountryId;
                var response = await repo.AddConsultantType(entity);
                if (response)
                {
                    var message = TranslateHelper.get("The record has been created successfully");
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = response, message = message });
                }
                else
                {
                    var message = TranslateHelper.get("There was an error creating this record");
                    return Request.CreateResponse(HttpStatusCode.OK,
                                     new { success = false, message = message });
                }

              
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        [HttpPost, Route("accredited-solicitors")]
        public async Task<HttpResponseMessage> AddAccreditedSolicitors([FromBody] AccreditedConsultantsViewModel entity)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.companyId = token.GetCompanyId;
                entity.countryId = (short)token.GetCountryId;
                var response = await repo.AddAccreditedConsultants(entity);
                if (response != null)
                {
                      var message = TranslateHelper.get("The record has been created successfully, and Sent For Approval");
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = response, message = message });
                }
                else
                {
                    var message = TranslateHelper.get("The record has been created successfully, and Sent For Approval");
                    return Request.CreateResponse(HttpStatusCode.OK,
                                     new { success = false, message = TranslateHelper.get("There was an error creating this record") });
                }

              
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        [HttpPut, Route("accredited-solicitors/{id}")]
        public async Task<HttpResponseMessage> UpdateAccreditedSolicitors([FromBody] AccreditedConsultantsViewModel entity, int id)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.companyId = token.GetCompanyId;
                entity.countryId = (short)token.GetCountryId;
                entity.accreditedConsultantId = id;

                var response = await repo.UpdateAccreditedConsultants(entity, id);
                if (response)
                {
                    var message = TranslateHelper.get("Successfully gone for Approval");
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = message });
                }
                {
                    var message = TranslateHelper.get(TranslateHelper.get("An unknown error has occured"));
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = message });
                }

              
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }

        [HttpDelete, Route("accredited-solicitors/{id}")]
        public async Task<HttpResponseMessage> DeleteAccreditedSolicitors(int id)
        {
            try
            {
                var response = await repo.DeleteAccreditedConsultantStates(id);
                if (response)
                {
                    var message = TranslateHelper.get("Deleted successfully");
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message });
                }
                else
                {
                    var message = TranslateHelper.get(TranslateHelper.get("An unknown error has occured"));
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message  });
                }
               
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("accredited-solicitors-awaiting-approval/{accreditedConsultantId}")]
        public HttpResponseMessage GetAccreditedSolicitorsAwaitingApproval(int accreditedConsultantId)
        {
            try
            {
                var data = repo.GetAccreditedSolicitorsAwaitingApprovals(token.GetStaffId, token.GetCompanyId, accreditedConsultantId);

                if (data == null)
                {
                    var message = TranslateHelper.get("No record found");
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message });
                  
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
        [Route("accredited-solicitors-approval")]
        public HttpResponseMessage GoForApprovalAsync([FromBody]ApprovalViewModel entity)
        {
            try
            {
                var token = new TokenDecryptionHelper();
                entity.BranchId = token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.staffId = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userIPAddress = Request.RequestUri.Host;

                var data = repo.GoForApproval(entity);

                if (data == 1)
                {
                    var message = TranslateHelper.get("Accredited Consultant has been approved successfully");
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message  });
                }
                else if (data == 2)
                {
                    var message = TranslateHelper.get("Accredited Consultant details has been disapproved");
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message });
                }
                else
                {
                    var message = TranslateHelper.get("Operation successful, request has been routed to the next approving office");
                    return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, message   });
                }
            }
            catch (ConditionNotMetException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $" {ex.Message}" });
            }
            catch (BadLogicException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{ex.Message}" });
            }
            catch (APIErrorException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{ex.Message}" });
            }
            catch (SecureException ex)
            {
                var message = TranslateHelper.get("Error: An unhandled error occured");
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message   });
            }
        }

        #endregion
        #region Principals
        [HttpGet, Route("accredited-principals")]
        public HttpResponseMessage GetAccreditedPrincipals()
        {
            try
            {
                var response = repo.GetAccreditedPrincipals(token.GetCompanyId);
                if (response != null)
                {
                    var message = TranslateHelper.get("Created successfully");
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message  });
                }
                else
                {
                    var message = TranslateHelper.get("No record found");
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message });
                }

             
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }
        [HttpPost, Route("accredited-principals")]
        public async Task<HttpResponseMessage> AddAccreditedPrincipals([FromBody] AccreditedPrincipalsViewModel entity)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.companyId = token.GetCompanyId;

                var response = await repo.AddAccreditedPrincipals(entity);
                if (response)
                {
                    var message = TranslateHelper.get("Created successfully");
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Created successfully" });
                }
                else
                {
                    var message = TranslateHelper.get(TranslateHelper.get("An unknown error has occured"));
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message });
                }

               
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
            }
        }
        [HttpPut, Route("accredited-principals/{id}")]
        public async Task<HttpResponseMessage> UpdateAccreditedPrincipals([FromBody] AccreditedPrincipalsViewModel entity, int id)
        {
            try
            {
                entity.createdBy = token.GetStaffId;
                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.companyId = token.GetCompanyId;

                var response = await repo.UpdateAccreditedPrincipals(entity, id);
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
        #endregion
        #region Auditors
        //[HttpGet, Route("accredited-auditors")]
        //public HttpResponseMessage GetAccreditedAuditors()
        //{
        //    try
        //    {
        //        var response = repo.GetAccreditedAuditors(token.GetCompanyId);
        //        if (response != null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Created successfully" });
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
        //    }
        //}
        //[HttpPost, Route("accredited-auditors")]
        //public async Task<HttpResponseMessage> AddAccreditedAuditors([FromBody] AccreditedAuditorsViewModel entity)
        //{
        //    try
        //    {
        //        entity.createdBy = token.GetStaffId;
        //        entity.userBranchId = (short)token.GetBranchId;
        //        entity.applicationUrl = HttpContext.Current.Request.Path;
        //        entity.companyId = token.GetCompanyId;

        //        var response = await repo.AddAccreditedAuditors(entity);
        //        if (response)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Created successfully" });
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
        //    }
        //}
        //[HttpPut, Route("accredited-auditors/{id}")]
        //public async Task<HttpResponseMessage> UpdateAccreditedAuditors([FromBody] AccreditedAuditorsViewModel entity, int id)
        //{
        //    try
        //    {
        //        entity.createdBy = token.GetStaffId;
        //        entity.userBranchId = (short)token.GetBranchId;
        //        entity.applicationUrl = HttpContext.Current.Request.Path;
        //        entity.companyId = token.GetCompanyId;

        //        var response = await repo.UpdateAccreditedAuditors(entity, id);
        //        if (response)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message =TranslateHelper.get("Updated successfully") });
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
        //    }
        //}
        #endregion
        #region Recovery Agents
        //[HttpGet, Route("accredited-recoveryagent")]
        //public HttpResponseMessage GetAccreditedRecoveryAgent()
        //{
        //    try
        //    {
        //        var response = repo.GetAccreditedAuditors(token.GetCompanyId);
        //        if (response != null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Created successfully" });
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
        //    }
        //}
        //[HttpPost, Route("accredited-recoveryagent")]
        //public async Task<HttpResponseMessage> AddAccreditedRecoveryAgents([FromBody] AccreditedRecoveryAgentViewModel entity)
        //{
        //    try
        //    {
        //        entity.createdBy = token.GetStaffId;
        //        entity.userBranchId = (short)token.GetBranchId;
        //        entity.applicationUrl = HttpContext.Current.Request.Path;
        //        entity.companyId = token.GetCompanyId;

        //        var response = await repo.AddAccreditedRecoveryAgents(entity);
        //        if (response)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = "Created successfully" });
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
        //    }
        //}
        //[HttpPut, Route("accredited-recoveryagent/{id}")]
        //public async Task<HttpResponseMessage> UpdateAccreditedRecoveryAgent([FromBody] AccreditedRecoveryAgentViewModel entity, int id)
        //{
        //    try
        //    {
        //        entity.createdBy = token.GetStaffId;
        //        entity.userBranchId = (short)token.GetBranchId;
        //        entity.applicationUrl = HttpContext.Current.Request.Path;
        //        entity.companyId = token.GetCompanyId;

        //        var response = await repo.UpdateAccreditedRecoveryAgent(entity, id);
        //        if (response)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message =TranslateHelper.get("Updated successfully") });
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException });
        //    }
        //}
        #endregion

        #region Loan Consultants

        [HttpGet]
        [Route("loan-consultant/application/{applicationId}")]
        public async Task<HttpResponseMessage> GetLoanConsultant(int applicationId)
        {
            try
            {
                List<LoanConsultantViewModel> response = await repo.GetLoanConsultant(applicationId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [Route("loan-consultant")]
        public HttpResponseMessage AddLoanConsultant([FromBody] LoanConsultantViewModel entity)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                bool response = repo.AddLoanConsultant(entity);
                if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been created successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpPut]
        [Route("loan-consultant/{id}")]
        public HttpResponseMessage EditLoanConsultant([FromBody] LoanConsultantViewModel entity, int id)
        {
            try
            {
                var isRemoteCall = HttpContext.Current.Request.Headers["isRemote"];
                if (!string.IsNullOrEmpty(isRemoteCall)) { entity.isRemoteCall = true; }
                else { entity.isRemoteCall = false; };
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                bool response = repo.EditLoanConsultant(id, entity);
                if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been modified successfully") });
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        [HttpDelete]
        [Route("loan-consultant/{id}")]
        public async Task<HttpResponseMessage> RemoveLoanConsultant(int id)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    createdBy = token.GetStaffId,
                    userIPAddress = Request.RequestUri.Host,
                };
                var isRemoteCall = HttpContext.Current.Request.Headers["isRemote"];
                if (!string.IsNullOrEmpty(isRemoteCall)) { user.isRemoteCall = true; }
                else { user.isRemoteCall = false; };
                bool response = await repo.RemoveLoanConsultant(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("The record has been removed successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.Message}", error = ex.InnerException });
            }
        }

        #endregion Loan Consultants

    }
}
