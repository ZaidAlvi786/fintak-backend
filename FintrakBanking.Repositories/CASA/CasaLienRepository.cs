using FintrakBanking.Common;
using FintrakBanking.Common.Enum;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Interfaces.CASA;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels.CASA;
using FintrakBanking.ViewModels.Finance; 
using FinTrakBanking.ThirdPartyIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinTrakBanking.ThirdPartyIntegration.Finacle;
using FintrakBanking.ViewModels.ThridPartyIntegration;
using FintrakBanking.Common.CustomException;
using static FinTrakBanking.ThirdPartyIntegration.TwoFactorAuthIntegration.TwoFactorAuthIntegrationService;
using FintrakBanking.ViewModels.Setups.General;
using System.Configuration;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.Setups.Credit;
using FintrakBanking.ViewModels;
using FintrakBanking.Repositories.Credit;
using System.Data.Entity;

namespace FintrakBanking.Repositories.CASA
{
    public class CasaLienRepository : ICasaLienRepository
    {
        private FinTrakBankingContext context;
        private IGeneralSetupRepository generalSetup;
        private IAuditTrailRepository auditTrail;
        private ITwoFactorAuthIntegrationService twoFactorAuth;
        private TransactionPosting tran;
        bool USE_TWO_FACTOR_AUTHENTICATION = false;
        bool USE_THIRD_PARTY_INTEGRATION = false;
        TBL_INTEGRATION_CONTROL globalIntegrationSetting = new TBL_INTEGRATION_CONTROL();
        //private ILoanOperationsRepository creditOperations;
        List<string> receiverEmailList = new List<string>();
        AlertsViewModel alert = new AlertsViewModel();

        // CONSUMER PROTECTION
        private string consumerProtectionEmailData;
        private string consumerProtectionAddressData;
        private string consumerProtectionPhoneNumberData;
        private string consumerProtectionFeesAndChargesData;
        private string consumerProtectionRepaymentsData;
        private string consumerProtectionLoanDetailsData;
        private string consumerProtectionLoanSpecificInformationData;

        // CONSUMER PROTECTION
        private readonly string consumerProtectionEmailHolder = "@{{consumerProtectionEmail}}";
        private readonly string consumerProtectionAddressHolder = "@{{consumerProtectionAddress}}";
        private readonly string consumerProtectionPhoneNumberHolder = "@{{consumerProtectionPhoneNumber}}";
        private readonly string consumerProtectionFeesAndChargesHolder = "@{{consumerProtectionFeesAndCharges}}";
        private readonly string consumerProtectionRepaymentsHolder = "@{{consumerProtectionRepayments}}";
        private readonly string consumerProtectionLoanDetailsHolder = "@{{consumerProtectionLoanDetails}}";
        private readonly string consumerProtectionLoanSpecificInformationHolder = "@{{consumerProtectionLoanSpecificInformation}}";

        public CasaLienRepository(IGeneralSetupRepository _genSetup, IAuditTrailRepository _auditTrail,
                                            //ILoanOperationsRepository _creditOperations, 
                                            FinTrakBankingContext _context, TransactionPosting tran, ITwoFactorAuthIntegrationService _twoFactorAuth)
        {
            this.context = _context;
            this.tran = tran;
            this.generalSetup = _genSetup;
            auditTrail = _auditTrail;
            this.twoFactorAuth = _twoFactorAuth;
            //this.memo = _memo;
            //this.creditOperations = _creditOperations;
            var setup = context.TBL_SETUP_GLOBAL.FirstOrDefault();
            globalIntegrationSetting = context.TBL_INTEGRATION_CONTROL.FirstOrDefault();
            USE_THIRD_PARTY_INTEGRATION = setup.USE_THIRD_PARTY_INTEGRATION;
            USE_TWO_FACTOR_AUTHENTICATION = setup.USE_TWO_FACTOR_AUTHENTICATION;
        }


