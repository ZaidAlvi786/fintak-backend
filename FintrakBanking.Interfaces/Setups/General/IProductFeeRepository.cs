using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.General;
using System.Collections.Generic;
using FintrakBanking.ViewModels.Setups.Finance;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.General
{
    public interface IProductFeeRepository
    {
        Task<IEnumerable<ProductFeeViewModel>> GetAllMappedFeeByProduct(int productId);
        Task<IEnumerable<ProductFeeViewModel>> GetAllMappedFeeByTempProduct(int productId);
        Task<IEnumerable<ChargeFeeViewModel>> GetUnmappedFeeToProduct(int productId);
        Task<ProductFeeViewModel> GetProductFee(int productFeeId);
        Task<List<ProductFeeViewModel>> GetTempProductFee(int productFeeId);
        Task<List<ProductFeeViewModel>> GetProductFeeAwaitingApprovals(int tempProductId);
        Task<int> AddProductFee(ProductFeeViewModel productFee);
        Task<int> AddTempProductFee(ProductFeeViewModel productFee);
        void ApproveProductFee(int productId, UserInfo user);
        Task<int> AddMultipleProductFee(List<ProductFeeViewModel> productFees);
        Task<bool> UpdateProductFee(int productFeeId, ProductFeeViewModel productFee);
        Task<bool> DeleteProductFee(int productFeeId, UserInfo user);
        Task<bool> DeleteMultipleProductFee(List<int> productFeeIds);
        bool DoesProductFeeExist(int productFeeId);
        Task<IEnumerable<dynamic>> GetFeesByProductId(int productId);
        Task<IEnumerable<dynamic>> GetSavedFee(int loanApplicationDetailId, bool forModifyFacility);
    }
}