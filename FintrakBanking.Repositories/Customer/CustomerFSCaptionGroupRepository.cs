using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FintrakBanking.ViewModels.Customer;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Customer;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Common.Enum;
using System.ComponentModel.Composition;
using FintrakBanking.Common;
using FintrakBanking.ViewModels;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FintrakBanking.Repositories.Customer
{
 
    public class CustomerFSCaptionGroupRepository : ICustomerFSCaptionGroupRepository
    {
        private FinTrakBankingContext context;
        private IGeneralSetupRepository _genSetup;
        private IAuditTrailRepository auditTrail;

        public CustomerFSCaptionGroupRepository(FinTrakBankingContext _context,
                                        IGeneralSetupRepository genSetup,
                                        IAuditTrailRepository _auditTrail)
        {
            this.context = _context;
            this._genSetup = genSetup;
            auditTrail = _auditTrail;
        }

        public async Task<bool> AddCustomerFSCaptionGroup(CustomerFSCaptionGroupViewModel entity)
        {
            var group = new TBL_CUSTOMER_FS_CAPTION_GROUP
            {
                FSCAPTIONGROUPNAME = entity.fsCaptionGroupName,
                POSITION = entity.position,
                //GroupDescription = entity.groupDescription,
                CREATEDBY = (int)entity.createdBy,
                DATETIMECREATED = _genSetup.GetApplicationDate()
            };

            context.TBL_CUSTOMER_FS_CAPTION_GROUP.Add(group);

            // Audit Section ---------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.CustomerFSCaptionGroupAdded,
                STAFFID = entity.createdBy,
                BRANCHID = (short)entity.userBranchId,
                DETAIL = $"{TranslateHelper.get("Added Customer FS Caption Group:")} { entity.fsCaptionGroupName } ",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = entity.applicationUrl,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now
            };

            this.auditTrail.AddAuditTrail(audit);

            //end of Audit section -------------------------------

            return await context.SaveChangesAsync() != 0;
        }

        public async Task<IEnumerable<CustomerFSCaptionGroupViewModel>> GetCustomerFSCaptionGroup()
        {
            var data = await (from a in context.TBL_CUSTOMER_FS_CAPTION_GROUP
                                where a.DELETED == false  
                                orderby a.POSITION
                                select new CustomerFSCaptionGroupViewModel
                                {
                                    fsCaptionGroupId = (short) a.FSCAPTIONGROUPID,
                                    fsCaptionGroupName = a.FSCAPTIONGROUPNAME,
                                    position = a.POSITION,     
                                    dateTimeCreated = a.DATETIMECREATED,
                                    createdBy = a.CREATEDBY
                                }).ToListAsync();
            return data;
        }

        public async Task<IEnumerable<CustomerFSCaptionGroupViewModel>> GetCustomerFSCaptionGroupWithoutRatio()
        {
            var data =await (from a in context.TBL_CUSTOMER_FS_CAPTION_GROUP
                        join b in context.TBL_CUSTOMER_FS_CAPTION on a.FSCAPTIONGROUPID equals b.FSCAPTIONGROUPID
                        where a.DELETED == false && b.ISRATIO == false
                        orderby a.POSITION
                        select new CustomerFSCaptionGroupViewModel
                        {
                            fsCaptionGroupId = (short)a.FSCAPTIONGROUPID,
                            fsCaptionGroupName = a.FSCAPTIONGROUPNAME,
                            position = a.POSITION,
                            dateTimeCreated = a.DATETIMECREATED,
                            createdBy = a.CREATEDBY
                        }).ToListAsync();
            return data.GroupBy(c=> c.fsCaptionGroupId).Select(x=>x.FirstOrDefault());
        }

        public async Task<CustomerFSCaptionGroupViewModel> GetCustomerFSCaptionGroupById(short fsCaptionGroupId)
        {
            var data =await (from a in context.TBL_CUSTOMER_FS_CAPTION_GROUP
                        where a.FSCAPTIONGROUPID == fsCaptionGroupId
                        select new CustomerFSCaptionGroupViewModel
                        {
                            fsCaptionGroupId = a.FSCAPTIONGROUPID,
                            fsCaptionGroupName = a.FSCAPTIONGROUPNAME,
                            position = a.POSITION,
                            dateTimeCreated = a.DATETIMECREATED,
                            createdBy = a.CREATEDBY
                        }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<bool> UpdateCustomerFSCaptionGroup(short groupId, CustomerFSCaptionGroupViewModel entity)
        {
            //var group = this.context.TBL_CUSTOMER_FS_CAPTION_GROUP.Find(groupId);
            var group = await this.context.TBL_CUSTOMER_FS_CAPTION_GROUP.FirstOrDefaultAsync(x=> x.FSCAPTIONGROUPID == groupId);
            //if (group == null) return false;

            group.FSCAPTIONGROUPNAME = entity.fsCaptionGroupName;
            group.POSITION = entity.position;
            group.LASTUPDATEDBY = (int)entity.createdBy;
            group.DATETIMEUPDATED = _genSetup.GetApplicationDate();

            // Audit Section ---------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.CustomerFSCaptionGroupUpdated,
                STAFFID = entity.createdBy,
                BRANCHID = (short)entity.userBranchId,
                DETAIL = $"{TranslateHelper.get("Updated Customer FS Caption Group:")} { entity.fsCaptionGroupName } ",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = entity.applicationUrl,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now
            };

            this.auditTrail.AddAuditTrail(audit);

            //end of Audit section -----------------------
            return await context.SaveChangesAsync() != 0;
        }

        public async Task<bool> DeleteCustomerFSCaptionGroup(int fsCaptionId, UserInfo user)
        {
            var data = await context.TBL_CUSTOMER_FS_CAPTION_GROUP.FindAsync(fsCaptionId);
            data.DELETED = true;
            data.DELETEDBY = (int)user.createdBy;
            data.DATETIMEDELETED = _genSetup.GetApplicationDate();

            // Audit Section ---------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.CustomerFSCaptionGroupDeleted,
                STAFFID = user.staffId,
                BRANCHID = (short)user.BranchId,
                DETAIL = $"{TranslateHelper.get("Deleted Customer FS Caption Group:")} { data.FSCAPTIONGROUPNAME }",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = user.applicationUrl,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now
            };

            this.auditTrail.AddAuditTrail(audit);

            //end of Audit section -----------------------
            return await context.SaveChangesAsync() != 0;
        }
    }
}
