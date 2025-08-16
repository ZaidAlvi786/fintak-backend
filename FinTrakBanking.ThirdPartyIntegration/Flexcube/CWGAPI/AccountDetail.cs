using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using FintrakBanking.ViewModels.Finance;
using FintrakBanking.ViewModels.ThridPartyIntegration;

namespace FinTrakBanking.ThirdPartyIntegration
{
    using FintrakBanking.Common.CustomException;
    using FintrakBanking.Entities.Models;
    using Newtonsoft.Json;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;

    namespace AccountInformation
    {
        public class AccountDetail
        {

            private FinTrakBankingContext context;
            string API_KEY, API_URL = string.Empty;
            private HttpClientHandler handler = new HttpClientHandler();
            private static HttpClient _httpClientInstance;
            private IEnumerable<TBL_API_URL> APIUrlConfig;


            public AccountDetail(FinTrakBankingContext _context)
            {
                this.context = _context;
                var configdata = context.TBL_SETUP_COMPANY.FirstOrDefault();
                API_KEY = configdata.APIKEY;
                API_URL = configdata.APIURL;
                APIUrlConfig = context.TBL_API_URL;
            }

            private void getAPIURLSettings(string typeName = null)
            {
                var apiConfig = APIUrlConfig.Where(x => x.TYPENAME.ToLower() == typeName.ToLower()).FirstOrDefault();
                if (apiConfig != null)
                {
                    API_URL = apiConfig.URL.Trim();
                    API_KEY = apiConfig.APIKEY;
                }
                if (apiConfig == null)
                {
                    apiConfig = APIUrlConfig.Where(x => x.TYPENAME.ToUpper() == "DEFAULT").FirstOrDefault();
                    API_URL = apiConfig.URL.Trim();
                    API_KEY = apiConfig.APIKEY;
                }
            }

            public async Task<GLAccountDetailsViewModel> APIOfficeAccountGetGeneralLedgerAccountRecord(string glNumber)
            {
                HttpClient client = new HttpClient(handler);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                ResponseMessageViewModel res = null;
                string responseMessage = "";
                handler.UseDefaultCredentials = true;
               // HttpClient client = new HttpClient(handler);
                var token = new AuthenticationHeaderValue("Authorization", API_KEY);
                _httpClientInstance = new HttpClient();
                _httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                client.Timeout = TimeSpan.FromSeconds(180);
                client.BaseAddress = new Uri(API_URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = token;
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, cert, chain, sslPolicyErrors) => true;
                requestDatetime = DateTime.Now;
                response = await client.GetAsync($"api/Office/GetGeneralLedgerAccountRecord/{glNumber}");
                responseDateTime = DateTime.Now;
                GLAccountDetailsViewModel result = null;
                if (response.IsSuccessStatusCode)
                {
                    GLAccountDetailsViewModel data = await response.Content.ReadAsAsync<GLAccountDetailsViewModel>();
                    if (data != null)
                    {
                        result = new GLAccountDetailsViewModel
                        {
                            accountName = data.accountName,
                            accountNumber = data.accountNumber,
                            balance = data.balance,
                            branch = data.branch,
                            currencyType = data.currencyType,
                            glSubHeadCode = data.glSubHeadCode,
                            partitionedFlag = data.partitionedFlag,
                            partitionedType = data.partitionedType,
                            product = data.product,
                            productName = data.productName,
                            productType = data.productType,
                            systemAccountFlag = data.systemAccountFlag,
                            response = response,
                        };

                        responseMessage = await response.Content.ReadAsStringAsync();
                        handler.Dispose();
                        client.Dispose();


                        var logs = new TBL_CUSTOM_API_LOGS
                        {
                            APIURL = $"api/Office/GetGeneralLedgerAccountRecord/{glNumber}",
                            LOGTYPEID = 8,
                            REFERENCENUMBER = glNumber,
                            REQUESTDATETIME = requestDatetime,
                            REQUESTMESSAGE = glNumber,
                            RESPONSEDATETIME = responseDateTime,
                            RESPONSEMESSAGE = responseMessage,
                        };
                        FinTrakBankingContext logContext = new FinTrakBankingContext();

                        logContext.TBL_CUSTOM_API_LOGS.Add(logs);

                        logContext.SaveChanges();
                        return result;
                    }
                    else throw new ConditionNotMetException("Account number not found on finacle");
                }
                else throw new ConditionNotMetException("Account Number Search. " + response.ReasonPhrase);

                //handler.Dispose();
               // client.Dispose();
               // return result;
            }

