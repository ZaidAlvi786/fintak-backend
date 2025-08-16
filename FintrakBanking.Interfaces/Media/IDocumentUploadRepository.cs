using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.General;

namespace FintrakBanking.Interfaces.Media
{
    public interface IDocumentUploadRepository
    {
        Task<bool> DeleteRecoveryDocumentUpload(int id);
        RecoveryReportingDocumentViewModel GetRecoveryReportDocument(int loanRecoveryReportApprovalId);
        IEnumerable<RecoveryReportingDocumentViewModel> getAllLoanRecoveryReportingDocuments(string referenceId);
        Task<int> AddRecoveryReportingDocumentUpload(RecoveryReportingDocumentViewModel model, byte[] buffer);
        IEnumerable<DocumentUploadViewModel> GetDocumentUploadsLmss(int staffId, int operationId, int targetId);
        DocumentUploadViewModel GetDocumentUpload(int id);
        DocumentUploadViewModel GetDocumentCreditBereau(int documentId);
        IEnumerable<DocumentUploadViewModel> GetDocumentUploads(int staffId);

        Task<int> AddDocumentUpload(DocumentUploadViewModel model, byte[] buffer);

        Task<bool> UpdateDocumentUpload(DocumentUploadViewModel model, int id, UserInfo user);

        Task<bool> DeleteDocumentUpload(int id, int documentTypeId, UserInfo user);
        DocumentUploadViewModel GetDocument(int documentId);
        IEnumerable<DocumentUploadViewModel> GetDocumentUploads(int getStaffId, int operationId, int targetId, bool isOperationSpecific);
        IEnumerable<DocumentUploadViewModel> GetDocumentUploadsLms(int getStaffId, int operationId, int targetId, bool isOperationSpecific, bool isLms = false);
        IEnumerable<DocumentUploadViewModel> GetDocumentDeleted(int staffId, int operationId, int targetId);
        IEnumerable<DocumentCategoryViewModel> GetDocumentCategories();
        IEnumerable<DocumentTypeViewModel> GetDocumentTypes(int id);
        Task<CustomerDocumentSearchViewModel> GetCustomerDocuments(DocumentUploadViewModel model, UserInfo user);
        //DocumentUploadViewModel GetUploadedDocument(DocumentUploadViewModel model);
        IEnumerable<DocumentUploadViewModel> GetDocumentUpload(IEnumerable<DocumentUploadViewModel> model);
    }
}
