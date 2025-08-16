using FintrakBanking.Common.Enum;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.General
{
    public interface IGeneralSetupRepository
    {
        DateTime CalculateMaturityDate(DateTime effectiveDate, TenorModeEnum tenorModeId, int tenor);

        Task<List<LookupViewModel>> GetAllTenorMode();

        Task<List<LookupViewModel>> GetAllCurrency();

        Task<IEnumerable<LookupViewModel>> GetAllCustomerType();

        Task<List<LookupViewModel>> GetAllDealClassificationType();

        Task<List<LookupViewModel>> GetAllDayCount();

        Task<List<LookupViewModel>> GetAllFeeAmortisationType();

        Task<List<LookupViewModel>> GetAllDealTypes();

        Task<List<LookupViewModel>> GetAllFSTypes();

        Task<IEnumerable<LookupViewModel>> GetSector();

        Task<List<LookupViewModel>> GetSubsector();

        Task<DateTime> GetApplicationEODLastRefreshedDate();

        int GetLoanApplicationRef();

        Task<List<LookupViewModel>> GetAllFrequencyTypes();
        //Task<bool>  SaveProductGroup(ProductGroupViewModel group);
        DateTime GetApplicationDate();

        Task<List<LookupViewModel>> GetAllOperationTypes();

        Task<List<LookupViewModel>> GetAllOperations();

        Task<List<LookupViewModel>> GetOperations(short operationTypeId);

        Task<List<SectorViewModel>> GetAllSectors();
        Task<List<GlobalSectorViewModel>> GetAllGlobalSectors();
        bool UpdateGlobalSector(GlobalSectorViewModel model, int id);

        Task<IEnumerable<SectorViewModel>> GetSectorsBySubSectorId(short ssId);

        Task<IEnumerable<SectorViewModel>> GetAllSubSectors();

        bool UpdateSector(SectorViewModel model, short id);
        bool DeleteSector(int Id, UserInfo user);
        bool AddSector(SectorViewModel model);

        // IEnumerable<int> GetStaffApprovalLevelIds(int staffId, int operationId);
        IEnumerable<int> GetStaffApprovalLevelIds(int staffId, int operationId, int staffRoleId = 0, bool isRemoteCall = false);
        IEnumerable<int> GetStaffApprovalLevelIdsWithoutRelief(int staffId, int operationId);
        List<int> GetRouteLevels(int operationId, int depth);

        Task<IEnumerable<LookupViewModel>> GetRegionByType(int regionTypeId);

        Task<IEnumerable<ProfileBusinessUnitViewModel>> GetProfileBusinessUnits();
        List<int> GetStaffRlieved(int staffId);
        Task<int> GetGroupId();
    }
}