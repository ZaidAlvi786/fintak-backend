using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.ViewModels.Credit;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using System.Threading.Tasks;
using FintrakBanking.APICore.core;
using FintrakBanking.Common;
using System.Threading;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/upload")]
    public class GenericDocumentUploadController : ApiControllerBase
    {
        TokenDecryptionHelper token = new TokenDecryptionHelper();
        private ILoanDocumentRepository repo;

        public GenericDocumentUploadController(ILoanDocumentRepository repo)
        {
            this.repo = repo;

        }

         [HttpPost] [ClaimsAuthorization]
        [Route("upload-document")]
        public async Task<HttpResponseMessage> UploadDocument()
        {
            //try
            //{
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

            int uploadType;
                if (!Int32.TryParse(provider.FormData["documentTypeId"], out uploadType))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("Upload Type is invali"));
                }

                LoanDocumentViewModel entity = populateViewModel(provider, uploadType);

                if (!provider.FileStreams.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("No file uploaded"));
                }

                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.createdBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var file = provider.Contents.FirstOrDefault();
                var buffer = await file.ReadAsByteArrayAsync();

                //file size
                int fileSize = buffer.Length;
                if (fileSize > (1048576))//max file size should come from database
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false , result = TranslateHelper.get("Exceed File Size. 3MB Maximum size is allowed") });
                }

                int response = repo.uploadDocument(entity, buffer);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = response != 1, result = response });
            //}
            //catch (SecureException ex)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.InnerException}" });
            //}
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("get-uploaded-document")]
        public async Task<HttpResponseMessage> GetUploadedDocument(LoanDocumentViewModel model)
        {
            try
            {
                var response = repo.getUploadedDocument(model);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("get-all-document-uploads")]
        public HttpResponseMessage GetAllUploadedDocument(LoanDocumentViewModel model)
        {
            try
            {
                var response = repo.getListOfUploadedDocument(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("get-all-document-operations-uploads")]
        public HttpResponseMessage GetAllUploadedOperationsDocument(LoanDocumentViewModel model)
        {
            try
            {
                var response = repo.getListOfUploadedOperationsDocument(model, (int)model.operationReviewId);
                //var response = repo.getListOfUploadedDocument(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        //[HttpPost] [ClaimsAuthorization]
        //[Route("get-all-document-uploads")]
        //public async Task<HttpResponseMessage> GetAllUploadedDocument(LoanDocumentViewModel model)
        //{
        //    try
        //    {
        //        var response = repo.getListOfUploadedDocument(model);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
        //    }
        //}

        [HttpPost] [ClaimsAuthorization]
        [Route("delete-uploaded-document")]
        public async Task<HttpResponseMessage> DeleteUploadedDocument(LoanDocumentViewModel model)
        {
            try
            {
                var response = repo.DeleteUploadedDocument(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, error = ex.InnerException, message = TranslateHelper.get(ex.Message) });
            }
        }

        private static LoanDocumentViewModel populateViewModel(MultipartFormDataMemoryStreamProvider provider, int uploadType)
        {
            var entity = new LoanDocumentViewModel
            {
                loanApplicationNumber = provider.FormData["loanApplicationNumber"],
                loanReferenceNumber = provider.FormData["loanReferenceNumber"],
                documentTitle = provider.FormData["documentTitle"],
                documentTypeId = (short)uploadType,
                fileName = provider.FormData["fileName"],
                fileExtension = provider.FormData["fileExtension"],
                physicalFileNumber = provider.FormData["physicalFileNumber"],
                physicalLocation = provider.FormData["physicalLocation"],
                SourceId = Convert.ToInt32( provider.FormData["SourceId"]),
                databaseTable = Convert.ToInt32(provider.FormData["databaseTable"]),
                conditionId = Convert.ToInt32(provider.FormData["conditionId"]),
                loanApplicationId = Convert.ToInt32(provider.FormData["loanApplicationId"]),
                checkListDefinitionId = Convert.ToInt32(provider.FormData["checkListDefinitionId"]),
                loanDetailId = Convert.ToInt32(provider.FormData["loanDetailId"]),
                collateralCustomerId = Convert.ToInt32(provider.FormData["collateralCustomerId"]),
                documentCode = provider.FormData["documentCode"],
                jobRequestCode = provider.FormData["jobRequestCode"],
                customerCode = provider.FormData["customerCode"],
                customerId = Convert.ToInt32(provider.FormData["customerId"]),
                staffCode = provider.FormData["staffCode"],
                overwrite = provider.FormData["overwrite"] == "true",
            };
            return entity;
        }
    }
}
