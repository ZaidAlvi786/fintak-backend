using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels; 
using FintrakBanking.ViewModels.Customer;

namespace FintrakBanking.Interfaces.Customer
{
    public interface ICustomerFSCaptionRepository
    {
        Task<IEnumerable<CustomerFSCaptionViewModel>> GetCustomerFSCaptions();
        Task<IEnumerable<CustomerFSCaptionViewModel>> GetCustomerFSCaptionByGroupId(short fsCaptionGroupId);
        Task<CustomerFSCaptionViewModel> GetCustomerFSCaptionById(int fsCaptionId);
        Task<IEnumerable<CustomerFSCaptionViewModel>> GetUnmappedCustomerFSCaption(short fsCaptionGroupId, int customerId, DateTime fsDate);
        Task<IEnumerable<CustomerFSCaptionViewModel>> GetUnmappedCustomerGroupFSCaption(short fsCaptionGroupId, int customerGroupId, DateTime fsDate);
        Task<bool> AddCustomerFSCaption(CustomerFSCaptionViewModel entity);       
        Task<bool> UpdateCustomerFSCaption(int fsCaptionId, CustomerFSCaptionViewModel entity);
        Task<bool> DeleteCustomerFSCaption(int fsCaptionId, UserInfo user);
        bool ValidateFSCaption(string captionName);
    }            
}
