using FintrakBanking.APICore.core;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Interfaces.CRMS;
using FintrakBanking.Interfaces.Finance;
using FintrakBanking.Interfaces.ThridPartyIntegration;
using FintrakBanking.ViewModels.Finance;
using FintrakBanking.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FintrakBanking.Common;
using System.Threading.Tasks;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/finacle-integration")]
    public class FinacleIntegrationController : ApiControllerBase
    {
        private IFinacleIntegrationRepository _repo;
        private IEndOfDayRepository repoEOD;
        private ICRMSRegulatories crmsRegulatories;

        public FinacleIntegrationController(IFinacleIntegrationRepository repo, IEndOfDayRepository _repoEOD, ICRMSRegulatories _crmsRegulatories)
        {
            _repo = repo;
            this.repoEOD = _repoEOD;
            this.crmsRegulatories = _crmsRegulatories;
        }

        #region
        [HttpPost]
        [Route("batch-posting/detail")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> GetBatchPostingDetail(DateRange model)
        {
            try
            {
                var response = await _repo.GetBatchPostingDetail(model.startDate, model.endDate, model.searchInfo);
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }
        }

        [HttpPost]
        [Route("batch-posting/main")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> GetBatchPostingMain(DateRange model)
        {
            try
            {
                var response = await _repo.GetBatchPostingMain(model.startDate, model.endDate, model.searchInfo);
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {TranslateHelper.get("an error occured")}" });
            }
        }


        [HttpPost]
        [Route("batch-posting/count")]
        [ClaimsAuthorization]
        public HttpResponseMessage GetBatchPostingCount(DateRange model)
        {
            try
            {
                //var response = repoEOD.GetBatchPostingMain(model.startDate, model.endDate, model.searchInfo);

                var response =  repoEOD.RefreshStagingMonitoring(model.startDate, model.endDate);
                if (!response.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }
        }

        [HttpPost]
        [Route("batch-posting/batchposting")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> GenerateBatchPosting(DateRange model)
        {

            try
            {
                var fileBytes = await crmsRegulatories.GenerateBatchPosting(model);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = fileBytes });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { data = "no-record", success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }

        }

  
         

        [HttpPost]
        [Route("daily-accrual-detail")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> GetDailyAccrualDetails(DateRange model )
        {

            try
            {
                var fileBytes = await _repo.GenerateExcell(model.date, model.loanAcct);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = fileBytes });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { data = TranslateHelper.get("no-record"), success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: an error occured" });
            }

        }


        [HttpPost]
        [Route("batch-posting/errorLog")]
        [ClaimsAuthorization]
        public async Task<HttpResponseMessage> GetEODErrorLogDetail(FinanceEndofdayViewModel model)
        {

            try
            {
                var fileBytes = await _repo.GetEODErrorLogDetail(model);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = fileBytes });
            }
            catch (ConditionNotMetException ce)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { data = TranslateHelper.get("no-record"), success = false, message = $"{TranslateHelper.get("Error")}: {ce.Message}" });
            }
            catch (BadLogicException be)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}: {be.Message}" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, message = $"{TranslateHelper.get("Error")}:  {TranslateHelper.get("an error occured")}" });
            }



        }


        #endregion
    }
}
