using FintrakBanking.ViewModels.Customer;
using System;
using System.Collections.Generic;

namespace FintrakBanking.ViewModels.Finance
{
 
    public partial class CurrencyExchangeRateViewModel
    {        
 
        public short currencyId { get; set; }

        public DateTime date { get; set; }

        public double buyingRate { get; set; }

        public double sellingRate { get; set; }

        public short baseCurrencyId { get; set; }

        public bool isBaseCurrency { get; set; }

        public string webRequestStatus { get; set; }
        public int companyId { get; set; }
        public string fromCurrencyCode { get; set; }
        public string toCurrencyCode { get; set; }
        public double exchangeRate { get; set; }
        public string channel_code { get; set; }
        public string branch_code { get; set; }
        public string from_ccycode { get; set; }
        public string to_ccycode { get; set; }
        public string rateCode { get; set; }
        public string response_code { get; set; }
        public string response_message { get; set; }
        public List<GetCurrencyRateResponse> GetCurrencyRateResponse { get; set; }
        public List<GetCcyRateResponse> GetCcyRateResponse { get; set; }

        public string responseMessage { get; set; }
        public double buy_rate { get; set; }
        public double sale_rate { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string account_no { get; set; }
        public string ccy_code1 { get; set; }
        public string ccy_code2 { get; set; }
    }

    public partial class ExchangeRateViewModel
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public CurrencyExchangeRateViewModel data { get; set; }
    }

    public class GetCurrencyRateResponse
    {
     public decimal buy_rate { get; set; }
     public decimal mid_rate { get; set; }
     public decimal sale_rate { get; set; }
    }
    public class GetCcyRateResponse
    {
     public decimal buy_rate { get; set; }
     public decimal mid_rate { get; set; }
     public decimal sale_rate { get; set; }
    }
}