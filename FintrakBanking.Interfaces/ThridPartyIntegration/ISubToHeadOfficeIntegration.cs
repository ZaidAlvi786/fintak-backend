using FintrakBanking.ViewModels.Setups.International;
using FintrakBanking.ViewModels.ThridPartyIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.ThridPartyIntegration
{
    public interface ISubToHeadOfficeIntegration
    {
        PostingResult PostFacilityApprovalnputs(HeadOfficeFacilityApprovalViewModel model);
        PostingResult PostLMSFacilityApprovalnputs(HeadOfficeFacilityApprovalViewModel model);
        List<InternationalCustomerViewModel> GlobalCustomerSearchKenya(SearchInternationalCustomerViewModel model);
        List<InternationalCustomerViewModel> GlobalCustomerSearchMozambique(SearchInternationalCustomerViewModel model);
        List<InternationalCustomerViewModel> GlobalCustomerSearchGhana(SearchInternationalCustomerViewModel model);
        List<InternationalCustomerViewModel> GlobalCustomerSearchSouthAfrica(SearchInternationalCustomerViewModel model);
        List<InternationalCustomerViewModel> GlobalCustomerSearchZambia(SearchInternationalCustomerViewModel model);
    }
}
