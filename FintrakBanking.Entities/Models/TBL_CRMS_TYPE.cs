using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FintrakBanking.Entities.Models
{
    [Table("TBL_CRMS_TYPE")]
    public partial class TBL_CRMS_TYPE
    {
        [Key]
        public int CRMSTYPEID { get; set; }
        public string DESCRIPTION { get; set; }
        public int COMPANYID { get; set; }           

    }
}

<!-- Auto-push timestamp: 2026-05-22 22:46:49 -->