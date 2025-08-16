using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.General;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FintrakBanking.APICore.Controllers
{
    //[EnableCors("AllDomain")]
    [RoutePrefix("api/v1/setups")]
    public class ProductFeeController : ApiControllerBase
    {
        private IProductFeeRepository repo;
        TokenDecryptionHelper token = new TokenDecryptionHelper();

        public ProductFeeController(IProductFeeRepository _repo)
        {
            this.repo = _repo;
        }


      [HttpGet] [ClaimsAuthorization]  
        [Route("fee/product/{id}")]
        public async Task<HttpResponseMessage> GetFee(int id)
        {
            try
            {
                var data =await repo.GetFeesByProductId(id);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data.ToList() });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("fee/saved-Facility/{loanApplicationDetailId}/{forModifyFacility}")]
        public async Task<HttpResponseMessage> GetSavedFee(int loanApplicationDetailId, bool forModifyFacility)
        {
            try
            {
                var data =await repo.GetSavedFee(loanApplicationDetailId, forModifyFacility);
                if (data.Count() == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data.ToList() });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("product-fee/all/{productId}")]
        public async Task<HttpResponseMessage> GetFeeByProduct(int productId)
        {
            try
            {
                var data =await repo.GetAllMappedFeeByProduct(productId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data.ToList() });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("product-fee/all/mapped/{productId}")]
        public async Task<HttpResponseMessage> GetAllMappedFeeByProduct(int productId)
        {
            try
            {
                var data =await repo.GetAllMappedFeeByProduct(productId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = true, result = data.ToList() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


      [HttpGet] [ClaimsAuthorization]  
        [Route("product-fee/unmapped/{productId}")]
        public async Task<HttpResponseMessage> GetUnmappedFeeToProduct(int productId)
        {
            try
            {
                var data = await repo.GetUnmappedFeeToProduct(productId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                            new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data.ToList() });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("product-fee/{productFeeId}")]
        public HttpResponseMessage GetProductFee(int productFeeId)
        {
            try
            {
                var data = repo.GetProductFee(productFeeId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                                new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("product-fee/temp/{productFeeId}")]
        public HttpResponseMessage GetTempProductFee(int productFeeId)
        {
            try
            {
                var data = repo.GetTempProductFee(productFeeId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                                new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("product-fee/approvals/temp{tempProductId}")]
        public async Task<HttpResponseMessage> GetProductFeeAwaitingApproval(int tempProductId)
        {
            try
            {
                var productFeeinfo = await repo.GetProductFeeAwaitingApprovals(tempProductId);

                if (productFeeinfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = productFeeinfo.ToList() });
            }
            catch (SecureException ex)
            {
                // errorLogger.LogError(ex, Request.RequestUri.Host, token.GetUsername);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        // POST api/values
         [HttpPost] [ClaimsAuthorization]
        [Route("product-fee")]
        public async Task<HttpResponseMessage> AddTempProductFee([FromBody] ProductFeeViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = Request.RequestUri.Host;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var recordId = await repo.AddProductFee(model);
                if (recordId >= 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                                    new
                                    {
                                        success = true,
                                        result = recordId,
                                        message = TranslateHelper.get("product fee has been created successfully")
                                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("product fee not created") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


         [HttpPost] [ClaimsAuthorization]
        [Route("product-fee/multiple")]
        public async Task<HttpResponseMessage> AddMultipleProductFee([FromBody] List<ProductFeeViewModel> model)
        {
            try
            {
                var recordId = await repo.AddMultipleProductFee(model);
                if (recordId >= 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                                        new { success = true, result = recordId, message = TranslateHelper.get("product fee(s) has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("product fee not created") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("product-fee/{productFeeId}")]
        public HttpResponseMessage UpdateProductFee(int productFeeId, [FromBody] ProductFeeViewModel model)
        {
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                                        new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            var data = repo.GetProductFee(productFeeId);
            if (data == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                                        new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            try
            {
                var token = new TokenDecryptionHelper();
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = Request.RequestUri.Host;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                repo.UpdateProductFee(productFeeId, model);

                return Request.CreateResponse(HttpStatusCode.OK,
                                        new { success = true, result = productFeeId, message = TranslateHelper.get("product fee has been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("product-fee/{productFeeId}")]
        public async Task<HttpResponseMessage> DeleteProductFee(int productFeeId)
        {
            var account = repo.GetProductFee(productFeeId);
            if (account == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                                            new { success = false, message = TranslateHelper.get("No Record Found") });
            }

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
                var response = await repo.DeleteProductFee(productFeeId, user);

                if (!response)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        success = false,
                        message = TranslateHelper.get("product fee has not been deleted successfully")
                    });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = true,
                    result = productFeeId,
                    message = TranslateHelper.get("product fee has been deleted successfully")
                });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("product-fee/multiple/{productFeeIds}")]
        public HttpResponseMessage DeleteMultipleProductFee(List<int> productFeeIds)
        {
            if (productFeeIds.Count <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                                                new { success = false, message = TranslateHelper.get("No Record Found") });
            }

            try
            {
                repo.DeleteMultipleProductFee(productFeeIds);

                return Request.CreateResponse(HttpStatusCode.OK,
                                                new { success = true, result = 1, message = TranslateHelper.get("product fee(s) has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
    }
}