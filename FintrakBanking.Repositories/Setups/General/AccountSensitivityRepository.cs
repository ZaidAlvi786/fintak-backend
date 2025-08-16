using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels.Setups.General;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FintrakBanking.Repositories.Setups.General
{

    
    public class AccountSensitivityRepository : IAccountSensitivityRepository
    {
        private FinTrakBankingContext context;

        public AccountSensitivityRepository(FinTrakBankingContext _context)
        {
            this.context = _context;
        }

        public async Task<AccountSensitivityViewModel> GetAccountSensitivityLevelsByLevelId(int sensitivityId)
        {
            var accountSensitivity = await (from a in context.TBL_CUSTOMER_SENSITIVITY_LEVEL
                                      where a.CUSTOMERSENSITIVITYLEVELID == sensitivityId
                                      select new AccountSensitivityViewModel
                                      {
                                          SensitivityDescription = a.DESCRIPTION,
                                          SensitivityId = a.CUSTOMERSENSITIVITYLEVELID,
                                          SensitivityLevel = a.CUSTOMERSENSITIVITYLEVELID
                                      }).SingleOrDefaultAsync();
            return accountSensitivity;
        }

        public async Task<IEnumerable<AccountSensitivityViewModel>> GetAllAccountSensitivityLevels()
        {
            var accountSensitivity = (from a in context.TBL_CUSTOMER_SENSITIVITY_LEVEL
                                      select new AccountSensitivityViewModel
                                      {
                                          SensitivityDescription = a.DESCRIPTION,
                                          SensitivityId = a.CUSTOMERSENSITIVITYLEVELID,
                                          SensitivityLevel = a.CUSTOMERSENSITIVITYLEVELID
                                      });
            return accountSensitivity;
        }
    }
}