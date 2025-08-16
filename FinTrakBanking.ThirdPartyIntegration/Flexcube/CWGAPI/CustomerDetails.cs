namespace FinTrakBanking.ThirdPartyIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using FintrakBanking.ViewModels.Customer;
    using FintrakBanking.ViewModels.ThridPartyIntegration;
    using FintrakBanking.ViewModels.CASA;
    using FintrakBanking.Entities.Models;
    using System.Linq;
    using FintrakBanking.ViewModels.Finance;
    using FintrakBanking.Common.Enum;
    using FintrakBanking.Common.CustomException;
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using System.Web.Script.Serialization;

    namespace CustomerInfo
    {
        public class CustomerDetails
        {
            private FinTrakBankingContext context;
            string API_KEY, API_URL = string.Empty;
            private List<TBL_API_URL> APIUrlConfig;


            public CustomerDetails(FinTrakBankingContext _context)
            {
                this.context = _context;
                APIUrlConfig = new List<TBL_API_URL>();

            }

            private void getAPIURLSettings(string typeName = null)
            {
                APIUrlConfig = context.TBL_API_URL.ToList();
                var apiConfig = APIUrlConfig.Where(x => x.TYPENAME.ToLower() == typeName.ToLower()).FirstOrDefault();
                if (apiConfig != null && !String.IsNullOrEmpty(apiConfig.URL))
                {
                    API_URL = apiConfig.URL.Trim();
                    API_KEY = apiConfig.APIKEY;
                }
                if (apiConfig == null || String.IsNullOrEmpty(apiConfig.URL))
                {
                    apiConfig = APIUrlConfig.Where(x => x.TYPENAME.ToUpper() == "DEFAULT").FirstOrDefault();

                    if (apiConfig != null) {
                        API_URL = apiConfig.URL.Trim();
                        API_KEY = apiConfig.APIKEY;
                    }
                }
            }

            public async Task<List<CustomerViewModels>> GetCustomerByAccountsNumber(string customerAccount)
            {
                IRestResponse response = null;
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                string responseMessage = "";
                RestRequest req = new RestRequest(Method.POST);
                CustomerViewModels records = new CustomerViewModels();
                List<CustomerViewModels> customers = new List<CustomerViewModels>();
                CustomerViewModels reqbody = null;
                try
                {
                    getAPIURLSettings("Default");

                    var baseURL = API_URL;
                    string fullURL = baseURL + "GetCustomerAcctsDetail";
                    RestClient client = new RestClient(fullURL);

                    reqbody = new CustomerViewModels()
                    {
                        channel_code = "FINTRAK",
                        account_no = customerAccount
                    };

                    requestDatetime = DateTime.Now;
                    

                    var jsonbody = new JavaScriptSerializer().Serialize(reqbody);
                    req.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
                    req.AddHeader("Content-Type", "application/json");
                    req.AddHeader("Accept", "application/json");
                    req.AddHeader("Authorization", API_KEY);

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    response = await client.ExecuteAsync<CustomerViewModels>(req);
                    //resp
                    var responbody = JsonConvert.DeserializeObject<CustomerViewModels>(response.Content);
                    responseDateTime = DateTime.Now;

                    if (response.IsSuccessful)
                    {

                        if (responbody == null || !responbody.response_message.ToLower().Contains("successful"))
                        {
                            throw new APIErrorException("API call error - " + responbody.response_message + " " + responbody.response_code + " " + DateTime.Now);
                        }
                        var rep = responbody;
                        records.response_code = rep.response_code;
                        records.response_message = rep.response_message;
                        records.getcustomeracctsdetailsresp = rep.getcustomeracctsdetailsresp;
                        
                        foreach (var customerModel in records.getcustomeracctsdetailsresp)
                        {
                            if (customerModel.customerType == "C") { customerModel.customerTypeId = 2; }
                            else { customerModel.customerTypeId = 1; }

                            if (customerModel.gender == "M") { customerModel.gender = "Male"; }
                            if (customerModel.gender == "F") { customerModel.gender = "Female"; }

                            if (customerModel.customerTypeId == (short)CustomerTypeEnum.Corporate || customerModel.customerType == "C")
                            {
                                customerModel.firstName = customerModel.companyName == null ? customerModel.company_name : customerModel.companyName;
                                customerModel.companyName = customerModel.company_name;
                            }

                            //string customerName = customerModel.customerName;
                            //string firstName = string.Empty;
                            //string middleName = string.Empty;
                            //string lastName = string.Empty;

                            //string[] splittedCustomerName = customerName.Split(' ');


                            //for (int a = 1; a < 3; a = a + 1)
                            //{
                            //    if (a == 1) firstName = splittedCustomerName[0];
                            //    if (a == 2) lastName = splittedCustomerName[1];
                            //    if (a == 3) middleName = splittedCustomerName[2];

                            //}
                            //if (splittedCustomerName[1] == "")
                            //{
                            //    lastName = splittedCustomerName[2];
                            //    middleName = null;
                            //}

                            string customerName = customerModel.customerName;
                            string firstName = string.Empty;
                            string middleName = string.Empty;
                            string lastName = string.Empty;

                            char[] delimiters = new char[] { ' ', '\r', '\n' };
                            int wordCount = customerName.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
                     
                            if (wordCount > 2)
                            {
                                string[] splittedCustomerName = customerName.Split(' ');

                                for (int a = 1; a < 4; a = a + 1)
                                {
                                    if (a == 1) firstName = splittedCustomerName[0];
                                    if (a == 2) middleName = splittedCustomerName[1];
                                    if (a == 3) lastName = splittedCustomerName[2];
                                }
                                if (middleName == "") middleName = customerModel.middleName;
                                if (lastName == "") lastName = customerModel.lastName;
                            }
                            else if(wordCount == 2)
                            {
                                string[] splittedCustomerName = customerName.Split(' ');

                                for (int a = 1; a < 3; a = a + 1)
                                {
                                    if (a == 1) firstName = splittedCustomerName[0];
                                    if (a == 2) lastName = splittedCustomerName[1];
                                    if (a == 3) middleName = splittedCustomerName[2];
                                }
                                if (splittedCustomerName[1] == "")
                                {
                                    lastName = splittedCustomerName[2];
                                    middleName = null;
                                }
                                if (lastName == "") lastName = customerModel.lastName;
                            }
                            else
                            {
                                firstName = customerModel.firstName;
                                middleName = customerModel.middleName;
                                lastName = customerModel.lastName;
                            }


                            var isPolitical = Convert.ToInt32(customerModel.isPEP);
                            customerModel.isPoliticallyExposed = isPolitical > 0 ? true : false;

                            var cus = new CustomerViewModels
                            {
                                creationMailSent = true,
                                customerCode = customerModel.custID,
                                customerSensitivityLevelId = customerModel.customerSensitivityLevelId,
                                customerTypeId = (short)customerModel.customerTypeId,
                                dateOfBirth = customerModel.dateofbirth,
                                dateTimeCreated = DateTime.Now,
                                emailAddress = customerModel.e_mail,
                                firstName = (customerModel.customerType == "C") ? customerModel.customerName : firstName,
                                gender = customerModel.gender,
                                lastName = lastName,
                                middleName = middleName,
                                isPoliticallyExposed = customerModel.isPoliticallyExposed,
                                relationshipOfficerId = customerModel.relationshipOfficerId,
                                customerBVN = customerModel.BVN,
                                dateOfRelationshipWithBank = customerModel.dateOpened,
                                nameofSignatories = customerModel.signatory,
                                
                                //the nulls
                                prospectCustomerCode = customerModel.prospectCustomerCode,
                                isProspect = customerModel.isProspect,
                                crmsCompanySizeId = customerModel.crmsCompanySizeId,
                                crmsLegalStatusId = customerModel.crmsLegalStatusId,
                                crmsRelationshipTypeId = customerModel.crmsRelationshipTypeId,
                                countryOfResidentId = customerModel.countryOfResidentId,
                                numberOfDependents = customerModel.numberOfDependents,
                                numberOfLoansTaken = customerModel.numberOfLoansTaken,
                                loanMonthlyRepaymentFromOtherBanks = customerModel.loanMonthlyRepaymentFromOtherBanks,
                                relationshipTypeId = customerModel.relationshipTypeId,
                                teamLDP = customerModel.teamLDP,
                                teamNPL = customerModel.teamNPL,
                                corr = customerModel.corr,
                                pastDueObligations = customerModel.pastDueObligations,
                                businessUnitId = customerModel.businessUnitId,
                                ownership = customerModel.ownership,
                                addressofSignatories = customerModel.addressofSignatories,
                                phoneNumberofSignatories = customerModel.phoneNumberofSignatories,
                                emailofSignatories = customerModel.emailofSignatories,
                                customerGlobalNumber = customerModel.customerGlobalNumber,
                                isGlobalCustomer = customerModel.isGlobalCustomer,
                                spouse = customerModel.spouse,
                                subSectorId = customerModel.subSectorId,
                                taxNumber = customerModel.taxNumber,
                                riskRatingId = customerModel.riskRatingId,
                                isInvestmentGrade = customerModel.isInvestmentGrade,
                                isRealatedParty = customerModel.isRealatedParty,
                                misCode = customerModel.misCode,
                                misStaff = customerModel.misStaff,
                                nationalityId = customerModel.nationalityId,
                                occupation = customerModel.occupation,
                                placeOfBirth = customerModel.placeOfBirth,
                                maidenName = customerModel.maidenName,
                                maritalStatus = customerModel.maritalStatus,
                                title = customerModel.title,

                            };

                            customers.Add(cus);
                        }
                    }
                    else
                    {
                        var log = new TBL_CUSTOM_API_LOGS
                        {
                            APIURL = fullURL,
                            LOGTYPEID = 8,
                            REFERENCENUMBER = customerAccount,
                            REQUESTDATETIME = requestDatetime,
                            REQUESTMESSAGE = new JavaScriptSerializer().Serialize(reqbody),
                            RESPONSEDATETIME = responseDateTime,
                            RESPONSEMESSAGE = "Failed " +response.ErrorMessage,
                        };

                        FinTrakBankingContext logContext = new FinTrakBankingContext();
                        logContext.TBL_CUSTOM_API_LOGS.Add(log);
                        logContext.SaveChanges();

                        throw new APIErrorException($"Core Banking API Error - GetCustomerAcctsDetail API is Currently Unavailable. Contact IT Admin for Support!");
                    }

                    responseMessage = responbody?.response_message;
                    return customers;
                }
                catch (APIErrorException ex)
                {
                    throw new APIErrorException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new APIErrorException($"Error 202 " + ex.Message);
                }
                finally
                {
                    if (customers.Count() > 0)
                    {
                        var logs = new TBL_CUSTOM_API_LOGS
                        {
                            APIURL = "GetCustomerAcctsDetail",
                            LOGTYPEID = 8,
                            REFERENCENUMBER = customerAccount,
                            REQUESTDATETIME = requestDatetime,
                            REQUESTMESSAGE = new JavaScriptSerializer().Serialize(reqbody),
                            RESPONSEDATETIME = responseDateTime,
                            RESPONSEMESSAGE = "Success " + records.response_message + " " + records.getcustomeracctsdetailsresp + " " + response.Content,
                        };

                        FinTrakBankingContext logContext = new FinTrakBankingContext();
                        logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                        logContext.SaveChanges();
                    }
                }
            }


            public async Task<CasaBalanceViewModel> GetCustomerAccountBalance(string customerAccount)
            {
                IRestResponse response = null;
                DateTime requestDatetime = DateTime.Now, responseDateTime = new DateTime();
                string responseMessage = "";
                RestRequest req = new RestRequest(Method.POST);
                CasaIntegrationViewModel records = new CasaIntegrationViewModel();
                List<CasaIntegrationViewModel> cas = new List<CasaIntegrationViewModel>();
                CasaIntegrationViewModel reqbody = null;

                try
                {
                    getAPIURLSettings("Default");

                    var baseURL = API_URL;
                    string fullURL = baseURL + "GetCustomerAcctsDetail";
                    RestClient client = new RestClient(fullURL);

                    reqbody = new CasaIntegrationViewModel()
                    {
                        channel_code = "FINTRAK",
                        account_no = customerAccount
                    };

                    requestDatetime = DateTime.Now;
                    responseDateTime = DateTime.Now;

                    var jsonbody = new JavaScriptSerializer().Serialize(reqbody);
                    req.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
                    req.AddHeader("Content-Type", "application/json");
                    req.AddHeader("Accept", "application/json");
                    req.AddHeader("Authorization", API_KEY);

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    response = await client.ExecuteAsync<CasaBalanceViewModel>(req);
                    var responbody = JsonConvert.DeserializeObject<CasaBalanceViewModel>(response.Content);

                    if (responbody == null || !responbody.response_message.ToLower().Contains("successful"))
                    {
                        throw new APIErrorException("API call error - " + responbody?.response_message + " " + responbody?.response_code + " " + DateTime.Now);
                    }
                    var rep = responbody;
                    records.response_code = rep.response_code;
                    records.response_message = rep.response_message;
                    records.getcustomeracctsdetailsresp = rep.getcustomeracctsdetailsresp;


                    if (responbody != null)
                    {
                        responbody.accountName = records.getcustomeracctsdetailsresp[0].accountName;
                        responbody.accountNo = records.getcustomeracctsdetailsresp[0].accountNo;
                        responbody.availableBalance = (decimal)records.getcustomeracctsdetailsresp[0].availableBalance;
                        responbody.currencyId = 1;
                        //accountStatusId = customerModel.AccountStatusId
                        responbody.customerCode = records.getcustomeracctsdetailsresp[0].customerNo;
                        responbody.product = records.getcustomeracctsdetailsresp[0].productName;
                        responbody.currencyType = records.getcustomeracctsdetailsresp[0].currencyCod;
                        responbody.accountStatus = records.getcustomeracctsdetailsresp[0].AccountStatus;
                        responbody.freezeStatus = records.getcustomeracctsdetailsresp[0].frozenFlag;
                        //freezeReason = accountAPI.freezeReason;
                        //lastTransactionDate = accountAPI.lastTransactionDate;
                        responbody.hasBalance = true;
                        responbody.isCasaAccountDetailAvailable = true;
                        //if (product != null)
                        //{
                        //responbody.productType = accountAPI.productType;
                        //responbody.productName = accountAPI.productName;
                        //}
                    }
                    else
                    {
                        responbody = new CasaBalanceViewModel();
                    }

                    //responseApi = await response.Content.ReadAsAsync<TransactionPostingViewModel>();
                    return responbody;
                }

                catch (Exception ex)
                {
                    var innerExceptionMessage = "";
                    if (ex.InnerException != null)
                        innerExceptionMessage = ex.InnerException.Message;

                    throw new APIErrorException($"Core Banking API Error - {ex.Message} - inner exception - {innerExceptionMessage}");
                }

                finally
                {
                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = $"{API_URL}GetLoanSummaryByAccountNo/{customerAccount}",
                        LOGTYPEID = 1,
                        REFERENCENUMBER = customerAccount,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = customerAccount,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMessage,
                    };
                    FinTrakBankingContext logContext = new FinTrakBankingContext();

                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);

                    logContext.SaveChanges();
                }
            }

            public async Task<List<CasaViewModel>> GetCustomerAccountsBalanceByCustomerCode(string customerCode)
            {
                IRestResponse response = null;
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                string responseMessage = "";
                RestRequest req = new RestRequest(Method.POST);
                CasaViewModel records = new CasaViewModel();
                List<CasaViewModel> casa = new List<CasaViewModel>();
                CasaViewModel reqbody = null;
                try
                {
                    getAPIURLSettings("Default");

                    var baseURL = API_URL;
                    string fullURL = baseURL + "GetAccountSummaryByCustomerID";
                    RestClient client = new RestClient(fullURL);

                    reqbody = new CasaViewModel()
                    {
                        channel_code = "FINTRAK",
                        customer_no = customerCode
                    };

                    requestDatetime = DateTime.Now;
                    responseDateTime = DateTime.Now;

                    var jsonbody = new JavaScriptSerializer().Serialize(reqbody);
                    req.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
                    req.AddHeader("Content-Type", "application/json");
                    req.AddHeader("Accept", "application/json");
                    req.AddHeader("Authorization", API_KEY);

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    response = await client.ExecuteAsync<CustomerViewModels>(req);
                    var responbody = JsonConvert.DeserializeObject<CustomerViewModels>(response.Content);
                   
                    responseDateTime = DateTime.Now;

                    if (response.IsSuccessful)
                    {
                        if (responbody == null || !responbody.response_message.ToLower().Contains("successful"))
                        {
                            var logs = new TBL_CUSTOM_API_LOGS
                            {
                                APIURL = $"{API_URL}GetAccountSummaryByCustomerID/{customerCode}",
                                LOGTYPEID = 5,
                                REFERENCENUMBER = customerCode,
                                REQUESTDATETIME = requestDatetime,
                                REQUESTMESSAGE = customerCode,
                                RESPONSEDATETIME = responseDateTime,
                                RESPONSEMESSAGE = "API call error - " + responbody.response_message + " " + responbody.response_code + " " + DateTime.Now,
                            };

                            FinTrakBankingContext logContext = new FinTrakBankingContext();
                            logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                            logContext.SaveChanges();
                            throw new APIErrorException("API call error - " + responbody.response_message + " " + responbody.response_code + " " + DateTime.Now);
                        }
                        var rep = responbody;
                        records.response_code = rep.response_code;
                        records.response_message = rep.response_message;
                        records.getacctsummarybycustomernosresp = rep.getacctsummarybycustomernosresp;

                        foreach (var d in records.getacctsummarybycustomernosresp)
                        {
                            casa.Add(new CasaViewModel
                            {
                                productAccountNumber = d.AccountNo,
                                productAccountName = d.AccountName,
                                productCode = d.ProductCode,
                                productName = d.ProductCodeDesc,
                                currency = d.CurrencyCode,
                                branchCode = d.BranchCode,
                                accountStatusName = d.AccountStatus,
                                //effectiveDate = d.lastTransactionDate,
                                availableBalance = (decimal)d.AvailableBalance,
                                ledgerBalance = (decimal)d.ClosingBalance,
                            });
                        }
                    }

                return casa;
                }
                catch (APIErrorException ex)
                {
                    throw new APIErrorException($"Core Banking API Error - {ex.Message}");
                }

                finally
                {
                    
                    responseDateTime = DateTime.Now;
                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = $"{API_URL}GetAccountSummaryByCustomerID/{customerCode}",
                        LOGTYPEID = 5,
                        REFERENCENUMBER = customerCode,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = customerCode,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMessage,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }
            }

            

            public async Task<string> CheckExposePerson(string customerCode)
            {
                IRestResponse response = null;
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                string responseMessage = "";
                PEPViewModel records = new PEPViewModel();
                RestRequest req = new RestRequest(Method.POST);
                PEPViewModel reqbody = null;
                try
                {
                    getAPIURLSettings("Default");
                    var baseURL = API_URL;
                    string fullURL = baseURL + "GetPEPDetails";
                    RestClient client = new RestClient(fullURL);

                    reqbody = new PEPViewModel()
                    {
                        channel_code = "FINTRAK",
                    };

                    var jsonbody = new JavaScriptSerializer().Serialize(reqbody);
                    req.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
                    req.AddHeader("Content-Type", "application/json");
                    req.AddHeader("Accept", "application/json");
                    req.AddHeader("Authorization", API_KEY);

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    string result = string.Empty;
                    response = await client.ExecuteAsync<PEPViewModel>(req);
                    var responbody = JsonConvert.DeserializeObject<PEPViewModel>(response.Content);

                    responseDateTime = DateTime.Now;

                    if (response.IsSuccessful)
                    {
                        if (responbody == null || !responbody.response_message.ToLower().Contains("successful"))
                        {
                            throw new APIErrorException("API call error - " + responbody.response_message + " " + responbody.response_code + " " + DateTime.Now);
                        }
                        var rep = responbody;
                        records.response_code = rep.response_code;
                        records.response_message = rep.response_message;
                        records.GetPEPDetails = rep.GetPEPDetails;

                        
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    throw new APIErrorException("Core Banking API Error - " + ex.Message);
                }

                finally
                {
                    
                    responseDateTime = DateTime.Now;
                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = $"{API_URL}GetExposedPerson/{customerCode}",
                        LOGTYPEID = 6,
                        REFERENCENUMBER = customerCode,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = customerCode,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMessage,
                    };
                    FinTrakBankingContext logContext = new FinTrakBankingContext();

                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);

                    logContext.SaveChanges();
                }
            }

            public async Task<BVNCustomerDetailsViewModel> BVNCustomerDetails(string customerCode)
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;


                HttpClient client = new HttpClient(handler);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                //ResponseMessageViewModel res = null;
                string responseMessage = "";
                getAPIURLSettings("BVN");

                string result = string.Empty;
                var token = new AuthenticationHeaderValue("Authorization", API_KEY);
                handler.UseDefaultCredentials = true;
                //HttpClient client = new HttpClient(handler);

                httpClientInstance = new HttpClient();
                httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                client.Timeout = TimeSpan.FromSeconds(180);
                client.DefaultRequestHeaders.Authorization = token;
                client.BaseAddress = new Uri(API_URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                requestDatetime = DateTime.Now;
                response = await client.GetAsync($"api/OfficeAccount/GetGeneralLedgerAccountRecord/{customerCode}");

                responseDateTime = DateTime.Now;
                BVNCustomerDetailsViewModel data = null;
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    dynamic dataObj = JsonConvert.DeserializeObject<string>(jsonString);

                    foreach (var d in dataObj)
                    {
                        data = (new BVNCustomerDetailsViewModel
                        {
                            accountNumber = d.accountNumber,
                            contactAddress = d.contactAddress,
                            dateOfBirth = d.dateOfBirth,
                            emailAddress = d.emailAddress,
                            firstName = d.firstName,
                            lastName = d.lastName,
                            middleName = d.middleName,
                            phoneNumber = d.phoneNumber

                        });
                    }

                }
                responseMessage = await response.Content.ReadAsStringAsync();
                handler.Dispose();
                client.Dispose();
                
                var logs = new TBL_CUSTOM_API_LOGS
                {
                    APIURL = $"{API_URL}api/OfficeAccount/GetGlAccountRecord?customerCode={customerCode}",
                    LOGTYPEID = 7,
                    REFERENCENUMBER = customerCode,
                    REQUESTDATETIME = requestDatetime,
                    REQUESTMESSAGE = customerCode,
                    RESPONSEDATETIME = responseDateTime,
                    RESPONSEMESSAGE = responseMessage,
                };
                FinTrakBankingContext logContext = new FinTrakBankingContext();

                logContext.TBL_CUSTOM_API_LOGS.Add(logs);

                logContext.SaveChanges();

                return data;
            }

            public async Task<InterestRateInquiryViewModel> GetInterestRateInquiry(string accountNumber, string accountType)
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;

                HttpClient client = new HttpClient(handler);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                InterestRateInquiryIntegrationViewModel accountAPI = new InterestRateInquiryIntegrationViewModel();
                //ResponseMessageViewModel res = null;
                string responseMessage = "";
                getAPIURLSettings("RateEnquiry");
                try
                {
                    handler.UseDefaultCredentials = true;

                    var token = new AuthenticationHeaderValue("Authorization", API_KEY);
                    httpClientInstance = new HttpClient();
                    httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                    client.Timeout = TimeSpan.FromSeconds(180);
                    client.BaseAddress = new Uri(API_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = token;

                    client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                    InterestRateInquiryViewModel accountOutput = new InterestRateInquiryViewModel();

                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    requestDatetime = DateTime.Now;
                    //response = await client.GetAsync($"api/InterestRateInquiry/GetInterestRateInquiry?model.accountNumber={accountNumber}&model.accountType={accountType}",
                    response = await client.GetAsync($"GetInterestRateInquiry/{accountNumber}");

                   
                    responseDateTime = DateTime.Now;
                    if (response.IsSuccessStatusCode)
                    {
                        //accountAPI = await response.Content.ReadAsAsync<InterestRateInquiryIntegrationViewModel>();

                        var responseData = await response.Content.ReadAsStringAsync();
                        JObject jsonString = JObject.Parse(responseData);
                        var data = jsonString["data"].ToString();

                        accountAPI = JsonConvert.DeserializeObject<InterestRateInquiryIntegrationViewModel>(data);

                        accountOutput.accountNumber = accountAPI.interestRateDetails.accountNumber;
                        accountOutput.accountType = accountAPI.interestRateDetails.accountType;
                        accountOutput.interestTableCode = accountAPI.interestRateDetails.interestTableCode;
                        accountOutput.interestSerialNumber = accountAPI.interestRateDetails.interestSerialNumber;
                        accountOutput.startDate = accountAPI.interestRateDetails.startDate;
                        accountOutput.endDate = accountAPI.interestRateDetails.endDate;
                        accountOutput.interestRateAmount = accountAPI.interestRateDetails.interestRateAmount;
                        accountOutput.lastChangedDate = accountAPI.interestRateDetails.lastChangedDate;
                    }

                    //responseApi = await response.Content.ReadAsAsync<TransactionPostingViewModel>();

                    responseMessage = await response.Content.ReadAsStringAsync(); //.ReadAsAsync<CasaIntegrationViewModel>();

                    handler.Dispose();
                    client.Dispose();

                    return accountOutput;
                }
                catch (Exception ex)
                {
                    var innerExceptionMessage = "";
                    if (ex.InnerException != null)
                        innerExceptionMessage = ex.InnerException.Message;

                    throw new APIErrorException($"Core Banking API Error - {ex.Message} - inner exception - {innerExceptionMessage}");
                }

                finally
                {
                    handler.Dispose();
                    client.Dispose();

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        //APIURL = $"api/InterestRateInquiry/GetInterestRateInquiry?model.accountNumber={accountNumber}&model.accountType={accountType}",
                        APIURL = $"{API_URL}GetInterestRateInquiry/{ accountNumber }",
                        LOGTYPEID = 18,
                        REFERENCENUMBER = accountNumber,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = accountNumber + '_' + accountType,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMessage,
                    };
                    FinTrakBankingContext logContext = new FinTrakBankingContext();

                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);

                    logContext.SaveChanges();
                }
            }

            //public async Task<List<CustomerTurnoverViewModel>> GetCustomerTransactions(string customerCode, int durationInMonths)
            //{
            //    //month = 48;
            //    //cifid = "483008974";
            //    HttpClientHandler handler = new HttpClientHandler();
            //    HttpClient httpClientInstance;

            //    //var endpointUrl = $"api/Customer/GetCustomerTransactions?Cif_Id={customerCode}&Month={durationInMonths}";
            //    //var endpointUrl = $"api/Customer/GetCustomerTransactions/{customerCode}/{durationInMonths}";
            //    var endpointUrl = $"api/Customer/GetCustomerTransactions/{customerCode}/07-2019";

            //    httpClientInstance = new HttpClient();
            //    httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
            //    //
            //    handler.UseDefaultCredentials = true;
            //    var token = new AuthenticationHeaderValue("Authorization", API_KEY);

            //    HttpClient client = new HttpClient(handler);
            //    client.Timeout = TimeSpan.FromSeconds(180);
            //    client.BaseAddress = new Uri(API_URL);
            //    client.DefaultRequestHeaders.Authorization = token;
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            //    HttpResponseMessage response = null;
            //    DateTime requestTime = new DateTime();
            //    DateTime responseTime = new DateTime();

            //    requestTime = DateTime.Now;
            //    response = await client.GetAsync(endpointUrl);
            //    responseTime = DateTime.Now;

            //    List<CustomerTurnoverViewModelAPI> result = null;

            //    List<CustomerTurnoverViewModel> accounts = new List<CustomerTurnoverViewModel>();

            //    var responseMessage = await response.Content.ReadAsStringAsync();

            //    if (response.IsSuccessStatusCode)
            //    {
            //        //result = await response.Content.ReadAsAsync<List<CustomerTurnoverViewModelAPI>>();

            //        var responseData = await response.Content.ReadAsStringAsync();
            //        JObject responseDataJsonString = JObject.Parse(responseData);

            //        var data = responseDataJsonString["data"].ToString();
            //        var apiData = JsonConvert.DeserializeObject<List<CustomerTurnoverViewModelAPI>>(data);


            //        //var jsonString = await response.Content.ReadAsStringAsync();

            //        //var apiData = JsonConvert.DeserializeObject<List<CustomerTurnoverViewModelAPI>>(jsonString);

            //        foreach (var item in apiData)
            //        {

            //            decimal amc = 0;
            //            Decimal.TryParse(item.amc.Replace(",", ""), out amc);

            //            decimal vat = 0;
            //            Decimal.TryParse(item.vat.Replace(",", ""), out vat);

            //            decimal management_Fee = 0;
            //            Decimal.TryParse(item.management_Fee.Replace(",", ""), out management_Fee);

            //            decimal commitment_Fees = 0;
            //            Decimal.TryParse(item.commitment_Fees.Replace(",", ""), out commitment_Fees);

            //            decimal com_Contigent_Liab = 0;
            //            Decimal.TryParse(item.com_Contigent_Liab.Replace(",", ""), out com_Contigent_Liab);

            //            decimal lc_Commission = 0;
            //            Decimal.TryParse(item.lc_Commission.Replace(",", ""), out lc_Commission);

            //            decimal sms_Alert = 0;
            //            Decimal.TryParse(item.sms_Alert.Replace(",", ""), out lc_Commission);

            //            accounts.Add(new CustomerTurnoverViewModel
            //            {
            //                accountNumber = item.foracid,
            //                customerCode = item.cust_Id,
            //                period = item.period,
            //                productName = item.schm_Type,
            //                max_Credit_Balance = item.max_Credit_Balance,
            //                max_Debit_Balance = item.max_Debit_Balance,
            //                min_Credit_Balance = item.min_Credit_Balance,
            //                min_Debit_Balance = item.min_Debit_Balance,
            //                credit_Turnover = item.credit_Turnover,
            //                debit_Turnover = item.debit_Turnover,
            //                amc = amc,
            //                vat = vat,
            //                management_Fee = management_Fee,
            //                commitment_Fees = commitment_Fees,
            //                com_Contigent_Liab = com_Contigent_Liab,
            //                lc_Commission = lc_Commission,
            //                sms_Alert = sms_Alert,
            //                month=item.month,
            //                year = item.year,

            //            });
            //        }

            //    }


            //    handler.Dispose();
            //    client.Dispose();

            //    FintrakBankingDatabaseCustomerTurnoverOperations(
            //        endpointUrl,
            //        customerCode,
            //        requestTime,
            //        responseTime,
            //        "Cif_Id={cifid}&Month={month}",
            //        responseMessage
            //    );

            //    return accounts;
            //}


            public async Task<List<CustomerTurnoverViewModel>> GetCustomerTransactions(string accountNumber, DateTime startDate, DateTime endDate )
            {
                IRestResponse response = null;
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                string responseMessage = "";
                RestRequest req = new RestRequest(Method.POST);
                CustomerTurnoverViewModel records = new CustomerTurnoverViewModel();
                List<CustomerTurnoverViewModel> rec = new List<CustomerTurnoverViewModel>();
                CustomerTurnoverGroupViewModel reqbody = null;

                var newStartDate = startDate.ToString("d-MMM-yyyy");
                var newEndDate = endDate.ToString("d-MMM-yyyy");

                getAPIURLSettings("Default");

                var baseURL = API_URL;
                string fullURL = baseURL + "GetAccountSummaryAndTransactions";
                RestClient client = new RestClient(fullURL);

                reqbody = new CustomerTurnoverGroupViewModel()
                {
                    channel_code = "FINTRAK",
                    account_no = accountNumber,
                    start_date = newStartDate,
                    end_date = newEndDate
                };

                
                responseDateTime = DateTime.Now;

                var jsonbody = new JavaScriptSerializer().Serialize(reqbody);
                req.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
                req.AddHeader("Content-Type", "application/json");
                req.AddHeader("Accept", "application/json");
                req.AddHeader("Authorization", API_KEY);

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                response = await client.ExecuteAsync<CustomerTurnoverViewModel>(req);
                var responbody = JsonConvert.DeserializeObject<CustomerTurnoverViewModel>(response.Content);

                responseDateTime = DateTime.Now;
                if (response.IsSuccessful)
                {
                    if (responbody == null || !responbody.response_message.ToLower().Contains("successful"))
                    {
                        throw new APIErrorException("API call error - " + responbody.response_message + " " + responbody.response_code + " " + DateTime.Now);
                    }
                    var rep = responbody;
                    records.response_code = rep.response_code;
                    records.response_message = rep.response_message;
                    records.getaccountsummaryandtrnxresp = rep.getaccountsummaryandtrnxresp;

                    foreach (var i in records.getaccountsummaryandtrnxresp) {

                        var record = new CustomerTurnoverViewModel
                        {

                            accountNumber = i.account_no,
                            productName = i.productcode_desc,
                            min_Debit_Balance = i.total_withdrawal,
                            max_Debit_Balance = i.total_withdrawal,
                            min_Credit_Balance = i.total_lodgement,
                            max_Credit_Balance = i.total_lodgement,
                            debit_Turnover = i.total_withdrawal,
                            credit_Turnover = i.total_lodgement,
                            sms_Alert = i.running_balance,
                    
                        };

                        rec.Add(record);

                    }
                }
                else
                {

                }

                    return rec;
            }

            public async Task<List<CustomerTurnoverViewModel>> GetCustomerInterestTransactions(string customerCode, int durationInMonths)
            {

                var currentDate = DateTime.Now;
                var startDate = DateTime.Now.AddMonths(-durationInMonths);
                
                var endDate = DateTime.Now.AddMonths(-durationInMonths);
                IRestResponse response = null;
                string responseMessage = "";
                RestRequest req = new RestRequest(Method.POST);
                CustomerTurnoverViewModel records = new CustomerTurnoverViewModel();
                List<CustomerTurnoverViewModel> accounts = new List<CustomerTurnoverViewModel>();
                CustomerTurnoverViewModel reqbody = null;
                getAPIURLSettings("Default");
                var baseURL = API_URL;
                string fullURL = baseURL + "GetAccountSummaryByCustomerID";
                RestClient client = new RestClient(fullURL);

                var endpointUrl = "";



                if (durationInMonths > 9)
                    endpointUrl = fullURL;
                else
                    endpointUrl = fullURL;


                DateTime requestTime = new DateTime();
                DateTime responseTime = new DateTime();

                reqbody = new CustomerTurnoverViewModel()
                {
                    channel_code = "FINTRAK",
                    customer_no = customerCode,
                    
                };

                

                var jsonbody = new JavaScriptSerializer().Serialize(reqbody);
                req.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
                req.AddHeader("Content-Type", "application/json");
                req.AddHeader("Accept", "application/json");
                req.AddHeader("Authorization", API_KEY);

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                requestTime = DateTime.Now;
                response = await client.ExecuteAsync<CustomerTurnoverViewModelAPI>(req);
                var responbody = JsonConvert.DeserializeObject<CustomerTurnoverViewModelAPI>(response.Content);


                responseTime = DateTime.Now;
                if (response.IsSuccessful)
                {
                    if (responbody == null || !responbody.response_message.ToLower().Contains("successful"))
                    {
                        throw new APIErrorException("API call error - " + responbody.response_message + " " + responbody.response_code + " " + DateTime.Now);
                    }
                    var rep = responbody;
                    records.response_code = rep.response_code;
                    records.response_message = rep.response_message;
                    records.getacctsummarybycustomernosresp = rep.getacctsummarybycustomernosresp;



                   // foreach (var item in records)
                   // {
                        decimal float_Charge = 0;
                        //Decimal.TryParse(item.float_Charge.Replace(",", ""), out float_Charge);
                        decimal interest = 0;
                        //Decimal.TryParse(item.interest.Replace(",", ""), out interest);

                        accounts.Add(new CustomerTurnoverViewModel
                        {
                            accountNumber = rep.getacctsummarybycustomernosresp[0].AccountNo,
                            customerCode = rep.getacctsummarybycustomernosresp[0].CustID,
                           
                            //the nulls
                            //period = 
                            productName = rep.getacctsummarybycustomernosresp[0].ProductCodeDesc,
                            interest = rep.getacctsummarybycustomernosresp[0].NetBalance,
                            float_Charge = rep.getacctsummarybycustomernosresp[0].Withdrawal,
                            //month = month,
                            //year = year,
                            month = startDate.Month,
                            year = startDate.Year,
                        });
                    };

                    return accounts;
                
                
            }
            private void FintrakBankingDatabaseCustomerTurnoverOperations(string baseUrl,
                string endpointUrl,
                string cifid,
                DateTime requestTime,
                DateTime responseTime,
                string requestMessage,
                string responseMessage
                
                )
            {

                var logs = new TBL_CUSTOM_API_LOGS
                {
                    APIURL = baseUrl + endpointUrl,
                    LOGTYPEID = 4,
                    REFERENCENUMBER = cifid,
                    REQUESTDATETIME = requestTime,
                    REQUESTMESSAGE = requestMessage,
                    RESPONSEDATETIME = responseTime,
                    RESPONSEMESSAGE = responseMessage,
                };

                FinTrakBankingContext logContext = new FinTrakBankingContext();
                logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                logContext.SaveChanges();
            }

        }
    }

}

