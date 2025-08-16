using FintrakBanking.Common.Enum;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.Common.CustomException;
using FintrakBanking.ViewModels.WorkFlow;
using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels;
using FintrakBanking.Common;
using System.Data.Entity;
using FintrakBanking.Interfaces.Setups.International;
using FintrakBanking.ViewModels.Setups.International;
using FintrakBanking.Interfaces.ThridPartyIntegration;

namespace FintrakBanking.Repositories.Setups.International
{
    public class InternationalCustomerRepository : IInternationalCustomerRepository
    {
        private FinTrakBankingContext context;
        private IGeneralSetupRepository genSetup;
        private IAuditTrailRepository auditTrail;
        private IWorkflow workFlow;
        private ISubToHeadOfficeIntegration subsidiaryRepo;
        bool USE_THIRD_PARTY_INTEGRATION = false;


        public InternationalCustomerRepository(FinTrakBankingContext _context,
            IGeneralSetupRepository _genSetup, IAuditTrailRepository _auditTrail, IWorkflow _workflow,
            ISubToHeadOfficeIntegration _subsidiaryRepo
)
        {
            context = _context;
            genSetup = _genSetup;
            auditTrail = _auditTrail;
            workFlow = _workflow;
            subsidiaryRepo = _subsidiaryRepo;
            var globalSetting = context.TBL_SETUP_GLOBAL.FirstOrDefault();
            USE_THIRD_PARTY_INTEGRATION = globalSetting.USE_THIRD_PARTY_INTEGRATION;
        }
        private IEnumerable<InternationalCustomerViewModel> GetInternationalCustomers()
        {
            var customer = context.TBL_CUSTOMER.Where(x => x.DELETED == false).Select(a => new InternationalCustomerViewModel
            {
                customerCode = a.CUSTOMERCODE ?? "N/A",
                dateOfBirth = a.DATEOFBIRTH ?? null,
                customerId = a.CUSTOMERID,
                emailAddress = a.EMAILADDRESS ?? "N/A",
                firstName = a.FIRSTNAME ?? "N/A",
                lastName = a.LASTNAME ?? "N/A",
                placeOfBirth = a.PLACEOFBIRTH ?? "N/A",
                customerBVN = a.CUSTOMERBVN ?? "N/A",
                country = context.TBL_COUNTRY.FirstOrDefault(x => x.COUNTRYID == a.COUNTRYOFRESIDENTID) != null ? context.TBL_COUNTRY.FirstOrDefault(x => x.COUNTRYID == a.COUNTRYOFRESIDENTID).NAME : "N/A",
                //customerGlobalNumber = a.CUSTOMERGLOBALNUMBER == null ? 0 : a.CUSTOMERGLOBALNUMBER,
                phone = context.TBL_CUSTOMER_PHONECONTACT.FirstOrDefault(x => x.CUSTOMERID == a.CUSTOMERID) != null ? context.TBL_CUSTOMER_PHONECONTACT.FirstOrDefault(x => x.CUSTOMERID == a.CUSTOMERID).PHONE : "N/A",

            }).ToList();

            return customer;
        }
        public IEnumerable<InternationalCustomerViewModel> GetInternationalCustomerSearch(SearchInternationalCustomerViewModel model)
        {
            using (var context = new FinTrakBankingContext())
            {
                if (USE_THIRD_PARTY_INTEGRATION)
                {
                    var customersHost = (from x in GetInternationalCustomers()
                                         where (x.firstName.ToLower() == model.firstNameSearch.ToLower()
                                               || x.lastName.ToLower() == model.lastNameSearch.ToLower()
                                               || x.placeOfBirth.ToLower().Contains(model.birthPlaceSearch.ToLower())
                                               || x.phone.Contains(model.phoneSearch)
                                               || x.emailAddress.ToLower().Contains(model.emailSearch.ToLower()))
                                               || x.birthDateSearch.Date == model.birthDateSearch.Date
                                         select x).ToList();


                    var customersKenya = subsidiaryRepo.GlobalCustomerSearchKenya(model).ToList();

                    var customersMozambique = subsidiaryRepo.GlobalCustomerSearchMozambique(model).ToList();
                    var customersGhana = subsidiaryRepo.GlobalCustomerSearchGhana(model).ToList();
                    var customersSA = subsidiaryRepo.GlobalCustomerSearchSouthAfrica(model).ToList();
                    var customersZambia = subsidiaryRepo.GlobalCustomerSearchZambia(model).ToList();

                    var result = customersHost.Union(customersKenya).Union(customersMozambique).Union(customersGhana).Union(customersSA).Union(customersZambia);

                    return result; 
                }
                else
                {
                    var customer = (from x in GetInternationalCustomers()
                                         where (x.firstName.ToLower() == model.firstNameSearch.ToLower()
                                               || x.lastName.ToLower() == model.lastNameSearch.ToLower()
                                               || x.placeOfBirth.ToLower().Contains(model.birthPlaceSearch.ToLower())
                                               || x.phone.Contains(model.phoneSearch)
                                               || x.emailAddress.ToLower().Contains(model.emailSearch.ToLower()))
                                               || x.birthDateSearch.Date == model.birthDateSearch.Date
                                         select x).ToList();

                    return customer;
                }
                
            }
        }
    }
}
