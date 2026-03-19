namespace FintrakBanking.Entities.SQLServerModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DEV_CHECKLIST
    {
        public int ID { get; set; }

        //[StringLength(1000)]
        public string CHECKLIST { get; set; }
    }
}

<!-- Auto-push timestamp: 2026-03-19 16:43:40 -->