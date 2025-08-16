using FintrakBanking.Interfaces.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Common.Enum;
using FintrakBanking.ViewModels.Setups.General;
using FintrakBanking.Common;
using System.Data.Entity;

namespace FintrakBanking.Repositories.Credit
{
    public class LoanPrincipalRepository : ILoanPrincipalRepository
    {
        private readonly FinTrakBankingContext _context;
        private readonly IGeneralSetupRepository _genSetup;
        private readonly IAuditTrailRepository _auditTrail;
        //   private TokenDecryptionHelper token = new TokenDecryptionHelper();



        public LoanPrincipalRepository(FinTrakBankingContext context, IGeneralSetupRepository genSetup,
                                IAuditTrailRepository auditTrail)
        {
            _context = context;
            _genSetup = genSetup;
            _auditTrail = auditTrail;
        }

        public async Task<string> AddLoanPrincipal(LoanPrincipalViewModel loanP)
        {
            if (loanP != null)
            {
                var value = new TBL_LOAN_PRINCIPAL
                {
                    PRINCIPALSREGNUMBER = loanP.principalsRegNumber,
                    NAME = loanP.name,
                    ACCOUNTNUMBER = loanP.accountNumber,
                    EMAILADDRESS = loanP.emailAddress,
                    PHONENUMBER = loanP.phoneNumber,
                    COMPANYID = loanP.companyId,
                    ADDRESS = loanP.address,
                    DATETIMECREATED = _genSetup.GetApplicationDate(),
                };

                _context.TBL_LOAN_PRINCIPAL.Add(value);
               await _context.SaveChangesAsync();

                var audit = new TBL_AUDIT
                {
                    AUDITTYPEID = (short)AuditTypeEnum.LoanPrincipalInserted,
                    STAFFID = loanP.staffId,
                    BRANCHID = (short)loanP.userBranchId,
                    DETAIL = $"{TranslateHelper.get("Loan principal with company id is added:")} {loanP.companyId} ",
                    IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                    URL = loanP.applicationUrl,
                    DEVICENAME = CommonHelpers.GetDeviceName(),
                    OSNAME = CommonHelpers.FriendlyName(),
                    APPLICATIONDATE = _genSetup.GetApplicationDate(),
                    SYSTEMDATETIME = DateTime.Now
                };
                this._auditTrail.AddAuditTrail(audit);

                return TranslateHelper.get("The record has been added successfully");

            }
            return TranslateHelper.get("The record has not been added");
        }

        public async Task<string> DeleteLoanPrincipal(LoanPrincipalViewModel loanPrincipal)
        {
            TBL_LOAN_PRINCIPAL data = _context.TBL_LOAN_PRINCIPAL.Find(loanPrincipal.principalId);
            if (data != null)
            {
                data.DATETIMEDELETED = loanPrincipal.dateTimeDeleted;
                data.DELETED = true;
                data.DELETEDBY = loanPrincipal.staffId;
                await _context.SaveChangesAsync();
                return TranslateHelper.get("The record has been deleted successfully");
            }
            return TranslateHelper.get("The record has not been deleted");
            // Audit Section ---------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.LoanPrincipalDeleted,
                STAFFID = loanPrincipal.staffId,
                BRANCHID = (short)loanPrincipal.userBranchId,
                DETAIL = $"{TranslateHelper.get("Deleted loan principal with id")}: {data.PRINCIPALID}",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = loanPrincipal.applicationUrl,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now
            };

            this._auditTrail.AddAuditTrail(audit);
        }

        public async Task< IEnumerable<LoanPrincipalViewModel>> GetLoanPrincipal(int conpanyId)
        {

            var data = await (from o in _context.TBL_LOAN_PRINCIPAL
                        where o.COMPANYID == conpanyId & o.DELETED == false
                        orderby o.NAME
                        select new LoanPrincipalViewModel
                        {

                            accountNumber = o.ACCOUNTNUMBER,
                            address = o.ADDRESS,
                            emailAddress = o.EMAILADDRESS,
                            name = o.NAME,
                            phoneNumber = o.PHONENUMBER,
                            principalsRegNumber = o.PRINCIPALSREGNUMBER,
                            principalId = o.PRINCIPALID,

                        }).ToListAsync();

            return data;


        }

        public async Task<LoanPrincipalViewModel> GetLoanPrincipal(int principalId, int companyId)
        {
            LoanPrincipalViewModel val = new LoanPrincipalViewModel();

            if (principalId != 0)
            {
                var data = await (from a in _context.TBL_LOAN_PRINCIPAL
                            where a.PRINCIPALID == principalId & a.COMPANYID == companyId & a.DELETED == false
                            select a).FirstOrDefaultAsync();

                val.accountNumber = data.ACCOUNTNUMBER;
                val.address = data.ADDRESS;
                val.emailAddress = data.EMAILADDRESS;
                val.name = data.NAME;
                val.phoneNumber = data.PHONENUMBER;
                val.principalsRegNumber = data.PRINCIPALSREGNUMBER;
            }

            return val;
        }


        public async Task<string> UpdateLoanPrincipal(LoanPrincipalViewModel model)
        {
            TBL_LOAN_PRINCIPAL val = _context.TBL_LOAN_PRINCIPAL.FirstOrDefault(x => x.PRINCIPALID == model.principalId);
            if (val != null)
            {

                val.ACCOUNTNUMBER = model.accountNumber;
                val.ADDRESS = model.address;
                val.EMAILADDRESS = model.emailAddress;
                val.NAME = model.name;
                val.PHONENUMBER = model.phoneNumber;
                val.PRINCIPALSREGNUMBER = model.principalsRegNumber;

                val.DATETIMEUPDATED = _genSetup.GetApplicationDate();
                val.LASTUPDATEDBY = model.createdBy;

                await _context.SaveChangesAsync();

                // Audit Section ---------------------------
                var audit = new TBL_AUDIT
                {
                    AUDITTYPEID = (short)AuditTypeEnum.LoanPrincipalUpdated,
                    STAFFID = model.staffId,
                    BRANCHID = (short)model.userBranchId,
                    DETAIL = $"{TranslateHelper.get("Update loan principal with id")} {model.principalId}",
                    IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                    URL = model.applicationUrl,
                    DEVICENAME = CommonHelpers.GetDeviceName(),
                    OSNAME = CommonHelpers.FriendlyName(),
                    APPLICATIONDATE = _genSetup.GetApplicationDate(),
                    SYSTEMDATETIME = DateTime.Now
                };

                this._auditTrail.AddAuditTrail(audit);

                return TranslateHelper.get("The record has been updated successful");
            }

            return TranslateHelper.get("The record has not been updated");
        }

        
    }
}
