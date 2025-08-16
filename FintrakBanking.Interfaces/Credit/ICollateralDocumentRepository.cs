using FintrakBanking.ViewModels.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface ICollateralDocumentRepository
    {
        Task<CollateralDocumentViewModel> GetCollateralDocument(int documentId);

        Task<IEnumerable<CollateralDocumentViewModel>> GetAllCollateralDocument();

        Task<IEnumerable<CollateralDocumentViewModel>> GetCustomerCollateralDocument(int collateralId);
        Task<IEnumerable<CollateralDocumentViewModel>> GetCustomerCollateralReleaseDocument(int collateralId);

        Task<bool> AddCollateralDocument(CollateralDocumentViewModel model, byte[] file);
        Task<bool> AddTempCollateralDocument(CollateralDocumentViewModel model, byte[] file);

        Task<bool> UpdateCollateralDocument(CollateralDocumentViewModel model, int documentId);

        Task<bool> AddCollateralVisitation(CollateralDocumentViewModel model, byte[] file);

        Task<bool> AddTempCollateralVisitation(CollateralDocumentViewModel model, byte[] file);

        Task<CollateralVisitationDocumentViewModel> GetCollateralVisitationDocument(int documentId);

        Task<CollateralVisitationDocumentViewModel> GetTempCollateralVisitationDocument(int collateralVisitationId);

        Task<IEnumerable<CollateralDocumentViewModel>> GetCollateralGuaranteeDocument(int targetId);
        Task<IEnumerable<CollateralDocumentViewModel>> GetTempAllCollateralDocument(int collateralId);

        Task<IEnumerable<CollateralDocumentViewModel>> GetTempCustomerCollateralDocument(int tempCollateralId);
    }
}
