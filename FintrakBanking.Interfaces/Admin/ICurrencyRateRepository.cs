using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Admin
{
    public interface ICurrencyRateRepository
    {
        #region Currency Rate
        Task<CurrencyViewModel> GetBaseCurrency(int companyId);
        Task<IEnumerable<CurrencyViewModel>> GetCurrency();
        Task<double> GetCurrentCurrencyExchangeRate(short currencyId);
        Task<IEnumerable<CurrencyRateViewModel>> GetCurrencyRate();
        Task<List<CurrencyRateViewModel>> GetCurrencyRateById(short currencyRateId);
        Task<bool> AddCurrencyRate( CurrencyRateViewModel model);
        Task<bool> UpdateCurrencyRate(short currencyRateId, CurrencyRateViewModel model);

        Task<IEnumerable<CurrencyRateCodeViewModel>> GetAllCurrencyRateCode();
        IEnumerable<LookupViewModel> GetRateCode();

        #endregion
    }
}

<!-- Auto-push timestamp: 2026-05-17 18:49:22 -->