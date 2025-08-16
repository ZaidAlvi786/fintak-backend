using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Customer;

namespace FintrakBanking.Interfaces.Customer
{
    public interface ICustomerFSRatioRepository
    {
        #region FS Ratio Caption
        Task<IEnumerable<CustomerFSRatioCaptionViewModel>> GetFSRatioCaption(int companyId);
        Task<IEnumerable<CustomerFSRatioCaptionViewModel>> GetFSRatioCaptionByFSCaptionGroupId(int companyId, int fSCaptionGroupId);
        Task<List<CustomerFSRatioCaptionViewModel>> GetFSRatioCaptionById(short ratioCaptionId);
        Task<bool> AddFSRatioCaption(CustomerFSRatioCaptionViewModel model);
        Task<bool> UpdateFSRatioCaption(short fsCaptionId, CustomerFSRatioCaptionViewModel model);
        Task<bool> DeleteFSRatioCaption(short fsCaptionId, UserInfo user);
        #endregion

        #region FS Ratio Detail
        Task<bool> AddFSRatioDetail(CustomerFSRatioDetailViewModel model);
        Task<bool> AddMultipleFSRatioDetail(List<CustomerFSRatioDetailViewModel> model);
        Task<IEnumerable<CustomerFSRatioDetailViewModel>> GetFSRatioDetail(short ratioCaptionId, short fsCaptionGroupId, int companyId);
        Task<CustomerFSRatioDetailViewModel> GetFSRatioDetailById(int ratioDetailId);
        Task<bool> UpdateFSRatioDetail(int ratioDetailId, CustomerFSRatioDetailViewModel model);
        Task<bool> DeleteFSRatioDetail(int ratioDetailId, UserInfo user);
        Task<bool> DeleteMultipleFSRatioDetail(List<int> ratioDetailId, UserInfo user);
        Task<IEnumerable<CustomerFSRatioDivisorTypeViewModel>> GetAllDivisorType();
        Task<IEnumerable<CustomerFSRatioValueTypeViewModel>> GetAllValueType();
        #endregion

        Task<List<CustomerFSRatioCaptionReportViewModel>> GetCustomerFSRatioValues(int customerId);
        decimal CalculateFSRatioValueForDerived(CustomerFSCaptionDetailViewModel entity);
    }
}