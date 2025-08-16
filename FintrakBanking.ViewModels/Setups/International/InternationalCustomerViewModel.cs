using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.ViewModels.Setups.International
{
    public class InternationalCustomerViewModel : GeneralEntity
    {
        public bool accountCreationComplete { get; set; }
        public short branchId { get; set; }
        public string branchName { get; set; }
        public bool creationMailSent { get; set; }
        public short customerSensitivityLevelId { get; set; }
        public string customerCode { get; set; }
        public short? customerTypeId { get; set; }
        public DateTime? dateOfBirth { get; set; }
        public int customerId { get; set; }
        public string emailAddress { get; set; }
        public string firstName { get; set; }
        public string gender { get; set; }
        public string lastName { get; set; }
        public string maidenName { get; set; }
        public string maritalStatus { get; set; }
        public string title { get; set; }
        public string middleName { get; set; }
        public string customerAccountNo { get; set; }
        public string customerTypeName { get; set; }
        public string misCode { get; set; }
        public string misStaff { get; set; }
        public int? nationalityId { get; set; }
        public string occupation { get; set; }
        public bool isPoliticallyExposed { get; set; }
        public int? relationshipOfficerId { get; set; }
        public string placeOfBirth { get; set; }
        public string spouse { get; set; }
        public short sectorId { get; set; }
        public string sectorName { get; set; }
        public short? subSectorId { get; set; }
        public string subSectorName { get; set; }
        public string taxNumber { get; set; }
        public short? riskRatingId { get; set; }
        public int? crmsRelationshipTypeId { get; set; }
        public int? crmsLegalStatusId { get; set; }
        public int? crmsCompanySizeId { get; set; }
        public string customerBVN { get; set; }
        public bool isProspect { get; set; }
        public decimal? pastDueObligations { get; set; }
        public int? countryOfResidentId { get; set; }
        public int? numberOfDependents { get; set; }
        public int? numberOfLoansTaken { get; set; }
        public decimal? loanMonthlyRepaymentFromOtherBanks { get; set; }
        public DateTime? dateOfRelationshipWithBank { get; set; }
        public int? relationshipTypeId { get; set; }
        public string teamLDP { get; set; }
        public string teamNPL { get; set; }
        public int? businessUnitId { get; set; }
        public string corr { get; set; }
        public string ownership { get; set; }
        public int customerGlobalNumber { get; set; }
        public bool isGlobalCustomer { get; set; }
        public bool isGlobalNumberAutomaticallyGenerated { get; set; }
        public string phone { get; set; }
        public string country { get; set; }
        public DateTime birthDateSearch { get; set; }
        public string firstNameSearch { get; set; }
        public string lastNameSearch { get; set; }
        public string birthPlaceSearch { get; set; }
        public string phoneSearch { get; set; }
        public string emailSearch { get; set; }
        public string responseMessage { get; set; }
        public string responseCode { get; set; }
    }
    public class SearchInternationalCustomerViewModel
    {
        public DateTime birthDateSearch { get; set; }
        public string firstNameSearch { get; set; }
        public string lastNameSearch { get; set; }
        public string birthPlaceSearch { get; set; }
        public string phoneSearch { get; set; }
        public string emailSearch { get; set; }
    }

}
