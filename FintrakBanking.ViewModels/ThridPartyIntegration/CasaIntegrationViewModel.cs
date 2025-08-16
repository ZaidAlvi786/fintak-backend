using FintrakBanking.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.ViewModels.ThridPartyIntegration
{
    public class CasaIntegrationViewModel : GeneralEntity
    {
        public string accountNumber  { get; set; }
        public string accountName { get; set; }
        public string product { get; set; }
        public string productType { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public string accountDetail { get { return (this.accountNumber + "(" + this.accountName + ")"); } }
        public string currencyType { get; set; }
        public decimal balance { get; set; }
        public string branch { get; set; }
        public string accountStatus  { get; set; }
        public string lastTransactionDate { get; set; }
        public string webRequestStatus { get; set; }
        public DateTime webRequestDate { get; set; }
        public string responseCode { get; set; }
        public string error { get; set; }
        public string field { get; set; }
        public string errorDescription { get; set; }
        public int currencyId { get; set; }
        public string customerCode { get; set; }

        public string freezeStatus { get; set; }
        public string freezeReason { get; set; }
        public string channel_code { get; set; }
        public string loan_accountno { get; set; }
        public string response_code { get; set; }
        public string response_message { get; set; }
        public List<GetCustomerAcctsDetailsResp> getcustomeracctsdetailsresp { get; set; }
        public List<LoanSummary>loanSummary { get; set; }
        public string account_no { get; set; }
    }

    public class LoanSummary
    {
      public string loan_accountno { get; set; }
      public string customer_no { get; set; }
        public string branch_code { get; set; }
        public string product_code { get; set; }
        public string product_category { get; set; }
        public string product_desc { get; set; }
        public string customer_name { get; set; }
        public string domicilairy_address { get; set; }
        public string customer_address { get; set; }
        public DateTime? original_startdate { get; set; }
        public DateTime? book_date { get; set; }
        public DateTime? value_date { get; set; }
        public DateTime? maturity_date { get; set; }
        public int? loan_tenor { get; set; }
        public string loan_status { get; set; }
        public string currency_code { get; set; }
        public decimal? amount_financed { get; set; }
        public string applicant_name { get; set; }
        public decimal? total_principaloutstanding { get; set; }
        public decimal? total_interestoutstanding { get; set; }
        public decimal? principal_overdue { get; set; }
        public decimal? principal_notdue { get; set; }
        public decimal? interest_notdue { get; set; }
        public decimal? interest_overdue { get; set; }
        public decimal? next_principaldue { get; set; }
        public decimal? next_interestdue { get; set; }
        public decimal? principal_paid { get; set; }
        public decimal? interest_paid { get; set; }
        public decimal? interest_penaltypaid { get; set; }
        public string total_unpaid_days { get; set; }
    }

    public class CustomerTransactionViewModels 
    {
        public int customerId { get; set; }
        public string customerCode { get; set; }
        public string branchCode { get; set; }
        public string contactAddress { get; set; }
        public string lastContactAddress  { get; set; }
        //public string lastName { get; set; }
        public string title { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string customerTypeName { get; set; }
        public string customerName { get { return this.firstName + " " + this.middleName + " " + this.lastName; } }
        public string fullName { get; set; }
        public string searchItem { get { return this.firstName + " " + this.middleName + " " + this.lastName + " " + this.customerCode; } }
        public string lastName { get; set; }
        public string gender { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string placeOfBirth { get; set; }
        public string nationality { get; set; }
        public string maritalStatus { get; set; }
        public string emailAddress { get; set; }
        public string maidenName { get; set; }
        public string spouse { get; set; }
        public string firstChildName { get; set; }
        public DateTime childDateOfBirth { get; set; }
        public string occupation { get; set; }
        public string customerType { get; set; }
        public string relationshipOfficerCode { get; set; }
        public string relationshipOfficerName { get; set; }
        public string politicallyExposedPerson  { get; set; }
        public string misCode { get; set; }
        public string staffCode { get; set; }
        public string fsCaptionGroupCode { get; set; }
        public DateTime dateofIncorporation { get; set; }
        public string actedOnBy { get; set; }
        public bool accountCreationComplete { get; set; }
        public bool creationMailSent { get; set; }
        public string customerSensitivityLevel { get; set; }
        public string taxIdNumber  { get; set; }
        public string electricMeterNumber { get; set; }
        public string businessTaxIdNumber { get; set; }
        public string bankVerificationNumber { get; set; }
        public string taxNumber { get; set; }
        public string officeAddress { get; set; }
        public string nearestLandmark { get; set; }
        public string paidUpCapital { get; set; }
        public string authorizedCapital { get; set; }
        public string employerDetails { get; set; }


        public string rcNumber { get; set; }
        //public DateTime dateOfBirth { get; set; }
        public string subSectorCode { get; set; }
        public string sectorCode { get; set; }
        //public string maritalStatus { get; set; }
        //public string emailAddress { get; set; }
        //public string maidenName { get; set; }
        //public string spouse { get; set; }
        //public string firstChildName { get; set; }
        //public DateTime childDateOfBirth { get; set; }
        //public string occupation { get; set; }
        //public string customerType { get; set; }
        //public string relationshipOfficerCode { get; set; }
        //public string relationshipOfficerName { get; set; }
        //public bool politicallyExposedPerson { get; set; }
    }

    public class CurrencyExchangeRateIntegrationViewModel 
    {

        public short currencyId { get; set; }

        public string currencyCode { get; set; }

        public string fromCurrencyCode { get; set; }

        public string toCurrencyCode { get; set; }

        public string rateCode  { get; set; }

        public DateTime webRequestDate  { get; set; }

        public string webRequestStatus { get; set; }

        public double buyingRate { get; set; }

        public double sellingRate { get; set; }

        public double exchangeRate  { get; set; }

        public short baseCurrencyId { get; set; }

        public bool isBaseCurrency { get; set; }

    }

    public class InterestRateInquiryIntegrationViewModel : GeneralEntity
    {
        public string webRequestDate { get; set; }
        public string webRequestStatus { get; set; }
        public string message { get; set; }
       
        public InterestRateDetails interestRateDetails { get; set; }
    }


    public class InterestRateDetails 
    {
        public string accountNumber { get; set; }
        public string accountType { get; set; }
        public string interestSerialNumber { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string lastChangedDate { get; set; }
        public string interestRateAmount { get; set; }
        public string interestTableCode { get; set; }
    }

    public class CustomerTurnoverGroupViewModel
    {
        public List<CustomerTurnoverViewModelAPI> Account { get; set; }
        public string channel_code { get; set; }
        public string account_no { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string response_code { get; set; }
        public string response_message { get; set; }
        public List<getaccountsummaryandtrnxresp> getaccountsummaryandtrnxresp { get; set; }
    }
    public class CustomerTurnoverViewModelAPI // TEMPORARY LOCATION
    {
        public string foracid { get; set; } // ": "2022072744",
        public string cust_Id { get; set; } // ": "483008974",
        public string schm_Type { get; set; } // ": "ODA|OVERDRAFT A/C",
        public string period { get; set; } // ": "Apr-15",
        public decimal? min_Debit_Balance { get; set; } // ": "",
        public decimal? max_Debit_Balance { get; set; } // ": "",
        public decimal? min_Credit_Balance { get; set; } // ": "34218.39",
        public decimal? max_Credit_Balance { get; set; } // ": "1050843.73",
        public decimal? debit_Turnover { get; set; } // ": "1159360.11",
        public decimal? credit_Turnover { get; set; } // ": "1207425.56",
        public string sms_Alert { get; set; } // ": "-176",
        public string amc { get; set; } // ": "",
        public string vat { get; set; } // ": "-92.50",
        public string management_Fee { get; set; } // ": "",
        public string commitment_Fees { get; set; } // ": "",
        public string com_Contigent_Liab { get; set; } // ": "",
        public string lc_Commission { get; set; } // ": 
        public string float_Charge { get; set; } // "2081981.94",
        public string interest { get; set; } // "2909416.54",
        public int? month { get; set; } // "0",
        public int? year { get; set; } // "0",
        public string response_message { get; set; }
        public string response_code { get; set; }
        public List<getaccountsummaryandtrnxresp> getaccountsummaryandtrnxresp { get; set; }
        public List<getacctsummarybycustomernosresp> getacctsummarybycustomernosresp { get; set; }

        //public string foracid { get; set; }
        //public string cusT_ID { get; set; }
        //public string schM_TYPE { get; set; }
        //public string period { get; set; }
        //public decimal? miN_DEBIT_BALANCE { get; set; }
        //public decimal? maX_DEBIT_BALANCE { get; set; }
        //public decimal? miN_CREDIT_BALANCE { get; set; }
        //public decimal? maX_CREDIT_BALANCE { get; set; }
        //public decimal? debiT_TURNOVER { get; set; }
        //public decimal? crediT_TURNOVER { get; set; }
        //public string smS_ALERT { get; set; }
        //public string amc { get; set; }
        //public string vat { get; set; }
        //public string managemenT_FEE { get; set; }
        //public string commitmenT_FEES { get; set; }
        //public string coM_CONTINGENT_LIAB { get; set; }
        //public string lC_COMMISION { get; set; }
    }

    public class CustomerTurnoverViewModel // TEMPORARY LOCATION
    {
        public string accountNumber { get; set; } // ": "2022072744",
        public string customerCode { get; set; } // ": "483008974",
        public string productName { get; set; } // ": "ODA|OVERDRAFT A/C",
        public string period { get; set; } // ": "Apr-15",
        public decimal? min_Debit_Balance { get; set; } // ": "",
        public decimal? max_Debit_Balance { get; set; } // ": "",
        public decimal? min_Credit_Balance { get; set; } // ": "34218.39",
        public decimal? max_Credit_Balance { get; set; } // ": "1050843.73",
        public decimal? debit_Turnover { get; set; } // ": "1159360.11",
        public decimal? credit_Turnover { get; set; } // ": "1207425.56",
        public decimal? sms_Alert { get; set; } // ": "-176",
        public decimal? amc { get; set; } // ": "",
        public decimal? vat { get; set; } // ": "-92.50",
        public decimal? management_Fee { get; set; } // ": "",
        public decimal? commitment_Fees { get; set; } // ": "",
        public decimal? com_Contigent_Liab { get; set; } // ": "",
        public decimal? lc_Commission { get; set; } // ": 
        public decimal? float_Charge { get; set; } // "2081981.94",
        public decimal? interest { get; set; } // "2909416.54",
        public int? month { get; set; } // "0",
        public int? year { get; set; } // "0",

        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string account_no { get; set; }
        public string channel_code { get; set; }
        public string response_message { get; set; }
        public string response_code { get; set; }
        public List<getaccountsummaryandtrnxresp> getaccountsummaryandtrnxresp { get; set; }
        public List<getacctsummarybycustomernosresp> getacctsummarybycustomernosresp { get; set; }

        public string customer_no { get; set; }
    }

    public class getaccountsummaryandtrnxresp
    {
      public int? ac_entry_sr_no { get; set; }
      public  string account_name { get; set; }
        public  string account_no { get; set; }
        public  double? acctopening_balance { get; set; }
        public  string auth_id { get; set; }
        public  decimal? available_balance { get; set; }
        public  int? batch_no { get; set; }
        public  string branch_address { get; set; }
        public  string branch_code { get; set; }
        public  string branch_name { get; set; }
        public  string ccy_code { get; set; }
        public  decimal? cleared_balance { get; set; }
        public  decimal? closing_balance { get; set; }
        public  string customer_no { get; set; }
        public  string drcr_indicator { get; set; }
        public  string flex_trn_refno { get; set; }
        public  string external_refno { get; set; }
        public  decimal? fcy_amount { get; set; }
        public  string has_cot { get; set; }
        public  string instrument_code { get; set; }
        public  decimal? lcy_amount { get; set; }
        public  decimal? noof_lodgement { get; set; }
        public  decimal? noof_withdrawal { get; set; }
        public  DateTime? posteddate { get; set; }
        public  string product_code { get; set; }
        public  string productcode_desc { get; set; }
        public  string row_number { get; set; }
        public  decimal? running_balance { get; set; }
        public  DateTime? stmt_date { get; set; }
        public  decimal? total_lodgement { get; set; }
        public  decimal? total_withdrawal { get; set; }
        public  decimal? tran_amount { get; set; }
        public  string tran_description { get; set; }
        public  string tran_event { get; set; }
        public  string tran_indicator { get; set; }
        public  string tran_narratio { get; set; }
        public  string tran_code { get; set; }
        public  DateTime? tran_date { get; set; }
        public  string tran_ref_no { get; set; }
        public  DateTime? tran_init_date { get; set; }
        public  decimal? uncleared_balance { get; set; }
        public  string user_id { get; set; }
        public  DateTime? value_date { get; set; }
        public string period { get; set; }
        public string productName { get; set; }
        public decimal? float_Chaerge { get; set; }
        public decimal? interest { get; set; }
    }

    //public class CustomerTurnoverInterestViewModels
    //{
    //    public string as_Of_Date { get; set; } // "1/31/2018 12:00:00 AM",
    //    public string account_Number { get; set; } // "2013995959",
    //    public string acct_Type { get; set; } // "Loan",
    //    public string float_Charge { get; set; } // "2081981.94",
    //    public string interest { get; set; } // "2909416.54",
    //    public string account_Name { get; set; } // "FORTE OIL PLC",
    //    public string cif_Id { get; set; } // "230009868"
    //}

    //public class InputVM
    //{
    //    public int cifid { get; set; }
    //    public DateTime fromdate { get; set; }
    //    public DateTime todate { get; set; }
    //}
}
