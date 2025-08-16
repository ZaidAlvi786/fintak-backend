using FintrakBanking.APICore.core;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.ViewModels;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.ViewModels.Credit;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common;
using System.Threading;

namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/credit")]
    public class LoanDocumentController : ApiControllerBase
    {
        TokenDecryptionHelper token = new TokenDecryptionHelper();
        private ILoanDocumentRepository repo;

        public LoanDocumentController(ILoanDocumentRepository repo)
        {
            this.repo = repo;
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("loan-document/application/{applicationNumber}")]
        public HttpResponseMessage GetLoanDocumentByApplication(string applicationNumber)
        {
            try
            {
                var data = repo.GetApplicationLoanDocument(applicationNumber);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("loan-document-appNo-refNo/")]
        public HttpResponseMessage GetLoanDocumentByApplicationNumberRefno(string refNo, string applicationNumber)
        {
            try
            {
                var data = repo.GetLoanDocumentByAppNoRefNo(refNo, applicationNumber);
                if(data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("loan-document/{loanDocumentId}")]
        public HttpResponseMessage GetLoanDocument(int loanDocumentId)
        {
            try
            {
                var data = repo.GetLoanDocument(loanDocumentId);

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

        //[HttpGet]
        //[Route("loan-document/company")]
        //public HttpResponseMessage GetLoanDocumentByCompanyId()
        //{
        //    try
        //    {
        //        var data = repo.GetAllLoanDocumentByCompanyId(token.GetCompanyId);

        //        if (data == null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
        //    }
        //}

         [HttpPost] [ClaimsAuthorization]
        [Route("loan-document")]
        public async Task<HttpResponseMessage> AddLoanDocument() // DEPRECATED
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

                var entity = new LoanDocumentViewModel
                {
                    loanApplicationNumber = provider.FormData["loanApplicationNumber"],
                    loanReferenceNumber = provider.FormData["loanReferenceNumber"],
                    documentTitle = provider.FormData["documentTitle"],
                    documentTypeId = (short)uploadType,
                    //SourceId = Convert.ToInt32( provider.FormData["sourceId"]),
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
                    physicalFileNumber = provider.FormData["physicalFileNumber"],
                    physicalLocation = provider.FormData["physicalLocation"],
                    isPrimaryDocument = provider.FormData["isPrimaryDocument"] == "true",
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
                var data = repo.AddLoanDocument(entity, buffer);

                if (data == 2)
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

       [HttpPut] [ClaimsAuthorization]
        [Route("loan-document/{loanDocumentId}")]
        public HttpResponseMessage UpdateLoanDocument([FromBody] LoanDocumentViewModel entity, int loanDocumentId)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = repo.UpdateLoanDocument(entity, loanDocumentId);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = TranslateHelper.get("The record has been updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("loan-document/applicationRefNum/{referenceNumber}")]
        public HttpResponseMessage GetLoanDocumentByApplicationReferenceNum(string referenceNumber)
        {
            try
            {
                var data = repo.GetLoanDocumentByReferenceNumber(referenceNumber).ToList();
                if (data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

        [HttpDelete] [ClaimsAuthorization]
        [Route("loan-document-delete/")]
        public HttpResponseMessage DeleteLoanDocument(string invoiceNo, string applicationNumber)
        {
            try
            {
                var data = repo.DeleteLoanDocument(invoiceNo, applicationNumber);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("Record has been deleted successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Record could not be deleted") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message), error = ex.InnerException, stack = ex.StackTrace });
            }
        }


        #region CREDIT BUREAU REPORT

        [HttpGet] [ClaimsAuthorization]  
        [Route("credit-bureau-report/{customerCreditBureauId}")]
        public HttpResponseMessage GetCreditBureauReportDocument(int customerCreditBureauId)
        {
            try
            {
                var data = repo.GetCreditBureauReportDocument(customerCreditBureauId).ToList();
                if (data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


      [HttpGet] [ClaimsAuthorization]  
        [Route("credit-bureau-report/{customerCreditBureauId}/{documentId}")]
        public HttpResponseMessage GetCreditBureauReportDocumentByDocumentID(int customerCreditBureauId, int documentId)
        {
            try
            {
                var data = repo.GetCreditBureauReportDocumentByDocumentID(customerCreditBureauId, documentId);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


         [HttpPost] [ClaimsAuthorization]
        [Route("credit-bureau-report")]
        public async Task<HttpResponseMessage> AddCreditBureauReportDocument()
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

                var entity = new LoanDocumentViewModel
                {
                    customerCreditBureauId = Convert.ToInt32(provider.FormData["customerCreditBureauId"]),
                    documentTitle = provider.FormData["documentTitle"],
                    documentTypeId = (short)uploadType,
                    //SourceId = Convert.ToInt32( provider.FormData["sourceId"]),
                    fileName = provider.FormData["fileName"],
                    fileExtension = provider.FormData["fileExtension"],
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
                var data = repo.AddCreditBureauReportDocument(entity, buffer);

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

       [HttpPut] [ClaimsAuthorization]
        [Route("credit-bureau-report/{documentId}")]
        public HttpResponseMessage UpdateCreditBureauReportDocument([FromBody] LoanDocumentViewModel entity, int documentId)
        {
            try
            {
                entity.userBranchId = (short)token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.lastUpdatedBy = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;

                var data = repo.UpdateCreditBureauReportDocument(entity, documentId);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = entity, message = TranslateHelper.get("The record has been updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

        #endregion

        #region COMMITTEE MINUTES

         [HttpPost] [ClaimsAuthorization]
        [Route("committee-minutes")]
        public async Task<HttpResponseMessage> AddCommitteDocument()
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

                var entity = new LoanDocumentViewModel
                {
                    loanApplicationNumber = provider.FormData["loanApplicationNumber"],
                    loanReferenceNumber = provider.FormData["loanReferenceNumber"],
                    documentTitle = provider.FormData["documentTitle"],
                    documentTypeId = (short)uploadType,
                    //SourceId = Convert.ToInt32( provider.FormData["sourceId"]),
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
                var data = repo.AddCommitteeDocument(entity, buffer);

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
        [Route("committee-minutes/application/{applicationNumber}")]
        public HttpResponseMessage GetCommitteeDocumentByApplication(string applicationNumber)
        {
            try
            {
                var data = repo.GetCommitteeDocument(applicationNumber);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }

      [HttpGet] [ClaimsAuthorization]  
        [Route("committee-minutes/{loanDocumentId}")]
        public HttpResponseMessage GetCommitteeDocument(int loanDocumentId)
        {
            try
            {
                var data = repo.GetCommitteeDocument(loanDocumentId);

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

        #endregion COMMITTEE MINUTES

        #region OPERATIONDOCUMENTATION 
        /*deprecated!!*/
        //[HttpGet]
        //[ClaimsAuthorization]
        //[Route("operation-documentation")]
        //public HttpResponseMessage GetAllPendingOperationDocumentation()
        //{
        //    try
        //    {
        //        var data = repo.GetAllPendingOperationDocumentation().ToList();
        //        if (data.Any())
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("No Record Found") });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
        //    }
        //}

        //[HttpGet]
        //[ClaimsAuthorization]
        //[Route("operation-documentation/deferral/{checker}")]
        //public HttpResponseMessage GetAllPendingDeferralDocumentation(bool checker)
        //{
        //    try
        //    {
        //        var data = repo.GetAllPendingDeferralDocumentation(checker).ToList();
        //        if (data.Any())
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("No Record Found") });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
        //    }
        //}

        //[HttpGet]
        //[ClaimsAuthorization]
        //[Route("operation-documentation-checker")]
        //public HttpResponseMessage GetAllPendingOperationDocumentationApproval()
        //{
        //    try
        //    {
        //        var data = repo.GetAllPendingOperationDocumentationApproval().ToList();
        //        if (data.Any())
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, message = TranslateHelper.get("No Record Found") });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
        //    }
        //}

        //[HttpPost]
        //[ClaimsAuthorization]
        //[Route("operation-documentation")]
        //public HttpResponseMessage AddOperationDocumentationApproval(OperationDocumentationViewModel param)
        //{

        //    param.createdBy = token.GetStaffId;
        //    try
        //    {
        //        var data = repo.AddOperationDocumentationApproval(param);
        //        if (data)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Documents sent for filing Approval" });
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data, message = "Documents not sent" });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get(ex.Message) });
        //    }
        //}

        #endregion OPERATIONDOCUMENTATION
    }
}
