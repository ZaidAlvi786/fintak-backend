using System;
using System.Collections.Generic;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels;
using FintrakBanking.Interfaces.Setups.Finance;
using FintrakBanking.ViewModels.Setups.Finance;
using FintrakBanking.Common.Enum;
using System.Linq;
using FinTrakBanking.ThirdPartyIntegration.Finacle;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common;
using System.Threading.Tasks;

namespace FintrakBanking.Repositories.Setups.Finance
{
    public class CustomChartOfAccountRepository : ICustomChartOfAccountRepository
    {
        private FinTrakBankingContext context;
        private IGeneralSetupRepository general;
        private IAuditTrailRepository audit;
        private IIntegrationWithFinacle finacle;

        public CustomChartOfAccountRepository(FinTrakBankingContext context, IGeneralSetupRepository general, IAuditTrailRepository audit, IIntegrationWithFinacle finacle)
        {
            this.context = context;
            this.general = general;
            this.audit = audit;
            this.finacle = finacle;
        }

        public async Task<bool> AddCustomChartOfAccount(CustomChartOfAccountViewModel model)
        {
            ValidateAccountId(model.accountId);

            var data = new TBL_CUSTOM_CHART_OF_ACCOUNT
            {
                ACCOUNTID = model.accountId,
                ACCOUNTNAME = model.accountName,
                CURRENCYCODE = model.currencyCode,
                PLACEHOLDERID = model.placeholderId,
            };

            context.TBL_CUSTOM_CHART_OF_ACCOUNT.Add(data);

            // Audit Section ---------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.CustomChartOfAccountAdded,
                STAFFID = model.createdBy,
                BRANCHID = (short)model.userBranchId,
                DETAIL = $"{TranslateHelper.get("Added Custom Chart Of Account")} '{ model.accountName }' ",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = model.applicationUrl,
                APPLICATIONDATE = general.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName()
            };
            this.audit.AddAuditTrail(audit);
            // End of Audit Section ---------------------

            return await context.SaveChangesAsync() != 0;
        }

        private void ValidateAccountId(string accountId)
        {
            var applicationSetup = context.TBL_SETUP_GLOBAL.FirstOrDefault();

            if (applicationSetup.USE_THIRD_PARTY_INTEGRATION == true)
            {
                if (finacle.ValidateGLNumber("100" + accountId) == null) // 100 is headoffice code
                {
                    throw new SecureException(TranslateHelper.get("The specified account id do not exist!"));
                }
            }
        }

        public async Task<bool> UpdateCustomChartOfAccount(CustomChartOfAccountViewModel model, int customChartOfAccountId)
        {
            ValidateAccountId(model.accountId);

            var data = await this.context.TBL_CUSTOM_CHART_OF_ACCOUNT.FindAsync(customChartOfAccountId);
            if (data == null)
            {
                return false;
            }

            data.ACCOUNTID = model.accountId;
            data.ACCOUNTNAME = model.accountName;
            data.CURRENCYCODE = model.currencyCode;
            data.PLACEHOLDERID = model.placeholderId;

            // Audit Section ---------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.CustomChartOfAccountUpdated,
                STAFFID = model.lastUpdatedBy,
                BRANCHID = (short)model.userBranchId,
                DETAIL = $"{TranslateHelper.get("Updated Custom Chart Of Account")} '{ model.accountName }' ",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = model.applicationUrl,
                APPLICATIONDATE = general.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName()
            };
            this.audit.AddAuditTrail(audit);
            // End of Audit Section ---------------------

            return await context.SaveChangesAsync() != 0;
        }

        public IEnumerable<CustomChartOfAccountViewModel> GetAllCustomChartOfAccount()
        {
            return this.context.TBL_CUSTOM_CHART_OF_ACCOUNT.Select(x => new CustomChartOfAccountViewModel
            {
                customAccountId = x.CUSTOMACCOUNTID,
                accountId = x.ACCOUNTID,
                accountName = x.ACCOUNTNAME,
                currencyCode = x.CURRENCYCODE,
                placeholderId = x.PLACEHOLDERID,
                isNostroAccount = x.ISNOSTROACCOUNT,
                detail = x.ACCOUNTID + "-(" + x.ACCOUNTNAME + "-" + x.CURRENCYCODE + ")"
            });
        }

        public async Task<CustomChartOfAccountViewModel> GetCustomChartOfAccount(int customChartOfAccountId)
        {
            var data = await this.context.TBL_CUSTOM_CHART_OF_ACCOUNT.FindAsync(customChartOfAccountId);

            if (data == null)
            {
                return null;
            }

            return new CustomChartOfAccountViewModel
            {
                customAccountId = data.CUSTOMACCOUNTID,
                accountId = data.ACCOUNTID,
                accountName = data.ACCOUNTNAME,
                currencyCode = data.CURRENCYCODE,
                placeholderId = data.PLACEHOLDERID,
                isNostroAccount = data.ISNOSTROACCOUNT,
                detail = data.ACCOUNTID + "-(" + data.ACCOUNTNAME + "-" + data.CURRENCYCODE + ")"
            };
        }

        public IEnumerable<CustomChartOfAccountViewModel> GetAllCustomChartOfAccountByCompanyId(int companyId)
        {
            var chartOfAccounts = this.GetAllCustomChartOfAccount().Where(x => x.companyId == companyId);
            return chartOfAccounts;
        }

        public IEnumerable<CustomChartOfAccountViewModel> GetnostroCustomChartOfAccountByCompanyId(int companyId)
        {
            var chartOfAccounts = this.GetAllCustomChartOfAccount().Where(x=>x.isNostroAccount == true);
            var b = chartOfAccounts.ToList();
            return chartOfAccounts;
        }
    }
}