        public string PlaceLien(CasaLienViewModel model, TwoFactorAutheticationViewModel twoFADetails = null)
        {
            var referenceNumber = CommonHelpers.GenerateRandomDigitCode(10);
            model.lienReferenceNumber = referenceNumber;

            //call     
            if (USE_TWO_FACTOR_AUTHENTICATION)
            {
                if (twoFADetails == null)
                    throw new TwoFactorAuthenticationException(TranslateHelper.get("Authentication token not specified. Specify the second factor authentication token"));

                if (twoFADetails.skipAuthentication == false)
                {
                    var authenticated = twoFactorAuth.Authenticate(twoFADetails.username, twoFADetails.passcode);

                    if (authenticated.authenticated == false)
                        throw new TwoFactorAuthenticationException(authenticated.message);
                }
            }

            if (USE_THIRD_PARTY_INTEGRATION && globalIntegrationSetting.USE_THIRDPARTY_LIEN )
            {

                ResponseMessage result = null;

                Task.Run(async () => { result = await tran.APIProcessLien(model, "PLACE"); }).GetAwaiter().GetResult();

                if (result.APIResponse != null)
                {
                    if (result.APIResponse.responseCode == "0")
                    {
                        PlaceLienSub(model);
                    }
                    else
                    {
                        throw new ConditionNotMetException(TranslateHelper.get("Core Banking API Error - ") + result.APIResponse.webRequestStatus);
                    }
                }
                else
                {
                    throw new APIErrorException(TranslateHelper.get("Core Banking API Error - ") + result.Message.ReasonPhrase);
                }

            }

            else
            {
                PlaceLienSub(model);
            }

            return referenceNumber;
        }

        private void PlaceLienSub(CasaLienViewModel model)
        {
            var validate = context.TBL_CASA_LIEN.Where(x => x.SOURCEREFERENCENUMBER == model.sourceReferenceNumber).FirstOrDefault();
            if (validate == null)
            {
                var data = new TBL_CASA_LIEN
                {
                    PRODUCTACCOUNTNUMBER = model.productAccountNumber,
                    LIENREFERENCENUMBER = model.lienReferenceNumber,
                    SOURCEREFERENCENUMBER = model.sourceReferenceNumber,
                    BRANCHID = model.branchId,
                    COMPANYID = model.companyId,
                    LIENAMOUNT = model.lienAmount,
                    DESCRIPTION = model.description,
                    LIENTYPEID = model.lienTypeId,
                    CREATEDBY = model.createdBy,
                    DATETIMECREATED = DateTime.Now,
                    LIENSTATUS = (int)LienStatusEnum.Active,
                    ISLIENREMOVED = false
                };

                context.TBL_CASA_LIEN.Add(data);

                // Audit Section ---------------------------            

                var audit = new TBL_AUDIT
                {
                    AUDITTYPEID = (short)AuditTypeEnum.LienPlaced,
                    STAFFID = model.createdBy,
                    BRANCHID = model.branchId,
                    DETAIL = $"{TranslateHelper.get("Applied lien with reference number:")} {model.lienReferenceNumber}",
                    IPADDRESS = model.userIPAddress,
                    URL = model.applicationUrl,
                    APPLICATIONDATE = generalSetup.GetApplicationDate(),
                    SYSTEMDATETIME = DateTime.Now,
                    DEVICENAME = CommonHelpers.GetDeviceName(),
                    OSNAME = CommonHelpers.FriendlyName()


                };
                this.auditTrail.AddAuditTrail(audit);

                //end of Audit section -------------------------------

                if (context.SaveChanges() > 0)
                {
                    if (model.createdBy != 1)
                    {
                        var customer = "";
                        var termLoan = context.TBL_LOAN.Where(t => t.LOANREFERENCENUMBER == model.sourceReferenceNumber).FirstOrDefault();
                        if(termLoan != null)
                        {
                            customer = context.TBL_CUSTOMER.Where(c=>c.CUSTOMERID == termLoan.CUSTOMERID).Select(c=>c.FIRSTNAME + " " +c.MIDDLENAME +" " + c.LASTNAME).FirstOrDefault();
                        }
                        var revolvingLoan = context.TBL_LOAN_REVOLVING.Where(t => t.LOANREFERENCENUMBER == model.sourceReferenceNumber).FirstOrDefault();
                        if (revolvingLoan != null)
                        {
                            customer = context.TBL_CUSTOMER.Where(c => c.CUSTOMERID == revolvingLoan.CUSTOMERID).Select(c => c.FIRSTNAME + " " + c.MIDDLENAME + " " + c.LASTNAME).FirstOrDefault();
                        }
                        var contingentLoan = context.TBL_LOAN_CONTINGENT.Where(t => t.LOANREFERENCENUMBER == model.sourceReferenceNumber).FirstOrDefault();
                        if (contingentLoan != null)
                        {
                            customer = context.TBL_CUSTOMER.Where(c => c.CUSTOMERID == contingentLoan.CUSTOMERID).Select(c => c.FIRSTNAME + " " + c.MIDDLENAME + " " + c.LASTNAME).FirstOrDefault();
                        }
                        var staff = context.TBL_STAFF.Find(model.createdBy);
                        var alertDetail = context.TBL_ALERT_TITLE.Where(x => x.BINDINGMETHOD == "LienAccountNotification").FirstOrDefault();
                        var emailList = GetBusinessUsersEmailsToGroupHead(staff.MISCODE);
                        alert.receiverEmailList.Add(emailList);
                        var alertTemplate = alertDetail.TEMPLATE;
                        var accountOfficer = staff.FIRSTNAME + " " + staff.LASTNAME + " " + staff.MIDDLENAME;
                        alertTemplate = alertTemplate.Replace("@{{accountOfficer}}", accountOfficer);
                        alertTemplate = alertTemplate.Replace("@{{customer}}", customer);
                        alertTemplate = alertTemplate.Replace("@{{accountNumber}}", model.productAccountNumber);
                        alertTemplate = alertTemplate.Replace("@{{sourceReferenceNumber}}", model.sourceReferenceNumber);
                        LogEmailAlert(alertTemplate, alertDetail.TITLE, alert.receiverEmailList, "11023", 11023, "LienAccountNotification");
                    }
                }
            }
        }

