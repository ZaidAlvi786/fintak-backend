using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Entities.Models
{
    public class TBL_TAT_SETUP
    {
        [Key]
        public int TATID { get; set; }
        public short PRODUCTCLASSPROCESSID { get; set; }
        public int TURNAROUNDTIME { get; set; }
        public bool EXCLUDEPUBLICHOLIDAYS { get; set; }
        //public DateTime DATETIMECREATED { get; set; }
        public int CREATEDBY { get; set; }
        //public DateTime DATETIMEUPDATED { get; set; }
        public int LASTUPDATEDBY { get; set; }
        public bool DELETED { get; set; }
        public int DELETEDBY { get; set; }
        //public DateTime DATETIMEDELETED { get; set; }
    }
}
