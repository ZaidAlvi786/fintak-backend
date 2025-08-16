using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.CASA;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Customer;
using FintrakBanking.ViewModels.WorkFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Customer
{
    public interface ICustomerRepository
    {
        Task<CustomerViewModels> GetCustomer(int custormerId);
        IEnumerable<CustomerViewModels> GetAllProspectiveCustomer();
        Task<IEnumerable<CustomerViewModels>> GetCustomerInGroupByGroupId(int groupId);
        IEnumerable<CustomerViewModels> GetCustomerGeneralInfoByLoanId(int loanApplicationId);

        IEnumerable<CustomerViewModels> GetCustomerByBranchId(int branchId);

        IEnumerable<CustomerViewModels> GetCustomerByCompanyId(int companyId);

        Task<IEnumerable<CustomerTypeViewModels>> GetCustomerType();
        Task<IEnumerable<CorporateCustomerTypeViewModels>> GetCorporateCustomerType();
        Task<IEnumerable<CustomerTypeViewModels>> GetCustomerTypeWithHybrid();

        IEnumerable<CustomerSupplierTypeViewModels> GetClientSupplierType();

        IEnumerable<CustomerIdentificationModeTypeViewModels> GetIdentificationMode();

        IEnumerable<CompanyDirectorTypeViewModels> GetDirectorsTypes();

        IEnumerable<CustomerAddressTypeViewModels> GetCustomerAddressType();

        IEnumerable<CustomerRiskRatingViewModels> GetCustomerRiskRating();

        IEnumerable<CustomerViewModels> GetCustomerByTypeId(int customerTypeId);

        IEnumerable<GroupCustomerMembersViewModel> GetCustomerAndType(int custormerId);

        Task<dynamic> GetCustomerRating(int custormerId);

        string AddCustomer(CustomerViewModels entity);

        //Task<bool> UpdateCustomer(int customerId, CustomerViewModels entity);
        Task<bool> UpdateCustomer(int customerId, CustomerViewModels entity);

        Task<bool> DeleteCustomer(int customerId, UserInfo user);

        Task<bool> DeleteChild(int childId, UserInfo user);

        Task<bool> DeleteUltimateBeneficial(int companyBeneficialId, UserInfo user);

        Task<bool> AddCustomerIdentification(CustomerIdentificationViewModels entity);

        Task<bool> AddCustomerEmploymentHistory(CustomerEmploymentHistoryViewModels entity);

        Task<bool> AddCustomerBvn(CustomerBvnViewModels entity);

        Task<bool> AddCustomerClientSupplier(CustomerClientOrSupplierViewModels entity);

        Task<bool> AddCustomerCompanyDirector(CustomerCompanyDirectorsViewModels entity);

        Task<bool> AddCustomerAddresses(CustomerAddressViewModels entity);

        Task<bool> AddCustomerPhoneContact(CustomerPhoneContactViewModels entity);

        Task<bool> AddCustomerCompanyInfomation(CustomerCompanyInfomationViewModels entity);

        Task<bool> AddCustomerChildren(List<CustomerChildrenViewModel> models, int staffId, short BranchId);

        Task<bool> AddCustomerNextOfKin(CustomerNextOfKinViewModels entity);

        IEnumerable<CustomerViewModels> CustomerSearch(int companyId, string search);
        IEnumerable<CustomerViewModels> CustomerSearch(int companyId, CustomerSearchItemViewModels search);
        IQueryable<CustomerSearchItemViewModels> CustomerSearchRealTime(int companyId, string search);
        Task<IEnumerable<CustomerViewModels>> SearchRandomCustomerBySearchQuery(string searchQuery);
        Task<IEnumerable<CustomerViewModels>> SearchRandomCustomersBySearchQuery(string searchQuery);

        Task<IEnumerable<KYCDocumentTypeViewModel>> GetKYCDocumentType();
        Task<IEnumerable<LookupViewModel>> GetAllCRMSLegalStatus();
        Task<IEnumerable<LookupViewModel>> GetAllCRMSCompanySize();
        Task<IEnumerable<LookupViewModel>> GetAllCRMSRelationshipType();

        Task<IEnumerable<LookupViewModel>> GetAllCRMSLegalStatusByType(int type);
        IEnumerable<LookupViewModel> GetAllCRMSCompanySizeByType(int type);
        Task<IEnumerable<LookupViewModel>> GetAllCRMSRelationshipTypeByType(int type);


        #region Single Customer Information By CustomerID
        Task<CustomerViewModels> GetSingleCustomerGeneralInfo(string customerCode);
        CustomerViewModels GetSingleCustomerGeneralInfoByCustomerId(int customerId);
        CustomerCompanyInfomationViewModels GetSingleCustomerCompanyInfo(int customerId);
        IEnumerable<CustomerAddressViewModels> GetSingleCustomerAddressInfo(int customerId);
        IEnumerable<CustomerPhoneContactViewModels> GetSingleCustomerPhoneContactInfo(int customerId);
        IEnumerable<CustomerBvnViewModels> GetSingleCustomerBVNInfo(int customerId);
        IEnumerable<CustomerIdentificationViewModels> GetSingleCustomerIdentificationInfo(int customerId);
        IEnumerable<CustomerEmploymentHistoryViewModels> GetSingleCustomerEmploymentHistoryInfo(int customerId);
        CustomerEmploymentHistoryViewModels GetSingleCustomerRelatedEmployer(int customerId);
        IEnumerable<CustomerCompanyDirectorsViewModels> GetSingleCustomerDirectorInfo(int customerId, short directorTypeId);
        IEnumerable<CustomerCompanyDirectorsViewModels> GetSingleCustomerShareholderInfo(int customerId, short customerTypeId);
        IEnumerable<CustomerClientOrSupplierViewModels> GetSingleCustomerClientOrSupplierInfo(int customerId, short clientTypeId);
        IEnumerable<CustomerChildrenViewModel> GetSingleCustomerChildrenInfo(int customerId);
        IEnumerable<CustomerCompanyBeneficiaryViewModels> GetShareholderUltimateBeneficial(int companyDirectorId);
        IEnumerable<CasaViewModel> GetCustomerCASAInformation(int customerId);
        IEnumerable<CasaViewModel> GetCustomerCASAInformation(string customerCode);
        IEnumerable<CustomerNextOfKinViewModels> GetSingleCustomerNextOfKinInfo(int customerId);
        #endregion

        #region  Customer Information Validation
        bool ValidateCustomerCode(string customerCode);
        bool ValidateCustomerBVN(int customerId, string customerBvn);
        bool ValidateCustomerRCnumber(int customerId, string rcNumber);
        bool ValidateCustomerTIN(int customerId, string tin);
        bool ValidateCustomerEmail(int customerId, string email);
        Task<bool> ValidateRelatedPartyEntry(int customerId, int companyDirectorId);

        //TBL_CUSTOMER_CLIENT_SUPPLIER
        bool ValidateClientSupplierEmail(int customerId, string email);
        bool ValidateClientSupplierRCnumber(int customerId, string rcNumber);
        bool ValidateClientSupplierTIN(int customerId, string taxNumber);
        bool CustomerInformationCompleted(int customerId, UserInfo user);

        //TEMP tables 
        bool ValidateCustomerModification(int customerId);
        bool ValidateModifiedCustomerRecord(int customerId);
        bool ValidateModifiedCompanyRecord(int customerId);
        bool ValidateModifiedAddressRecord(int customerId);
        bool ValidateModifiedPhoneRecord(int customerId);
        #endregion
        Task<IEnumerable<CustomerInformationApprovalViemModel>> GetAllCustomerInformationAwaitingApproval(int staffId, int companyId);
        int GoForApproval(ApprovalViewModel entity);

        #region Customer Temporary Information 
        CustomerViewModels GetSingleCustomerGeneralInfoByCustomerId(int customerId, int targetId);
        CustomerCompanyInfomationViewModels GetSingleCustomerCompanyInfo(int customerId, int targetId);
        IEnumerable<CustomerAddressViewModels> GetSingleCustomerAddressInfo(int customerId, int targetId);
        IEnumerable<CustomerClientOrSupplierViewModels> GetSingleCustomerClientOrSupplierInfo(int customerId, short clientTypeId, int targetId);
        IEnumerable<CustomerNextOfKinViewModels> GetSingleCustomerNextOfKinInfo(int customerId, int targetId);
        IEnumerable<CustomerCompanyDirectorsViewModels> GetSingleCustomerDirectorInfo(int customerId, short directorTypeId, int targetId);
        IEnumerable<CustomerCompanyDirectorsViewModels> GetSingleCustomerShareholderInfo(int customerId, short customerTypeId, int targetId);
        IEnumerable<CustomerPhoneContactViewModels> GetSingleCustomerPhoneContactInfo(int customerId, int targetId);
        IEnumerable<CustomerEmploymentHistoryViewModels> GetSingleCustomerEmploymentHistoryInfo(int customerId, int targetId);
        #endregion

        Task<IEnumerable<CustomerRelatedPartyViewModel>> GetCustomerRelatedParty(int customerId);
        Task<bool> DeleteEmployment(int placeOfWorkId, UserInfo user);
        Task<bool> DeleteNextOfKin(int nextOfKinId, UserInfo user);

        Task<bool> Deletcontact(int phoneContactId, UserInfo user);
        Task<bool> DeleteRelatedParty(int relatedPartyId, UserInfo user);
        Task<bool> Deleteaddress(int addressId, UserInfo user);
        Task<bool> AddUpdateCustomerRelatedParty(CustomerRelatedPartyViewModel entity);
        bool UpdatePropectToCustomer(int customerId, CustomerViewModels entity);
        Task<IEnumerable<CustomerRelatedDirectorViewModel>> DirectorRelatedCustomer(string bvn);
        Task<IEnumerable<CustomerViewModels>> GetCustomerGeneralInfoByLMSLoanId(int loanApplicationId);
        Task<IEnumerable<CustomerViewModels>> SearchRandomSingleCustomersBySearchQuery(string searchQuery);
        IEnumerable<CustomerViewModels> SearchRandomSingleCorporateCustomersBySearchQuery(string searchQuery);
        IEnumerable<CustomerViewModels> SearchRandomGroupCustomersBySearchQuery(string searchQuery);
        IEnumerable<CustomerViewModels> SearchGroupCustomersBySearchQuery(string searchQuery, int groupId);
        bool GetPoliticallyExposedPerson(string customerCode);
        void UpdateCustomerCollateralId(string customerCode);
        Task<bool> refreshCustomerAccount(int customerCode);
        bool UpdateCustomerInformation(string customerCode, string accountNumber, int createdBy);
        bool SaveBulkFsCaptionEntries(List<MultipleFsCaptionOutputViewModel> models, UserInfo user);
        Tuple<List<MultipleFsCaptionOutputViewModel>, bool> PreBulkFsCaption(byte[] file, UserInfo user, bool isFinal, int customerId);
    }
}