using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using System.Globalization;
using System.Threading.Tasks;
using FintrakBanking.Common;
using System.Threading;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FintrakBanking.APICore.Controllers
{
    //[EnableCors("AllDomain")]
    [RoutePrefix("api/v1/setups")]
    public class CompanyController : ApiControllerBase
    {
        private ICompanyRepository repo;
        TokenDecryptionHelper token = new TokenDecryptionHelper();
        public CompanyController(ICompanyRepository _repo)
        {
            this.repo = _repo;
        }

        //[HttpGet]
        //[Route("")]
        //public HttpResponseMessage GetAllCompany()
        //{
        //    try
        //    {
        //        var companys = repo.GetAllCompany().ToList();
        //        if (companys == null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK,
        //               new { success = false, result = companys, message = TranslateHelper.get("No Record Found") });
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            success = true,
        //            result = companys

        //        });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
        //    }

        //}


        [HttpGet]
        [ClaimsAuthorization]
        [Route("company")]
        public HttpResponseMessage GetCompanies() 
        {
            try
            {
                var companys = repo.GetCompanies().ToList();
                if (companys == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, result = companys, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = true,
                    result = companys

                });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-login-company")]
        public HttpResponseMessage GetLoginCompany()
        {
            try
            {
                var company = repo.GetCompanyViewModel(token.GetCompanyId);
                if (company == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                           new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, result = company });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("company/{companyId}")]
        public HttpResponseMessage Get(int companyId)
        {
            try
            {
                var company = repo.GetCompanyViewModel(companyId);
                if (company == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                           new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, result = company });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("languages")]
        public async Task<HttpResponseMessage> GetLanguages()
        {
            try
            {
                var data = await repo.GetLanguages();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                           new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("nature-of-business")]
        public HttpResponseMessage GetNatureOfBusiness()
        {
            try
            {
                var data = repo.GetNatureOfBusiness();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                           new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        // POST api/values
        /*[HttpPost]
        [ClaimsAuthorization]
        [Route("company")]
        public HttpResponseMessage AddCompany([FromBody] CompanyViewModel model)
        {
            try
            {
                var data = repo.AddCompany(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                               new { success = true, message = "company has been created successfully" });
                }
                else
                    return Request.CreateResponse(HttpStatusCode.OK,
                               new { success = false, message = "company not created" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }*/


        [HttpPost]
        [ClaimsAuthorization]
        [Route("company")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> AddCompany()
        {

            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, TranslateHelper.get("Unsupported media type"));
            }

            MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
            Task.Factory
                .StartNew(() => provider = Request.Content.ReadAsMultipartAsync(provider).Result,
                    CancellationToken.None,
                    TaskCreationOptions.LongRunning, // guarantees separate thread
                    TaskScheduler.Default)
                .Wait();

            if (!provider.FileStreams.Any())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("No file uploaded."));
            }

            var entity = new CompanyViewModel();
            entity.fileName = provider.FormData["fileName"];
            entity.fileExtension = provider.FormData["fileExtension"];
            entity.imagePath = provider.FormData["imagePath"];
            entity.companyName = provider.FormData["companyName"];
            entity.address = provider.FormData["address"];
            entity.telephone = provider.FormData["telephone"];
            entity.email = provider.FormData["email"];
            entity.languageId = Convert.ToInt16(provider.FormData["languageId"]);

            var dateOfIncorp = provider.FormData["dateOfIncorporation"];
            var dateOfIncorp2 = dateOfIncorp.Substring(0, 15);
            entity.dateOfIncorporation = DateTime.ParseExact(dateOfIncorp2, "ddd MMM dd yyyy", CultureInfo.InvariantCulture);

            entity.countryId = Convert.ToInt32(provider.FormData["countryId"]);
            entity.currencyId = Convert.ToInt16(provider.FormData["currencyId"]);
            entity.natureOfBusinessId = Convert.ToInt16(provider.FormData["natureOfBusinessId"]);
            entity.nameOfScheme = provider.FormData["nameOfScheme"];
            entity.functionsRegistered = provider.FormData["functionsRegistered"];

            var authorisedShareCap = provider.FormData["authorisedShareCapital"] == "" ? "0.0" : provider.FormData["authorisedShareCapital"];
            entity.authorisedShareCapital = decimal.Parse(authorisedShareCap);


            entity.nameOfRegistrar = provider.FormData["nameOfRegistrar"];
            entity.nameOfTrustees = provider.FormData["nameOfTrustees"];
            entity.formerManagersTrustees = provider.FormData["formerManagersTrustees"];

            var dateOfRenewalOfReg = provider.FormData["dateOfRenewalOfRegistration"];
            if (dateOfRenewalOfReg == "")
            {
                entity.dateOfRenewalOfRegistration = null;

            }
            else
            {
                var dateOfRenewalOfReg2 = dateOfRenewalOfReg.Substring(0, 15);
                entity.dateOfRenewalOfRegistration = DateTime.ParseExact(dateOfRenewalOfReg, "ddd MMM dd yyyy", CultureInfo.InvariantCulture);
            }

            var dateOfCommence = provider.FormData["dateOfCommencement"];
            if (dateOfCommence == "")
            {
                entity.dateOfCommencement = null;

            }
            else
            {
                var dateOfCommence2 = dateOfCommence.Substring(0, 15);
                entity.dateOfCommencement = DateTime.ParseExact(dateOfCommence2, "ddd MMM dd yyyy", CultureInfo.InvariantCulture);
            }

            entity.initialFloatation = Convert.ToInt32(provider.FormData["initialFloatation"]);
            entity.initialSubscription = Convert.ToInt32(provider.FormData["initialSubscription"]);
            entity.registeredBy = provider.FormData["registeredBy"];
            entity.parentId = Convert.ToInt32(provider.FormData["parentId"]);
            entity.website = provider.FormData["website"];
            entity.trusteesAddress = provider.FormData["trusteesAddress"];
            entity.investmentObjective = provider.FormData["investmentObjective"];  
            
            entity.userBranchId = (short)token.GetBranchId;
            entity.userIPAddress = HttpContext.Current.Request.UserHostAddress;
            entity.applicationUrl = HttpContext.Current.Request.Path;
            entity.createdBy = token.GetStaffId;
            entity.companyId = token.GetCompanyId;

            var file = provider.Contents.FirstOrDefault();
            var buffer = await file.ReadAsByteArrayAsync();
            bool response = repo.AddCompany(entity, buffer);


            if (response) return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = TranslateHelper.get("Company has been created successfully") });
            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("company not created") });

        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("company/{companyId}")]
        public HttpResponseMessage UpdateCompany(int companyId, [FromBody] CompanyViewModel model)
        {
            try
            {
                var data = repo.UpdateCompany(companyId, model);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, message = TranslateHelper.get("company has been updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("company has not been updated successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpPut]
        [ClaimsAuthorization]
        [Route("companys/{companyId}")]
        public HttpResponseMessage UpdateCompanies(int companyId, [FromBody] CompanyViewModel model)
        {
            try
            {
                var data = repo.UpdateCompanies(companyId, model);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, message = TranslateHelper.get("Changes Saved successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("Saved changes not successfull") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("single/obligor-limit/{Id}")]

        public HttpResponseMessage UpdateSingleObligorLimit(int Id, [FromBody] CompanyViewModel model)
        {
            try
            {
                var data = repo.UpdateSingleObligorLimit(Id, model);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, message = TranslateHelper.get("Changes Saved successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("Saved changes not successfull") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        #region Company Director
        [HttpGet]
        [ClaimsAuthorization]
        [Route("company-director")]
        public async Task<HttpResponseMessage> GetCompanyDirectors()
        {
            try
            {
                var data = await repo.GetCompanyDirectors();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("company-director-by-companyId")]
        public async Task<HttpResponseMessage> GetCompanyDirectorsByCompanyId()
        {
            try
            {
                var data = await repo.GetCompanyDirectorsByCompanyId(token.GetCompanyId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("company-director-by-companyId")]
        public async Task<HttpResponseMessage> GetCustomerCompanyDirectorsByCompanyId(int companyId)
        {
            try
            {
                var data = await repo.GetCustomerCompanyDirectorsByCompanyId(companyId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                       new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("company-director")]
        public HttpResponseMessage AddUpdateCompanyDirector([FromBody]CompanyDirectorsViewModel entity)
        {
            try
            {
                string createUpdate = "";
                if (entity.companyDirectorId != 0 || entity.companyDirectorId > 0)
                {
                    createUpdate = "updated";
                }
                else
                {
                    createUpdate = "created";
                    if (repo.ValidateCompanyDirectorBVN(entity.companyId, entity.bvn))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                            new { success = false, message = $"{TranslateHelper.get("Company Director with BVN")} {entity.bvn} {TranslateHelper.get("already exist for the select company")}." });
                    }
                    if (repo.ValidateCompanyDirectorEmail(entity.companyId, entity.email))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                                new { success = false, message = $"{TranslateHelper.get("Company Director with email")} {entity.email} {TranslateHelper.get("already exist for the select company")}." });
                    }
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.createdBy = token.GetStaffId;

                var data = repo.AddUpdateCompanyDirector(entity);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data, message = $"{TranslateHelper.get("The record has been")} {createUpdate} {TranslateHelper.get("successfully")}" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("There was an error")} {createUpdate} {TranslateHelper.get("this record")}" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }

        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("company-director")]
        public HttpResponseMessage DeleteCustomer(int companyDirectorId)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                };

                var data = repo.DeleteCompanyDirector(companyDirectorId, user);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                          new { success = true, message = TranslateHelper.get("The record has been deleted successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("There was an error deleted this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = $"{TranslateHelper.get("There was an error deleted this record")} {e.Message}" });
            }
        }

        #endregion
    }
}