using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.ViewModels.Credit
{
    public class UserCurrencyViewFilter
    {
        public int DefaultCurrencyId { get; set; }

        public bool CanSeeLocalCurrency { get; set; }

        public bool CanSeeForeignCurrency { get; set; }
    }
}

<!-- Auto-push timestamp: 2026-01-01 20:11:23 -->