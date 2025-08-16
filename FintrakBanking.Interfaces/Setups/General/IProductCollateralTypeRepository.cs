using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups;
using FintrakBanking.ViewModels.Setups.General;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.General
{
    public interface IProductCollateralTypeRepository
    {
        Task<IEnumerable<ProductCollateralTypeViewModel>> GetCollateralTypeByProduct(int productId);
        Task<IEnumerable<ProductCollateralTypeViewModel>> GetMappedCollateralTypeByProduct(int productId);
        Task<IEnumerable<CollateralTypeViewModel>> GetUnmappedCollateralToProduct(int productId);
        Task<ProductCollateralTypeViewModel> GetProductCollateralTypeViewModel(int productCollateralTypeId);
        Task<int> AddProductCollateralType(ProductCollateralTypeViewModel collateralType);
        Task<int> AddMultipleProductCollateralType(List<ProductCollateralTypeViewModel> collateralTypes);
        void ApproveProductCollateral(int productId, UserInfo user);
        Task<int> AddTempProductCollateralType(ProductCollateralTypeViewModel productCollateral);
        Task<bool> DeleteProductCollateralType(int productCollateralTypeId, UserInfo user);
        Task<bool> DeleteMultipleProductCollateralType(List<int> productCollateralTypeIds, UserInfo user);
        bool DoesProductCollateralExist(int productCollateralTypeId);
    }
}