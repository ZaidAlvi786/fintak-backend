using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels.Setups.Credit;

namespace FintrakBanking.Interfaces.Setups.Credit
{
    public interface ICrmsRegulatoryRepository
    {
        #region CRMS CREDIT TYPE PRODUCT
        Task<IEnumerable<CrmsRegulatoryViewModel>> GetAllRegulatorySetup();
        Task<IEnumerable<CrmsRegulatoryTypeViewModel>> GetAllRegulatoryType();
        Task<bool> AddRegulatory(CrmsRegulatoryViewModel model);
        Task<bool> UpdateRegulatory(CrmsRegulatoryViewModel model, int regulatoryId);
        Task<IEnumerable<CrmsRegulatoryViewModel>> GetRegulatoryByTypeId(int crmsTypeId, int companyId);

        Task<bool> DeleteRegulatory(int regulatoryId, short userBranchId, int companyId, int lastUpdatedBy, string applicationUrl, string userIPAddress);
        #endregion
    }
}
