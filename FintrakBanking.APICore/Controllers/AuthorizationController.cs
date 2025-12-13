using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.ErrorLogger;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common;

namespace FintrakBanking.APICore.Controllers
{

    [RoutePrefix("api/v1")]
    public class AuthorizationController : ApiControllerBase
    {
        private IAuthorizationRepository repo;
        IErrorLogRepository errorLogger;

        private TokenDecryptionHelper token = new TokenDecryptionHelper();

        public AuthorizationController(IAuthorizationRepository _repo,
                                        IErrorLogRepository _errorLogger)
        {
            this.repo = _repo;
            this.errorLogger = _errorLogger;
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("setup/groups")]
         public async Task<HttpResponseMessage> GetGroups()
        {
            string returnMessage = string.Empty;
            try
            {
                var groups = await repo.GetGroups();
                if (groups.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = true, result = groups, count = groups.Count });
                }

                return Request.CreateResponse(HttpStatusCode.NotFound,
                  new { success = false, message = TranslateHelper.get("No group found") });
            }
            catch (SecureException e)
            {

                returnMessage = e.Message;
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError,
                  new { success = false, message = $"{TranslateHelper.get("There was error from the endpoint")} {returnMessage}" });
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("setup/group/add")]
         public async Task<HttpResponseMessage> AddGroup([FromBody] GroupModel model)
        {
            TokenDecryptionHelper token = null;
            try
            {
                token = new TokenDecryptionHelper();
                model.createdBy = token.GetStaffId;

                var data = await repo.AddGroup(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = model, message = TranslateHelper.get("Group has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                  new { success = false, message = TranslateHelper.get("There was an error creating this group") });
            }
            catch (SecureException e)
            {

                //this.errorLogger.LogError(e, HttpContext.Current.Request.Path, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                  new { success = false, message = $"{TranslateHelper.get("There was an error creating this group")} {e.Message}" });
            }

        }

        //[HttpPost("setup/group")]
        //public async Task<IActionResult> AddGroup([FromBody]GroupModel grpModel)
        //{
        //    TokenDecryptionHelper token = null;
        //    try
        //    {
        //        token = new TokenDecryptionHelper(this.HttpContext);
        //        var response = await repo.AddGroup(grpModel);
        //        if (response)
        //        {
        //            return Created("", new { success = true, result = grpModel, message = "Group has been created successfully" });
        //        }

        //        returnnew { success = false, message = "There was an error creating this group" });
        //    }
        //    catch (SecureException e)
        //    {

        //        this.errorLogger.LogError(e, Request.Path.Value, token.GetUsername);
        //        returnnew { success = false, message = $"There was an error creating this group {e.Message}" });
        //    }
        //}

        [HttpPut]
        [ClaimsAuthorization]
        [Route("setup/group/{groupId}")]
         public async Task<HttpResponseMessage> AddGroup(short groupId, [FromBody] GroupViewModel grpModel)
        {
            try
            {
                var data = await repo.UpdateGroup(groupId, grpModel);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = grpModel });
                }

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                  new { success = false, message = $"{TranslateHelper.get("There was an error updating this group")} {grpModel}" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                  new { success = false, message = $"{TranslateHelper.get("There was an error updating this group")} {e.Message}" });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("setup/activities")]
         public async Task<HttpResponseMessage> GetActivities()
        {
            try
            {
                var activity = await repo.GetActivities();
                if (!activity.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                  new { success = false, message = TranslateHelper.get("No activity found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = true, result = activity, count = activity.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                  new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("setup/activities/group/{grpId}")]
         public async Task<HttpResponseMessage> GetActivitiesByGroupId(int grpId)
        {
            try
            {
                var activities = await repo.GetActivitiesByGroupId(grpId);
                if (!activities.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                  new { success = false, message = TranslateHelper.get("No activity found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = true, result = activities });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                  new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("setup/activities/role/{id}")]
         public async Task<HttpResponseMessage> GetActivitiesByRoleId(int id)
        {
            var activities = await repo.GetActivitiesByRoleId(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = activities });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("setup/groups/role/{id}")]
         public async Task<HttpResponseMessage> GetGroupsByRoleId(int id)
        {
            var groups = await repo.GetGroupsByRoleId(id);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = groups });
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("setup/group/activities")]
        public async Task<HttpResponseMessage> AddActivitiesGroup([FromBody]GroupViewModel grpModel)
        {

            try
            {
                var token = new TokenDecryptionHelper();
                grpModel.createdBy = token.GetStaffId;
                var data = await repo.AddActivitiesToGroup(grpModel);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = grpModel, message = TranslateHelper.get("Activity(s) successfully mapped to group") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("There was an error creating this group") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = $"{TranslateHelper.get("There was an error creating this group")} {e.Message}" });
            }
        }


        [HttpGet]
        [Route("logged-in-users-number")]
         public async Task<HttpResponseMessage> GetLoggedInUsersNumber()//(int? userId = null)
        {
            var response = await repo.GetLoggedInUsersNumber(token.GetStaffId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }

        [HttpGet]
        [Route("logout-all-users")]
         public async Task<HttpResponseMessage> LogOutAllUsers()//(int? userId = null)
        {
            var response = await repo.LogOutAllUsers(token.GetStaffId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        }
        
    }
}

<!-- Auto-push timestamp: 2025-12-13 20:10:27 -->