            public async Task<TDAccountRecordViewModel> APIOfficeAccountGetTermDepositAccountRecord(
                string teamDepositAccountNumber)
            {
                
                try
                {
                    IRestResponse response = null;
                    DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                    string responseMessage = "";
                    RestRequest req = new RestRequest(Method.POST);
                    TDAccountRecordViewModel records = new TDAccountRecordViewModel();
                    List<TDAccountRecordViewModel> rec = new List<TDAccountRecordViewModel>();
                    TDAccountRecordViewModel reqbody = null;

                    getAPIURLSettings("Default");

                    var baseURL = API_URL;
                    string fullURL = baseURL + "GetFixedDepositInfo";
                    RestClient client = new RestClient(fullURL);

                    reqbody = new TDAccountRecordViewModel()
                    {
                        channel_code = "FINTRAK",
                        account_no = teamDepositAccountNumber
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

                    response = await client.ExecuteAsync<TDAccountRecordViewModel>(req);
                    var responbody = JsonConvert.DeserializeObject<TDAccountRecordViewModel>(response.Content);

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
                        records.FixedDepositInfo = rep.FixedDepositInfo;

                        if (records.FixedDepositInfo.Count() > 1)
                        {

                        }
                        else { 
                            
                            records = new TDAccountRecordViewModel
                            {
                                
                                productCode = records.FixedDepositInfo[0].product,
                                //the nulls
                                accountName = records.FixedDepositInfo[0].accountName,
                                accountNumber = records.FixedDepositInfo[0].accountNumber,
                                balance = records.FixedDepositInfo[0].balance,
                                branch = records.FixedDepositInfo[0].branch,
                                currencyType = records.FixedDepositInfo[0].currencyType,
                                customerCode = records.FixedDepositInfo[0].customerCode,
                                productName = records.FixedDepositInfo[0].productName,
                                productType = records.FixedDepositInfo[0].productType,
                                lienAmount = records.FixedDepositInfo[0].lienAmount,
                                response = records.response_code,
                                isSuccess = true,
                                //their extras
                                customer_name = records.FixedDepositInfo[0].customer_name,
                                branch_code = records.FixedDepositInfo[0].branch_code,
                                reference_no = records.FixedDepositInfo[0].reference_no,
                                user_referenceno = records.FixedDepositInfo[0].user_referenceno,
                                start_date = records.FixedDepositInfo[0].start_date,
                                book_date = records.FixedDepositInfo[0].book_date,
                                value_date = records.FixedDepositInfo[0].value_date,
                                maturity_date = records.FixedDepositInfo[0].maturity_date,
                                initial_deposit = records.FixedDepositInfo[0].initial_deposit,
                                settlement_account = records.FixedDepositInfo[0].settlement_account,
                                tenor = records.FixedDepositInfo[0].tenor,
                                interest_rate = records.FixedDepositInfo[0].interest_rate,
                                settlementacct_ccycode = records.FixedDepositInfo[0].settlementacct_ccycode,
                                settlementacct_brcode = records.FixedDepositInfo[0].settlementacct_brcode,
                                maturity_amount = records.FixedDepositInfo[0].maturity_amount,
                                rollover_allowed =records.FixedDepositInfo[0].rollover_allowed,
                                payment_method = records.FixedDepositInfo[0].payment_method,
                                product = records.FixedDepositInfo[0].product,
                                deposit_ccycode = records.FixedDepositInfo[0].deposit_ccycode,
                                contract_status = records.FixedDepositInfo[0].contract_status,
                                };
                            }
                    }
                    else
                    {
                        throw new APIErrorException($"Core Banking API Error - GetCustomerByAccountNumber API is Currently Unavailable. Contact IT Admin or ESB Team for Support!");
                    }

                    responseMessage = responbody?.response_message;
                    return records;

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
                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = $"{API_URL}GetLoanDetails",
                        LOGTYPEID = 9,
                        REFERENCENUMBER = teamDepositAccountNumber,
                        REQUESTDATETIME = DateTime.Now,
                        REQUESTMESSAGE = teamDepositAccountNumber,
                        RESPONSEDATETIME = DateTime.Now,
                        //RESPONSEMESSAGE = responbody.re,
                    };



                    FinTrakBankingContext logContext = new FinTrakBankingContext();
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }
            }
        }
    }

}