        public int AddConsumerProtection(ConsumerProtectionViewModel model)
        {
            var data = new TBL_CONSUMER_PROTECTION
            {
                ACTUALAMOUNTBORROWED = model.actualAmountBorrowed,
                ANNUALINTERESTRATE = model.annualInterestRate,
                //CONSUMERPROTECTIONID = model.consumerProtectionId,
                INSURANCE = model.insurance,
                LOANAMOUNT = model.loanAmount,
                LOANAPR = model.loanAPR,
                MONTHLYPAYMENT = model.monthlyPayment,
                TERMOFLOANSINYEARS = model.termOfLoanInYears,
                TOTALFEES = model.totalFees,
                TOTALFEESANDCHARGES = model.totalFeesAndCharges,
                
                BRANCHID = model.branchId,
                COMPANYID = model.companyId,
                
                CREATEDBY = model.createdBy,
                DATETIMECREATED = DateTime.Now,
               
            };

            context.TBL_CONSUMER_PROTECTION.Add(data);

            // Audit Section ---------------------------            

            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.LienPlaced,
                STAFFID = model.createdBy,
                BRANCHID = model.branchId,
                DETAIL = $"{TranslateHelper.get("Added Consumer Protection with Actual Amount Borrowed:")} {model.actualAmountBorrowed}",
                IPADDRESS = model.userIPAddress,
                URL = model.applicationUrl,
                APPLICATIONDATE = generalSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName()


            };
            this.auditTrail.AddAuditTrail(audit);

            //end of Audit section -------------------------------

            if (context.SaveChanges() > 0) { return data.CONSUMERPROTECTIONID; }

            return 0;
        }

        public async Task<IEnumerable<ConsumerProtectionViewModel>> GetAllConsumerProtections(int companyId)
        {
            var result = await context.TBL_CONSUMER_PROTECTION.Select(O => new ConsumerProtectionViewModel
            {
                actualAmountBorrowed = O.ACTUALAMOUNTBORROWED,
                annualInterestRate = O.ANNUALINTERESTRATE,
                consumerProtectionId = O.CONSUMERPROTECTIONID,
                insurance = O.INSURANCE,
                loanAmount = O.LOANAMOUNT,
                loanAPR = O.LOANAPR,
                monthlyPayment = O.MONTHLYPAYMENT,
                termOfLoanInYears = O.TERMOFLOANSINYEARS,
                totalFees = O.TOTALFEES,
                totalFeesAndCharges = O.TOTALFEESANDCHARGES,

                branchId = O.BRANCHID,
                companyId = O.COMPANYID,

                createdBy = O.CREATEDBY,
                dateTimeCreated = O.DATETIMECREATED,
            }).ToListAsync();

            return result;
        }

