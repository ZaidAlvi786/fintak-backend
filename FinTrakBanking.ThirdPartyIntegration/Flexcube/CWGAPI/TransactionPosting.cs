namespace FinTrakBanking.ThirdPartyIntegration
{
    using FintrakBanking.Common;
    using FintrakBanking.Common.CustomException;
    using FintrakBanking.Entities.Models;
    using FintrakBanking.ViewModels.CASA;
    using FintrakBanking.ViewModels.Credit;
    using FintrakBanking.ViewModels.Customer;
    using FintrakBanking.ViewModels.Finance;
    using FintrakBanking.ViewModels.Flexcube;
    using FintrakBanking.ViewModels.Setups.International;
    using FintrakBanking.ViewModels.ThridPartyIntegration;
    using FinTrakBanking.ThirdPartyIntegration.FCUBSCLServiceKenya;
    using Microsoft.Office.Interop.Excel;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;
    using System.Web.UI.WebControls.WebParts;
    using System.Xml.Serialization;
    using static FintrakBanking.ViewModels.ThridPartyIntegration.FCUBSCLServiceViewModel;

    namespace Finacle
    {
        public class TransactionPosting
        {

            private FinTrakBankingContext context;
            string API_KEY, API_URL = string.Empty;
            private IEnumerable<TBL_API_URL> APIUrlConfig;

            

            //  private IIntegrationWithFinacle finacle;

            public TransactionPosting(FinTrakBankingContext _context)
            {
                this.context = _context;
                var configdata = context.TBL_SETUP_COMPANY.FirstOrDefault();
                APIUrlConfig = context.TBL_API_URL;
                API_KEY = configdata.APIKEY;
                API_URL = configdata.APIURL;

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

            public async Task<CurrencyExchangeRateViewModel> GetExchangeRate(string fromCurrencyCode, string toCurrencyCode, string rateCode)
            {
                IRestResponse response = null;
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                string responseMessage = "";
                RestRequest req = new RestRequest(Method.POST);
                CurrencyExchangeRateViewModel exchangeRateOutput = new CurrencyExchangeRateViewModel();
                List<CurrencyExchangeRateViewModel> rec = new List<CurrencyExchangeRateViewModel>();
                CurrencyExchangeRateViewModel reqbody = null;
                var branchCode = context.TBL_COMPANY.FirstOrDefault().HEADOFFICEBRANCHCODE;
                var forGhana = false;
                var contryCode = context.TBL_COMPANY.FirstOrDefault().COUNTRYCODE;
                try
                {

                    getAPIURLSettings("Default");

                    var baseURL = API_URL;
                    string fullURL = baseURL + "GetCurrencyRate";
                    RestClient client = new RestClient(fullURL);

                    if (contryCode == "GHS")
                    {
                        reqbody = new CurrencyExchangeRateViewModel()
                        {
                            channel_code = "FINTRAK",
                            branch_code = branchCode,
                            ccy_code1 = fromCurrencyCode,
                            ccy_code2 = toCurrencyCode
                        };
                    }
                    else
                    {
                        reqbody = new CurrencyExchangeRateViewModel()
                        {
                            channel_code = "FINTRAK",
                            branch_code = branchCode,
                            from_ccycode = fromCurrencyCode,
                            to_ccycode = toCurrencyCode
                        };
                    }
                    requestDatetime = DateTime.Now;
                    responseDateTime = DateTime.Now;

                    var jsonbody = new JavaScriptSerializer().Serialize(reqbody);
                    req.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
                    req.AddHeader("Content-Type", "application/json");
                    req.AddHeader("Accept", "application/json");
                    req.AddHeader("Authorization", API_KEY);

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    response = await client.ExecuteAsync<CurrencyExchangeRateViewModel>(req);
                    var responbody = JsonConvert.DeserializeObject<CurrencyExchangeRateViewModel>(response.Content);

                    responseDateTime = DateTime.Now;

                    if (response.IsSuccessful)
                    {
                        if (responbody.response_code == "99") { return responbody; }
                        if (responbody == null || !responbody.response_message.ToLower().Contains("successful"))
                        {
                            throw new APIErrorException("API call error - " + responbody.response_message + " " + responbody.response_code + " " + DateTime.Now);
                        }
                        var rep = responbody;
                        exchangeRateOutput.response_code = rep.response_code;
                        exchangeRateOutput.response_message = rep.response_message;
                        var currencyId = context.TBL_CURRENCY.Where(x => x.CURRENCYCODE == rep.fromCurrencyCode).Select(x => x.CURRENCYID).FirstOrDefault();
                        if ( contryCode == "GHS")
                        {
                            exchangeRateOutput.GetCcyRateResponse = rep.GetCcyRateResponse;
                            exchangeRateOutput.sellingRate = (double)rep.GetCcyRateResponse[0].sale_rate;
                            exchangeRateOutput.buyingRate = (double)rep.GetCcyRateResponse[0].buy_rate;
                            exchangeRateOutput.exchangeRate = (double)rep.GetCcyRateResponse[0].sale_rate;
                        }
                        else if (contryCode == "MZN")
                        {
                            exchangeRateOutput.GetCurrencyRateResponse = rep.GetCurrencyRateResponse;
                            exchangeRateOutput.sellingRate = (double)rep.GetCurrencyRateResponse[0].mid_rate;
                            exchangeRateOutput.buyingRate = (double)rep.GetCurrencyRateResponse[0].mid_rate;
                            exchangeRateOutput.exchangeRate = (double)rep.GetCurrencyRateResponse[0].mid_rate;
                        }
                        else
                        {
                            exchangeRateOutput.GetCurrencyRateResponse = rep.GetCurrencyRateResponse;
                            exchangeRateOutput.sellingRate = (double)rep.GetCurrencyRateResponse[0].sale_rate;
                            exchangeRateOutput.buyingRate = (double)rep.GetCurrencyRateResponse[0].buy_rate;
                            exchangeRateOutput.exchangeRate = (double)rep.GetCurrencyRateResponse[0].sale_rate;
                        }

                        exchangeRateOutput.fromCurrencyCode = fromCurrencyCode;
                        exchangeRateOutput.toCurrencyCode = toCurrencyCode;
                        exchangeRateOutput.currencyId = (short)currencyId;
                        exchangeRateOutput.date = DateTime.Now;
                        exchangeRateOutput.webRequestStatus = responbody.response_message;

                    }
                    else
                    {
                        return responbody;
                    }


                    return exchangeRateOutput;
                }
                catch (APIErrorException ex)
                {
                    throw new APIErrorException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new APIErrorException($"Error" + ex.Message);
                }
                finally
                {
                  

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = $"{API_URL}GetExchangeRateProduct/{fromCurrencyCode}/{toCurrencyCode}/{rateCode}/{DateTime.Now.Date}",
                        LOGTYPEID = 3,
                        REFERENCENUMBER = fromCurrencyCode + "--" + toCurrencyCode + "--" + rateCode,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = fromCurrencyCode + "--" + toCurrencyCode + "--" + rateCode,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMessage,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }
            }
            private bool AddCustomLien(LienProcessViewModel entity)
            {
                bool output = false;
                //foreach (var item in entity)
                //{
                var data = new TBL_CUSTOM_LIEN_PROCESS();
                {
                    data.ACCOUNTID = entity.account;
                    data.AMOUNT = entity.lienAmount;
                    data.CURRENCYCODE = entity.lienAccountCurrency;
                    data.CONSUMED = false;
                    data.DATETIMECONSUMED = null;
                    data.DATETIMECREATED = DateTime.Now;
                    data.LIENTYPE = entity.lienProcessType;
                    data.REASONCODE = entity.lienReasonCode;
                    data.LIENREFERENCENUMBER = entity.lienUniqueReferenceNumber;
                    data.DESCRIPTION = entity.lienReason;
                }
                ;
                context.TBL_CUSTOM_LIEN_PROCESS.Add(data);
                //};

                context.SaveChanges();
                output = true;
                return output;

            }



            //public async Task<bool> APITransactionPosting(List<FinanceTransactionViewModel> model)
            //{

            //    var token = new AuthenticationHeaderValue("Authorization", API_KEY);
            //    bool output = false;
            //    var dta = context.TBL_SETUP_GLOBAL.ToList();
            //    TransactionPostingViewModel responseModel = new TransactionPostingViewModel();
            //    List<TransactionPostingViewModel> apiModel = new List<TransactionPostingViewModel>();
            //    foreach (var item in model)
            //    {


            //        apiModel.Add(new TransactionPostingViewModel
            //            {

            //                accounts =
            //                    item.casaAccountId
            //                        .ToString(), //item.casaAccountId!= null ? context.TBL_CASA.FirstOrDefault(x => x.CASAACCOUNTID == item.casaAccountId).PRODUCTACCOUNTNUMBER : context.TBL_CHART_OF_ACCOUNT.FirstOrDefault(x => x.GLACCOUNTID == item.glAccountId).ACCOUNTCODE,                       
            //                amounts = item.creditAmount > 0
            //                    ? "C" + item.creditAmount.ToString()
            //                    : "D" + item.debitAmount.ToString(),
            //                //amounts = item.sourceReferenceNumber,
            //                narration = item.description,
            //                referenceNumber = item.batchCode,
            //                currencyType = context.TBL_CURRENCY.FirstOrDefault(x => x.CURRENCYID == item.currencyId)
            //                    .CURRENCYCODE,
            //                operationId =
            //                    item.operationId, // != null ? context.TBL_CASA.FirstOrDefault(x => x.CASAACCOUNTID == item.operationId).PRODUCTACCOUNTNUMBER : context.TBL_CHART_OF_ACCOUNT.FirstOrDefault(x => x.GLACCOUNTID == item.glAccountId).ACCOUNTCODE,
            //            }
            //        );
            //    }

            //    handler.UseDefaultCredentials = true;
            //    HttpClient client = new HttpClient(handler);

            //    httpClientInstance = new HttpClient();
            //    httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
            //    client.Timeout = TimeSpan.FromSeconds(180);
            //    client.DefaultRequestHeaders.Authorization = token;
            //    client.BaseAddress = new Uri(API_URL);
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(
            //        new MediaTypeWithQualityHeaderValue("application/json"));

            //    ServicePointManager.ServerCertificateValidationCallback +=
            //        (sender, cert, chain, sslPolicyErrors) => true;
            //    HttpResponseMessage response = client.PostAsync("api/Transactions/PostTransactions", new StringContent(
            //        new JavaScriptSerializer().Serialize(apiModel), Encoding.UTF8, "application/json")).Result;

            //    if (response.IsSuccessStatusCode)
            //    {
            //        responseModel = await response.Content.ReadAsAsync<TransactionPostingViewModel>();

            //    }

            //    ResponseViewModel responseAPI = new ResponseViewModel();
            //    responseAPI.responseCode = responseModel.responseCode;
            //    responseAPI.webRequestDate = responseModel.webRequestDate;
            //    responseAPI.webRequestStatus = responseModel.webRequestStatus;

            //    handler.Dispose();
            //    client.Dispose();
            //    if (responseModel.responseCode == "0")
            //    {
            //        AddCustomTransactions(apiModel);
            //        output = true;
            //    }
            //    else
            //    {
            //        output = false;
            //        throw new SecureException($"Transaction {responseAPI.webRequestStatus}");
            //    }

            //    return output;


            //}

            public async Task<ResponseMessage> ApiPostCrossCurrencyTransactions(List<TransactionPostingViewModel> model)
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;

                HttpClient client = new HttpClient(handler);
                var objData = new JavaScriptSerializer().Serialize(model);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                ResponseMessageViewModel responseApi = new ResponseMessageViewModel();
                ResponseMessage responseMsg = null;
                string responseMessage = "";

                getAPIURLSettings();
                try
                {
                    var token = new AuthenticationHeaderValue("Authorization", API_KEY);

                    var dta = context.TBL_SETUP_GLOBAL.ToList();

                    handler.UseDefaultCredentials = true;


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
                    response = client.PostAsync("api/Transactions/PostCrossCurrencyTransactions", new StringContent(
                                                    new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;
                    responseDateTime = DateTime.Now;
                    //ResponseMessageViewModel responseApi = new ResponseMessageViewModel();
                    //ResponseMessage responseMsg = null;
                    bool result = false;

                    if (response.IsSuccessStatusCode)
                    {
                        result = response.IsSuccessStatusCode;
                        await response.Content.ReadAsAsync<TransactionPostingViewModel>();

                        var res = new ResponseMessageViewModel
                        {
                            responseCode = responseApi.responseCode,
                            webRequestDate = responseApi.webRequestDate,
                            webRequestStatus = responseApi.webRequestStatus,
                            serialNumber = responseApi.serialNumber,
                            message = responseApi.message
                        };
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = res,
                            APIStatus = result,
                            Message = response
                        };

                    }
                    else
                    {
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = null,
                            APIStatus = result,
                            Message = response
                        };
                    }

                    responseMessage = await response.Content.ReadAsStringAsync();
                    //handler.Dispose();
                    //client.Dispose();
                    return responseMsg;
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
                        APIURL = "api/Transactions/PostCrossCurrencyTransactions",
                        LOGTYPEID = model.FirstOrDefault().operationId,
                        REFERENCENUMBER = model.FirstOrDefault().referenceNumber,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = objData,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMessage,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();

                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);

                    logContext.SaveChanges();
                }

            }

            public async Task<ResponseMessage> ApiTransactionPosting(List<TransactionPostingViewModel> model, bool isCrossCurrency = false)
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;

                HttpClient client = new HttpClient(handler);
                var inputJson = new JavaScriptSerializer().Serialize(model);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                TransactionPostingViewModel responseApi = new TransactionPostingViewModel();
                ResponseMessage responseMsg = null;
                string responseJson = "";

                getAPIURLSettings("PostTransactions");
                string apiUrl = "api/Transactions/PostTransactions";

                try
                {
                    var token = new AuthenticationHeaderValue("Authorization", API_KEY);
                    var dta = context.TBL_SETUP_GLOBAL.ToList();
                    handler.UseDefaultCredentials = true;
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


                    if (isCrossCurrency == true)
                    {
                        apiUrl = "api/Transactions/PostCrossCurrencyTransactions";
                    }

                    response = client.PostAsync(apiUrl, new StringContent(
                                                    new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;

                    //response = client.PostAsync("api/Transactions/PostCrossCurrencyTransactions", new StringContent(
                    //                                 new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;


                    responseDateTime = DateTime.Now;


                    if (response.IsSuccessStatusCode)
                    {

                        responseApi = await response.Content.ReadAsAsync<TransactionPostingViewModel>();

                        var res = new ResponseMessageViewModel
                        {
                            responseCode = responseApi.responseCode,
                            webRequestDate = responseApi.webRequestDate,
                            webRequestStatus = responseApi.webRequestStatus,

                        };
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = res,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response
                        };
                    }
                    else
                    {
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = null,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response
                        };
                    }

                    responseJson = await response.Content.ReadAsStringAsync();

                    responseMsg.responseMessage = responseJson;
                    //handler.Dispose();
                    //client.Dispose();

                    return responseMsg;
                }
                catch (Exception ex)
                {
                    var innerExceptionMessage = "";
                    if (ex.InnerException != null)
                        innerExceptionMessage = ex.InnerException.Message;
                    //if (responseJson == string.Empty) responseJson = innerExceptionMessage;

                    throw new APIErrorException($"Core Banking API Error - {ex.Message} - inner exception - {innerExceptionMessage}");
                }

                finally
                {
                    handler.Dispose();
                    client.Dispose();

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = API_URL + apiUrl,
                        LOGTYPEID = model.FirstOrDefault().operationId,
                        REFERENCENUMBER = model.FirstOrDefault().sourceReferenceNumber,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = inputJson,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseJson,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();

                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);

                    logContext.SaveChanges();
                }


                //context.SaveChanges();
            }

            public async Task<ResponseMessage> APIProcessLien(CasaLienViewModel model, string lienType)
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;
                LienProcessViewModel responseModel = new LienProcessViewModel();
                bool output = false;
                HttpClient client = new HttpClient(handler);
                //var objData = new JavaScriptSerializer().Serialize(model);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                //TransactionPostingViewModel responseApi = new TransactionPostingViewModel();
                ResponseMessage responseMsg = null;
                //string responseMessage = "";
                string responseJson = "";
                string serialiseModel = "";
                getAPIURLSettings("ProcessLien");

                try
                {
                    var currencyCode = model.currencyCode;

                    if (model.isTermDeposit == false)
                    {
                        currencyCode = context.TBL_CASA.Where(x =>
                                x.PRODUCTACCOUNTNUMBER == model.productAccountNumber && x.COMPANYID == model.companyId)
                            .Select(x => x.TBL_CURRENCY.CURRENCYCODE).FirstOrDefault();
                    }

                    LienAPIProcessViewModel apiModel = new LienAPIProcessViewModel
                    {
                        account = model.productAccountNumber,
                        lienProcessType = lienType, //"PLACE" or LIFTLIEN
                        lienReasonCode = "VIA",
                        lienReason = model.description,
                        lienAmount = String.Format("{0:0.00}", model.lienAmount), //model.lienAmount,  //
                        lienAccountCurrency = currencyCode,
                        lienUniqueReferenceNumber = model.lienReferenceNumber,
                    };

                    var token = new AuthenticationHeaderValue("Authorization", API_KEY);

                    handler.UseDefaultCredentials = true;
                    //HttpClient client = new HttpClient(handler);

                    httpClientInstance = new HttpClient();
                    httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                    client.Timeout = TimeSpan.FromSeconds(180);
                    client.BaseAddress = new Uri(API_URL);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = token;

                    serialiseModel = new JavaScriptSerializer().Serialize(apiModel);

                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                    requestDatetime = DateTime.Now;
                    response = client.PostAsync("api/Lien/ProcessLien", new StringContent(serialiseModel
                        , Encoding.UTF8, "application/json")).Result;
                    responseDateTime = DateTime.Now;
                    //if (response.IsSuccessStatusCode)
                    //{
                    //    responseModel = await response.Content.ReadAsAsync<LienProcessViewModel>();
                    //}

                    //ResponseViewModel responseAPI = new ResponseViewModel();
                    //responseAPI.responseCode = responseModel.responseCode;
                    //responseAPI.webRequestDate = responseModel.webRequestDate;
                    //responseAPI.webRequestStatus = responseModel.webRequestStatus;
                    //responseAPI.referenceNumber = responseModel.referenceNumber;
                    //responseMessage = await response.Content.ReadAsStringAsync();

                    //handler.Dispose();
                    //client.Dispose();

                    //if (responseModel.responseCode == "0")
                    //{
                    //    output = true;
                    //}
                    //else
                    //{
                    //    output = false;
                    //}

                    if (response.IsSuccessStatusCode)
                    {

                        responseModel = await response.Content.ReadAsAsync<LienProcessViewModel>();

                        var res = new ResponseMessageViewModel
                        {
                            responseCode = responseModel.responseCode,
                            webRequestDate = responseModel.webRequestDate,
                            webRequestStatus = responseModel.webRequestStatus,

                        };
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = res,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response
                        };
                    }
                    else
                    {
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = null,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response
                        };
                    }

                    responseJson = await response.Content.ReadAsStringAsync();

                    return responseMsg;
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
                        APIURL = "api/Lien/ProcessLien",
                        LOGTYPEID = 2,
                        REFERENCENUMBER = model.sourceReferenceNumber,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = serialiseModel, //objData,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseJson,
                    };
                    FinTrakBankingContext logContext = new FinTrakBankingContext();

                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);

                    logContext.SaveChanges();
                }


            }
            public async Task<ResponseMessage> APIPostInterestRate(InterestRateInquiryViewModel model, string accountType)
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;
                InterestRateInquiryViewModel responseModel = new InterestRateInquiryViewModel();
                bool output = false;
                HttpClient client = new HttpClient(handler);
                //var objData = new JavaScriptSerializer().Serialize(model);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                ResponseMessage responseMsg = null;
                string responseMessage = "";
                string serialiseModel = "";

                try
                {
                    InterestRateDetails apiModel = new InterestRateDetails
                    {
                        accountNumber = model.accountNumber,
                        accountType = accountType,
                        interestTableCode = model.interestTableCode,
                        startDate = model.startDate,
                        endDate = model.endDate,
                        interestRateAmount = model.interestRateAmount,

                    };

                    var token = new AuthenticationHeaderValue("Authorization", API_KEY);

                    handler.UseDefaultCredentials = true;

                    httpClientInstance = new HttpClient();
                    httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                    client.Timeout = TimeSpan.FromSeconds(180);
                    client.BaseAddress = new Uri(API_URL);
                    client.DefaultRequestHeaders.Accept.Clear();

                    client.DefaultRequestHeaders.Authorization = token;

                    serialiseModel = new JavaScriptSerializer().Serialize(apiModel);

                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                    requestDatetime = DateTime.Now;
                    response = client.PostAsync("api/InterestRateInquiry/PostInterestRate", new StringContent(serialiseModel,
                                                 Encoding.UTF8, "application/json")).Result;
                    responseDateTime = DateTime.Now;

                    if (response.IsSuccessStatusCode)
                    {

                        responseModel = await response.Content.ReadAsAsync<InterestRateInquiryViewModel>();

                        var res = new ResponseMessageViewModel
                        {
                            responseCode = responseModel.responseCode,
                            webRequestDate = responseModel.webRequestDate,
                            webRequestStatus = responseModel.webRequestStatus,

                        };
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = res,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response
                        };
                    }
                    else
                    {
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = null,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response
                        };
                    }

                    return responseMsg;
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
                        APIURL = "api/InterestRateInquiry/PostInterestRate",
                        LOGTYPEID = 19,
                        REFERENCENUMBER = model.accountNumber,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = serialiseModel, //objData,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseModel.webRequestStatus,
                    };
                    FinTrakBankingContext logContext = new FinTrakBankingContext();

                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);

                    logContext.SaveChanges();
                }


            }


            #region FLEXCUBE POSTING INTEGRATIONS

            public async Task<ResponseMessage> ApiTransactionFacilityCreationPosting(FlexcubeCreateFacilityViewModel model, short loanSystemTypeId)
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;

                HttpClient client = new HttpClient(handler);
                var inputJson = new JavaScriptSerializer().Serialize(model);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                ResponseMessageFacilityViewModel responseApi = new ResponseMessageFacilityViewModel();
                ResponseMessage responseMsg = null;
                string responseJson = "";
                getAPIURLSettings("FacilityCreation");
                string apiUrl = "FCUBSCreateFacility";
                try
                {
                    var token = new AuthenticationHeaderValue("Authorization", API_KEY);
                    var dta = context.TBL_SETUP_GLOBAL.ToList();
                    handler.UseDefaultCredentials = true;
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

                    //model.p_account_no = "0739938402";
                    response = client.PostAsync(apiUrl, new StringContent(
                                                    new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;
                    responseJson = await response.Content.ReadAsStringAsync();
                    responseDateTime = DateTime.Now;

                    if (response.IsSuccessStatusCode)
                    {
                        responseApi = await response.Content.ReadAsAsync<ResponseMessageFacilityViewModel>();

                        var res = new ResponseMessageViewModel
                        {
                            responseCode = responseApi.response_code,
                            message = responseApi.response_message,
                            serialNumber = responseApi.bo_code,
                            webRequestDate = DateTime.Now,
                            webRequestStatus = responseApi.bo_message,
                            responseStatus = responseApi.response_code == "00" ? true : false,
                        };

                        responseMsg = new ResponseMessage
                        {
                            APIResponse = res,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response
                        };
                    }
                    else
                    {
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = null,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response,
                            responseMessage = responseJson
                        };
                    }

                    return responseMsg;
                }
                catch (Exception ex)
                {
                    var innerExceptionMessage = "";
                    if (ex.InnerException != null)
                        innerExceptionMessage = ex.InnerException.Message;
                    //if (responseJson == string.Empty) responseJson = innerExceptionMessage;

                    throw new APIErrorException($"Core Banking API Error - {ex.Message} - inner exception - {innerExceptionMessage}");
                }

                finally
                {
                    handler.Dispose();
                    client.Dispose();

                    var loanMapping = new TBL_THIRDPARTY_LOAN_MAPPING
                    {
                        LOANAPPLICATIONID = model.loanApplicationId,
                        LOANSYSTEMTYPEID = loanSystemTypeId,
                        FACILITYMAPPINGID = responseApi.facility_id,
                        BOOKINGCODE = responseApi.bo_code,
                    };

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = API_URL + apiUrl,
                        LOGTYPEID = 2,
                        REFERENCENUMBER = model.sourceReferenceNumber,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = inputJson,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseJson,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();

                    logContext.TBL_THIRDPARTY_LOAN_MAPPING.Add(loanMapping);
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }


                //context.SaveChanges();
            }

            public async Task<ResponseMessage> ApiTransactionLoanCreationPosting(FlexcubeCreateLoanAccountViewModel model, short loanSystemTypeId)
            {

                IRestResponse response = null;
                DateTime requestDatetime = DateTime.Now, responseDateTime = new DateTime();
                ResponseMessage responseMsg = null;
                RestRequest req = new RestRequest(Method.POST);
                FlexcubeCreateLoanAccountViewModel responseApi = new FlexcubeCreateLoanAccountViewModel();
                List<FlexcubeCreateLoanAccountViewModel> rec = new List<FlexcubeCreateLoanAccountViewModel>();
                FlexcubeCreateLoanAccountViewModel reqbody = null;
                //Temporarily generate random numbers to serve as reference numbers
                var referenceNumber = CommonHelpers.GenerateRandomDigitCode(10);


                HttpClientHandler handler = new HttpClientHandler();
                //HttpClient httpClientInstance;
                string inputJson = "";
                //HttpClient client = new HttpClient(handler);
               // DateTime requestDatetime = DateTime.Now, responseDateTime = new DateTime();
                //HttpResponseMessage response = null;
                //ResponseMessageLoanCreationViewModel responseApi = new ResponseMessageLoanCreationViewModel();
                //ResponseMessage responseMsg = null;
                string responseJson = "";

                getAPIURLSettings("CreateLoan");
                var baseURL = API_URL;
                string fullURL = baseURL + "FCUBSCreateLoanAccount";
                RestClient client = new RestClient(fullURL);

                string channelCode = "";
                string appUserId = "";
                var sourceRecord = context.TBL_LOAN.Where(x => x.LOANREFERENCENUMBER == model.sourceReferenceNumber).FirstOrDefault();
                var appNo = context.TBL_API_URL.Where(x => x.TYPENAME == "CreateLoan").FirstOrDefault().USERID;
                var contryCode = context.TBL_COMPANY.FirstOrDefault().COUNTRYCODE;

                channelCode = "FINTRAK";
                if (contryCode == "MZN")
                {
                    appUserId = "FINTRAKUSR";
                }
                else if(contryCode == "KES" || contryCode == "GHS" || contryCode == "ZMW")
                {
                    appUserId = "FINTRAK";
                }
                //else if (contryCode == "GHS")
                //{
                //    appUserId = "FINTRAK";
                //}
                //else if (contryCode == "ZMW")
                //{
                //    appUserId = "FINTRAK";
                //}

                try
                {
                    if (contryCode == "MZN")
                    {
                        reqbody = new FlexcubeCreateLoanAccountViewModel()
                        {
                            channel_code = channelCode,
                            user_refno = referenceNumber,//model.account_no,
                            product_code = model.product_code,
                            account_no = "00101030000032",//model.account_no,
                            maturity_date = model.maturity_date,
                            amt_financed = "100000",//model.amount_financed,
                            maker_id = "FINTRAKUSR",//model.maker_id,
                            checker_id = "FINTRAKUSR",//model.checker_id,
                            udevals = new List<UdeVal>
                            {
                                new UdeVal { ude_id = "OVD_INT", ude_value = "2", resolved_value = "2" },
                                new UdeVal { ude_id = "INTEREST_RATE", ude_value = "4", resolved_value = "2" },
                                new UdeVal { ude_id = "ARGFEE_STD_RATE", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "FLAT_INTEREST_RATE", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "PNL_INT", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "ARGF_MZN", ude_value = "2", resolved_value = "2" },
                                new UdeVal { ude_id = "DISB_FEE_STD", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "DISB_FEE_STD1", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "DISB_FEE_STD2", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "HANDLING_FEE", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "HAND_STDY", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "INT_STMP_DTY", ude_value = "0.5", resolved_value = "0.5" },
                                new UdeVal { ude_id = "LOSS_POOL_RATE", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "MIN_MGMT_FEES", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "ODIN_STDUTY", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "ODPR_STDUTY", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "PREPAYMENT_FEE_RATE", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "YR_INSURANCE", ude_value = "0", resolved_value = "0" }
                            }
                        };
                    }
                    else if (contryCode == "GHS")
                    {
                        reqbody = new FlexcubeCreateLoanAccountViewModel()
                        {
                            channel_code = channelCode,
                            user_refno = referenceNumber,//model.account_no,
                            product_code = model.product_code,
                            account_no = model.account_no,
                            maturity_date = model.maturity_date,
                            amt_financed = "100000",//model.amount_financed,
                            maker_id = "FINTRAKUSR",//model.maker_id,
                            checker_id = "FINTRAKUSR",//model.checker_id,
                            udevals = new List<UdeVal>
                            {
                                new UdeVal { ude_id = "ACC_INT1", ude_value = "2", resolved_value = "2" },
                                new UdeVal { ude_id = "INTEREST_RATE", ude_value = "2.5", resolved_value = "2.5" },
                                new UdeVal { ude_id = "IN_OD_CHRG", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "MGT_FEE1", ude_value = "5", resolved_value = "5" },
                                new UdeVal { ude_id = "PENAL_CHARG", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "PEN_ACQ1", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "PRN_OD_CHRG", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "PROCS_FEE1", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "PROTECT_FEE1", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "TAXRATE", ude_value = "0", resolved_value = "0" }
                                
                            }
                        };
                    }
                    else if (contryCode == "ZMW")
                    {
                        reqbody = new FlexcubeCreateLoanAccountViewModel()
                        {
                            channel_code = channelCode,
                            user_refno = referenceNumber,//model.account_no,
                            product_code = model.product_code,
                            account_no = "00101030000032",//model.account_no,
                            maturity_date = model.maturity_date,
                            amt_financed = "100000",//model.amount_financed,
                            maker_id = "FINTRAKUSR",//model.maker_id,
                            checker_id = "FINTRAKUSR",//model.checker_id,
                            udevals = new List<UdeVal>
                            {
                                new UdeVal { ude_id = "ADVSRY_FEE", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "APPRSL_FEE", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "COMMIT_FEE1", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "IN_OD_CHRG", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "INS_HOME1", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "INS_HOME1", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "INTEREST_RATE", ude_value = "13", resolved_value = "13" },
                                new UdeVal { ude_id = "MGT_FEE1", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "PENAL_CHARG", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "PRN_OD_CHRG", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "PROCS_FEE1", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "PROPERTY_VAL", ude_value = "0.5", resolved_value = "0.5" },
                                new UdeVal { ude_id = "RENANN_FEE", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "RESTRU_FEE", ude_value = "0", resolved_value = "0" },
                                new UdeVal { ude_id = "TAXRATE", ude_value = "0", resolved_value = "0" }
                            }
                        };
                    }

                    //var token = new AuthenticationHeaderValue("Authorization", API_KEY);
                    //var dta = context.TBL_SETUP_GLOBAL.ToList();
                    //handler.UseDefaultCredentials = true;
                    //httpClientInstance = new HttpClient();
                    //httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                    //client.Timeout = TimeSpan.FromSeconds(180);
                    //client.DefaultRequestHeaders.Authorization = token;
                    //client.BaseAddress = new Uri(API_URL);
                    //client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(
                    //new MediaTypeWithQualityHeaderValue("application/json"));

                    //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    requestDatetime = DateTime.Now;
                    //response = client.PostAsync(apiUrl, new StringContent(new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;
                    //responseJson = await response.Content.ReadAsStringAsync();
                    //inputJson = new JavaScriptSerializer().Serialize(model);

                    var jsonbody = new JavaScriptSerializer().Serialize(reqbody);
                    req.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
                    req.AddHeader("Content-Type", "application/json");
                    req.AddHeader("Accept", "application/json");
                    req.AddHeader("Authorization", API_KEY);

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    response = await client.ExecuteAsync<FlexcubeCreateLoanAccountViewModel>(req);
                    var responbody = JsonConvert.DeserializeObject<FlexcubeCreateLoanAccountViewModel>(response.Content);

                    responseDateTime = DateTime.Now;

                    if (response.IsSuccessful)
                    {
                        //responseApi = await response.Content.ReadAsAsync<ResponseMessageLoanCreationViewModel>();

                        //string responseData = await response.Content.ReadAsStringAsync();
                        //var responseDataArray = responseData.Split(new[] { "</NS1:CREATEACCOUNT_FSFS_REQ>" }, StringSplitOptions.None);

                        if (responbody.response_code == "99") 
                        {
                            var resp = new ResponseMessageViewModel
                            {
                                //message = responseApi.response_desc,
                                message = responbody.bo_message != null ? responbody.bo_message : responbody.response_desc,
                                responseCode = responbody.response_code,
                                responseStatus = responbody.response_code == "00" ? true : false,
                                responseDesc = responbody.response_desc,
                                responseBoCode = responbody.bo_code,
                                //APIMessage = response,
                                webRequestDate = DateTime.Now,
                                //webRequestStatus = responseApi.webRequestStatus,
                            };
                            responseMsg = new ResponseMessage
                            {
                                APIResponse = resp,
                                APIStatus = response.IsSuccessful,
                                //Message = response
                            };

                            return responseMsg;
                        }

                        if(responbody.response_code == "34")
                        {
                            var resp = new ResponseMessageViewModel
                            {
                                //message = responseApi.response_desc,
                                message = responbody.bo_message != null ? responbody.bo_message : responbody.response_message,
                                responseCode = responbody.response_code,
                                responseStatus = responbody.response_code == "00" ? true : false,
                                //responseDesc = responbody.response_desc,
                                //responseBoCode = responbody.bo_code,
                                //APIMessage = response,
                                webRequestDate = DateTime.Now,
                                //webRequestStatus = responseApi.webRequestStatus,
                            };
                            responseMsg = new ResponseMessage
                            {
                                APIResponse = resp,
                                APIStatus = response.IsSuccessful,
                                //Message = response
                            };

                            return responseMsg;
                        }
                        if (responbody == null || !responbody.response_desc.ToLower().Contains("successful"))
                        {
                            throw new APIErrorException("API call error - " + responbody.response_message + " " + responbody.response_code + " " + DateTime.Now);
                        }

                        
                       // responseApi = JsonConvert.DeserializeObject<ResponseMessageLoanCreationViewModel>(responseData);

                        var res = new ResponseMessageViewModel
                        {
                            //message = responseApi.response_desc,
                            message = responbody.bo_message != null ? responbody.bo_message :   responbody.response_desc,
                            responseCode = responbody.response_code,
                            responseStatus = responbody.response_code == "00" ? true : false,
                            responseDesc = responbody.response_desc,
                            responseBoCode = responbody.bo_code,
                            //APIMessage = response,
                            webRequestDate = DateTime.Now,
                            //webRequestStatus = responseApi.webRequestStatus,
                        };

                        responseMsg = new ResponseMessage
                        {
                            APIResponse = res,
                            APIStatus = response.IsSuccessful,
                            Msg = response.Content,
                            //Msg = new JavaScriptSerializer().Serialize(responbody)
                        };
                    }
                    else
                    {
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = null,
                            APIStatus = response.IsSuccessful,
                            //Message = response
                        };
                    }

                    //responseMsg.APIResponse.message = responseJson;
                    //handler.Dispose();
                    //client.Dispose();

                    return responseMsg;
                }
                catch (Exception ex)
                {
                    var innerExceptionMessage = "";
                    if (ex.InnerException != null)
                        innerExceptionMessage = ex.InnerException.Message;
                    //if (responseJson == string.Empty) responseJson = innerExceptionMessage;

                    throw new APIErrorException($"Core Banking API Error - {ex.Message} - inner exception - {innerExceptionMessage}");
                }

                finally
                {
                    handler.Dispose();
                    //client.Dispose();
                    responseJson = responseMsg.Msg; 
                    var loanMapping = new TBL_THIRDPARTY_LOAN_MAPPING
                    {
                        LOANAPPLICATIONID = model.loanApplicationId,
                        LOANSYSTEMTYPEID = loanSystemTypeId,
                        FACILITYMAPPINGID = responseApi.reference_no,
                        BOOKINGCODE = responseApi.bo_code,
                    };


                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = fullURL,
                        LOGTYPEID = 2,
                        REFERENCENUMBER = model.sourceReferenceNumber,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = new JavaScriptSerializer().Serialize(reqbody),
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseJson,
                    };



                    FinTrakBankingContext logContext = new FinTrakBankingContext();

                    //var sourceRecord = logContext.TBL_LOAN.Where(x => x.LOANREFERENCENUMBER == model.sourceReferenceNumber).FirstOrDefault()
                    if (sourceRecord != null)
                    {
                        sourceRecord.COREBANKINGREF = model.sourceReferenceNumber;
                    }

                    logContext.TBL_THIRDPARTY_LOAN_MAPPING.Add(loanMapping);
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();

                    context.SaveChanges();
                }
          
            }

            //CRMSCodeGeneration
            public async Task<ResponseMessage> ApiFetchCBMCRMSCode(CRMSCodeGeneration model, short loanSystemTypeId)
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;

                HttpClient client = new HttpClient(handler);
                var inputJson = new JavaScriptSerializer().Serialize(model);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                ResponseMessageCRMSCodeViewModel responseApi = new ResponseMessageCRMSCodeViewModel();
                ResponseMessage responseMsg = null;
                string responseJson = "";
                getAPIURLSettings("crmsCode"); //Check TBL_API_URL
                string apiUrl = "submitReturnV2";
                try
                {
                    var token = new AuthenticationHeaderValue("Authorization", API_KEY);
                    var dta = context.TBL_SETUP_GLOBAL.ToList();
                    handler.UseDefaultCredentials = true;
                    httpClientInstance = new HttpClient();
                    httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                    client.Timeout = TimeSpan.FromSeconds(1800);
                    client.DefaultRequestHeaders.Authorization = token;
                    client.BaseAddress = new Uri(API_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    requestDatetime = DateTime.Now;

                    response = client.PostAsync(apiUrl, new StringContent(
                                                    new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;
                    responseJson = await response.Content.ReadAsStringAsync();
                    responseDateTime = DateTime.Now;

                    if (response.IsSuccessStatusCode)
                    {
                        responseApi = await response.Content.ReadAsAsync<ResponseMessageCRMSCodeViewModel>();

                        var res = new ResponseMessageViewModel
                        {
                            //responseCode = responseApi.response_code,
                            message = responseApi.submit_return,
                            //serialNumber = responseApi.bo_code,
                            webRequestDate = DateTime.Now,
                            //webRequestStatus = responseApi.bo_message,
                            responseStatus = responseApi.submit_return.ToLower().Contains("successful") == true ? true : false,
                        };

                        responseMsg = new ResponseMessage
                        {
                            APIResponse = res,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response
                        };
                    }
                    else
                    {
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = null,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response,
                            responseMessage = responseApi.submit_return
                        };
                    }

                    return responseMsg;
                }
                catch (Exception ex)
                {
                    var innerExceptionMessage = "";
                    if (ex.InnerException != null)
                        innerExceptionMessage = ex.InnerException.Message;
                    //if (responseJson == string.Empty) responseJson = innerExceptionMessage;

                    throw new APIErrorException($"Core Banking API Error - {ex.Message} - inner exception - {innerExceptionMessage}");
                }

                finally
                {
                    handler.Dispose();
                    client.Dispose();

                    var loanMapping = new TBL_THIRDPARTY_LOAN_MAPPING
                    {
                        LOANAPPLICATIONID = model.loanApplicationDetailId,
                        LOANSYSTEMTYPEID = loanSystemTypeId,
                        FACILITYMAPPINGID = null,
                        BOOKINGCODE = null,
                    };

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = API_URL + apiUrl,
                        LOGTYPEID = 2,
                        REFERENCENUMBER = model.sourceReferenceNumber,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = inputJson,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseJson,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();

                    logContext.TBL_THIRDPARTY_LOAN_MAPPING.Add(loanMapping);
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }


                //context.SaveChanges();
            }

            // CreditCheck
            public async Task<ResponseMessage> FlexcubeCreditCheck(CreditCheckViewModel model)
            {
                var API_URL = "http://10.111.13.47:7002/crms/v1/";
                HttpClientHandler _handler = new HttpClientHandler();

                _handler.UseDefaultCredentials = true;
                HttpClient client = new HttpClient(_handler);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                // HttpClient client = new HttpClient(_handler);
                var inputJson = new JavaScriptSerializer().Serialize(model);
                ResponseMessageCreditCheckViewModel responseAPI = new ResponseMessageCreditCheckViewModel();
                //string responseAPI = "";
                //DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                ResponseMessage responseMsg = null;
                string responseJson = "";
                //getAPIURLSettings("CreditCheck");
                string apiUrl = "creditCheck";

                try
                {
                    var token = new AuthenticationHeaderValue("Authorization", API_KEY);

                    client = new HttpClient();
                    client.DefaultRequestHeaders.ConnectionClose = false;
                    client.Timeout = TimeSpan.FromSeconds(180);
                    client.DefaultRequestHeaders.Authorization = token;
                    client.BaseAddress = new Uri(API_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                    requestDatetime = DateTime.Now;

                    response = client.PostAsync(apiUrl, new StringContent(
                                                new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;
                    responseJson = await response.Content.ReadAsStringAsync();

                    responseDateTime = DateTime.Now;
                    responseMsg = null;

                    if (response.IsSuccessStatusCode && responseJson.Contains("\"creditCheck\":"))
                    {
                        responseAPI = await response.Content.ReadAsAsync<ResponseMessageCreditCheckViewModel>();

                        var data = responseAPI.creditCheck;
                        //var data = "<CreditCheck><Credit><CRMSRefNumber>00082/20080104/89141</CRMSRefNumber><CreditType>Advances/ Overdraft</CreditType><CreditLimit>5000</CreditLimit><OutstandingAmount>0</OutstandingAmount><EffectiveDate>13-11-2007</EffectiveDate><Tenor>null</Tenor><ExpiryDate>01-01-1900</ExpiryDate><GrantingInstitution>Keystone Bank Limited</GrantingInstitution><PerformanceStatus>GOOD</PerformanceStatus></Credit><Credit><CRMSRefNumber>00011/20151211/433391</CRMSRefNumber><CreditType>Advances/ Overdraft</CreditType><CreditLimit>0</CreditLimit><OutstandingAmount>11903.08</OutstandingAmount><EffectiveDate>31-12-1999</EffectiveDate><Tenor>null</Tenor><ExpiryDate>04-04-2015</ExpiryDate><GrantingInstitution>First Bank Plc</GrantingInstitution><PerformanceStatus>BAD</PerformanceStatus></Credit><Credit><CRMSRefNumber>00011/20161007/560963</CRMSRefNumber><CreditType>Fixed Term Loan</CreditType><CreditLimit>9036744.86</CreditLimit><OutstandingAmount>4.06</OutstandingAmount><EffectiveDate>29-09-2016</EffectiveDate><Tenor>null</Tenor><ExpiryDate>28-04-2018</ExpiryDate><GrantingInstitution>First Bank Plc</GrantingInstitution><PerformanceStatus>BAD</PerformanceStatus></Credit><Summary>Total Number of Credits: 3 | Total Number of Performing Credits: 1 | Total Number of Non-Performing Credits: 2</Summary></CreditCheck>";

                        var dataArray = Encoding.ASCII.GetBytes(data);
                        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(CRMSCreditCheckViewModel));
                        StringReader rdr = new StringReader(data);
                        CRMSCreditCheckViewModel responseObject = (CRMSCreditCheckViewModel)serializer.Deserialize(rdr);

                        responseMsg = new ResponseMessage
                        {
                            //APIResponse = specificRes,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response,
                            responseMessage = responseJson,
                            responseObject = responseObject,

                        };
                    }
                    else
                    {
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = null,
                            APIStatus = false, //response.IsSuccessStatusCode,
                            Message = response,
                            responseMessage = responseJson
                        };
                    }
                    _handler.Dispose();
                    client.Dispose();
                    return responseMsg;
                }
                catch (Exception ex)
                {
                    var innerExceptionMessage = "";
                    if (ex.InnerException != null)
                        innerExceptionMessage = ex.InnerException.Message;

                    throw new APIErrorException($"Core Banking API Error - Kindly Contact the System Administrator!");
                    //throw new APIErrorException($"Core Banking API Error - {ex.Message} - inner exception - {innerExceptionMessage}");
                }
                finally
                {
                    _handler.Dispose();
                    client.Dispose();

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = API_URL + apiUrl,
                        LOGTYPEID = 11,
                        // REFERENCENUMBER = model.sanctionReferenceNumber,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = inputJson,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseJson,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();

                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }
            }


            #endregion END OF  FLEXCUBE POSTING INTEGRATIONS

            public async Task<ResponseMessage> ApiOfferLetterPosting(CFLNotifyResponse model, string refNumber)
            {

                IRestResponse response = null;
                DateTime requestDatetime = DateTime.Now, responseDateTime = new DateTime();
                ResponseMessage responseMsg = null;
                RestRequest req = new RestRequest(Method.POST);
                CFLNotifyResponse responseApi = new CFLNotifyResponse();
                CFLNotifyResponse reqbody = null;
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;
                var inputJson = new JavaScriptSerializer().Serialize(model);
                string responseJson = "";
                getAPIURLSettings("CASHFLOW");

                var baseURL = API_URL;
                string fullURL = baseURL + "lendingofficer/cflnotifystatus";
                RestClient client = new RestClient(fullURL);

                var reasonForRejection = "";
                if(model.statusCode == "05")
                {
                    reasonForRejection = model.comment;
                    model.workflowStage = "Rejected";
                }

                reqbody = new CFLNotifyResponse()
                {
                    statusCode =  model.statusCode,
                    requestId = model.requestId,
                    workflowStage = model.workflowStage,
                    isInsurance = model.isInsurance,
                    reasonForRejection = reasonForRejection,  
                    actionByName = model.actionByName,
                    comment = model.comment,
                    message = model.comment
                };
                inputJson = new JavaScriptSerializer().Serialize(reqbody);
                try
                {

                    var jsonbody = new JavaScriptSerializer().Serialize(reqbody);
                    req.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
                    req.AddHeader("Content-Type", "application/json");
                    req.AddHeader("Accept", "application/json");
                    req.AddHeader("X-API-Key", API_KEY);

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    response = await client.ExecuteAsync<CFLNotifyResponse>(req);
                    var responbody = JsonConvert.DeserializeObject<CFLNotifyResponse>(response.Content);

                    responseDateTime = DateTime.Now;

                   

                    if (response.IsSuccessful)
                    {

                        responseApi = responbody;

                        if(responbody.ResponseCode == "00")
                        {
                            var res = new CFLNotifyResponse
                            {
                                statusCode = responseApi.statusCode,
                                requestId = responseApi.requestId,
                                workflowStage = responseApi.workflowStage,

                            };
                            responseMsg = new ResponseMessage
                            {
                                CFLNotifyResponse = res,
                                APIStatus = response.IsSuccessful,
                                //Message = response.ToString()
                            };
                        }
                        else
                        {
                            throw new APIErrorException("CFL notify status API " + responbody.ResponseCode + " " + responbody.ResponseMessage);
                        }
                        
                    }
                    else
                    {
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = null,
                            APIStatus = response.IsSuccessful,
                            //Message = response.ErrorException
                        };
                    }

                    responseJson = responbody?.ToString();
                    responseMsg.responseMessage = responseJson;
                    if (response.IsSuccessful == false) 
                    { 
                        responseMsg.responseMessage = response.ErrorMessage;
                        //response.
                    }
                    //handler.Dispose();
                    //client.Dispose();

                    return responseMsg;
                }
                catch (Exception ex)
                {
                    var innerExceptionMessage = "";
                    if (ex.InnerException != null)
                        innerExceptionMessage = ex.InnerException.Message;
                    //if (responseJson == string.Empty) responseJson = innerExceptionMessage;

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = fullURL,
                        LOGTYPEID = 14,
                        REFERENCENUMBER = refNumber,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = inputJson,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMsg?.responseMessage + " " + new JavaScriptSerializer().Serialize(response),
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();

                    throw new APIErrorException($"Core Banking API Error - {ex.Message} - inner exception - {innerExceptionMessage}");
                }

                finally
                {
                    var serializedObject = JsonConvert.SerializeObject(response, Formatting.Indented);

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = fullURL,
                        LOGTYPEID = 14,
                        REFERENCENUMBER = refNumber,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = inputJson,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMsg?.responseMessage + " " + serializedObject,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }

            }

            public async Task<ResponseMessage> ReferBackThroughAPI(OfferLetterResponse model, string refNumber)
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;

                HttpClient client = new HttpClient(handler);
                var inputJson = new JavaScriptSerializer().Serialize(model);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                OfferLetterResponse responseApi = new OfferLetterResponse();
                ResponseMessage responseMsg = null;
                string responseJson = "";
                getAPIURLSettings("CASHFLOW");
                string apiUrl = "CallBack/refer-back";

                try
                {
                    var token = new AuthenticationHeaderValue("Basic", API_KEY);
                    handler.UseDefaultCredentials = true;
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

                    response = client.PostAsync(apiUrl, new StringContent(
                                                    new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;

                    responseDateTime = DateTime.Now;

                    if (response.IsSuccessStatusCode)
                    {
                        responseApi = await response.Content.ReadAsAsync<OfferLetterResponse>();
                        var res = new OfferLetterResponse
                        {
                            StatusCode = responseApi.StatusCode,
                            RequestId = responseApi.RequestId,
                            WorkflowStage = responseApi.WorkflowStage,

                        };
                        responseMsg = new ResponseMessage
                        {
                            APIOffetResponse = res,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response
                        };
                    }
                    else
                    {
                        responseMsg = new ResponseMessage
                        {
                            APIResponse = null,
                            APIStatus = response.IsSuccessStatusCode,
                            Message = response
                        };
                    }

                    responseJson = await response.Content.ReadAsStringAsync();
                    responseMsg.responseMessage = responseJson;
                    //handler.Dispose();
                    //client.Dispose();

                    return responseMsg;
                }
                catch (Exception ex)
                {
                    var innerExceptionMessage = "";
                    if (ex.InnerException != null)
                        innerExceptionMessage = ex.InnerException.Message;
                    //if (responseJson == string.Empty) responseJson = innerExceptionMessage;
                    throw new APIErrorException($"Core Banking API Error - {ex.Message} - inner exception - {innerExceptionMessage}");
                }

                finally
                {
                    handler.Dispose();
                    client.Dispose();

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = API_URL + apiUrl,
                        LOGTYPEID = 14,
                        REFERENCENUMBER = refNumber,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = inputJson,
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseJson,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }

            }

            public async Task<ApprovalPostingResult> ApprovalPostingToHeadOffice(HeadOfficeFacilityApprovalViewModel model)
            {
                 HttpClientHandler handler = new HttpClientHandler();
                 HttpClient httpClientInstance;
                ApprovalPostingResult responseMsg = null;
                 HttpClient client = new HttpClient(handler);
                 DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                 HttpResponseMessage response = null;
                 string responseMessage = "";

                 try
                 {
                     getAPIURLSettings("ApprovalPosting");
                     handler.UseDefaultCredentials = true;
                     httpClientInstance = new HttpClient();
                     client.Timeout = TimeSpan.FromSeconds(180);
                     client.BaseAddress = new Uri(API_URL);
                     client.DefaultRequestHeaders.Accept.Clear();
                     client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                     client.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");
                     client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", API_KEY);
                     httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                     ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                     requestDatetime = DateTime.Now;
                     responseDateTime = DateTime.Now;

                    response = await  client.PostAsync("subsidiary/subsidiary-loan-approval-inputs", new StringContent(new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));

                     if (response.IsSuccessStatusCode)
                     {
                         var rep = await response.Content.ReadAsAsync<ApprovalPostingResult>();

                         if (!rep.responseMessage.ToLower().Contains("success"))
                         {
                             throw new APIErrorException("API call error - " + rep.responseMessage + " " + rep.responseCode + " " + DateTime.Now);
                         }

                          responseMsg = new ApprovalPostingResult
                         {
                             responseMessage = rep.responseMessage,
                             responseCode = rep.responseCode
                         };

                       // return responseMsg;
                    }
                    else
                    {
                        throw new APIErrorException("API call error - " + response.ReasonPhrase + " " + response.StatusCode + " " + DateTime.Now);
                    }

                    return responseMsg;

                }
                 catch (APIErrorException ex)
                 {
                     throw new APIErrorException(ex.Message);
                 }
                 catch (Exception ex)
                 {
                     throw new APIErrorException($"Error" + ex.Message);
                 }
                 finally
                 {
                     handler.Dispose();
                     client.Dispose();



                     var logs = new TBL_CUSTOM_API_LOGS
                     {
                         APIURL = $"{API_URL}subsidiary/subsidiary-loan-approval-inputs",
                         LOGTYPEID = 3,
                         REFERENCENUMBER = model.loanApplicationTypeName,
                         REQUESTDATETIME = requestDatetime,
                         REQUESTMESSAGE = new JavaScriptSerializer().Serialize(model),
                         RESPONSEDATETIME = responseDateTime,
                         RESPONSEMESSAGE = responseMessage,
                     };

                     FinTrakBankingContext logContext = new FinTrakBankingContext();
                     logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                     logContext.SaveChanges();
                 }
            }

            public async Task<ApprovalPostingResult> LmsApprovalPostingToHeadOffice(HeadOfficeFacilityApprovalViewModel model)
            {
                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;
                ApprovalPostingResult responseMsg = null;
                HttpClient client = new HttpClient(handler);
                DateTime requestDatetime = DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                string responseMessage = "";

                try
                {
                    getAPIURLSettings("ApprovalPosting");
                    handler.UseDefaultCredentials = true;
                    httpClientInstance = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(180);
                    client.BaseAddress = new Uri(API_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", API_KEY);
                    httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    requestDatetime = DateTime.Now;
                    responseDateTime = DateTime.Now;

                    response = await client.PostAsync("subsidiary/subsidiary-lms-approval-inputs", new StringContent(new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        var rep = await response.Content.ReadAsAsync<ApprovalPostingResult>();

                        if (!rep.responseMessage.ToLower().Contains("success"))
                        {
                            throw new APIErrorException("API call error - " + rep.responseMessage + " " + rep.responseCode + " " + DateTime.Now);
                        }

                        responseMsg = new ApprovalPostingResult
                        {
                            responseMessage = rep.responseMessage,
                            responseCode = rep.responseCode
                        };

                        // return responseMsg;
                    }
                    else
                    {
                        throw new APIErrorException("API call error - " + response.ReasonPhrase + " " + response.StatusCode + " " + DateTime.Now);
                    }

                    return responseMsg;

                }
                catch (APIErrorException ex)
                {
                    throw new APIErrorException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new APIErrorException($"Error" + ex.Message);
                }
                finally
                {
                    handler.Dispose();
                    client.Dispose();



                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = $"{API_URL}subsidiary/subsidiary-loan-approval-inputs",
                        LOGTYPEID = 3,
                        REFERENCENUMBER = model.loanApplicationTypeName,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = new JavaScriptSerializer().Serialize(model),
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMessage,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }
            }
            public async Task<List<InternationalCustomerViewModel>> GlobalCustomerSearchKenya(SearchInternationalCustomerViewModel model)
            {

                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;
                ResponseMessage responseMsg = null;
                HttpClient client = new HttpClient(handler);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                string responseMessage = "";

                try
                {
                    handler.UseDefaultCredentials = true;
                    getAPIURLSettings("ApprovalPosting");
                    httpClientInstance = new HttpClient();
                    //var token = new AuthenticationHeaderValue("Basic", API_KEY);
                    httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                    client.Timeout = TimeSpan.FromSeconds(180);
                    client.BaseAddress = new Uri(API_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Authorization = token;

                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                    requestDatetime = DateTime.Now;
                    responseDateTime = DateTime.Now;
                    List<InternationalCustomerViewModel> records = new List<InternationalCustomerViewModel>();

                    response = client.PostAsync("search-international-customers", new StringContent(
                           new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        JObject responseDataJsonString = JObject.Parse(jsonString);
                        var jsonDataString = responseDataJsonString["data"].ToString();
                        var objData = JsonConvert.DeserializeObject<List<InternationalCustomerViewModel>>(jsonDataString);

                        foreach (var a in objData)
                        {
                            records.Add(new InternationalCustomerViewModel
                            {
                                customerCode = a.customerCode ?? "N/A",
                                dateOfBirth = a.dateOfBirth ?? null,
                                customerId = a.customerId,
                                emailAddress = a.emailAddress ?? "N/A",
                                firstName = a.firstName ?? "N/A",
                                lastName = a.lastName ?? "N/A",
                                placeOfBirth = a.placeOfBirth ?? "N/A",
                                customerBVN = a.customerBVN ?? "N/A",
                                country = a.country ?? "N/A",
                                phone = a.phone ?? "N/A",
                            });
                        }
                    }

                    handler.Dispose();
                    client.Dispose();
                    responseMessage = await response.Content.ReadAsStringAsync();
                    return records;
                }
                catch (APIErrorException ex)
                {
                    throw new APIErrorException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new APIErrorException($"Error" + ex.Message);
                }
                finally
                {
                    handler.Dispose();
                    client.Dispose();

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = $"{API_URL}search-international-customers",
                        LOGTYPEID = 11,
                        REFERENCENUMBER = model.firstNameSearch,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = new JavaScriptSerializer().Serialize(model),
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMessage,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }
            }

            public async Task<List<InternationalCustomerViewModel>> GlobalCustomerSearchMozambique(SearchInternationalCustomerViewModel model)
            {

                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;
                ResponseMessage responseMsg = null;
                HttpClient client = new HttpClient(handler);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                string responseMessage = "";

                try
                {
                    handler.UseDefaultCredentials = true;
                    getAPIURLSettings("ApprovalPosting");
                    httpClientInstance = new HttpClient();
                    //var token = new AuthenticationHeaderValue("Basic", API_KEY);
                    httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                    client.Timeout = TimeSpan.FromSeconds(180);
                    client.BaseAddress = new Uri(API_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Authorization = token;

                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                    requestDatetime = DateTime.Now;
                    responseDateTime = DateTime.Now;
                    List<InternationalCustomerViewModel> records = new List<InternationalCustomerViewModel>();

                    response = client.PostAsync("search-international-customers", new StringContent(
                           new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        JObject responseDataJsonString = JObject.Parse(jsonString);
                        var jsonDataString = responseDataJsonString["data"].ToString();
                        var objData = JsonConvert.DeserializeObject<List<InternationalCustomerViewModel>>(jsonDataString);

                        foreach (var a in objData)
                        {
                            records.Add(new InternationalCustomerViewModel
                            {
                                customerCode = a.customerCode ?? "N/A",
                                dateOfBirth = a.dateOfBirth ?? null,
                                customerId = a.customerId,
                                emailAddress = a.emailAddress ?? "N/A",
                                firstName = a.firstName ?? "N/A",
                                lastName = a.lastName ?? "N/A",
                                placeOfBirth = a.placeOfBirth ?? "N/A",
                                customerBVN = a.customerBVN ?? "N/A",
                                country = a.country ?? "N/A",
                                phone = a.phone ?? "N/A",
                            });
                        }
                    }

                    handler.Dispose();
                    client.Dispose();
                    responseMessage = await response.Content.ReadAsStringAsync();
                    return records;
                }
                catch (APIErrorException ex)
                {
                    throw new APIErrorException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new APIErrorException($"Error" + ex.Message);
                }
                finally
                {
                    handler.Dispose();
                    client.Dispose();

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = $"{API_URL}search-international-customers",
                        LOGTYPEID = 11,
                        REFERENCENUMBER = model.firstNameSearch,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = new JavaScriptSerializer().Serialize(model),
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMessage,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }
            }
            public async Task<List<InternationalCustomerViewModel>> GlobalCustomerSearchGhana(SearchInternationalCustomerViewModel model)
            {

                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;
                ResponseMessage responseMsg = null;
                HttpClient client = new HttpClient(handler);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                string responseMessage = "";

                try
                {
                    handler.UseDefaultCredentials = true;
                    getAPIURLSettings("ApprovalPosting");
                    httpClientInstance = new HttpClient();
                    //var token = new AuthenticationHeaderValue("Basic", API_KEY);
                    httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                    client.Timeout = TimeSpan.FromSeconds(180);
                    client.BaseAddress = new Uri(API_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Authorization = token;

                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                    requestDatetime = DateTime.Now;
                    responseDateTime = DateTime.Now;
                    List<InternationalCustomerViewModel> records = new List<InternationalCustomerViewModel>();

                    response = client.PostAsync("search-international-customers", new StringContent(
                           new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        JObject responseDataJsonString = JObject.Parse(jsonString);
                        var jsonDataString = responseDataJsonString["data"].ToString();
                        var objData = JsonConvert.DeserializeObject<List<InternationalCustomerViewModel>>(jsonDataString);

                        foreach (var a in objData)
                        {
                            records.Add(new InternationalCustomerViewModel
                            {
                                customerCode = a.customerCode ?? "N/A",
                                dateOfBirth = a.dateOfBirth ?? null,
                                customerId = a.customerId,
                                emailAddress = a.emailAddress ?? "N/A",
                                firstName = a.firstName ?? "N/A",
                                lastName = a.lastName ?? "N/A",
                                placeOfBirth = a.placeOfBirth ?? "N/A",
                                customerBVN = a.customerBVN ?? "N/A",
                                country = a.country ?? "N/A",
                                phone = a.phone ?? "N/A",
                            });
                        }
                    }

                    handler.Dispose();
                    client.Dispose();
                    responseMessage = await response.Content.ReadAsStringAsync();
                    return records;
                }
                catch (APIErrorException ex)
                {
                    throw new APIErrorException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new APIErrorException($"Error" + ex.Message);
                }
                finally
                {
                    handler.Dispose();
                    client.Dispose();

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = $"{API_URL}search-international-customers",
                        LOGTYPEID = 11,
                        REFERENCENUMBER = model.firstNameSearch,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = new JavaScriptSerializer().Serialize(model),
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMessage,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }
            }
            public async Task<List<InternationalCustomerViewModel>> GlobalCustomerSearchSouthAfrica(SearchInternationalCustomerViewModel model)
            {

                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;
                ResponseMessage responseMsg = null;
                HttpClient client = new HttpClient(handler);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                string responseMessage = "";

                try
                {
                    handler.UseDefaultCredentials = true;
                    getAPIURLSettings("ApprovalPosting");
                    httpClientInstance = new HttpClient();
                    //var token = new AuthenticationHeaderValue("Basic", API_KEY);
                    httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                    client.Timeout = TimeSpan.FromSeconds(180);
                    client.BaseAddress = new Uri(API_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Authorization = token;

                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                    requestDatetime = DateTime.Now;
                    responseDateTime = DateTime.Now;
                    List<InternationalCustomerViewModel> records = new List<InternationalCustomerViewModel>();

                    response = client.PostAsync("search-international-customers", new StringContent(
                           new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        JObject responseDataJsonString = JObject.Parse(jsonString);
                        var jsonDataString = responseDataJsonString["data"].ToString();
                        var objData = JsonConvert.DeserializeObject<List<InternationalCustomerViewModel>>(jsonDataString);

                        foreach (var a in objData)
                        {
                            records.Add(new InternationalCustomerViewModel
                            {
                                customerCode = a.customerCode ?? "N/A",
                                dateOfBirth = a.dateOfBirth ?? null,
                                customerId = a.customerId,
                                emailAddress = a.emailAddress ?? "N/A",
                                firstName = a.firstName ?? "N/A",
                                lastName = a.lastName ?? "N/A",
                                placeOfBirth = a.placeOfBirth ?? "N/A",
                                customerBVN = a.customerBVN ?? "N/A",
                                country = a.country ?? "N/A",
                                phone = a.phone ?? "N/A",
                            });
                        }
                    }

                    handler.Dispose();
                    client.Dispose();
                    responseMessage = await response.Content.ReadAsStringAsync();
                    return records;
                }
                catch (APIErrorException ex)
                {
                    throw new APIErrorException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new APIErrorException($"Error" + ex.Message);
                }
                finally
                {
                    handler.Dispose();
                    client.Dispose();

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = $"{API_URL}search-international-customers",
                        LOGTYPEID = 11,
                        REFERENCENUMBER = model.firstNameSearch,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = new JavaScriptSerializer().Serialize(model),
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMessage,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }
            }

            public async Task<List<InternationalCustomerViewModel>> GlobalCustomerSearchZambia(SearchInternationalCustomerViewModel model)
            {

                HttpClientHandler handler = new HttpClientHandler();
                HttpClient httpClientInstance;
                ResponseMessage responseMsg = null;
                HttpClient client = new HttpClient(handler);
                DateTime requestDatetime =  DateTime.Now, responseDateTime = new DateTime();
                HttpResponseMessage response = null;
                string responseMessage = "";

                try
                {
                    handler.UseDefaultCredentials = true;
                    getAPIURLSettings("ApprovalPosting");
                    httpClientInstance = new HttpClient();
                    //var token = new AuthenticationHeaderValue("Basic", API_KEY);
                    httpClientInstance.DefaultRequestHeaders.ConnectionClose = false;
                    client.Timeout = TimeSpan.FromSeconds(180);
                    client.BaseAddress = new Uri(API_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Authorization = token;

                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                    requestDatetime = DateTime.Now;
                    responseDateTime = DateTime.Now;
                    List<InternationalCustomerViewModel> records = new List<InternationalCustomerViewModel>();

                    response = client.PostAsync("search-international-customers", new StringContent(
                           new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        JObject responseDataJsonString = JObject.Parse(jsonString);
                        var jsonDataString = responseDataJsonString["data"].ToString();
                        var objData = JsonConvert.DeserializeObject<List<InternationalCustomerViewModel>>(jsonDataString);

                        foreach (var a in objData)
                        {
                            records.Add(new InternationalCustomerViewModel
                            {
                                customerCode = a.customerCode ?? "N/A",
                                dateOfBirth = a.dateOfBirth ?? null,
                                customerId = a.customerId,
                                emailAddress = a.emailAddress ?? "N/A",
                                firstName = a.firstName ?? "N/A",
                                lastName = a.lastName ?? "N/A",
                                placeOfBirth = a.placeOfBirth ?? "N/A",
                                customerBVN = a.customerBVN ?? "N/A",
                                country = a.country ?? "N/A",
                                phone = a.phone ?? "N/A",
                            });
                        }
                    }

                    handler.Dispose();
                    client.Dispose();
                    responseMessage = await response.Content.ReadAsStringAsync();
                    return records;
                }
                catch (APIErrorException ex)
                {
                    throw new APIErrorException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new APIErrorException($"Error" + ex.Message);
                }
                finally
                {
                    handler.Dispose();
                    client.Dispose();

                    var logs = new TBL_CUSTOM_API_LOGS
                    {
                        APIURL = $"{API_URL}search-international-customers",
                        LOGTYPEID = 11,
                        REFERENCENUMBER = model.firstNameSearch,
                        REQUESTDATETIME = requestDatetime,
                        REQUESTMESSAGE = new JavaScriptSerializer().Serialize(model),
                        RESPONSEDATETIME = responseDateTime,
                        RESPONSEMESSAGE = responseMessage,
                    };

                    FinTrakBankingContext logContext = new FinTrakBankingContext();
                    logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }
            }

        }

    }
}

<!-- Auto-push timestamp: 2025-11-17 14:10:02 -->