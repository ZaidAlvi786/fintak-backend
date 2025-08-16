using FintrakBanking.Common.CustomException;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.ThridPartyIntegration;
using FintrakBanking.ViewModels.Setups.International;
using FintrakBanking.ViewModels.ThridPartyIntegration;
using FinTrakBanking.ThirdPartyIntegration.Finacle;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinTrakBanking.ThirdPartyIntegration.SubToHeadOffice
{
    public class SubToHeadOfficeIntegration : ISubToHeadOfficeIntegration
    {
        private FinTrakBankingContext _context;
        private string API_KEY, API_URL = string.Empty;
        private IEnumerable<TBL_API_URL> APIUrlConfig;
        private TransactionPosting transaction;
        public SubToHeadOfficeIntegration(FinTrakBankingContext context, TransactionPosting transaction)
        {
            this._context = context;
            this.transaction = transaction;
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

        public PostingResult PostFacilityApprovalnputs(HeadOfficeFacilityApprovalViewModel model)
        {
            {
                ApprovalPostingResult result = null;
                Task.Run(async () => result = await transaction.ApprovalPostingToHeadOffice(model)).GetAwaiter().GetResult();

                if (result != null && result.responseMessage != null)
                {
                    if (result.responseCode == "00")
                    {
                        //string str = result.APIResponse.webRequestStatus;
                        return new PostingResult { posted = true, responseCode = result.responseCode };
                    }
                    else
                    {
                        TrackFailedTransactions(model);
                        throw new ConditionNotMetException("API call error - Response Code:" + result.responseCode + ". Response Message:" + result.responseMessage); //message result.APIResponse.webRequestStatus
                    }
                }
                else
                {
                    TrackFailedTransactions(model);
                    throw new APIErrorException("API call Error - Kindly contact the administrator. Response Code:" + result.responseCode + ". Response Message:" + result.responseMessage);
                }

            }
        }

        public PostingResult PostLMSFacilityApprovalnputs(HeadOfficeFacilityApprovalViewModel model)
        {
            {
                ApprovalPostingResult result = null;
                Task.Run(async () => result = await transaction.LmsApprovalPostingToHeadOffice(model)).GetAwaiter().GetResult();

                if (result != null && result.responseMessage != null)
                {
                    if (result.responseCode == "00")
                    {
                        //string str = result.APIResponse.webRequestStatus;
                        return new PostingResult { posted = true, responseCode = result.responseCode };
                    }
                    else
                    {
                        TrackFailedTransactions(model);
                        throw new ConditionNotMetException("API call error - Response Code:" + result.responseCode + ". Response Message:" + result.responseMessage); //message result.APIResponse.webRequestStatus
                    }
                }
                else
                {
                    TrackFailedTransactions(model);
                    throw new APIErrorException("API call Error - Kindly contact the administrator. Response Code:" + result.responseCode + ". Response Message:" + result.responseMessage);
                }

            }
        }

        public List<InternationalCustomerViewModel> GlobalCustomerSearchKenya(SearchInternationalCustomerViewModel model)
        {
            {
                var data = new List<InternationalCustomerViewModel>();
                Task.Run(async () => data = await transaction.GlobalCustomerSearchKenya(model)).GetAwaiter().GetResult();
                return data;

            }
        }

        public List<InternationalCustomerViewModel> GlobalCustomerSearchMozambique(SearchInternationalCustomerViewModel model)
        {
            {
                var data = new List<InternationalCustomerViewModel>();
                Task.Run(async () => data = await transaction.GlobalCustomerSearchMozambique(model)).GetAwaiter().GetResult();
                return data;

            }
        }

        public List<InternationalCustomerViewModel> GlobalCustomerSearchGhana(SearchInternationalCustomerViewModel model)
        {
            {
                var data = new List<InternationalCustomerViewModel>();
                Task.Run(async () => data = await transaction.GlobalCustomerSearchGhana(model)).GetAwaiter().GetResult();
                return data;

            }
        }

        public List<InternationalCustomerViewModel> GlobalCustomerSearchSouthAfrica(SearchInternationalCustomerViewModel model)
        {
            {
                var data = new List<InternationalCustomerViewModel>();
                Task.Run(async () => data = await transaction.GlobalCustomerSearchSouthAfrica(model)).GetAwaiter().GetResult();
                return data;

            }
        }

        public List<InternationalCustomerViewModel> GlobalCustomerSearchZambia(SearchInternationalCustomerViewModel model)
        {
            {
                var data = new List<InternationalCustomerViewModel>();
                Task.Run(async () => data = await transaction.GlobalCustomerSearchZambia(model)).GetAwaiter().GetResult();
                return data;

            }
        }

        private void TrackFailedTransactions(HeadOfficeFacilityApprovalViewModel model)
        {
           
            _context.TBL_FAILED_TRANSACTIONS.Add(new TBL_FAILED_TRANSACTIONS
            {
                LOANAPPLICATIONID = model.loanApplicationId,
                STATUS = false,
                DATETIMECREATED = model.dateTimeCreated,
                CREATEDBY = model.createdBy,
                DESTINATION = model.destination,
                REQUESTBODY = model.requestBody

            });
        }
        
    }

}



