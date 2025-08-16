using FintrakBanking.Common;
using FintrakBanking.Common.Enum;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Admin;
using FintrakBanking.ViewModels.Finance;
using FinTrakBanking.ThirdPartyIntegration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FintrakBanking.Repositories.Admin
{
    public class CurrencyRateRepository : ICurrencyRateRepository
    {
        private FinTrakBankingContext context;
        private IGeneralSetupRepository _genSetup;
        private IAuditTrailRepository auditTrail;
        private IntegrationWithFlexcube integration;

        public CurrencyRateRepository(FinTrakBankingContext _context,
                                                    IGeneralSetupRepository genSetup,
                                                    IAuditTrailRepository _auditTrail,
                                                    IntegrationWithFlexcube _integration )
        {
            this.context = _context;
            this._genSetup = genSetup;
            auditTrail = _auditTrail;
            this.integration =_integration;
    }

        public async Task<IEnumerable<CurrencyViewModel>> GetCurrency()
        {
            var data = await (from a in context.TBL_CURRENCY
                        where a.INUSE == true
                        select new CurrencyViewModel
                        {
                            currencyId = a.CURRENCYID,
                            lookupId = a.CURRENCYID,
                            currencyCode = a.CURRENCYCODE,
                            currencyName = a.CURRENCYNAME,
                            lookupName = a.CURRENCYNAME,
                            lookupCode = a.CURRENCYNAME + " " + a.CURRENCYCODE
                        }).ToListAsync();
            return data;
        }

        public async Task<CurrencyViewModel> GetBaseCurrency(int companyId)
        {
          var baseCurrency = await (from a in context.TBL_COMPANY
             join c in context.TBL_CURRENCY on a.CURRENCYID equals c.CURRENCYID
             where a.COMPANYID == companyId
             select  new CurrencyViewModel
             {
                 currencyId = a.CURRENCYID,
                 currencyCode = c.CURRENCYCODE,
                 currencyName = c.CURRENCYNAME
             }).FirstOrDefaultAsync();

            return baseCurrency;
        }


        public async Task<IEnumerable<CurrencyRateViewModel>> GetCurrencyRate()
        {
            var data = await (from a in context.TBL_CURRENCY_EXCHANGERATE
                        where a.DELETED == false 
                        select new CurrencyRateViewModel
                        {
                            currencyRateId = a.CURRENCYRATEID,
                            currencyId = a.CURRENCYID,
                            baseCurrencyId = a.BASECURRENCYID,
                            currency = a.TBL_CURRENCY1.CURRENCYNAME,
                            baseCurrency = a.TBL_CURRENCY.CURRENCYNAME,
                            rateCodeId = a.RATECODEID,
                            rateCodeName = a.TBL_CURRENCY_RATECODE.RATECODEDESCRIPTION,
                            buyingRate = a.EXCHANGERATE,
                            sellingRate = a.EXCHANGERATE,
                            date = a.DATE,
                           rateCode =a.TBL_CURRENCY_RATECODE.RATECODE,
                            dateTimeCreated = a.DATETIMECREATED,
                            createdBy=a.CREATEDBY,
                            exchangeRate = a.EXCHANGERATE

                        }).ToListAsync();
            return data;
        }

        public async Task<double> GetCurrentCurrencyExchangeRate(short currencyId)
        {
            return await (from a in context.TBL_CURRENCY_EXCHANGERATE.Where(x=>x.CURRENCYID == currencyId && x.RATECODEID == 1
                        && x.DELETED == false) select a.EXCHANGERATE).FirstOrDefaultAsync();
        }
        public async Task<List<CurrencyRateViewModel>> GetCurrencyRateById(short currencyRateId)
        {
            var data = await (from a in context.TBL_CURRENCY_EXCHANGERATE
                        where a.CURRENCYID == currencyRateId && a.DELETED == false
                        
                        select new CurrencyRateViewModel
                        {
                            currencyRateId = a.CURRENCYRATEID,
                            currencyId = a.CURRENCYID,
                            baseCurrencyId = a.BASECURRENCYID,
                            currency = a.TBL_CURRENCY.CURRENCYNAME,
                            baseCurrency = a.TBL_CURRENCY1.CURRENCYNAME,
                            buyingRate = a.EXCHANGERATE,
                            sellingRate = a.EXCHANGERATE,
                            date = a.DATE,
                            rateCodeId = a.RATECODEID,
                            rateCodeName = a.TBL_CURRENCY_RATECODE.RATECODEDESCRIPTION,
                            dateTimeCreated = a.DATETIMECREATED,
                            createdBy = a.CREATEDBY,
                            exchangeRate = a.EXCHANGERATE,

                        }).OrderByDescending(x=>x.dateTimeCreated).ToListAsync();
            return data;
        }

        public async Task<bool> AddCurrencyRate(CurrencyRateViewModel model)
        {
            var data = new TBL_CURRENCY_EXCHANGERATE
            {
                 CURRENCYID = model.currencyId,
                 BASECURRENCYID = model.baseCurrencyId,
                 EXCHANGERATE = model.exchangeRate,
                 RATECODEID = model.rateCodeId,
                 DATE = model.date,
                 CREATEDBY = (int)model.createdBy,
                 DATETIMECREATED = _genSetup.GetApplicationDate()
            };

            context.TBL_CURRENCY_EXCHANGERATE.Add(data);

            // Audit Section ---------------------------
            var audit_Currency = (context.TBL_CURRENCY.FirstOrDefault(x => x.CURRENCYID == model.currencyId)).CURRENCYNAME;
            var audit_BaseCurrency = (context.TBL_CURRENCY.FirstOrDefault(x => x.CURRENCYID == model.baseCurrencyId)).CURRENCYNAME;

            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.CurrencyRateAdded,
                STAFFID = model.createdBy,
                BRANCHID = (short)model.userBranchId,
                DETAIL = $"{TranslateHelper.get("Added Currency Rate")} :  { data.EXCHANGERATE } {TranslateHelper.get("for date")} : '{data.DATE} ' {TranslateHelper.get("on")} {audit_BaseCurrency} {TranslateHelper.get("to:")} {audit_Currency} {TranslateHelper.get("conversion")}",
                URL = model.applicationUrl,
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                 IPADDRESS = CommonHelpers.GetLocalIpAddress(),        
                 DEVICENAME = CommonHelpers.GetDeviceName(),
                 OSNAME = CommonHelpers.FriendlyName()
            };

            this.auditTrail.AddAuditTrail(audit);
            //end of Audit section -------------------------------

            return await context.SaveChangesAsync() != 0;
        }

        public async Task<bool> UpdateCurrencyRate(short currencyRateId, CurrencyRateViewModel model)
        {
            var data = await this.context.TBL_CURRENCY_EXCHANGERATE.FindAsync(currencyRateId);
            if (data == null) return false;

            data.CURRENCYID = model.currencyId;
            data.BASECURRENCYID = model.baseCurrencyId;
            data. EXCHANGERATE = model.exchangeRate;
            data.RATECODEID = model.rateCodeId;
            data.DATE = model.date;

            data.LASTUPDATEDBY = (int)model.createdBy;
            data.DATETIMEUPDATED = _genSetup.GetApplicationDate();

            // Audit Section ---------------------------
            var audit_Currency = (context.TBL_CURRENCY.FirstOrDefault(x => x.CURRENCYID == model.currencyId)).CURRENCYNAME;
            var audit_BaseCurrency = (context.TBL_CURRENCY.FirstOrDefault(x => x.CURRENCYID == model.baseCurrencyId)).CURRENCYNAME;

            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.CurrencyRateUpdated,
                STAFFID = model.createdBy,
                BRANCHID = (short)model.userBranchId,
                DETAIL = $"Updated Currency Rate : { data.EXCHANGERATE } for date: '{data.DATE}' on {audit_BaseCurrency} to: {audit_Currency} conversion",
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
                URL = model.applicationUrl,
                IPADDRESS = model.userIPAddress
            };

            this.auditTrail.AddAuditTrail(audit);

            //end of Audit section -----------------------
            return await context.SaveChangesAsync() != 0;
        }

        public IEnumerable<LookupViewModel> GetRateCode()
        {
            return (from data in context.TBL_CURRENCY_RATECODE
                    select new LookupViewModel()
                    {
                        lookupId = data.RATECODEID,
                        lookupName = data.RATECODE
                    });
        }

        public async Task<IEnumerable<CurrencyRateCodeViewModel>> GetAllCurrencyRateCode()
        {
            return await (from data in context.TBL_CURRENCY_RATECODE
                    select new CurrencyRateCodeViewModel()
                    {
                        rateCode = data.RATECODE,
                        rateCodeDescription = data.RATECODEDESCRIPTION,
                        rateCodeId = data.RATECODEID
                    }).ToListAsync();
        }


    }
}
