using FintrakBanking.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Customer
{
   public interface IKYCDocumentUploadRepository
    {
       Task<bool> DeleteConditionDocumentBydocumentId(int documentId, int staffId);
        Task<IEnumerable<ConditionsPrecedentUploadViewModel>> GetDeletedLoanConditionDocumentByContionId(int conditionId);
        IEnumerable<CustomerDocumentUploadViewModel> GetKYCDocumentUploadByCustomerId(int customerId);
        Task<bool> KYCDocumentUpload(CustomerDocumentUploadViewModel model, byte[] file);

        Task<CheckListDocumentUploadViewModel> CheckListDocumentUploadViewModel(int definitionId, int statusId, int detailId, bool isProductBased, int? customerId = null, int? checklistItemId = null, int? checkListTypeId = null, DateTime? checklistDate = null);
        Task<int> CheckListDocumentUpload(CheckListDocumentUploadViewModel model, byte[] file);
        bool RemoveCheckListDocument(int definitionId, int statusId, int detailId, bool isProductBased);

        Task<ConditionsPrecedentUploadViewModel> GetLoanConditionDocumentBydocumentId(int documentId);
        Task<ConditionsPrecedentUploadViewModel> GetLoanConditionDocumentByConditionId(int conditionId, int loanApplicationId);
        IEnumerable<ConditionsPrecedentUploadViewModel> GetLoanConditionDocumentByContionId(int conditionId);
        Task<bool> ConditionsPrecedentDocumentUpload(ConditionsPrecedentUploadViewModel model, byte[] file);
        bool RemoveConditionPrecedentDocument(int conditionId, int loanApplicationId);
    }
}
