using System;
using System.Collections.Generic;
using System.Linq;
using FintrakBanking.Common.CustomException;
using FintrakBanking.Common.Enum;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.Common;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FintrakBanking.Repositories.Credit
{
    public class DeferredFeeRepository : IDeferredFeeRepository
    {
        private FinTrakBankingContext context;
        private IGeneralSetupRepository general;
        private IAuditTrailRepository audit;
        private IAdminRepository admin;
        private IWorkflow workflow;

        public DeferredFeeRepository(
                FinTrakBankingContext _context,
                IGeneralSetupRepository _general,
                IAuditTrailRepository _audit,
                IAdminRepository _admin,
                IWorkflow _workflow
            )
        {
            this.context = _context;
            this.general = _general;
            this.audit = _audit;
            this.admin = _admin;
            this.workflow = _workflow;
        }

        public async Task<LoanChargeFeeViewModel> GetDeferredFee(int id)
        {
            var entity = await context.TBL_DEFERRED_LOAN_FEE.FirstOrDefaultAsync(x => x.DEFERREDLOANFEEID == id && x.DELETED == false);
            return new LoanChargeFeeViewModel
                {
                    applicationDetailIdId = entity.LOANAPPLICATIONDETAILID,
                    feeTargetId = entity.CHARGEFEEID,
                    loanSystemTypeId = entity.LOANSYSTEMTYPEID,
                    description = entity.DESCRIPTION,
                    chargeFeeName = entity.TBL_CHARGE_FEE.CHARGEFEENAME,
                    feeAmount = entity.FEEAMOUNT,
                };
        }

        public async Task<IEnumerable<LoanChargeFeeViewModel>> GetDeferredFees()
        {
            return await context.TBL_DEFERRED_LOAN_FEE.Where(x => x.DELETED == false)
                .Select(x => new LoanChargeFeeViewModel
                {
                    applicationDetailIdId = x.LOANAPPLICATIONDETAILID,
                    feeTargetId = x.CHARGEFEEID,
                    loanSystemTypeId = x.LOANSYSTEMTYPEID,
                    description = x.DESCRIPTION,
                    chargeFeeName = x.TBL_CHARGE_FEE.CHARGEFEENAME,
                    feeAmount = x.FEEAMOUNT,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<LoanChargeFeeViewModel>> GetLoanDetailDeferredFees(int loanDetailId)
        {
            {
                var data = await (from x in context.TBL_DEFERRED_LOAN_FEE
                            where x.LOANAPPLICATIONDETAILID == loanDetailId
                            select new LoanChargeFeeViewModel()
                            {
                                applicationDetailIdId = x.LOANAPPLICATIONDETAILID,
                                feeTargetId = x.CHARGEFEEID,
                                loanSystemTypeId = x.LOANSYSTEMTYPEID,
                                description = x.DESCRIPTION,
                                chargeFeeName = x.TBL_CHARGE_FEE.CHARGEFEENAME,
                                feeAmount = x.FEEAMOUNT,
                            })
                .ToListAsync();
                return data;
            }
        }

        public async Task<bool> AddDeferredFee(List<LoanChargeFeeViewModel> model, UserInfo user)
        {
            foreach (var fee in model)
            {
                    var entity = new TBL_DEFERRED_LOAN_FEE
                {
                    LOANAPPLICATIONDETAILID = fee.applicationDetailIdId,
                    LOANSYSTEMTYPEID = fee.loanSystemTypeId,
                    CASAACCOUNTID = fee.casaAccountId,
                    CHARGEFEEID = fee.chargeFeeId,
                    FEEAMOUNT = fee.feeAmount,
                    DESCRIPTION = fee.description,
                    EFFECTIVEDATE = fee.effectiveDate,
                    CREATEDBY = user.createdBy,
                    DATETIMECREATED = general.GetApplicationDate(),
                };

                context.TBL_DEFERRED_LOAN_FEE.Add(entity);

                var auditStaff = (context.TBL_STAFF.Where(x => x.STAFFID == user.createdBy).Select(x => x.STAFFCODE));
                // Audit Section ---------------------------
                this.audit.AddAuditTrail(new TBL_AUDIT
                {
                    AUDITTYPEID = (short)AuditTypeEnum.TermSheetAdded,
                    STAFFID = user.createdBy,
                    BRANCHID = (short)user.BranchId,
                    DETAIL = $"TBL_DEFERRED_LOAN_FEE '{entity.DESCRIPTION}' {TranslateHelper.get("created by")} {auditStaff}",
                    IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                     URL = user.applicationUrl,
                    APPLICATIONDATE = general.GetApplicationDate(),
                    SYSTEMDATETIME = DateTime.Now,
                    DEVICENAME = CommonHelpers.GetDeviceName(),
                    OSNAME = CommonHelpers.FriendlyName()
                });
                // Audit Section end ------------------------
            }
            return await context.SaveChangesAsync() != 0;
        }

        public async Task<bool> UpdateDeferredFee(LoanChargeFeeViewModel model, int id, UserInfo user)
        {
            var entity = this.context.TBL_DEFERRED_LOAN_FEE.Find(id);
            entity.LOANAPPLICATIONDETAILID = model.applicationDetailIdId;
            entity.LOANSYSTEMTYPEID = model.loanSystemTypeId;
            entity.CASAACCOUNTID = model.casaAccountId;
            entity.CHARGEFEEID = model.chargeFeeId;
            entity.FEEAMOUNT = model.feeAmount;
            entity.DESCRIPTION = model.description;
            entity.EFFECTIVEDATE = model.effectiveDate;
            entity.CREATEDBY = user.createdBy;
            entity.DATETIMECREATED = general.GetApplicationDate();
            entity.LASTUPDATEDBY = user.createdBy;
            entity.DATETIMEUPDATED = DateTime.Now;

            var auditStaff = (context.TBL_STAFF.Where(x => x.STAFFID == user.createdBy).Select(x => x.STAFFCODE));
            // Audit Section ---------------------------
            this.audit.AddAuditTrail(new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.TermSheetUpdated,
                STAFFID = user.createdBy,
                BRANCHID = (short)user.BranchId,
                DETAIL = $"TBL_DEFERRED_LOAN_FEE '{entity.DESCRIPTION}' {TranslateHelper.get("was updated by")} {auditStaff}",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(), 
                TARGETID = entity.DEFERREDLOANFEEID,
                URL = user.applicationUrl,
                APPLICATIONDATE = general.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName()
            });
            // Audit Section end ------------------------

            return await context.SaveChangesAsync() != 0;
        }

        public async Task<bool> DeleteDeferredFee(int id, UserInfo user)
        {
            var entity = this.context.TBL_DEFERRED_LOAN_FEE.Find(id);
            entity.DELETED = true;
            entity.DELETEDBY = user.createdBy;
            entity.DATETIMEDELETED = general.GetApplicationDate();

            var auditStaff = (context.TBL_STAFF.Where(x => x.STAFFID == user.createdBy).Select(x => x.STAFFCODE));
            // Audit Section ---------------------------
            this.audit.AddAuditTrail(new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.TermSheetDeleted, //still missing its value
                STAFFID = user.createdBy,
                BRANCHID = (short)user.BranchId,
                DETAIL = $"TBL_DEFERRED_LOAN_FEE '{entity.DESCRIPTION}' {TranslateHelper.get("was deleted by")} {auditStaff}",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                TARGETID = entity.DEFERREDLOANFEEID,
                URL = user.applicationUrl,
                APPLICATIONDATE = general.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName()
            });
            // Audit Section end ------------------------

            return await context.SaveChangesAsync() != 0;
        }
    }
}

// kernel.Bind<ITermSheetRepository>().To<TermSheetRepository>();
// TermSheetAdded = ???, TermSheetUpdated = ???, TermSheetDeleted = ???,
