using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FintrakBanking.ViewModels.credit;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;

namespace FintrakBanking.Interfaces.credit
{
    public interface ILcIssuanceRepository
    {
        #region LCISSUANCE
        Task<List<LcIssuanceApprovalViewModel>> SearchLc(string searchString);
        Task<List<LcIssuanceApprovalViewModel>> SearchLcLMS(string searchString);

        Task<IEnumerable<LcIssuanceViewModel>> GetLcIssuance(int id);
        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcEnhancementByLcEnhancementId(int tempLcIssuanceId);
        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuances(int staffId);
        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuancesForEnhancement(int staffId);
        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuancesForExtension(int staffId);

        Task<IEnumerable<CamProcessedLoanViewModel>> GetIFFLinesForLCByCustomerId(int CustomerId, int companyId, int staffId, int branchId);

        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuancesForRelease(int staffId);
        Task<IEnumerable<LcReleaseAmountViewModel>> GetReleasesForLcIssuance(int lcIssuanceId);

        Task<LcReleaseAmountViewModel> AddLCReleaseAmount(LcReleaseAmountViewModel entity);

        Task<LcReleaseAmountViewModel> UpdateLCReleaseAmount(LcReleaseAmountViewModel entity);

        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuancesForApproval(int staffId);
        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuancesForEnhancementApproval(int staffId);
        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuancesForExtensionApproval(int staffId);
        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuancesForCancelationApproval(int staffId);

        Task<IEnumerable<LcIssuanceApprovalViewModel>> GetLcIssuancesForReleaseApproval(int staffId);

        //IEnumerable<LcIssuanceViewModel> GetLcIssuanceByIssuanceId(int id);

        Task<LcIssuanceViewModel> AddLcIssuance(LcIssuanceViewModel model);
        Task<bool> UpdateLcIssuance(LcIssuanceViewModel model, int id, UserInfo user);
        Task<bool> DeleteLcIssuance(int id, UserInfo user);

        Task<LcIssuanceViewModel> AddLcEnhancement(LcIssuanceViewModel model);
        Task<bool> UpdateLcEnhancement(LcIssuanceViewModel model, int id, UserInfo user);
        Task<bool> DeleteLcEnhancement(int id, UserInfo user);

        Task<LcIssuanceViewModel> AddLcExtension(LcIssuanceViewModel model);
        Task<bool> UpdateLcExtension(LcIssuanceViewModel model, int id, UserInfo user);
        Task<bool> DeleteLcExtension(int id, UserInfo user);

        Task<bool> AddLcArchive(int LcIssuanceId, int operationId);
        Task<bool> UpdateOldLcWithEnhancement(int tempLcIssuanceId);

        #endregion LCISSUANCE

        //#region LCDOCUMENT
        //LcDocumentViewModel GetLcDocument(int id);

        //IEnumerable<LcDocumentViewModel> GetLcDocuments();

        //bool AddLcDocument(LcDocumentViewModel model);

        //bool UpdateLcDocument(LcDocumentViewModel model, int id);

        //bool DeleteLcDocument(int id);
        //#endregion LCDOCUMENT

        //#region SHIPPING
        //LcShippingViewModel GetLcShipping(int id);

        //IEnumerable<LcShippingViewModel> GetLcShippings();

        //bool AddLcShipping(LcShippingViewModel model);

        //bool UpdateLcShipping(LcShippingViewModel model, int id);

        //bool DeleteLcShipping(int id);
        //#endregion SHIPPING

        //#region LCCONDITIONS
        //LcConditionViewModel GetLcCondition(int id);

        //IEnumerable<LcConditionViewModel> GetLcConditions();

        //bool AddLcCondition(LcConditionViewModel model);

        //bool UpdateLcCondition(LcConditionViewModel model, int id);

        //bool DeleteLcCondition(int id);
        //#endregion LCCONDITIONS
    }
}
