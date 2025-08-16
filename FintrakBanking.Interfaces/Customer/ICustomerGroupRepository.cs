using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Customer;
using FintrakBanking.ViewModels.WorkFlow;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Customer
{
    public interface ICustomerGroupRepository
    {
        Task<IEnumerable<KYCItemViewModel>> GetKYCItems(int companyId);
        Task<bool> AddKycItem(KYCItemViewModel entity);
        Task<bool> UpdatedKycItem(int KYCItemId ,KYCItemViewModel entity) ;

        #region tbl_Customer Group Repository
        Task<bool> AddCustomerGroup(CustomerGroupViewModel entity);
        bool AddTempCustomerGroup(CustomerGroupViewModel entity);
        IEnumerable<CustomerGroupViewModel> GetCustomerGroup();
        CustomerGroupViewModel GetCustomerGroupByCustomerId(int customerGroupId);
        Task<bool> UpdateCustomerGroup(int groupId, CustomerGroupViewModel entity);
        Task<bool> UpdateCustomerGroupForApproval(int groupId, CustomerGroupViewModel entity);
        Task<bool> DeleteCustomerGroup(int groupId, UserInfo user);
        Task<IEnumerable<CustomerGroupViewModel>> GetCustomerGroupsAwaitingApprovals(int staffId, int companyId);
        Task<bool> GoForApproval(ApprovalViewModel entity);
        Task<bool> GoForGroupMappingApproval(ApprovalViewModel entity);
        IEnumerable<CustomerGroupViewModel> CustomerGroupSearch(string search);
        Task<IEnumerable<CustomerGroupMappingViewModel>> GetCustomerGroupMapsAwaitingApprovals(int staffId, int companyId);
        #endregion tbl_Customer Group Repository

        #region tbl_Customer Group Mapping repository
        bool DoesGroupNameExist(string groupName, string groupCode);
        Task<bool> AddCustomerGroupMapping(CustomerGroupMappingViewModel entity);
        bool AddTempCustomerGroupMapping(CustomerGroupMappingViewModel entity);
        Task<bool> AddMultipleCustomerGroupMapping(List<CustomerGroupMappingViewModel> customerGroups, int createdBy, short userBranchId, int companyId);
        Task<bool> AddCustomerGroupRelationshipTypes(LookupViewModel model);
        Task<IEnumerable<CustomerGroupMappingViewModel>> GetCustomerGroupMapping();
        Task<IEnumerable<CustomerGroupMappingViewModel>> GetCustomerGroupMappingByGroupId(int customerGroupId);
        Task<CustomerGroupMappingViewModel> GetCustomerGroupMappingByGroupMapId(int groupMapId);
        Task<IEnumerable<LookupViewModel>> GetCustomerGroupRelationshipTypes();
        Task<bool> UpdateCustomerGroupMapping(int groupMapId, CustomerGroupMappingViewModel entity);
        Task<bool> UpdateCustomerGroupMappingForApproval(int groupMapId, CustomerGroupMappingViewModel entity);
        Task<bool> DeleteCustomerGroupMapping(int groupMapId, UserInfo user);
        Task<IEnumerable<GroupCustomerMembersViewModel>> GetGroupMembersByGroupId(int customerGroupId, int companyId);
        Task<IQueryable<CustomerGroupViewModel>> SearchForCustomerGroupRealtime(int companyId, string searchQuery);
        Task<CustomerGroupViewModel> GetCustomerGroupDetailsByGroupId(int customerGroupId);
        Task<IEnumerable<CustomerGroupViewModel>> SearchForCustomerGroup(int companyId, string searchQuery);
        Task<IEnumerable<CustomerGroupMappingViewModel>> GetAllCustomerGroupMappingByGroupId(int customerGroupId);
        #endregion tbl_Customer Group Mapping repository
        Task<List<CurrentCustomerExposure>> GetGroupExposureByCustomerId(int customerId, int companyId);
        Task<List<CurrentCustomerExposure>> GetGroupExposureByGroupId(int customerGroupId, int companyId);
        Task<IEnumerable<CustomerGroupViewModel>> GetAllTempCustomerGroups();
        Task<IEnumerable<CustomerGroupMappingViewModel>> GetTempCustomerGroupMappingByGroupId(int customerGroupId);
    }
}