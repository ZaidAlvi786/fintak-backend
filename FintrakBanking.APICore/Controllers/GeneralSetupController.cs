using FintrakBanking.APICore.core;
using FintrakBanking.Common.Enum;
using FintrakBanking.Interfaces.Setups.Credit;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using FintrakBanking.APICore.JWTAuth;
using System.Collections.Generic;
using FintrakBanking.ViewModels;
using System.Web;
using System.Threading.Tasks;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/setups")]
    public class GeneralSetupController : ApiControllerBase
    {
        private IGeneralSetupRepository repo;
        private ICollateralTypeRepository collateralRepo;
        private IStaffRepository staffRepo;

        TokenDecryptionHelper token = new TokenDecryptionHelper();

        public GeneralSetupController(IGeneralSetupRepository _repo, ICollateralTypeRepository _collateralRepo, IStaffRepository _staffRepo)
        {
            this.repo = _repo;
            this.collateralRepo = _collateralRepo;
            this.staffRepo = _staffRepo;
        }

        #region General Setups

      [HttpGet] [ClaimsAuthorization]  
        [Route("calculate-maturity-date/effective-date/{effectiveDate}/tenor-mode/{tenorModeId}/tenor/{tenor}")]
          public async Task<HttpResponseMessage> GetMaturityDate(DateTime effectiveDate, short tenorModeId, int tenor)
        {
            try
            {
                //var token = new TokenDecryptionHelper(this.HttpContext);
                var data = repo.CalculateMaturityDate(effectiveDate, (TenorModeEnum)tenorModeId, tenor);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = 1 });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("tenor-mode")]
          public async Task<HttpResponseMessage> GetAllTenorMode()
        {
            try
            {
                var data = await repo.GetAllTenorMode();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("currency")]
        public async Task<HttpResponseMessage> GetAllCurrency()
        {
            try
            {
                var data = await repo.GetAllCurrency();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("loanApplicationReferance")]
          public async Task<HttpResponseMessage> GetLoanApplicationRef()
        {
            try
            {
                var data = repo.GetLoanApplicationRef();
                if (data == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

       

      [HttpGet] [ClaimsAuthorization]  
        [Route("customer-type")]
          public async Task<HttpResponseMessage> GetAllCustomerType()
        {
            try
            {
                var data = await repo.GetAllCustomerType();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data});  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


      [HttpGet] [ClaimsAuthorization]  
        [Route("deal-classification-type")]
        public async Task<HttpResponseMessage> GetAllDealClassificationType()
        {
            try
            {
                var data = await repo.GetAllDealClassificationType();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("application-last-refreshed-date")]
        public async Task<HttpResponseMessage> GetApplicationEODLastRefreshedDate()
        {
            try
            {
                var data = await repo.GetApplicationEODLastRefreshedDate();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpGet] [ClaimsAuthorization]  
        [Route("application-date")]
          public async Task<HttpResponseMessage> GetApplicaionDate()
        {
            try
            {
                var data = repo.GetApplicationDate();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
      [HttpGet] [ClaimsAuthorization]  
        [Route("sector")]
          public async Task<HttpResponseMessage> GetSector()
        {
            try
            {
                var data = await repo.GetSector();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("subsector")]
          public async Task<HttpResponseMessage> GetSubsector()
        {
            try
            {
                var data = await repo.GetSubsector();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("day-count")]
        public async Task<HttpResponseMessage> GetAllDayCount()
        {
            try
            {
                var data = await repo.GetAllDayCount();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });//Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("fee-amortisation-type")]
        public async Task<HttpResponseMessage> GetAllFeeAmortisationType()
        {
            try
            {
                var data = await repo.GetAllFeeAmortisationType();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("deal-types")]
        public async Task<HttpResponseMessage> GetAllDealTypes()
        {
            try
            {
                var data = await repo.GetAllDealTypes();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("fs-types")]
          public async Task<HttpResponseMessage> GetAllFSTypes()
        {
            try
            {
                var data = await repo.GetAllFSTypes();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("frequency-types")]
          public async Task<HttpResponseMessage> GetAllFrequencyTypes()
        {
            try
            {
                var data = await repo.GetAllFrequencyTypes();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("operation-types")]
          public async Task<HttpResponseMessage> GetAllOperationTypes()
        {
            try
            {
                var data = await repo.GetAllOperationTypes();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("operation")]
          public async Task<HttpResponseMessage> GetAllOperations()
        {
            try
            {
                var data = await repo.GetAllOperations();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data }); //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("operation/{operationTypeId}")]
          public async Task<HttpResponseMessage> GetOperations(short operationTypeId)
        {
            try
            {
                var data = await repo.GetOperations(operationTypeId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("collateral-types")]
          public HttpResponseMessage GetAllCollateralTypes()
        {
            try
            {
                var data = collateralRepo.GetCollateralTypes();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });  //Ok(accounts);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("sectors")]
          public async Task<HttpResponseMessage> GetAllSectors()
        {
            try
            {
                var data = await repo.GetAllSectors();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("global-sectors")]
          public async Task<HttpResponseMessage> GetAllGlobalSectors()
        {
            try 
            {
                var data = await repo.GetAllGlobalSectors();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }



        [HttpPost]
        [ClaimsAuthorization]
        [Route("sectors")]
          public async Task<HttpResponseMessage> AddSector([FromBody] SectorViewModel model)
        {
            try
            {
                model.companyId = token.GetCompanyId;
                model.createdBy = token.GetStaffId;
                model.userBranchId = (short)token.GetBranchId;
                var data = repo.AddSector(model);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, message = TranslateHelper.get("Sector has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("Sector has not been created successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut] [ClaimsAuthorization]
        [Route("sectors/{id}")]
          public async Task<HttpResponseMessage> UpdateSector([FromBody] SectorViewModel model ,short id)
        {
            try
            {
                model.companyId = token.GetCompanyId;
                model.createdBy = token.GetStaffId;
                model.lastUpdatedBy = token.GetStaffId;
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.Path;
                var data = repo.UpdateSector( model, id);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, message = TranslateHelper.get("Sector has been updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("Sector has not been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("global-sectors/{id}")]
          public async Task<HttpResponseMessage> UpdateGlobalSector([FromBody] GlobalSectorViewModel model, int id)
        {
            try
            {
                model.companyId = token.GetCompanyId;
                model.createdBy = token.GetStaffId;
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.Path;
                var data = repo.UpdateGlobalSector(model, id);

               return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, message = TranslateHelper.get("Global Sector limit has been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("sectors/{id}")]
          public HttpResponseMessage DeleteSector(int id)
        {
            try
            {
                UserInfo user = new UserInfo();
                user.companyId = token.GetCompanyId;
                user.staffId = token.GetStaffId;
                user.BranchId = (short)token.GetBranchId;
                user.userIPAddress = HttpContext.Current.Request.Path;
                var data = repo.DeleteSector(id, user);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, message = TranslateHelper.get("Sector has been deleted successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("Sector has not been deleted successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("region/type/{regionTypeId}")]
        public async Task<HttpResponseMessage> RegionByType(int regionTypeId)
        {
            var data = await repo.GetRegionByType(regionTypeId);
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("subsector/{subSectorId}/sectors")]
          public async Task<HttpResponseMessage> GetAllSectorsBySubSectorId(short subSectorId)
        {
            try
            {
                var data = await repo.GetSectorsBySubSectorId(subSectorId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("subsectors")]
          public async Task<HttpResponseMessage> GetAllSubSectors()
        {
            try
            {
                var data = await repo.GetAllSubSectors();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        


        //[HttpGet]
        //[Route("casa/account-status")]
        //  public async Task<HttpResponseMessage> GetCasaAccountStatus()
        //{
        //     
        //     
        //    {
        //        try
        //        {
        //            var data = repo.GetCasaAccountStatus();
        //            if (data == null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
        //            }
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList() });
        //        }
        //        catch (SecureException ex)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
        //        }
        //         
        //    });
        //}




        //[HttpGet("productgroups", Name = "GroupGet")]
        //public IActionResult GetProductGroup()
        //{
        //    try
        //    {
        //        var productGroups = repo.GetAllProductGroup();
        //        return Ok(productGroups);
        //    }
        //    catch (SecureException ex)
        //    {
        //        return BadRequest(new { error = true, message = TranslateHelper.get(ex.Message) });
        //    }
        //}

        //[HttpPost("productgroup/add")]
        //public IActionResult SaveProductGroup([FromBody]ProductGroupViewModel model)
        //{
        //    try
        //    {
        //        if (repo.SaveProductGroup(model))
        //        {
        //            return Created("", model);
        //        }
        //    }
        //    catch (SecureException ex)
        //    {
        //        return BadRequest();
        //    }

        //    return BadRequest();
        //}

        #endregion General Setups


        [HttpGet]
        [ClaimsAuthorization]
        [Route("profile-business-unit")]
          public async Task<HttpResponseMessage> GetProfileBusinessUnits()
        {
            IEnumerable<ProfileBusinessUnitViewModel> response = await repo.GetProfileBusinessUnits();
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, count = response.Count() });
        }
    }

}