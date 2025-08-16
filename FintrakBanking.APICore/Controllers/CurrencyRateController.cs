using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.ViewModels.Admin;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{

    [RoutePrefix("api/v1/admin")]
    public class CurrencyRateController : ApiControllerBase
    {
        private ICurrencyRateRepository repo;
        TokenDecryptionHelper token = new TokenDecryptionHelper();

        public CurrencyRateController(ICurrencyRateRepository _repo)
        {
            this.repo = _repo;
        }

      [HttpGet] [ClaimsAuthorization]  [Route("currency")]
        public async Task<HttpResponseMessage> GetCurrency()
        {
            
                var data = await repo.GetCurrency();
            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList() });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "No data found" });
            }
        }





      [HttpGet] [ClaimsAuthorization]  
        [Route("currency-ratecode")]
        public async Task<HttpResponseMessage> GetCurrencyRaceCode()
        {
            try
            {
                var data = await repo.GetAllCurrencyRateCode();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [Route("rate-code")]
        public HttpResponseMessage GetRateCode()
        {
            try
            {
                var data = repo.GetRateCode();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("base-currency")]
        public async Task<HttpResponseMessage> GetBaseCurrency()
        {
            try
            {
                var data = await repo.GetBaseCurrency(token.GetCompanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  [Route("currency-rate")]
        public async Task<HttpResponseMessage> GetCurrencyRate()
        {
            try
            {
                var data = await repo.GetCurrencyRate();
                return Request.CreateResponse(HttpStatusCode.OK,new { success = true, result = data.ToList() });
            }
            catch (SecureException ex)
            {
                    return Request.CreateResponse(HttpStatusCode.OK,new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("currency-exchange-rate/{currencyId}")]
        public async Task<HttpResponseMessage> GetCurrentCurrencyExchangeRate(short currencyId)
        {
            try
            {
                var data = await repo.GetCurrentCurrencyExchangeRate(currencyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  [Route("currency-rate/{currencyId}")]
        public async Task<HttpResponseMessage> GetCurrencyRateById(short currencyId)
        {
            try
            {
                var data = await repo.GetCurrencyRateById(currencyId);
                return Request.CreateResponse(HttpStatusCode.OK,new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

         [HttpPost] [ClaimsAuthorization][Route("currency-rate")]
        public async Task<HttpResponseMessage> AddFSRatioCaption( [FromBody] CurrencyRateViewModel model)
        {
            try
            {
                    model.createdBy = token.GetStaffId;
                    model.userBranchId = (short)token.GetBranchId;
                    //model.userIPAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    model.applicationUrl = HttpContext.Current.Request.Path;
                    model.createdBy = token.GetStaffId;
                    model.companyId = token.GetCompanyId;

                var data = await repo.AddCurrencyRate(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }

       [HttpPut] [ClaimsAuthorization][Route("currency-rate/{currencyId}")]
        public async Task<HttpResponseMessage> UpdateFSRatioCaption(short currencyId, [FromBody] CurrencyRateViewModel model)
        {
            try
            {
                    model.userBranchId = (short)token.GetBranchId;
                    //model.userIPAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    model.applicationUrl = HttpContext.Current.Request.Path;
                    model.createdBy = token.GetStaffId;

                var data = await repo.UpdateCurrencyRate(currencyId, model);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });

                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {e.Message}" });
            }
        }
    }
}