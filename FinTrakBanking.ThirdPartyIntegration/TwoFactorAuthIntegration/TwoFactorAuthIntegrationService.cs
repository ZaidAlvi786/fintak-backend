using FintrakBanking.Common.CustomException;
using FintrakBanking.Entities.Models;
using FintrakBanking.ViewModels.Finance;
using FinTrakBanking.ThirdPartyIntegration.TwoFactorAuthService;
using FinTrakBanking.ThirdPartyIntegration.Mz2FAService;
using Newtonsoft.Json;
using RestSharp;
//using FinTrakBanking.ThirdPartyIntegration.TwoFactorAuthSoapService1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using static FinTrakBanking.ThirdPartyIntegration.TwoFactorAuthIntegration.TwoFactorAuthIntegrationService;
using System.Diagnostics;

namespace FinTrakBanking.ThirdPartyIntegration.TwoFactorAuthIntegration
{
    public class TwoFactorAuthIntegrationService : ITwoFactorAuthIntegrationService
    {
        FinTrakBankingContext logContext = new FinTrakBankingContext();
        public TwoFactorAutheticationOutputViewModel Authenticate(string staffCode, string passCode)
        {
            try
            {
                var message = string.Empty;
                var responseDateTime = DateTime.Now;
                var groupName = "accessbankplc.com";
                var forZambia = false;
                var forGhana = false;
                var countryCode = logContext.TBL_COMPANY.FirstOrDefault().COUNTRYCODE;
                if (countryCode == "ZMW") { forZambia = true; }
                if (countryCode == "GHS") { forGhana = true; }
                var AppId = "FINTRAK";
                var requestDatetime = DateTime.Now;
                var binding = new BasicHttpBinding();
                var output = new TwoFactorAutheticationOutputViewModel();
                IRestResponse response = null;

                if (forZambia)
                {
                    if (logContext.TBL_SETUP_GLOBAL.FirstOrDefault().USE_THIRD_PARTY_INTEGRATION == true && logContext.TBL_SETUP_GLOBAL.FirstOrDefault().USE_TWO_FACTOR_AUTHENTICATION == true)
                    {
                        
                        string responseMessage = "";
                        RestRequest req = new RestRequest(Method.POST);
                        TwoFactorAutheticationOutputViewModel reqBody = null;
                        string fullURL = "http://10.241.8.182/entrustmiddleware/api/entrust/authenticategenericgrouptoken";
                        RestClient clients = new RestClient(fullURL);

                        var reqbody = new TwoFactorAutheticationOutputViewModel()
                        {
                            AppId = "FINTRAK",
                            GroupName = "ACCESSZAMBIA",
                            UserId = staffCode,
                            Token = passCode
                        };

                        requestDatetime = DateTime.Now;
                        responseDateTime = DateTime.Now;

                        var jsonbody = new JavaScriptSerializer().Serialize(reqbody);
                        req.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("Accept", "application/json");

                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        response = clients.Execute<TwoFactorAutheticationOutputViewModel>(req);
                        var responbody = JsonConvert.DeserializeObject<TwoFactorAutheticationOutputViewModel>(response.Content);
                        if (responbody != null)
                        {
                            if (responbody.ResponseMessage.ToLower().Contains("successful"))
                            {
                                output.message = "Authentication Successful!";
                                output.authenticated = true;
                            }
                            else
                            {
                                output.authenticated = false;
                                output.message = "Two Factor Authentication Failed! " + response.ErrorMessage;
                            }
                        }
                        else
                        {
                            output.authenticated = false;
                            output.message = "Two Factor Authentication Failed! " + response.ErrorMessage;
                        }
                    }
                   
                }
                else if (forGhana)
                {
                       if (logContext.TBL_SETUP_GLOBAL.FirstOrDefault().USE_THIRD_PARTY_INTEGRATION == true && logContext.TBL_SETUP_GLOBAL.FirstOrDefault().USE_TWO_FACTOR_AUTHENTICATION == true)
                        {

                            string responseMessage = "";
                            RestRequest req = new RestRequest(Method.POST);
                            TwoFactorAutheticationOutputViewModel reqBody = null;
                            string fullURL = "http://api.ghana.accessbankplc.com/bankapi.stagging/api/v1/auth/VerifyUser2faToken";
                            RestClient clients = new RestClient(fullURL);

                            var reqbody = new TwoFactorAutheticationOutputViewModel()
                            {
                                UserType= "Internal",
                                UserId = staffCode,
                                Token = passCode,
                                Timestamp = DateTime.Now,
                                Processor = "credit360",
                                Channel = "credit360"
                            };

                            requestDatetime = DateTime.Now;
                            responseDateTime = DateTime.Now;

                            var jsonbody = new JavaScriptSerializer().Serialize(reqbody);
                            req.AddParameter("application/json", jsonbody, ParameterType.RequestBody);
                            req.AddHeader("Content-Type", "application/json");
                            req.AddHeader("Accept", "application/json");
                            req.AddHeader("user", "credit360");
                            req.AddHeader("password", "cr3d1t360$$");


                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                            response = clients.Execute<TwoFactorAutheticationOutputViewModel>(req);
                            var responbody = JsonConvert.DeserializeObject<TwoFactorAutheticationOutputViewModel>(response.Content);
                            if (responbody != null)
                            {
                                if (responbody.ResponseMessage.ToLower().Contains("successful"))
                                {
                                    output.message = "Authentication Successful!";
                                    output.authenticated = true;
                                }
                                else
                                {
                                    output.authenticated = false;
                                    output.message = "Two Factor Authentication Failed! " + response.ErrorMessage;
                                }
                            }
                            else
                            {
                                output.authenticated = false;
                                output.message = "Two Factor Authentication Failed! " + response.ErrorMessage;
                            }
                        }

                    
                }

                else
                {

                    var client = new ServiceSoapClient();
                    client.Open();
                    //var res = client.ResponseOnlyAsync(staffCode, passCode);
                    var res = client.AuthenticateUser(staffCode, passCode, groupName);
                    responseDateTime = DateTime.Now;

                    //var status = int.Parse(res.Split('~')[0]);
                    message = res;

                    output = new TwoFactorAutheticationOutputViewModel()
                    {
                        message = message
                    };

                    //if (res.ToLower().Contains("success")) 
                    if (res.Contains("01"))
                    {
                        output.authenticated = false;
                        output.message = "Authentication Failed! " + message;
                    }
                    else
                    {
                        output.message = "Authentication Successful!";
                        output.authenticated = true;
                    }

                };
                if (staffCode != null)
                {
                    if (forZambia)
                    {
                        var logs = new TBL_CUSTOM_API_LOGS
                        {
                            // APIURL = "https://esbentuser.accessbankplc.com:7085/Service?wsdl", 
                            APIURL = "http://10.241.8.182/entrustmiddleware/api/entrust/authenticategenericgrouptoken",
                            LOGTYPEID = 15,
                            REFERENCENUMBER = staffCode,
                            REQUESTDATETIME = requestDatetime,
                            REQUESTMESSAGE = $"CustId : {staffCode} , PassCode : {passCode}",
                            RESPONSEDATETIME = responseDateTime,
                            RESPONSEMESSAGE = output?.message + " " + response?.ErrorMessage
                        };
                        logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    }
                    if (forGhana)
                    {
                        var logs = new TBL_CUSTOM_API_LOGS
                        {
                            // APIURL = "https://esbentuser.accessbankplc.com:7085/Service?wsdl", 
                            APIURL = "http://api.ghana.accessbankplc.com/bankapi.stagging/api/v1/auth/VerifyUser2faToken",
                            LOGTYPEID = 15,
                            REFERENCENUMBER = staffCode,
                            REQUESTDATETIME = requestDatetime,
                            REQUESTMESSAGE = $"CustId : {staffCode} , PassCode : {passCode}",
                            RESPONSEDATETIME = responseDateTime,
                            RESPONSEMESSAGE = output?.message + " " + response?.ErrorMessage
                        };
                        logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    }
                    else
                    {
                       var logs = new TBL_CUSTOM_API_LOGS
                        {
                            // APIURL = "https://esbentuser.accessbankplc.com:7085/Service?wsdl", 
                            APIURL = "https://10.238.19.26/ACCESS_STAFF/Service.asmx?wsdl",
                            LOGTYPEID = 15,
                            REFERENCENUMBER = staffCode,
                            REQUESTDATETIME = requestDatetime,
                           REQUESTMESSAGE = $"CustId : {staffCode} , PassCode : {passCode} Message : {message}",
                           RESPONSEDATETIME = responseDateTime,
                            RESPONSEMESSAGE = output?.message + " " + response?.ErrorMessage
                        };
                        logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    }


                    //logContext.TBL_CUSTOM_API_LOGS.Add(logs);
                    logContext.SaveChanges();
                }
                return output;
            }
            catch (TwoFactorAuthenticationException ex)
            {
                throw new TwoFactorAuthenticationException(ex.Message);
            }

        }

        public interface ITwoFactorAuthIntegrationService
        {
            TwoFactorAutheticationOutputViewModel Authenticate(string staffCode, string passCode);
        }
    }
}
