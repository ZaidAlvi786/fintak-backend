using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FintrakBanking.Interfaces.Setups.Approval;
using FintrakBanking.ViewModels.Setups.Approval;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.ViewModels;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using FintrakBanking.APICore.core;
using System.Web;
using FintrakBanking.Common.CustomException;
using FintrakBanking.ViewModels.Setups.International;
using FintrakBanking.Interfaces.Customer;
using FintrakBanking.Interfaces.Setups.International;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/setups")]
    public class InternationalCustomerController : ApiControllerBase
    {
        private ICustomerRepository custRepo;
        private IInternationalCustomerRepository repo;
        public InternationalCustomerController(IInternationalCustomerRepository _repo, ICustomerRepository _custRepo)
        {
            repo = _repo;
            custRepo = _custRepo;
        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("search-international-customers")]
        public HttpResponseMessage GetInternationalCustomerSearch([FromBody] SearchInternationalCustomerViewModel entity)
        {

            try
            {

                var data = repo.GetInternationalCustomerSearch(entity);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = ex.Message });
            }

        }
    }
}