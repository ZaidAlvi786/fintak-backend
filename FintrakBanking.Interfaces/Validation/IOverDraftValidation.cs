using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Validation
{
  public  interface IOverDraftValidation
    {
        Task<bool> ODTopupValidation(int loanId, DateTime topupDate, decimal odLimit);
    }
}
