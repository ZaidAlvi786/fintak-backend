using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Common;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Interfaces.ErrorLogger;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Authentication;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FintrakBanking.Common.Enum;
using FintrakBanking.Entities.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System.Web;
using FintrakBanking.Common.CustomException;
using System.Text;
using Microsoft.AspNet.Identity;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/auth")]
    public class AuthenticationController : ApiController
    {
        private TokenDecryptionHelper token = new TokenDecryptionHelper();

        private readonly IAuthenticationRepository _repo;
        private readonly IAuditTrailRepository _auditTrail;
        private readonly IErrorLogRepository _errorLogger;
        // private IAdminRepository _adminRepo;
        private readonly IGeneralSetupRepository _genSetup;
        private readonly FinTrakBankingContext _context;

        public AuthenticationController(
                IAuthenticationRepository repo,
                IErrorLogRepository errorLogger,
                // IAdminRepository adminRepo,
                IAuditTrailRepository auditTrail,
                IGeneralSetupRepository genSetup,
                FinTrakBankingContext context
            )
        {
            this._repo = repo;
            // _adminRepo = adminRepo;
            this._errorLogger = errorLogger;
            this._auditTrail = auditTrail;
            _genSetup = genSetup;
            this._context = context;
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("user")]
         public async Task<HttpResponseMessage> GetAllUsers()
        {

            if (_repo != null)
            {
                var users = await _repo.GetAllUsers();
                if (!users.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { success = false, message = TranslateHelper.get("No user found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = users });
            }

            return Request.CreateResponse(HttpStatusCode.NotFound, new { success = false, message = $"{ TranslateHelper.get("No user found")}" });

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("user")]
        public async Task<HttpResponseMessage> AddUser([FromBody] UserViewModel user)
        {

            if (await _repo.IsUserExisting(user.username.ToLower()))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("A user with this username already exit") });
            }

            user.createdBy = token.GetStaffId;
            user.lastUpdatedBy = token.GetStaffId;

            var response = await _repo.CreateUser(user);

            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.Created, new { success = true, result = user, message = TranslateHelper.get("User has been created successfully") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });

        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("user/{userId}")]
        public async Task<HttpResponseMessage> DeleteUser(int userId)
        {

            var response = await _repo.DeleteUser(userId);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("Operation was successful") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });

        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("user/{userId}")]
        public async Task<HttpResponseMessage> UpdateUser(int userId, [FromBody] UserViewModel user)
        {
            //try
            //{
            var response = await _repo.UpdateUser(userId, user);
            if (response)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("User has been successfully updated") });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error occured while updating user") });
            //}
            //catch (SecureException ex)
            //{
            //    _errorLogger.LogError(ex, Request.RequestUri.Host, token.GetUsername);

            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            //}
        }

        //Group

        [HttpGet]
        [ClaimsAuthorization]
        [Route("group")]
         public  HttpResponseMessage GetGroups()
        {
            //try
            //{
            if (_repo != null)
            {
                var groups =  _repo.GetAllGroups().Select(x => new
                {
                    groupId = x.GROUPID,
                    groupName = x.GROUPNAME
                }).ToList();

                if (groups.Any() == false)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No group found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = groups.ToList() });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("An unknown error has occured") });
            //}
            //catch (SecureException ex)
            //{
            //    _errorLogger.LogError(ex, Request.RequestUri.Host, token.GetUsername);

            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            //}
        }
        //public string GetIpAddress(HttpRequestMessage request)
        //{
        //    if (!request.Properties.ContainsKey(HttpContext)) return null;
        //    dynamic context = request.Properties[HttpContext];
        //    return context != null ? (string)context.Request.UserHostAddress : null;
        //}

        [HttpPost]// [ClaimsAuthorization]
        [Route("token")]
         public async Task<HttpResponseMessage> GetTokenAsync([FromBody] TokenVM user)
        {
            //  return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = user , password =  user.password, username = user.username, validTo = user.validTo, encodedToken = user.encodedToken});

            try
            {
                byte[] pass = Convert.FromBase64String(user.password);
                string password = Encoding.UTF8.GetString(pass);


                user.password = StaticHelpers.EncryptSha512(password, StaticHelpers.EncryptionKey);
                string ipAddressStr = String.Empty;
                if (token.LoginCode == null) ipAddressStr = token.LoginCode.Split('@')[1];

                _repo.SessionInfo = await _repo.CheckSessionState(user.username.ToLower(), ipAddressStr);
                var foundUser = await _repo.FindUserByUserNameAndPassword(user.username.ToLower(), user.password);

                if (foundUser == null)
                {
                    var found = await _repo.GetSingleUserByUserName(user.username.ToLower());

                    if (found.branchId != null)
                    {
                        var audit1 = new TBL_AUDIT
                        {
                            AUDITTYPEID = (short)AuditTypeEnum.LoginFailed,
                            STAFFID = found.staffId,
                            BRANCHID = (short)found.branchId,
                            DETAIL = $"{user.username} login failed",
                            IPADDRESS = CommonHelpers.GetLocalIpAddress(),//CommonHelpers.GetUserIP(),
                            URL = Request.RequestUri.AbsoluteUri,
                            APPLICATIONDATE = _genSetup.GetApplicationDate(),
                            SYSTEMDATETIME = DateTime.Now,
                            TARGETID = -1,
                            OSNAME = "Testing"
                            //OSNAME = CommonHelpers.FriendlyName()
                            // OSNAME = "test",
                        };

                        _auditTrail.AddAuditTrail(audit1);
                    }

                    _context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = TranslateHelper.get("1001 Login Failure.") });
                }

                var currUser = foundUser;
                var userRole = await _repo.GetDashboardStaffRole(currUser.staffId);
                var userActivities =  _repo.GetUserActivitiesByUser(currUser.user_id);

                if (currUser.branchId != null)
                {
                    var audit = new TBL_AUDIT
                    {
                        AUDITTYPEID = (short)AuditTypeEnum.LoggedIn,
                        STAFFID = currUser.staffId,
                        BRANCHID = (short)currUser.branchId,
                        DETAIL = $"{currUser.username} logged in",
                        IPADDRESS = CommonHelpers.GetLocalIpAddress(),//CommonHelpers.GetUserIP(),
                        URL = Request.RequestUri.AbsoluteUri,
                        APPLICATIONDATE = _genSetup.GetApplicationDate(),
                        SYSTEMDATETIME = DateTime.Now,
                        TARGETID = -1,
                        OSNAME = "",
                        //OSNAME = CommonHelpers.FriendlyName()
                        //OSNAME = "test",
                    };

                    _auditTrail.AddAuditTrail(audit);
                }

                _context.SaveChanges();

                //var ttttt = HttpUtility.HtmlDecode(user.encodedToken);
                //var dat = HttpUtility.HtmlDecode(user.validTo);
                byte[] data = Convert.FromBase64String(user.encodedToken);
                string encodedToken = Encoding.UTF8.GetString(data);
                byte[] data2 = Convert.FromBase64String(user.validTo);
                string validTo = Encoding.UTF8.GetString(data2);
                var groupId = 0;
                try
                {
                    groupId = await _genSetup.GetGroupId();
                }
                catch(Exception ex)
                {
                    var mess = ex.Message;
                }
                // build the json response
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = true,
                    access_token = encodedToken,
                    expiration = validTo,
                    userInfo = new UserInfo
                    {
                        branchName = currUser.branchName,
                        companyName = currUser.companyName,
                        userName = currUser.username,
                        activities = userActivities,
                        staffId = currUser.staffId,
                        staffName = currUser.staffName,
                        sessionStatusInfo = currUser.sessionStatusInfo,
                        applicationDate = _genSetup.GetApplicationDate(),
                        lastLoginDate = currUser.lastLoginDate,
                        staffRole = userRole.lookupName,
                        corrMatrixId = currUser.corrMatrixId,
                        corrMatrixDescription = currUser.corrMatrixDescription,
                        businessUnitName = currUser.businessUnitName,
                        staffRoleId = userRole.lookupId,
                        staffRoleCode = userRole.lookupCode,
                        companyGroupId = groupId
                    }
                });

            }
            catch (SecureException ex)
            {
                //{ throw ex; }
                //string str = string.Empty;
                //_errorLogger.LogError(ex, Request.RequestUri.Host, token.GetUsername);
                //if (CommonHelpers.IsNumeric(CommonHelpers.Left(ex.Message, 4)))
                //{
                //    str = ex.Message.Replace("1001", "");
                //}
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            
        }
    }

        [HttpPost] //[ClaimsAuthorization]
        [Route("endpendingsession")]
        public IHttpActionResult SignOutUser([FromBody] TokenVM user)
        {
            //try
            //{

            //var audit = new TBL_AUDIT()
            //{
            //    AUDITTYPEID = (short)AuditTypeEnum.LoggedOut,
            //    STAFFID = token.GetStaffId,
            //    BRANCHID = (short)token.GetBranchId,
            //    DETAIL = $"{token.GetUsername} ended a pending session",
            //    IPADDRESS = CommonHelpers.GetUserIP(),
            //    URL = Request.RequestUri.AbsoluteUri,
            //    APPLICATIONDATE = _genSetup.GetApplicationDate(),
            //    SYSTEMDATETIME = DateTime.Now,
            //    TARGETID = -1
            //};

            //auditTrail.AddAuditTrail(audit);

            var res = _repo.ClearLoginToken(user.username);
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return this.Ok(new { success = true, message = "Session Ended. Login To Continue" });

            //}
            //catch (SecureException ex)
            //{
            //    _errorLogger.LogError(ex, Request.RequestUri.Host, token.GetUsername);

            //    return this.Ok(new { success = false, message = $"An unknown error occured while generate token {ex.Message}" });
            //}
        }

        [HttpPost] //[ClaimsAuthorization]
        [Route("logOut")]
        public async Task<IHttpActionResult> LogOut()
        {
            //try
            //{
            _repo.ClearLoginToken(token.GetUsername);
            var staffDetails = _repo.GetSingleUserByUserName(token.GetUsername);

            if (staffDetails == null)
            {
                return this.Ok(new { success = false, message = "User Not Found" });
            }


            //Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            var authTypes = new string[] { DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.ExternalBearer, DefaultAuthenticationTypes.TwoFactorCookie, CookieAuthenticationDefaults.AuthenticationType, "Bearer" };
            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            var audit = new TBL_AUDIT()
            {
                AUDITTYPEID = (short)AuditTypeEnum.LoggedOut,
                STAFFID = token.GetStaffId,
                BRANCHID = (short)token.GetBranchId,
                DETAIL = $"{token.GetUsername} logged out",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),//CommonHelpers.GetUserIP(),
                URL = Request.RequestUri.AbsoluteUri,
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                TARGETID = -1,
                OSNAME = CommonHelpers.FriendlyName()
            };

             _auditTrail.AddAuditTrail(audit);

            _context.SaveChanges();

            return this.Ok(new { success = true, message = TranslateHelper.get("User Logged Off") });

            //}
            //catch (SecureException ex)
            //{
            //    _errorLogger.LogError(ex, Request.RequestUri.Host, token.GetUsername);
            //    return this.Ok(new { success = false, message = $"An unknown error occured {ex.Message}" });
            //}

        }

        [HttpPost] //[ClaimsAuthorization]
        [Route("logout-idle")]
        public async Task<IHttpActionResult> LogOutIdle()
        {
            //try
            //{
            var isFirstLogOut = _repo.ClearLoginToken(token.GetUsername);
            //if (!isFirstLogOut)
            //{
            //    return this.Ok(new { success = true, message = "User Logged Off" });
            //}
            var staffDetails = await _repo.GetSingleUserByUserName(token.GetUsername);

            if (staffDetails == null || staffDetails.username == "")
            {
                return this.Ok(new { success = false, message = TranslateHelper.get("User Not Found") });
            }


            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);


            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.LoggedOut,
                STAFFID = token.GetStaffId,
                BRANCHID = (short)token.GetBranchId,
                DETAIL = $"{token.GetUsername} {TranslateHelper.get("logged out due to system idle timeout")} " + (DateTime.Now).ToLongTimeString(),
                IPADDRESS = CommonHelpers.GetLocalIpAddress(), //CommonHelpers.GetUserIP(),
                URL = Request.RequestUri.AbsoluteUri,
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                TARGETID = -1,
                OSNAME = CommonHelpers.FriendlyName()
            };

            _auditTrail.AddAuditTrail(audit);

            _context.SaveChanges();

            return this.Ok(new { success = true, message = TranslateHelper.get("User Logged Off") });

            //}
            //catch (SecureException ex)
            //{
            //    _errorLogger.LogError(ex, Request.RequestUri.Host, token.GetUsername);
            //    return this.Ok(new { success = false, message = $"An unknown error occured {ex.Message}" });
            //}

        }

        [HttpPost] //[ClaimsAuthorization]
        [Route("passwordchange")]
        public async Task<IHttpActionResult> PasswordChange(PasswordChangeViewModel pwdChange)
        {
            var check = await _repo.ValidateOldPassword(pwdChange.username, StaticHelpers.EncryptSha512(pwdChange.currentPassword, StaticHelpers.EncryptionKey));
            if (!check)
            {
                return this.Ok(new { success = false, message = TranslateHelper.get("Invalid current password") });
            }
            if (!_repo.ValidatePasswordPolicy(pwdChange.newPassword))
            {
                return this.Ok(new { success = false, message = TranslateHelper.get("Password must contain at least 8 characters, a number, lowercare and uppercase") });
            }
            var password = new PasswordChangeViewModel
            {
                username = pwdChange.username,
                currentPassword = StaticHelpers.EncryptSha512(pwdChange.currentPassword, StaticHelpers.EncryptionKey),
                newPassword = StaticHelpers.EncryptSha512(pwdChange.newPassword, StaticHelpers.EncryptionKey),
            };

            var res = _repo.PasswordChange(password);
            return this.Ok(new { success = true, message = TranslateHelper.get("Password Change was successful") });

        }

        private IAuthenticationManager Authentication => Request.GetOwinContext().Authentication;

    }

}