using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels.Setups.General;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using FintrakBanking.ViewModels;
using System.ComponentModel.Composition;
using FintrakBanking.Common.Enum;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Entities.DocumentModels;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common;
using System.Data.Entity;
using FintrakBanking.Entities.StagingModels;

namespace FintrakBanking.Repositories.Setups.General
{
    [Export(typeof(ICountryRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CountryRepository : ICountryRepository
    {
        private FinTrakBankingContext context;
        private IAuditTrailRepository auditTrail;
        private IGeneralSetupRepository _genSetup;
        private FinTrakBankingStagingContext stagingContext;


        public CountryRepository(IAuditTrailRepository _auditTrail,
                                    IGeneralSetupRepository genSetup, FinTrakBankingContext _context, FinTrakBankingStagingContext _staging)
        {
            this.context = _context;
            this.auditTrail = _auditTrail;
            this._genSetup = genSetup;
            this.stagingContext = _staging;
        }
        private bool SaveAll()
        {
            return this.context.SaveChanges() > 0;
        }
        public async Task<IEnumerable<LookupViewModel>> GetAllCityClass()
        {
            var data = await (from a in context.TBL_CITY_CLASS
                              select new LookupViewModel
                              {
                                 lookupId = a.CITYCLASSID,
                                 lookupName = a.CITYCLASSNAME
                              }).ToListAsync();
            return data;
        }

        public async Task<bool> AddCity(CityViewModel entity)
        {
            var data = await context.TBL_CITY.Where(c => c.CITYNAME == entity.cityName && c.LOCALGOVERNMENTID == entity.localGovernmentId).Select(c=>c).FirstOrDefaultAsync();
            if (data==null)
            {
                var cityEntity = new TBL_CITY
                {
                    CITYNAME = entity.cityName,
                    LOCALGOVERNMENTID = entity.localGovernmentId,
                    CITYCLASSID = entity.cityClassId,
                    ALLOWEDFORCOLLATERAL = entity.allowedForCollateral

                };
                context.TBL_CITY.Add(cityEntity);
                bool res;
                try
                {
                    res = await context.SaveChangesAsync() != 0;
                    return res;

                }
                catch ( Exception ex) {
                    var test = ex;
                }
                return false;
            }
            else
                throw new SecureException(TranslateHelper.get("Record already exist"));
         
        }
        public async Task<bool> UpdateCity(CityViewModel entity, int id)
        {
            try
            {
                var cityEntity = await context.TBL_CITY.FindAsync(id);
                {
                    cityEntity.CITYNAME = entity.cityName;
                    cityEntity.CITYCLASSID = 1;
                    cityEntity.ALLOWEDFORCOLLATERAL = entity.allowedForCollateral;
                    cityEntity.LOCALGOVERNMENTID = entity.localGovernmentId;
                }

                return await context.SaveChangesAsync() != 0;
            }
            catch(Exception e)
            {
                throw e;
            }

        }

        public async Task<IEnumerable<CityViewModel>> GetCity()
        {
            var cityEntity = await (from a in context.TBL_CITY
                              join l in context.TBL_LOCALGOVERNMENT on a.LOCALGOVERNMENTID equals l.LOCALGOVERNMENTID
                              join s in context.TBL_STATE on l.STATEID equals s.STATEID
                              select new CityViewModel
                              {
                                  cityId = a.CITYID,
                                  cityName = a.CITYNAME,
                                  localGovt = context.TBL_LOCALGOVERNMENT.Where(l=>l.LOCALGOVERNMENTID==a.LOCALGOVERNMENTID).Select(l=>l.NAME).FirstOrDefault(),
                                  stateName = s.STATENAME,
                                 // cityClassId = a.CITYCLASSID,
                                  cityClassName = context.TBL_CITY_CLASS.Where(o=>o.CITYCLASSID==a.CITYCLASSID).Select(o=>o.CITYCLASSNAME).FirstOrDefault(),
                                  allowedForCollateral = a.ALLOWEDFORCOLLATERAL,
                                  stateId = s.STATEID,
                                  localGovernmentId = a.LOCALGOVERNMENTID
                              }).ToListAsync();
            return cityEntity;
        }
         public async Task<IEnumerable<CityViewModel>> GetCity(int Id)
         {
            var cityEntity = await (from a in context.TBL_CITY where a.LOCALGOVERNMENTID==Id
                                select new CityViewModel
                                {
                                    cityId = a.CITYID,
                                    cityName = a.CITYNAME,
                                    localGovernmentId = a.LOCALGOVERNMENTID
                                }).ToListAsync();
            return cityEntity;
         }

        public async Task<bool> AddLocalGovt(LocalGovtViewModel entity)
        {
            var cityEntity = new TBL_LOCALGOVERNMENT
            {
                NAME = entity.localGovtName,
                STATEID = entity.stateId,
            };
            context.TBL_LOCALGOVERNMENT.Add(cityEntity);
            return await context.SaveChangesAsync() != 0;
        }
        public async Task<bool> UpdateLocalGovt(LocalGovtViewModel entity, int id)
        {
            var cityEntity = await context.TBL_LOCALGOVERNMENT.FindAsync(id);
            {
                cityEntity.NAME = entity.localGovtName;
                cityEntity.STATEID = entity.stateId;
            }

            return await context.SaveChangesAsync() != 0;
        }
        public async Task<IEnumerable<LocalGovtViewModel>> GetLocalGovt()
        {
            var cityEntity = await (from a in context.TBL_LOCALGOVERNMENT
                              select new LocalGovtViewModel
                              {
                                  localGovernmentId = a.LOCALGOVERNMENTID,
                                  localGovtName = a.NAME,
                                  stateId = a.STATEID,
                                  stateName = a.TBL_STATE.STATENAME
                              }).ToListAsync();
            return cityEntity;
        }
        public LocalGovtViewModel GetLocalGovtById(int id)
        {
            var cityEntity = (from a in context.TBL_LOCALGOVERNMENT
                              where a.LOCALGOVERNMENTID==id
                              select new LocalGovtViewModel
                              {
                                  localGovernmentId = a.LOCALGOVERNMENTID,
                                  localGovtName = a.NAME,
                                  stateId = a.STATEID,
                                  stateName = a.TBL_STATE.STATENAME
                              });
            return cityEntity.FirstOrDefault();
        }

        public async Task<List<LocalGovtViewModel>> GetLocalGovtByStateId(int stateId)
        {
            var cityEntity = await (from a in context.TBL_LOCALGOVERNMENT
                              where a.STATEID == stateId
                              select new LocalGovtViewModel
                              {
                                  localGovernmentId = a.LOCALGOVERNMENTID,
                                  localGovtName = a.NAME,
                                  stateId = a.STATEID,
                              }).ToListAsync();
            return cityEntity;
        }
        public async Task<CityViewModel> GetCityById(int cityId)
        {
            var cityEntity = await (from a in context.TBL_CITY
                              where a.CITYID == cityId
                              select new CityViewModel
                              {
                                  cityId = a.CITYID,
                                  cityName = a.CITYNAME,
                                  stateId = a.TBL_LOCALGOVERNMENT.STATEID,
                                  localGovernmentId = a.LOCALGOVERNMENTID,
                                  cityClassId = a.CITYCLASSID,
                                  cityClassName = a.TBL_CITY_CLASS.CITYCLASSNAME,
                                  allowedForCollateral = a.ALLOWEDFORCOLLATERAL
                              }).FirstOrDefaultAsync();
            return cityEntity;
        }
        public IEnumerable<LocalGovtViewModel> GetLGAByStateId(int stateId)
        {
            var stateEntity = (from a in context.TBL_LOCALGOVERNMENT
                               where a.STATEID == stateId
                               select new LocalGovtViewModel
                               {
                                   localGovernmentId = a.LOCALGOVERNMENTID,
                                   localGovtName = a.NAME,
                                   stateId = a.STATEID,
                                   
                               });
            return stateEntity;
        }
        public IEnumerable<CityViewModel> GetCityByLGAId(int lgaId)
        {
            var stateEntity = (from a in context.TBL_CITY
                               where a.LOCALGOVERNMENTID == lgaId
                               select new CityViewModel
                               {
                                   cityId = a.CITYID,
                                   cityName = a.CITYNAME,
                                   stateId = a.TBL_LOCALGOVERNMENT.STATEID,
                                   localGovernmentId = a.LOCALGOVERNMENTID,
                                   cityClassId = a.CITYCLASSID,
                                   cityClassName = a.TBL_CITY_CLASS.CITYCLASSNAME,
                                   allowedForCollateral = a.ALLOWEDFORCOLLATERAL
                               });
            return stateEntity;
        }
        public async Task<int> GetLgaCity(int cityId)
        {
            //var stateEntity = (from a in context.TBL_CITY
            //                   where a.CITYID == cityId
            //                   select new CityViewModel
            //                   {
            //                       localGovernmentId = a.LOCALGOVERNMENTID
            //                   });
            var rec = await context.TBL_CITY.Where(x => x.CITYID == cityId).Select(m => m.LOCALGOVERNMENTID).FirstOrDefaultAsync();
            return rec;
        }
        public IEnumerable<CityViewModel> GetCityByStateId(int stateId)
        {
            var stateEntity = (from a in context.TBL_CITY
                               where a.TBL_LOCALGOVERNMENT.STATEID == stateId
                               select new CityViewModel
                               {
                                   cityId = a.CITYID,
                                   cityName = a.CITYNAME,
                                   stateId = a.TBL_LOCALGOVERNMENT.STATEID,
                                   localGovernmentId = a.LOCALGOVERNMENTID,
                                   cityClassId = a.CITYCLASSID,
                                   cityClassName = a.TBL_CITY_CLASS.CITYCLASSNAME,
                                   allowedForCollateral = a.ALLOWEDFORCOLLATERAL
                               });
            return stateEntity;
        }

        public IEnumerable<Object> GetAllCitiesByContryId(int countryId)
        {
            return from c in context.TBL_COUNTRY
                   join st in context.TBL_STATE
                    on c.COUNTRYID equals st.COUNTRYID
                   join ct in context.TBL_CITY on st.STATEID equals ct.TBL_LOCALGOVERNMENT.STATEID
                   where c.COUNTRYID == countryId
                   select new
                   {
                       cityId = ct.CITYID,
                       cityName = ct.CITYNAME,
                       stateId = st.STATEID,
                       stateName = st.STATENAME,
                       cityClassId = ct.CITYCLASSID,
                       cityClassName = ct.TBL_CITY_CLASS.CITYCLASSNAME,
                       allowedForCollateral = ct.ALLOWEDFORCOLLATERAL
                   };

        }

        public IEnumerable<StateViewModel> GetStateByCountryId(int countryId)
        {
            var stateEntity = (from a in context.TBL_STATE
                               where a.COUNTRYID == countryId
                               select new StateViewModel
                               {
                                   CountryId = a.COUNTRYID,
                                   StateName = a.STATENAME,
                                   StateId = a.STATEID,
                                   CountryName = context.TBL_COUNTRY.FirstOrDefault(j => j.COUNTRYID == a.COUNTRYID).NAME, // ?? string.Empty,
                                   CollateralSearchChargeAmount = a.COLLATERALSEARCHCHARGEAMOUNT,
                                   ChartingAmount = a.CHARTINGAMOUNT ?? 0,
                                   VerificationAmount = a.VERIFICATIONAMOUNT ?? 0
                               });
            return stateEntity;
        }
        public IEnumerable<StateViewModel> GetStateByCompanyId(int companyId)
        {
            var countryId = context.TBL_COMPANY.Find(companyId).COMPANYID;
            if (countryId != 0)
            {
                var stateEntity = (from a in context.TBL_STATE
                                   where a.COUNTRYID == countryId
                                   select new StateViewModel
                                   {
                                       CountryId = a.COUNTRYID,
                                       StateName = a.STATENAME,
                                       StateId = a.STATEID,
                                       CountryName = a.TBL_COUNTRY.NAME, //context.TBL_COUNTRY.FirstOrDefault(j => j.COUNTRYID == a.COUNTRYID).NAME ?? string.Empty,
                                       CollateralSearchChargeAmount = a.COLLATERALSEARCHCHARGEAMOUNT,
                                       ChartingAmount = a.CHARTINGAMOUNT ?? 0,
                                       VerificationAmount = a.VERIFICATIONAMOUNT ?? 0
                                   });
                return stateEntity;
            }
            return null;
        }

        public async Task<bool> UpdateState(StateViewModel entity, int stateId)
        {
            var state = await context.TBL_STATE.FindAsync(stateId);
            if (state != null)
            {
                state.COLLATERALSEARCHCHARGEAMOUNT = entity.CollateralSearchChargeAmount;
                state.CHARTINGAMOUNT = entity.ChartingAmount;
                state.VERIFICATIONAMOUNT = entity.VerificationAmount;
                state.STATENAME = entity.StateName;
                state.LASTUPDATEDBY = entity.createdBy;
                state.DATETIMEUPDATED = DateTime.Now;
            }
           
            // Audit Section ----------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.StateUpdated,
                STAFFID = entity.createdBy,
                BRANCHID = (short)entity.userBranchId,
                DETAIL = $"{TranslateHelper.get("Updated tbl_State with Id:")} {entity.StateId} ",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = entity.applicationUrl,
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
            };

            auditTrail.AddAuditTrail(audit);
            return SaveAll();
        }
        public IEnumerable<CountryViewModel> GetCountry(int countryId)
        {
            var countryEntity = (from a in context.TBL_COUNTRY where a.COUNTRYID == countryId 

                                 select new CountryViewModel
                                 {
                                     CountryId = a.COUNTRYID,
                                     CountryName = a.NAME
                                 }).ToList();
            return countryEntity;
        }
        public async Task<IEnumerable<CountryViewModel>> GetCountry()
        {
            var countryEntity = await (from a in context.TBL_COUNTRY                              

                                 select new CountryViewModel
                                 {
                                     CountryId = a.COUNTRYID,
                                     CountryName = a.NAME
                                 }).ToListAsync();
            return countryEntity;
        }
        public IEnumerable<StateViewModel> GetState()
        {
            var stateEntity = (from a in context.TBL_STATE

                               select new StateViewModel
                               {
                                   CountryId = a.COUNTRYID,
                                   StateName = a.STATENAME,
                                   StateId = a.STATEID
                               });
            return stateEntity.ToList();
        }

        public bool SetStateChartingAndVerificationAmounts(StateViewModel entity, int stateId)
        {
            var state = context.TBL_STATE.Find(stateId);
            if (state != null)
            {
                state.CHARTINGAMOUNT = entity.ChartingAmount;
                state.VERIFICATIONAMOUNT = entity.VerificationAmount;
            }

            // Audit Section ----------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.DepartmentUpdated,
                STAFFID = entity.createdBy,
                BRANCHID = (short)entity.userBranchId,
                DETAIL = $"{TranslateHelper.get("Updated tbl_State with Id:")} {entity.StateId} ",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = entity.applicationUrl,
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
            };

            auditTrail.AddAuditTrail(audit);
            return SaveAll();
        }

        public IEnumerable<SubsidiariesViewModel> GetSubsidiaries()
        {
            var subsidiary = (from a in stagingContext.STG_SUBSIDIARIES

                              select new SubsidiariesViewModel
                              {
                                  SubsidiaryId = a.SUBSIDIARYID,
                                  SubsidiaryName = a.SUBSIDIARYNAME,
                                  CountryId = a.COUNTRYID,
                                  Location = a.LOCATION,
                                  UrlLink = a.URLLINK,
                                  isActive = a.ISACTIVE
                              }).ToList();
            foreach (var x in subsidiary)
            {
                using (var context = new FinTrakBankingContext())
                {
                    var country = context.TBL_COUNTRY.Where(c => c.COUNTRYID == x.CountryId).FirstOrDefault();
                    x.CountryName = country.NAME;
                }
            }
            return subsidiary;
        }

        public bool AddSubsidiaries(SubsidiariesViewModel entity)
        {
            var data = stagingContext.STG_SUBSIDIARIES.Where(c => c.SUBSIDIARYNAME == entity.SubsidiaryName).Select(c => c).FirstOrDefault();
            if (data == null)
            {
                var subsidiaries = new STG_SUBSIDIARIES
                {
                    SUBSIDIARYNAME = entity.SubsidiaryName,
                    COUNTRYID = entity.CountryId,
                    LOCATION = entity.Location,
                    URLLINK = entity.UrlLink,
                    ISACTIVE = entity.isActive
                };
                stagingContext.STG_SUBSIDIARIES.Add(subsidiaries);
                bool res;
                try
                {
                    res = stagingContext.SaveChanges() != 0;
                    return res;

                }
                catch (Exception ex)
                {
                    var test = ex;
                }
                return false;
            }
            else
                throw new SecureException("Record already exist");

        }
        public bool UpdateSubsidiaries(SubsidiariesViewModel entity, int id)
        {
            var subsidiares = stagingContext.STG_SUBSIDIARIES.Find(id);
            {
                subsidiares.SUBSIDIARYNAME = entity.SubsidiaryName;
                subsidiares.COUNTRYID = entity.CountryId;
                subsidiares.LOCATION = entity.Location;
                subsidiares.URLLINK = entity.UrlLink;
                subsidiares.ISACTIVE = entity.isActive;
            }

            return stagingContext.SaveChanges() != 0;
        }
    }
}