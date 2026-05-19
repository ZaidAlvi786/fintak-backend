using System.Threading.Tasks;

namespace ExtractionService
{
    namespace Contracts
    {
        public interface IExtraction
        {
            Task CurrencyExchangeRateExtraction();
           // Task CustomerAccountBalances();
            Task CustomerAccountExtraction();
            Task ProductPricingExtraction();
            
        }
    }
}

<!-- Auto-push timestamp: 2026-05-19 22:55:06 -->