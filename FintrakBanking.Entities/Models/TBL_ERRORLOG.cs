namespace FintrakBanking.Entities.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TBL_ERRORLOG")]
    public partial class TBL_ERRORLOG
    {
        [Key]
        public int ERRORLOGID { get; set; }

        //[StringLength(50)]
        public string USERNAME { get; set; }

        //[StringLength(100)]
        public string ERRORTYPE { get; set; }

        //[StringLength(100)]
        public string ERRORSOURCE { get; set; }

        public string ERRORMESSAGE { get; set; }
        
        //[StringLength(250)]
        public string APIENDPOINT { get; set; }

        //[StringLength(250)]
        public string ERRORPATH { get; set; }

        public int? STATUSCODE { get; set; }

        public DateTime TIMEUTC { get; set; }

        public string ALLXML { get; set; }
    }
}
