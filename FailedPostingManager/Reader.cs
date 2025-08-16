using FintrakBanking.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.Common.Enum;

namespace FailedPostingManager
{
    public  class Reader
    {
        FinTrakBankingContext dbContext;
        public Reader()
        {
          dbContext = new FinTrakBankingContext();
        }


        public List<FailedTransactionsViewModel> get()
        {
            var pendingTransactions = dbContext.TBL_FAILED_TRANSACTIONS.Where((x) => x.STATUS == false).
                Select(x => new FailedTransactionsViewModel
                {
                    transactionId = x.TRANSACTIONID,
                    loanApplicationId = x.LOANAPPLICATIONID,
                    status = x.STATUS,
                    dateTimeCreated = x.DATETIMECREATED,
                    createdBy = x.CREATEDBY,
                    destination = x.DESTINATION,
                    requestBody = x.REQUESTBODY

                }).ToList();
            return pendingTransactions;
        }

        public async Task<bool> Update(int failedTransactionId, Boolean status)
        {
            var pendingTransactions = await dbContext.TBL_FAILED_TRANSACTIONS.FindAsync(failedTransactionId);
            pendingTransactions.STATUS = status;
           var response  =  await dbContext.SaveChangesAsync();
            return response > 0;
           
        }
    }
}

