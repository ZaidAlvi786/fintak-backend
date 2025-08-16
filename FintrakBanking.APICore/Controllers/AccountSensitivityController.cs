using FintrakBanking.APICore.core;
using FintrakBanking.Interfaces.Setups.General;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using System.Threading.Tasks;
using FintrakBanking.Common;

namespace FintrakBanking.APICore.Controllers
{

    [RoutePrefix("api/v1/accountsensitivity")]
    public class AccountSensitivity : ApiControllerBase
    {
        private IAccountSensitivityRepository repo;

        public AccountSensitivity(IAccountSensitivityRepository _repo)
        {
            repo = _repo;
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("getaccountsensitivity")]
        public async Task<HttpResponseMessage> GetSensitivityLevels( )
        {  
                try
                {
                    var sensitivityLevel = await repo.GetAllAccountSensitivityLevels();
                    return Request.CreateResponse(HttpStatusCode.OK, new { result= sensitivityLevel });
                }
                catch (SecureException ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,new { error = true, message = TranslateHelper.get(ex.Message) });
                }
                  
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("getaccountsensitivity/{levelId}")]
        public async Task<HttpResponseMessage> GetSensitivityLevels(  int levelId)
        { 
                try
                {
                    var sensitivityLevel = await repo.GetAccountSensitivityLevelsByLevelId(levelId);
                    return Request.CreateResponse(HttpStatusCode.OK,new { success = true, result = sensitivityLevel });
                }
                catch (SecureException ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { error = true, message = TranslateHelper.get(ex.Message) });
                }
                 
        }
    }
}