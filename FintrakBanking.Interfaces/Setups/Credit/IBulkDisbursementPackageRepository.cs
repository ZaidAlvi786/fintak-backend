using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.Credit
{
    public interface IBulkDisbursementPackageRepository
    {
        // for scheme interfaces
       Task<IEnumerable<BulkDisbursementSetupSchemeViewModel>> GetAllBulkDisburseSchemeByApplicationReferenceNumber(string referenceNumber);
       Task<IEnumerable<BulkDisbursementSetupSchemeViewModel>> GetAllBulkDisbursementScheme();
       Task<IEnumerable<BulkDisbursementSetupSchemeViewModel>> GetAllBulkDisbursementSchemeByProductId(int productId);
       Task<IEnumerable<BulkDisbursementSetupSchemeViewModel>> GetAllBulkDisbursementSchemeByDisburseSchemeId(int disburseSchemeId);
       Task<bool> AddBulkDisbursementScheme(BulkDisbursementSetupSchemeViewModel model);
       Task<bool> AddMultipleBulkDisbursementScheme(List<BulkDisbursementSetupSchemeViewModel> models);
       Task<bool> UpdateBulkDisbursementScheme(int disbursementSchemeId, BulkDisbursementSetupSchemeViewModel model);
       Task<bool> DeleteBulkDisbursementScheme(int disbursementPackageId, UserInfo user);
       Task<IEnumerable<BulkDisbursementSetupSchemeViewModel>> SearchLoanApplicationDetails(int companyId, string searchQuery);
       Task<IEnumerable<BulkDisbursementSetupSchemeViewModel>> SchemeSearch(string searchString);
    }
}