        public async Task<List<LoadedDocumentSectionViewModel>> GetLoadedDocumentationConsumerProtection(int staffId, int operationId, int targetId, UserInfo user)
        {
            // int staffId, is REDUNDANT!
            var printedDoc = "";
            var rawSections = await context.TBL_DOC_TEMPLATE_DETAIL
                .Where(x => x.DELETED == false && x.OPERATIONID == operationId && x.TARGETID == targetId)
                .OrderBy(x => x.POSITION)
                .Select(x => new LoadedDocumentSectionViewModel
                {
                    position = x.POSITION,
                    sectionId = x.DOCUMENTDETAILID,
                    title = x.TITLE,
                    description = x.DESCRIPTION,
                    canEdit = x.CANEDIT, // system
                    // editable = sectionIds.Contains(x.TEMPLATESECTIONID),
                    templateSectionId = x.TEMPLATESECTIONID,
                    templateDocument = x.TEMPLATEDOCUMENT, // placeholder find replace
                })
                .ToListAsync();

            List<LoadedDocumentSectionViewModel> replacedSections = new List<LoadedDocumentSectionViewModel>();
            InitForConsumerProtection(targetId);

            foreach (var raw in rawSections)
            {
                var templateId = context.TBL_DOC_TEMPLATE_SECTION.Find(raw.templateSectionId)?.TEMPLATEID;

                raw.templateDocument = Replace(raw.templateDocument);

                replacedSections.Add(raw);
                printedDoc = raw.title;
            }

            var staff = context.TBL_STAFF.Find(staffId);

            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.DocumentTemplatePrinted,
                STAFFID = staffId,
                BRANCHID = (short)user.BranchId,
                DETAIL = $"{TranslateHelper.get("Printed Document Template")} '{ printedDoc }' ",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = "localhost",//model.applicationUrl,
                APPLICATIONDATE = generalSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName()
            };
            this.auditTrail.AddAuditTrail(audit);
            context.SaveChanges();

