using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Customer;

namespace FintrakBanking.Interfaces.Customer
{
    public interface ICustomerFSCaptionGroupRepository
    {
       Task<IEnumerable<CustomerFSCaptionGroupViewModel>> GetCustomerFSCaptionGroup();
       Task<IEnumerable<CustomerFSCaptionGroupViewModel>> GetCustomerFSCaptionGroupWithoutRatio();
       Task<CustomerFSCaptionGroupViewModel> GetCustomerFSCaptionGroupById(short fsCaptionGroupId);
       Task<bool> AddCustomerFSCaptionGroup(CustomerFSCaptionGroupViewModel entity);       
       Task<bool> UpdateCustomerFSCaptionGroup(short groupId, CustomerFSCaptionGroupViewModel entity);
       Task<bool> DeleteCustomerFSCaptionGroup(int fsCaptionId, UserInfo user);
    }
}
