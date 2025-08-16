using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.Credit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface ICreditTemplateRepository
    {
        Task<List<LoadedDocumentSectionViewModel>> GetLoadedExceptionDocumentation(int staffId, int operationId, int targetId, UserInfo user);
        Task<LoadedDocumentSectionViewModel> GetExceptionDocumentSection(int staffId, int operationId, int targetId, int sectionId);
        Task<List<LoadedDocumentSectionViewModel>> getRecoveryAnalysisDocumentation(int staffId, int operationId, int targetId, string referenceId, UserInfo user, int templateId);
        Task<LoadedDocumentSectionViewModel> GetRecoveryAnalysisDocumentSection(int staffId, int operationId, int targetId, string referenceId, int sectionId);
        Task<bool> LoadDocumentTemplateLMS(DocumentTemplateViewModel entity);
        Task<CreditTemplateViewModel> GetCreditTemplate(int creditTemplateId);

        IEnumerable<CreditTemplateViewModel> GetAllCreditTemplate();

        IEnumerable<CreditTemplateViewModel> GetAllCreditTemplateByLevelProduct(int levelId, int productId, int companyId);

        IEnumerable<CreditTemplateViewModel> GetAllCreditTemplateByProductClass(int productClassId, int staffId);

        IEnumerable<CreditTemplateViewModel> GetCreditTemplateByLevelId(int approvalLevelId, int companyId);

        Task<bool> AddCreditTemplate(CreditTemplateViewModel model);

        Task<bool> UpdateCreditTemplate(CreditTemplateViewModel model, int creditTemplateId);
        Task<bool> DeleteCreditTemplate(int creditTemplateId);
        //form CAM setup
        Task<IEnumerable<DocumentTemplateViewModel>> GetAllDocumentTemplateSetup();
        Task<bool> AddDocumentTemplate(DocumentTemplateViewModel model);
        Task<bool> UpdateDocumentTemplate(DocumentTemplateViewModel model, int documentTemplateId);
        Task<bool> DeleteDocumentTemplate(int documentTemplateId);
        IEnumerable<DocumentTemplateSectionViewModel> GetAllDocumentTemplateSectionSetup(int templateId);
        IEnumerable<DocumentTemplateSectionRoleViewModel> GetAllDocumentTemplateSectionRoleSetup(int templateSectionId);
        Task<bool> AddDocumentTemplateSection(DocumentTemplateSectionViewModel model);
        Task<bool> UpdateDocumentTemplateSection(DocumentTemplateSectionViewModel model, int documentTemplateId);
        Task<bool> DeleteDocumentTemplateSection(int documentTemplateId, short userBranchId, int companyId, int lastUpdatedBy, string applicationUrl, string userIPAddress);
        Task<bool> AddDocumentTemplateSectionRole(DocumentTemplateSectionRoleViewModel model);
        Task<bool> UpdateDocumentTemplateSectionRole(DocumentTemplateSectionRoleViewModel model);
        Task<bool> DeleteDocumentTemplateSectionRole(int sectionRoleId, short userBranchId, int companyId, int lastUpdatedBy, string applicationUrl, string userIPAddress);

        // form CAM impl
        Task<List<LoadedDocumentSectionViewModel>> GetLoadedDocumentSections(int staffId, int operationId, int targetId);
        List<LoadedDocumentSectionViewModel> GetLoadedDocumentation(int staffId, int operationId, int targetId, UserInfo user,bool isThirdPartyFacility = false);
        Task<List<LoadedDocumentSectionViewModel>> GetLoadedDocumentationGeneric(int staffId, int operationId, int targetId, int targetIdForWorkFlow, UserInfo user, int customerId);
        Task<bool> LoadDocumentTemplate(DocumentTemplateViewModel entity);
        Task<bool> SaveLoadedDocumentSection(LoadedDocumentSectionViewModel entity);
        LoadedDocumentSectionViewModel GetDocumentSection(int staffId, int operationId, int targetId, int sectionId, int customerId = 0, int targetIdForWorkFlow = 0, bool isGeneric = false);
        Task<LoadedDocumentSectionViewModel> GetThirdPartyLoanDocumentSection(int staffId, int operationId, int targetId, int sectionId);


        Task<List<DocumentTemplateViewModel>> GetDocumentTemplates(int staffId, int operationId, int companyId);
        Tuple<bool, decimal> GetIsLegalLendingLimitViolated(int operationId, int targetId);
        bool SaveApprovedDocumentation(int staffId, int operationId, int targetId);
        List<LoadedDocumentSectionViewModel> GetSavedDocumentation(int operationId, int targetId);
        Task<LoadedDocumentSectionViewModel> GetDocumentSectionBulkLiquidation(int staffId, int operationId, int targetId, int sectionId);
        Task<List<LoadedDocumentSectionViewModel>> GetLoadedDocumentBulkLiquidation(int staffId, int operationId, int targetId, UserInfo user);
        Task<List<LoadedDocumentSectionViewModel>> GetLoadedDocumentationBulkLiquidation(int staffId, int operationId, int targetId, UserInfo user);
        Task<InsurancePolicy> GetInsurancePolicyConfirmationStatus(int staffId, int appDetailId);
        Task<InsurancePolicyRecordViewModel> GetInsurancePolicyConfirmationStatusByAppDetailId(int staffId, int appDetailId);
    }
}
