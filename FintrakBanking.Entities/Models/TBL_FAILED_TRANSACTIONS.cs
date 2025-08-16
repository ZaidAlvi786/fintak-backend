using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Entities.Models
{
    [Table("TBL_FAILED_TRANSACTIONS")]
    public class TBL_FAILED_TRANSACTIONS
    {
        [Key]
        public int TRANSACTIONID { get; set; }
        public int LOANAPPLICATIONID { get; set; }
        public bool STATUS { get; set; }
        public DateTime DATETIMECREATED { get; set; }
        public int CREATEDBY { get; set; }
        public string DESTINATION { get; set; }
        public string REQUESTBODY { get; set; }


    }
}