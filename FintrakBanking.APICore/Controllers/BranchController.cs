using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/setups")]
    public class BranchController : ApiControllerBase
    {
        private readonly IBranchRepository _repo;
        readonly TokenDecryptionHelper _token = new TokenDecryptionHelper();

        public BranchController(IBranchRepository repo)
        {
            this._repo = repo;
        }

        #region Region Setup
        [HttpGet] [ClaimsAuthorization]  
        [Route("region")]
        public async Task<HttpResponseMessage> GetRegion()
        {
            try
            {
                var branchRegionViewModels = await _repo.GetAllRegion();
                var data = branchRegionViewModels;
                var regionViewModels = data as BranchRegionViewModel[] ?? data.ToArray();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = regionViewModels, count = regionViewModels.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
         [HttpPost] [ClaimsAuthorization]
        [Route("region")]
        public async Task<HttpResponseMessage> AddBranchRegion([FromBody]BranchRegionViewModel entity)
        {
            try
            {
                string createUpdate = "";
                if (entity.regionId != 0 || entity.regionId < 0)
                {
                    createUpdate = "updated";
                }
                else
                {
                    createUpdate = "created";
                    if (await  _repo.ValidateRegionName(entity.regionName.Trim()))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                      new { success = false, message = $"{TranslateHelper.get("Region with name")} {entity.regionName} {TranslateHelper.get("already exist")}." });
                    }
                }
              
                entity.companyId = _token.GetCompanyId;
                entity.userBranchId = (short)_token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = _token.GetStaffId;

                var data = _repo.AddUpdateBranchRegion(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = $"{TranslateHelper.get("The record has been")} {createUpdate} {TranslateHelper.get("successfully")}" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"There was an error {createUpdate} this record" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("region-staff")]
        public async Task<HttpResponseMessage> GetAllRegionStaff(int regionId)
        {
            try
            {
                var branchRegionStaffViewModels = await _repo.GetAllRegionStaff(regionId);
                var data = branchRegionStaffViewModels;
                var regionStaffViewModels = data as BranchRegionStaffViewModel[] ?? data.ToArray();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = regionStaffViewModels, count = regionStaffViewModels.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("region-staff")]
        public HttpResponseMessage AddUpdateBranchRegionStaff([FromBody]BranchRegionStaffViewModel entity)
        {
            try
            {
                entity.companyId = _token.GetCompanyId;
                entity.userBranchId = (short)_token.GetBranchId;
                entity.applicationUrl = CommonHelpers.GetLocalIpAddress();//HttpContext.Current.Request.Path;
                entity.createdBy = _token.GetStaffId;

                var data = _repo.AddUpdateBranchRegionStaff(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = $"{TranslateHelper.get("Operation successfull")}" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("There was an error on this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("region-staff-type")]
        public async Task<HttpResponseMessage> GetAllRegionStaffType()
        {
            try
            {
                var branchRegionStaffTypeViewModels = await _repo.GetAllRegionStaffType();
                var data = branchRegionStaffTypeViewModels;
                var regionStaffTypeViewModels = data as LookupViewModel[] ?? data.ToArray();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = regionStaffTypeViewModels, count = regionStaffTypeViewModels.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("region-staff/{id}")]
        public async Task<HttpResponseMessage> DeleteBranchRegionStaffAsync(short id)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = _token.GetBranchId,
                    companyId = _token.GetCompanyId,
                    staffId = _token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                };
                var region = await _repo.DeleteBranchRegionStaff(id, user);
                if (region)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = region, message = TranslateHelper.get("Record Deleted successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("There was an error on this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }



        #endregion

        #region Branch Setup

        [HttpGet] [ClaimsAuthorization]  
        [Route("branch")]
        public async Task<HttpResponseMessage> GetBranch()
        {
            try
            {
                var data = await _repo.GetAllBranch();
                var branchViewModels = data as BranchViewModel[] ?? data.ToArray();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = branchViewModels, count = branchViewModels.Count() });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("branch/{id}")]
        public HttpResponseMessage GetBranch(short id)
        {
            try
            {
                var branch = _repo.GetBranch(id);
                return Request.CreateResponse<BranchViewModel>(HttpStatusCode.OK, branch);
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("branch-by-company/{companyid}")]
        public HttpResponseMessage GetAllBranchByCompany(int companyid)
        {
            try
            {
                var branch = _repo.GetAllBranchByCompanyId(companyid);
                return Request.CreateResponse<List<BranchViewModel>>(HttpStatusCode.OK, branch.ToList());
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("branch/company")]
        public HttpResponseMessage GetBranchByCompany()
        {
            try
            {
                var branch = _repo.GetAllBranchByCompanyId(_token.GetCompanyId);
                return Request.CreateResponse<List<BranchViewModel>>(HttpStatusCode.OK, branch.ToList());

            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("branch")]
        public async Task<HttpResponseMessage> AddBranchAsync([FromBody]AddBranchViewModel model)
        {
            try
            {
                model.createdBy = _token.GetStaffId;
                model.userBranchId = (short)_token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.companyId = _token.GetCompanyId;
                var result = await _repo.AddBranch(model);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = result, message = "Branch has been created successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = "There was an error saving this record, Branch Name Exist" });

            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

       [HttpPut] [ClaimsAuthorization]
        [Route("branch/{id}")]
        public async Task<HttpResponseMessage> UpdateBranchAsync([FromBody] BranchViewModel model, short id)
        {

            try
            {
                model.createdBy = _token.GetStaffId;
                model.userBranchId = (short)_token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.companyId = _token.GetCompanyId;
                var result = await _repo.UpdateBranch(model, id);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = result, message = "Branch has been updated successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }

        }


       [HttpPut] [ClaimsAuthorization]
        [Route("branches/{id}")]
        public HttpResponseMessage UpdateBranch([FromBody] BranchViewModel model, short id)
        {

            try
            {
                model.createdBy = _token.GetStaffId;
                model.userBranchId = (short)_token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.companyId = _token.GetCompanyId;
                var result =  _repo.UpdateBranches(model, id);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = result, message = "Changes saved successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "Saved changes not successfull" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("branch/{id}")]
        public async Task<HttpResponseMessage> DeleteBranchAsync([FromBody] short id)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = _token.GetBranchId,
                    companyId = _token.GetCompanyId,
                    staffId = _token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                };
                var branch = await _repo.DeleteBranch(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, Ok(branch));
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("branch-search")]
        public HttpResponseMessage SearchForBranch(string searchQuery)
        {
            
                var data = _repo.GetSearchedBranch(searchQuery);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, result = data });
           
        }

        #endregion Branch Setup

        [HttpGet]
        [ClaimsAuthorization]
        [Route("collection-retail-cron-setup")]
        public async Task<HttpResponseMessage> GetCollectionRetailCronJobSetup()
        {
                var data = await _repo.GetCollectionRetailCronJobSetup();
                
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "" });
                }
            
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("collection-retail-computation-variables-setup")]
        public async Task<HttpResponseMessage> GetCollectionRetailComputationVariablesSetup()
        {
            var data = await _repo.GetCollectionRetailComputationVariablesSetup();

            if (data != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "" });
            }

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("add-collection-retail-cron-setup")]
        public async Task<HttpResponseMessage> AddRetailCollectionCronJobAsync([FromBody]CollectionsRetailCronSetupViewModel model)
        {
            try
            {
                model.createdBy = _token.GetStaffId;
                model.userBranchId = (short)_token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.companyId = _token.GetCompanyId;
                var result = await _repo.AddRetailCollectionCronJobAsync(model);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = result, message = TranslateHelper.get("Cron setup has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = "There was an error saving this record" });

            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("add-collection-retail-computation-variable-setup")]
        public async Task<HttpResponseMessage> AddRetailCollectionComputationVariableAsync([FromBody]CollectionsRetailComputationVariableSetupViewModel model)
        {
            try
            {
                model.createdBy = _token.GetStaffId;
                model.userBranchId = (short)_token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.companyId = _token.GetCompanyId;
                var result = await _repo.AddRetailCollectionComputationVariableAsync(model);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = result, message = TranslateHelper.get("Computation setup has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = "There was an error saving this record" });

            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("collection-retail-cron-setup/{id}")]
        public HttpResponseMessage UpdateCollectionRetailCronJob([FromBody] CollectionsRetailCronSetupViewModel model, short id)
        {

            try
            {
                model.createdBy = _token.GetStaffId;
                model.userBranchId = (short)_token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.companyId = _token.GetCompanyId;
                var result = _repo.UpdateCollectionRetailCronJob(model, id);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = result, message = TranslateHelper.get("Changes saved successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Saved changes not successfull") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }

        }


        [HttpPut]
        [ClaimsAuthorization]
        [Route("collection-retail-computation-variable-setup/{id}")]
        public HttpResponseMessage UpdateCollectionRetailComputationVariables([FromBody] CollectionsRetailComputationVariableSetupViewModel model, short id)
        {

            try
            {
                model.createdBy = _token.GetStaffId;
                model.userBranchId = (short)_token.GetBranchId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.companyId = _token.GetCompanyId;
                var result = _repo.UpdateCollectionRetailComputationVariables(model, id);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = result, message = TranslateHelper.get("Changes saved successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Saved changes not successfull") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }

        }



        [HttpDelete]
        [ClaimsAuthorization]
        [Route("collection-retail-cron-setup/{id}")]
        public async Task<HttpResponseMessage> DeleteRetailCollectionCronJobAsync([FromUri] short id)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = _token.GetBranchId,
                    companyId = _token.GetCompanyId,
                    staffId = _token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                };
                var collection = await _repo.DeleteRetailCollectionCronJobAsync(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = collection, message = TranslateHelper.get("Successfully deleted") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }


        [HttpDelete]
        [ClaimsAuthorization]
        [Route("collection-retail-computation-variables-setup/{id}")]
        public async Task<HttpResponseMessage> DeleteRetailCollectionComputationVariablesAsync([FromUri] short id)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = _token.GetBranchId,
                    companyId = _token.GetCompanyId,
                    staffId = _token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                };
                var collection = await _repo.DeleteRetailCollectionComputationVariablesAsync(id, user);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = collection, message = TranslateHelper.get("Successfully deleted") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }


    }

}