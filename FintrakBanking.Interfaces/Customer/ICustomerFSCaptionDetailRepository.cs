using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Customer;

namespace FintrakBanking.Interfaces.Customer
{
    public interface ICustomerFSCaptionDetailRepository
    {
        #region Customer FS Detail

        Task<IEnumerable<CustomerFSCaptionDetailViewModel>> GetMappedCustomerFsCaptionDetail(int customerId, short fsCaptionGroupId, DateTime fsDate);
        Task<IEnumerable<CustomerFSCaptionDetailViewModel>> GetAllMappedCustomerFsCaptionDetail(int customerId, DateTime fsDate);
        Task<IEnumerable<CustomerFSCaptionDetailViewModel>> GetMappedCustomerFsCaptions(int customerId);
        Task<CustomerFSCaptionDetailViewModel> GetCustomerFSCaptionDetailById(int fsdetailId);
        Task<bool> AddCustomerFSCaptionDetail(CustomerFSCaptionDetailViewModel entity);
        Task<bool> AddMultipleCustomerFSCaptionDetail(List<CustomerFSCaptionDetailViewModel> entities);
        Task<bool> UpdateCustomerFSCaptionDetail(int fsdetailId, CustomerFSCaptionDetailViewModel entity);
        Task<bool> DeleteCustomerFSCaptionDetail(int fsdetailId, UserInfo user);
        Task<bool> DeleteMultipleCustomerFSCaptionDetail(List<int> fsdetailIds, UserInfo user);

        #endregion Customer FS Detail

        #region Customer Group FS Detail

        Task<IEnumerable<CustomerGroupFSCaptionDetailViewModel>> GetMappedCustomerGroupFsCaptionDetail(int customerGroupId, short fsCaptionGroupId, DateTime fsDate);
        Task<IEnumerable<CustomerGroupFSCaptionDetailViewModel>> GetMappedCustomerGroupFsCaptions(int customerGroupId);
        Task<CustomerGroupFSCaptionDetailViewModel> GetCustomerGroupFSCaptionDetailById(int fsdetailId);
        Task<bool> AddCustomerGroupFSCaptionDetail(CustomerGroupFSCaptionDetailViewModel entity);
        bool AddMultipleCustomerGroupFSCaptionDetail(List<CustomerGroupFSCaptionDetailViewModel> entities);
        Task<bool> UpdateCustomerGroupFSCaptionDetail(int fsdetailId, CustomerGroupFSCaptionDetailViewModel entity);
        Task<bool> DeleteCustomerGroupFSCaptionDetail(int fsdetailId, UserInfo user);
        bool DeleteMultipleCustomerGroupFSCaptionDetail(List<int> fsdetailIds, UserInfo user);

        #endregion Customer Group FS Detail
    }
}
