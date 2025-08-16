using FintrakBanking.Common;
using FintrakBanking.Common.Enum;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Interfaces.Setups.Credit;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels.Setups.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Repositories.Setups.Credit
{
   public class CrmsRegulatoryRepository : ICrmsRegulatoryRepository
    {
        private FinTrakBankingContext context;
        private IGeneralSetupRepository general;
        private IAuditTrailRepository audit;
        public CrmsRegulatoryRepository(FinTrakBankingContext context, IGeneralSetupRepository general, IAuditTrailRepository audit)
        {
            this.context = context;
            this.general = general;
            this.audit = audit;
        }
        #region CRMS REGULATORY
        public async Task<IEnumerable<CrmsRegulatoryViewModel>> GetAllRegulatorySetup()
        {
            var data = this.context.TBL_CRMS_REGULATORY.Where(x => x.DELETED == false).Select(x => new CrmsRegulatoryViewModel
            {
                regulatoryId = x.CRMSREGULATORYID,
                crmsTypeId = x.CRMSTYPEID,
                customerTypeId = x.CUSTOMERTYPEID,
                companyId = x.COMPANYID,
                code = x.CODE,
                description = x.DESCRIPTION,
            }).ToList();

            return data;
        }
        public async Task<IEnumerable<CrmsRegulatoryTypeViewModel>> GetAllRegulatoryType()
        {
            return this.context.TBL_CRMS_TYPE.Select(x => new CrmsRegulatoryTypeViewModel
            {
                crmsTypeId = x.CRMSTYPEID,
                companyId = x.COMPANYID,
                description = x.DESCRIPTION,

            });
        }
        public async Task<bool> AddRegulatory(CrmsRegulatoryViewModel model)
        {
            //if (String.IsNullOrEmpty(model.templateDocument)) { throw new SecureException("Document is blank. Cannot create a blank document!"); }

            var data = new TBL_CRMS_REGULATORY
            {
                COMPANYID = model.companyId,
                CRMSTYPEID=model.crmsTypeId,
                CODE = model.code,
                CUSTOMERTYPEID = model.customerTypeId,
                DESCRIPTION = model.description,
                CREATEDBY = (int)model.createdBy,
                DATETIMECREATED = general.GetApplicationDate()
            };

            context.TBL_CRMS_REGULATORY.Add(data);

            // Audit Section ---------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.CrmsRegulatorySetupAdded,
                STAFFID = model.createdBy,
                BRANCHID = (short)model.userBranchId,
                DETAIL = $"{TranslateHelper.get("Added CrmsRegulatory")} '{ model.description }' ",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = model.applicationUrl,
                APPLICATIONDATE = general.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName()
            };
            this.audit.AddAuditTrail(audit);
            // End of Audit Section ---------------------

            return context.SaveChanges() != 0;
        }
        public async Task<bool> UpdateRegulatory(CrmsRegulatoryViewModel model, int regulatoryId)
        {
            var data = this.context.TBL_CRMS_REGULATORY.Find(regulatoryId);
            if (data == null)
            {
                return false;
            }
            data.CRMSTYPEID = model.crmsTypeId;
            data.CODE = model.code;
            data.CUSTOMERTYPEID = model.customerTypeId;
            data.DESCRIPTION = model.description;
            data.LASTUPDATEDBY = model.lastUpdatedBy;
            data.DATETIMEUPDATED = general.GetApplicationDate();

            // Audit Section ---------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.CrmsRegulatorySetupUpdated,
                STAFFID = model.lastUpdatedBy,
                BRANCHID = (short)model.userBranchId,
                DETAIL = $"{TranslateHelper.get("Updated CrmsRegulatory with ID=")} ' {data.CRMSREGULATORYID} {TranslateHelper.get("with Description=")} { data.DESCRIPTION }' ",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = model.applicationUrl,
                APPLICATIONDATE = general.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName()
            };
            this.audit.AddAuditTrail(audit);
            // End of Audit Section ---------------------

            return context.SaveChanges() != 0;
        }
        public async Task<bool> DeleteRegulatory(int regulatoryId, short userBranchId, int companyId, int lastUpdatedBy, string applicationUrl, string userIPAddress)
        {
            var data = this.context.TBL_CRMS_REGULATORY.Find(regulatoryId);
            if (data != null)
            {
                data.DELETED = true;
            }
            // Audit Section ---------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.CrmsRegulatorySetupDeleted,
                STAFFID = lastUpdatedBy,
                BRANCHID = userBranchId,
                DETAIL = $"{TranslateHelper.get("Deleted CrmsRegulatory with ID=")} ' {data.CRMSREGULATORYID} {TranslateHelper.get("with Description=")} { data.DESCRIPTION }' ",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = applicationUrl,
                APPLICATIONDATE = general.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName()
            };
            this.audit.AddAuditTrail(audit);
            // End of Audit Section ---------------------
            return context.SaveChanges() != 0;
        }
        public async Task<IEnumerable<CrmsRegulatoryViewModel>> GetRegulatoryByTypeId(int crmsTypeId, int companyId)
        {
            var data = await GetAllRegulatorySetup();

            return data.Where(x=>x.crmsTypeId == crmsTypeId);
        }

        #endregion

    }
}
