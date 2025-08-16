using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Entities.StagingModels
{
    [Table("STG_SUBSIDIARIES")]
    public class STG_SUBSIDIARIES
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int SUBSIDIARYID { get; set; }
        public string SUBSIDIARYNAME { get; set; }
        public int COUNTRYID { get; set; }
        public string LOCATION { get; set; }
        public string URLLINK { get; set; }
        public bool ISACTIVE { get; set; }
    }
}
