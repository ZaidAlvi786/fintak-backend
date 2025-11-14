using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Entities.Models
{
    [Table("TBL_STAFF_REMOTE_NT_IDS")]
    public class TBL_STAFF_REMOTE_NT_IDS
    {
        [Key]
         public int STAFFREMOTENTID { get; set; }
         public int LOCALNETWORKID { get; set; }
         public int REMOTENETWORKID { get; set; }
         public string COUNTRYCODE { get; set; }
         public string STAFFROLECODE { get; set; }
        public string STAFFNT { get; set; }

    }
}


<!-- Auto-push timestamp: 2025-11-14 14:09:38 -->