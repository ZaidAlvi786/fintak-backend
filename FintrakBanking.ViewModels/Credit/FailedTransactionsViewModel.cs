using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.ViewModels.Credit
{
    public class FailedTransactionsViewModel
    {
        public int transactionId { get; set; }
        public int loanApplicationId { get; set; }
        public bool status { get; set; }
        public DateTime dateTimeCreated { get; set; }
        public int createdBy { get; set; }
        public string destination { get; set; }
        public string requestBody { get; set; }
    }
}
