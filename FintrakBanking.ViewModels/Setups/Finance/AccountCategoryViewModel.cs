using System;
using System.ComponentModel.DataAnnotations;

namespace FintrakBanking.ViewModels.Setups.Finance
{
    public class AccountCategoryViewModel : GeneralEntity
    {
        public short accountCategoryId { get; set; }

        [Required]
        public string accountCategoryName { get; set; }

        public bool aeleted { get; set; } 
    }
}

<!-- Auto-push timestamp: 2026-04-09 12:54:16 -->