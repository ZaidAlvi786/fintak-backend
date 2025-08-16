using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.Interfaces.Customer;
using FintrakBanking.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FintrakBanking.Common.CustomException;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.Common;
using System.Threading;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/kyc")]
    public class KYCDocumentUploadController : ApiControllerBase
    {
        TokenDecryptionHelper token = new TokenDecryptionHelper();
        private IKYCDocumentUploadRepository repo;

        public KYCDocumentUploadController(IKYCDocumentUploadRepository repo)
        {
            this.repo = repo;
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("document-upload/{customerId}")]
        public HttpResponseMessage GetLoanDocument(int customerId)
        {
            try
            {
                var data = repo.GetKYCDocumentUploadByCustomerId(customerId);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
         [HttpPost] [ClaimsAuthorization]
        [Route("document-upload")]
        public async Task<HttpResponseMessage> KYCDocumentUpload()
        {
            try
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

                int uploadType;
                if (!Int32.TryParse(provider.FormData["documentTypeId"], out uploadType))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("Upload Type is invalid"));
                }

                var entity = new CustomerDocumentUploadViewModel
                {
                    customerId = Convert.ToInt32(provider.FormData["customerId"]),
                    customerCode = provider.FormData["customerCode"],
                    documentTitle = provider.FormData["documentTitle"],
                    documentTypeId = (short)uploadType,
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                    physicalFileNumber = provider.FormData["physicalFileNumber"],
                    physicalLocation = provider.FormData["physicalLocation"],
                };

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
                var data = await repo.KYCDocumentUpload(entity, buffer);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.InnerException}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("checklist-upload/")]
        public async Task<HttpResponseMessage> GetLoanDocument(int definitionId, int statusId, int detailId, bool isProductBased = false, int? customerId = null, int? checkListItemId = null, int? checkListTypeId = null, DateTime? checklistDate=null)
        {
            try
            {
                var data = await repo.CheckListDocumentUploadViewModel(definitionId , statusId, detailId, isProductBased, customerId, checkListItemId, checkListTypeId, checklistDate);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
         [HttpPost] [ClaimsAuthorization]  
        [Route("checklist-upload-view/")]
        public async Task<HttpResponseMessage> GetLoanChecklistDocument(ChecklistSearchViewModel model)
        {
            try
            {
                var data = await repo.CheckListDocumentUploadViewModel(model.checkListDefinitionId, model.checkListStatusId, model.targetId, model.isproductbased, model.customerId, model.checkListItemId, model.checkListTypeId, model.checklistDate);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

         [HttpPost] [ClaimsAuthorization]
        [Route("checklist-document-upload")]
        public async Task<HttpResponseMessage> CheckListDocumentUpload()
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

            int uploadType;
                if (!Int32.TryParse(provider.FormData["checkListDefinitionId"], out uploadType))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("Upload Type is invalid"));
                }

                var entity = new CheckListDocumentUploadViewModel
                {

                    checkListDefinitionId = Convert.ToInt32(provider.FormData["checkListDefinitionId"]),
                    checkListStatusId = Convert.ToInt32(provider.FormData["checkListStatusId"]),
                    loanApplicationId = Convert.ToInt32(provider.FormData["loanApplicationId"]),
                    loanDetailsId = Convert.ToInt32(provider.FormData["loanDetailsId"]),
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                    physicalFileNumber = provider.FormData["physicalFileNumber"],
                    physicalLocation = provider.FormData["physicalLocation"],
                    overwrite = provider.FormData["overwrite"] == "true",

                };

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
                int response = await repo.CheckListDocumentUpload(entity, buffer);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = response != 1, result = response });

        }


        [HttpDelete] [ClaimsAuthorization]
        [Route("checklist-upload-delete/definitionId/{definitionId}/statusId/{statusId}/detailId/{detailId}/isProductBased/{isProductBased}")]
        public HttpResponseMessage RemoveChecklistDocument(int definitionId, int statusId, int detailId, bool isProductBased)
        {
            try
            {
                var data = repo.RemoveCheckListDocument(definitionId, statusId, detailId, isProductBased);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("Checklist removed successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Checklist not removed")});
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("condition-precedent-upload-delete")]
        public HttpResponseMessage RemoveConditionPrecedentDocument(int conditionId, int loanApplicationId)
        {
            try
            {
                var data = repo.RemoveConditionPrecedentDocument(conditionId, loanApplicationId);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("Record removed successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Record not removed") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet] [ClaimsAuthorization]  
        [Route("loan-conditions-precedent")]
        public HttpResponseMessage GetLoanConditionDocumentByConditionId(int conditionId)
        {
            try
            {
                var data = repo.GetLoanConditionDocumentByContionId(conditionId);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-conditions-precedent-deleted")]
        public async Task<HttpResponseMessage> GetDeletedLoanConditionDocumentByContionId(int conditionId)
        {
            try
            {
                var data = await repo.GetDeletedLoanConditionDocumentByContionId(conditionId);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpGet] [ClaimsAuthorization]  
        [Route("loan-conditions-precedent-upload/conditionId/{conditionId}/loanApplicationId/{loanApplicationId}")]
        public async Task<HttpResponseMessage> GetLoanConditionPrecedentUpload(int conditionId,int loanApplicationId)
        {
            try
            {
                var data = await repo.GetLoanConditionDocumentByConditionId(conditionId, loanApplicationId);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }
        [HttpGet] [ClaimsAuthorization]  
        [Route("loan-conditions-precedent-documentId/{documentId}")]
        public async Task<HttpResponseMessage> GetLoanConditionDocumentBydocumentId(int documentId)
        {
            try
            {
                var data = await repo.GetLoanConditionDocumentBydocumentId(documentId);

                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("delete-conditions-precedent-documentId/")]
        public async Task<HttpResponseMessage> DeleteConditionDocumentBydocumentId(int documentId)
        {
            try
            {
                bool data = await repo.DeleteConditionDocumentBydocumentId(documentId, token.GetStaffId);

                if (data == false)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Error deleting record") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("Record deleted successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpPost] [ClaimsAuthorization]
        [Route("loan-conditions-precedent-upload")]
        public async Task<HttpResponseMessage> ConditionsPrecedentDocumentUpload()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported media type.");
                }

                MultipartFormDataMemoryStreamProvider provider = new MultipartFormDataMemoryStreamProvider();
                Task.Factory
                    .StartNew(() => provider = Request.Content.ReadAsMultipartAsync(provider).Result,
                        CancellationToken.None,
                        TaskCreationOptions.LongRunning, // guarantees separate thread
                        TaskScheduler.Default)
                    .Wait();

                int uploadType;
                if (!Int32.TryParse(provider.FormData["conditionId"], out uploadType))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, TranslateHelper.get("Upload Type is invalid"));
                }

                var entity = new ConditionsPrecedentUploadViewModel
                {
                    conditionId = Convert.ToInt32(provider.FormData["conditionId"]),
                    loanApplicationId = Convert.ToInt32(provider.FormData["loanApplicationId"]),
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                    physicalFileNumber = provider.FormData["physicalFileNumber"],
                    physicalLocation = provider.FormData["physicalLocation"],
                };

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
                var data = await repo.ConditionsPrecedentDocumentUpload(entity, buffer);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {ex.InnerException}" });
            }
        }
    }
}
