namespace FintrakBanking.Entities.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FINTRAKBANKING.TBL_CUSTOMER_EDUCATIONLEVELTYP")]
    public partial class TBL_CUSTOMER_EDUCATIONLEVELTYP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EDUCATIONLEVELTYPEID { get; set; }

        [Required]
        [StringLength(200)]
        public string EDUCATIONLEVEL { get; set; }
    }
}

<!-- Auto-push timestamp: 2026-02-05 22:15:07 -->