            return replacedSections;
        }

        public async Task<LoadedDocumentSectionViewModel> GetDocumentSectionConsumerProtection(int staffId, int operationId, int targetId, int sectionId)
        {
            var staff = await context.TBL_STAFF.FindAsync(staffId);
            List<int> sectionIds = new List<int>();

            if (staff != null)
            {
                sectionIds = await context.TBL_DOC_TEMPLATE_SECTION_ROLE
                    .Where(x => x.DELETED == false && x.STAFFROLEID == staff.STAFFROLEID)
                    .Select(x => x.TEMPLATESECTIONID)
                    .ToListAsync();
            }


            var doc = context.TBL_DOC_TEMPLATE_DETAIL.FirstOrDefault(x => x.OPERATIONID == operationId && x.DOCUMENTDETAILID == sectionId);
            var section = context.TBL_DOC_TEMPLATE_SECTION.FirstOrDefault(s => s.TEMPLATESECTIONID == doc.TEMPLATESECTIONID);
            if (doc == null) return new LoadedDocumentSectionViewModel();

            InitForConsumerProtection(targetId);

            return new LoadedDocumentSectionViewModel
            {
                sectionId = doc.DOCUMENTDETAILID,
                title = doc.TITLE,
                description = doc.DESCRIPTION,
                templateDocument = Replace(doc.TEMPLATEDOCUMENT),
                canEdit = section.CANEDIT,
                editable = section.CANEDIT && sectionIds.Contains(doc.TEMPLATESECTIONID),
            };
        }

        //public string GetConsumerProtectionMemoById(int companyId, int consumerProtectionById)
        //{
        //    var result = context.TBL_CONSUMER_PROTECTION.Where(O => O.CONSUMERPROTECTIONID == consumerProtectionById).Select(O => new ConsumerProtectionViewModel()
        //    {
        //        actualAmountBorrowed = O.ACTUALAMOUNTBORROWED,
        //        annualInterestRate = O.ANNUALINTERESTRATE,
        //        consumerProtectionId = O.CONSUMERPROTECTIONID,
        //        insurance = O.INSURANCE,
        //        loanAmount = O.LOANAMOUNT,
        //        loanAPR = O.LOANAPR,
        //        monthlyPayment = O.MONTHLYPAYMENT,
        //        termOfLoanInYears = O.TERMOFLOANSINYEARS,
        //        totalFees = O.TOTALFEES,
        //        totalFeesAndCharges = O.TOTALFEESANDCHARGES,

        //        branchId = O.BRANCHID,
        //        companyId = O.COMPANYID,

        //        createdBy = O.CREATEDBY,
        //        dateTimeCreated = O.DATETIMECREATED,
        //    }).FirstOrDefault();

        //    var test = LoadConsumerProtectionMemo(result);
        //    return test;
        //}

        private string LoadConsumerProtectionMemo(ConsumerProtectionViewModel model)
        {

            return "";
        }

        public bool ReleaseLien(CasaLienViewModel model, TwoFactorAutheticationViewModel twoFADetails = null, bool require2FA = true)
        {
            var existingLien = context.TBL_CASA_LIEN.Where(x => x.LIENREFERENCENUMBER == model.lienReferenceNumber).FirstOrDefault();

            if (existingLien == null)
                throw new SecureException($"{TranslateHelper.get("Cannot release lien because lien with reference number")} {model.lienReferenceNumber} {TranslateHelper.get("does not exist")}");


            var lienSum = context.TBL_CASA_LIEN.Where(x => x.LIENREFERENCENUMBER == model.lienReferenceNumber).Sum(y => y.LIENAMOUNT);

            if (lienSum <= 0)
                throw new SecureException($"{TranslateHelper.get("Cannot release lien because lien with reference number")} {model.lienReferenceNumber} {TranslateHelper.get("has already been released")}");

            model.lienAmount = existingLien.LIENAMOUNT;
            model.sourceReferenceNumber = existingLien.SOURCEREFERENCENUMBER;
            model.productAccountNumber = existingLien.PRODUCTACCOUNTNUMBER;
            //model.description = data.DESCRIPTION;

            //call
            if (USE_TWO_FACTOR_AUTHENTICATION  && require2FA == true)
            {
                if (twoFADetails == null)
                    throw new TwoFactorAuthenticationException(TranslateHelper.get("Authentication token not specified. Specify the second factor authentication token"));

                var authenticated = twoFactorAuth.Authenticate(twoFADetails.username, twoFADetails.passcode);

                if (authenticated.authenticated == false)
                    throw new TwoFactorAuthenticationException(authenticated.message);
            }

            ReleaseLienSub(model, existingLien);
            // this block of code is commented because release lien is not applicable in access bank 
            /*if (USE_THIRD_PARTY_INTEGRATION)
            {

                ResponseMessage result = null;

                Task.Run(async () => { result = await tran.APIProcessLien(model, "LIFTLIEN"); }).GetAwaiter().GetResult();

                if (result.APIResponse != null)
                    if (result.APIResponse.responseCode == "0")
                    {
                        ReleaseLienSub(model, existingLien);
                    }
                    else
                    {
                        throw new ConditionNotMetException(result.APIResponse.webRequestStatus);
                    }

            }
            else
            {
                ReleaseLienSub(model, existingLien);
            }*/

            return true;
        }

        private void ReleaseLienSub(CasaLienViewModel model, TBL_CASA_LIEN existingLien)
        {
            var data = new TBL_CASA_LIEN
            {
                PRODUCTACCOUNTNUMBER = existingLien.PRODUCTACCOUNTNUMBER,
                LIENREFERENCENUMBER = existingLien.LIENREFERENCENUMBER,
                SOURCEREFERENCENUMBER = existingLien.SOURCEREFERENCENUMBER,
                BRANCHID = model.branchId,
                COMPANYID = model.companyId,
                LIENAMOUNT = Math.Abs(existingLien.LIENAMOUNT) * -1,
                DESCRIPTION = "Lien Release -- " + model.description,
                LIENTYPEID = existingLien.LIENTYPEID,
                CREATEDBY = model.createdBy,
                DATETIMECREATED = DateTime.Now

            };

            existingLien.ISLIENREMOVED = true;
            context.TBL_CASA_LIEN.Add(data);

            // Audit Section ---------------------------            

            //var audit = new TBL_AUDIT
            //{
            //    AUDITTYPEID = (short)AuditTypeEnum.LienReleased,
            //    STAFFID = model.createdBy,
            //    BRANCHID = model.branchId,
            //    DETAIL = $"Released lien with reference number: {existingLien.LIENREFERENCENUMBER}",
            //    IPADDRESS = model.userIPAddress,
            //    URL = model.applicationUrl,
            //    APPLICATIONDATE = generalSetup.GetApplicationDate(),
            //    SYSTEMDATETIME = DateTime.Now
            //};
            //this.auditTrail.AddAuditTrail(audit);

            //end of Audit section -------------------------------

            context.SaveChanges() ;
            
        }

        private string GetBusinessUsersEmailsToGroupHead(string accountOfficerMIsCode)
        {
            string emailList = "";

            var accountOfficer = context.TBL_STAFF.Where(x => x.MISCODE.ToLower() == accountOfficerMIsCode.ToLower()).FirstOrDefault();
            if (accountOfficer != null)
            {
                emailList = accountOfficer.EMAIL;
                if (accountOfficer.SUPERVISOR_STAFFID != null)
                {
                    var relationshipManager = context.TBL_STAFF.Where(x => x.STAFFID == accountOfficer.SUPERVISOR_STAFFID).FirstOrDefault();
                    if (relationshipManager != null)
                    {
                        emailList = emailList + ";" + relationshipManager.EMAIL;
                        if (relationshipManager.SUPERVISOR_STAFFID != null)
                        {
                            var zonalHead = context.TBL_STAFF.Where(x => x.STAFFID == relationshipManager.SUPERVISOR_STAFFID).FirstOrDefault();
                            if (zonalHead != null)
                            {
                                emailList = emailList + ";" + zonalHead.EMAIL;

                                var groupHead = context.TBL_STAFF.Where(x => x.STAFFID == zonalHead.SUPERVISOR_STAFFID).FirstOrDefault();

                                if (groupHead != null)
                                {
                                    emailList = emailList + ";" + groupHead.EMAIL;
                                }
                            }
                        }
                    }
                }

            }

            return emailList;
        }
        public void LogEmailAlert(string messageBody, string alertSubject, List<string> recipients, string referenceCode, int targetId, string operationMehtod)
        {
            try
            {
                var title = alertSubject.Trim();
                if (title.Contains("&"))
                {
                    title = title.Replace("&", "AND");
                }
                if (title.Contains("."))
                {
                    title = title.Replace(".", "");
                }

                string recipient = string.Join("", recipients.ToArray());
                string messageSubject = title;
                string messageContent = messageBody;
                MessageLogViewModel messageModel = new MessageLogViewModel
                {
                    MessageSubject = messageSubject,
                    MessageBody = messageContent,
                    MessageStatusId = 1,
                    MessageTypeId = 1,
                    FromAddress = ConfigurationManager.AppSettings["SupportEmailAddr"],
                    ToAddress = $"{recipient}",
                    DateTimeReceived = DateTime.Now,
                    SendOnDateTime = DateTime.Now,
                    ReferenceCode = referenceCode,
                    targetId = targetId,
                    operationMethod = operationMehtod,
                };
                SaveMessageDetails(messageModel);
            }
            catch (Exception ex)
            {
                new SecureException(ex.ToString());
            }
        }

        private void SaveMessageDetails(MessageLogViewModel model)
        {
            var message = new TBL_MESSAGE_LOG()
            {
                //MessageId = model.MessageId,
                MESSAGESUBJECT = model.MessageSubject,
                MESSAGEBODY = model.MessageBody,
                MESSAGESTATUSID = model.MessageStatusId,
                MESSAGETYPEID = model.MessageTypeId,
                FROMADDRESS = model.FromAddress,
                TOADDRESS = model.ToAddress,
                DATETIMERECEIVED = model.DateTimeReceived,
                SENDONDATETIME = model.SendOnDateTime,
                ATTACHMENTCODE = model.ReferenceCode,
                ATTACHMENTTYPEID = (short)AttachementTypeEnum.JobRequest,
                TARGETID = (int)model.targetId,
                OPERATIONMETHOD = model.operationMethod
            };

            context.TBL_MESSAGE_LOG.Add(message);
            context.SaveChanges();

        }



        private bool InitForConsumerProtection(int consumerProtectionById)
        {
            var result = GetConsumerProtectionMemoById(consumerProtectionById);

            // CONSUMER PROTECTION
            this.consumerProtectionLoanDetailsData = ConsumerProtectionLoanDetailsHtml(result);
            this.consumerProtectionLoanSpecificInformationData = ConsumerProtectionLoanSpecificInformationHtml(result);
            this.consumerProtectionRepaymentsData = ConsumerProtectionRepaymentsHtml(result);
            this.consumerProtectionFeesAndChargesData = ConsumerProtectionFeesAndChargesHtml(result);

            return true;
        }

        public ConsumerProtectionViewModel GetConsumerProtectionMemoById(int consumerProtectionById)
        {
            var result = context.TBL_CONSUMER_PROTECTION.Where(O => O.CONSUMERPROTECTIONID == consumerProtectionById).Select(O => new ConsumerProtectionViewModel()
            {
                actualAmountBorrowed = O.ACTUALAMOUNTBORROWED,
                annualInterestRate = O.ANNUALINTERESTRATE,
                consumerProtectionId = O.CONSUMERPROTECTIONID,
                insurance = O.INSURANCE,
                loanAmount = O.LOANAMOUNT,
                loanAPR = O.LOANAPR,
                monthlyPayment = O.MONTHLYPAYMENT,
                termOfLoanInYears = O.TERMOFLOANSINYEARS,
                totalFees = O.TOTALFEES,
                totalFeesAndCharges = O.TOTALFEESANDCHARGES,

                branchId = O.BRANCHID,
                companyId = O.COMPANYID,

                createdBy = O.CREATEDBY,
                dateTimeCreated = O.DATETIMECREATED,
            }).FirstOrDefault();

            return result;
        }

        public string ConsumerProtectionLoanDetailsHtml(ConsumerProtectionViewModel model)
        {
            //var consumer = GetConsumerProtectionMemoById();
            var result = String.Empty;
            var n = 0;

            result = result + $@"        
                <table style='font face: arial; size:12px' border=1 width=900 cellpadding=10 cellspacing=0>
                    <tr>
                        <td>
                            <table style='font face: arial; size:12px' border=1 width=410 cellpadding=10 cellspacing=0>
                                <tr>
                                    <th colspan='2'><b>THE LOAN</b></th>
                                </tr>
                                <tr>
                                    <td>{TranslateHelper.get("Loan amount:")}</td>
                                    <td>N { model.loanAmount }</td>
                                </tr>
                                <tr>
                                    <td>{TranslateHelper.get("Tenor:")}</td>
                                    <td>{ model.termOfLoanInYears } {TranslateHelper.get("months / years")} <b>{TranslateHelper.get("(delete whichever is not applicable)")}</b></td>
                                </tr>
                                <tr>
                                    <td>{TranslateHelper.get("Interest rate:")}</td>
                                    <td>{ model.annualInterestRate } % {TranslateHelper.get("Variable / Fixed")} <b>{TranslateHelper.get("(delete whichever is not applicable)")}</b></td>
                                </tr>
                                <tr>
                                    <td>{TranslateHelper.get("Collateral:")}</td>
                                    <td>{TranslateHelper.get("Yes / No")} <b>{TranslateHelper.get("(delete whichever is not applicable)")}</b></td>
                                </tr>
                   
                            </table>
                        </td>

                        <td>
                            <table style='font face: arial; size:12px' border=1 width=410 cellpadding=10 cellspacing=0>
                                <tr>
                                    <th><b>{TranslateHelper.get("TOTAL COST TO CONSUMER")}</b></th>
                                </tr>
                                <tr>
                                    <td>{TranslateHelper.get("Total amount you will")} N { model.actualAmountBorrowed } {TranslateHelper.get("pay back")}  </td>
                                </tr>
                                <tr>
                                    <td>{TranslateHelper.get("This means you will")} N { model.loanAmount } {TranslateHelper.get("for every")} N { model.actualAmountBorrowed } {TranslateHelper.get("pay back borrowed")}</td>
                                </tr>
                                <tr>
                                    <td>{TranslateHelper.get("Annual Percentage Rate (APR)")} {model.loanAPR } % {TranslateHelper.get("This reflects the total cost of the credit on a yearly basis expressed as percentage, using the information at the disclosure date. It is a useful tool for comparison with similar loans")}</td>
                                </tr>
                   
                            </table>
                        </td>

                    </tr>";
            result = result + $"</table>";
            return result;
        }

        public string ConsumerProtectionLoanSpecificInformationHtml(ConsumerProtectionViewModel model)
        {
            var result = String.Empty;
            var n = 0;

            result = result + $@"        
                <table style='font face: arial; size:12px' border=1 width=900 cellpadding=10 cellspacing=0>
                    <tr>
                        <th colspan='2'>
                            <b>{TranslateHelper.get("Specific information about your loan")}</b>
                        </th>
                    </tr>

                    <tr>
                        <td>
                            {TranslateHelper.get("Loan received")}
                        </td>

                        <td>
                            N { model.loanAmount }
                        </td>
                    </tr>

                    <tr>
                        <td>
                            {TranslateHelper.get("Interest rate")} <br/>
                            < b>{TranslateHelper.get("(Variable interest rates may change)")}</b>
                        </td>

                        <td>
                            { model.annualInterestRate } %
                        </td>
                    </tr>

                    <tr>
                        <td>
                            {TranslateHelper.get("Total interest charges (Total interest you will pay)")} <br/>
                            < b>{TranslateHelper.get("(Total interest may increase for variable interest rates)")}</b>
                        </td>

                        <td>
                            N { model.totalFees }
                        </td>
                    </td>
                    <tr>
                        <td>
                            {TranslateHelper.get("Total fees and charges*")} <br/>
                            < b>{TranslateHelper.get("(Total other charges you will pay throughout the duration of the loan)")}</b>
                        </td>

                        <td>
                            N { model.totalFeesAndCharges }
                        </td>
                    </tr>
                    <tr>
                        <td>
                            {TranslateHelper.get("Total cost of credit")} <br/>
                            < b>{TranslateHelper.get("(This is made up of total interest and all other charges  for the tenor of the loan")}</b>

                        </td>

                        <td>
                            N { model.totalFeesAndCharges + ((decimal)model.annualInterestRate * model.actualAmountBorrowed) } 
                        </td>
                    </tr>";
            result = result + $"</table>";
            return result;
        }

        public string ConsumerProtectionRepaymentsHtml(ConsumerProtectionViewModel model)
        {
            var result = String.Empty;
            var n = 0;

            result = result + $@"        
                <table style='font face: arial; size:12px' border=1 width=900 cellpadding=10 cellspacing=0>
                    <tr>
                        <th colspan='2'>
                            {TranslateHelper.get("Repayments")}
                        </th>
                    </tr>

                    <tr>
                        <td>
                            {TranslateHelper.get("Repayment amount (see attached repayment schedule)")}<br/>
                            < b>{TranslateHelper.get("Amount you will need to repay on due date")}</b>
                        </td>

                        <td>
                            N { model.loanAmount } {TranslateHelper.get("month / quarter for tenor of loan")}
                        </td>
                    </tr>

                    <tr>
                        <td>
                            {TranslateHelper.get("Date of first repayment")}
                        </td>

                        <td>
                            { model.dateTimeCreated }
                        </td>
                    </tr>

                    <tr>
                        <td>
                            {TranslateHelper.get("Date on which other repayments are due")}
                        </td>

                        <td>
                            { model.termOfLoanInYears } {TranslateHelper.get("in each week / month for tenor of loan after the first repayment period")} 
                        </td>
                    </tr>

                    <tr>
                        <td>
                            {TranslateHelper.get("Total number of repayments")}
                        </td>

                        <td>
                            { model.termOfLoanInYears }
                        </td>
                    </td>
                    <tr>
                        <td colspan='2'>
                            <b>{TranslateHelper.get("*Note that the amount required to be paid (for each repayment and total) does not include fees which are dependent on events that may not occur (for example, late payment fees)")}</b>
                        </td>

                    </tr>";
            result = result + $"</table>";
            return result;
        }

        public string ConsumerProtectionFeesAndChargesHtml(ConsumerProtectionViewModel model)
        {
            var result = String.Empty;
            var n = 0;

            result = result + $@"        
                <table style='font face: arial; size:12px' border=1 width=900 cellpadding=10 cellspacing=0>
                    <tr>
                        <td>
                            <table style='font face: arial; size:12px' border=1 width=410 cellpadding=10 cellspacing=0>
                                <tr>
                                    <th colspan='2'><b>{TranslateHelper.get("(A) credit prover’s fees")}</b></th>
                                </tr>
                               
                                <tr>
                                    <td colspan='2'>{TranslateHelper.get("(List all applicable lending fees)")}</td>
                                </tr>
                                <tr>
                                    <td>(1)</td>
                                    <td>N { model.totalFees }</td>
                                </tr>
                                <tr>
                                    <td>(2)</td>
                                    <td>N { model.totalFees }</td>
                                </tr>

                                <tr>
                                    <td><b>{TranslateHelper.get("Total (A)")}</b></td>
                                    <td>N { model.totalFees + model.totalFees }</td>
                                </tr>

                                <tr>
                                    <td>{TranslateHelper.get("Total Fees and charges")} (A + B)</td>
                                    <td>N { model.totalFees + model.totalFees }</td>
                                </tr>
                   
                            </table>
                        </td>

                        <td>
                            <table style='font face: arial; size:12px' border=1 width=410 cellpadding=10 cellspacing=0>
                                <tr>
                                    <th colspan='2'><b>{TranslateHelper.get("(B) Third party fees/charges")}</b></th>
                                </tr>
                                <tr>
                                    <td colspan='2'>{TranslateHelper.get("(List all applicable 3rd party fees)")}</td>
                                </tr>
                                <tr>
                                    <td>(1)</td>
                                    <td>N { model.totalFees }</td>
                                </tr>
                                <tr>
                                    <td>(2)</td>
                                    <td>N { model.totalFees }</td>
                                </tr>
                   
                                <tr>
                                    <td><b>{TranslateHelper.get("Total (B)")}</b></td>
                                    <td>N { model.totalFees + model.totalFees}</td>
                                </tr>
                            </table>
                        </td>

                    </tr>";
            result = result + $"</table>";
            return result;
        }

        private string Replace(string content) // placeholders replace
        {
            // CONSUMER PROTECTION
            content = content.Replace(consumerProtectionLoanDetailsHolder, consumerProtectionLoanDetailsData);
            content = content.Replace(consumerProtectionLoanSpecificInformationHolder, consumerProtectionLoanSpecificInformationData);
            content = content.Replace(consumerProtectionRepaymentsHolder, consumerProtectionRepaymentsData);
            content = content.Replace(consumerProtectionFeesAndChargesHolder, consumerProtectionFeesAndChargesData);

            return content;
        }

    }
}
