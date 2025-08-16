using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.ViewModels.ThridPartyIntegration
{
    public class HeadOfficeFacilityApprovalViewModel

    {
        public int loanApplicationId { get; set; }

        public int? loanApplicationDetailId { get; set; }

        public string applicationReferenceNumber { get; set; }

        public string relatedReferenceNumber { get; set; }

        public int? subsidiaryId { get; set; }

        public int? customerId { get; set; }

        public int? customerGlobalId { get; set; }

        public DateTime? applicationDate { get; set; }

        public double? interestRate { get; set; }

        public int? applicationTenor { get; set; }

        public int? approvalStatusId { get; set; }

        public int? approvalLevelId { get; set; }

        public int? approvalLevelGlobalCode { get; set; }

        public int? toStaffId { get; set; }

        public float? applicationStatusId { get; set; }

        public string operationName { get; set; }

        public string productClassName { get; set; }

        public string productName { get; set; }

        public string productClassProcess { get; set; }

        public string loanApplicationTypeName { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string middleName { get; set; }

        public string businessUnitShortCode { get; set; }

        public decimal? applicationAmount { get; set; }

        public decimal? totalExposureAmount { get; set; }

        public DateTime? systemDateTime { get; set; }

        public DateTime? systemArrivalDateTime { get; set; }

        public int createdBy { get; set; }

        public int? lastUpdatedBy { get; set; }

        public DateTime dateTimeCreated { get; set; }

        public DateTime? dateTimeUpdated { get; set; }

        public bool? deleted { get; set; }

        public int? deletedBy { get; set; }

        public DateTime? dateTimeDeleted { get; set; }

        public string responseMessage { get; set; }

        public string responseCode { get; set; }
        public string countryCode { get; set; }
        public string destination { get; set; }
        public string requestBody { get; set; }
        public int? staffRoleId { get; set; }
        public int? trailId { get; set; }
        public string staffRoleCode { get; set; }
        public bool actedOn { get; set; }
        public int targetId { get; set; }
        public int operationId { get; set; }
    }



    public class ApprovalPostingViewModel

    {
        public int loanApplicationId { get; set; }

        public int loanApplicationDetailId { get; set; }

        public string applicationReferenceNumber { get; set; }

        public string relatedReferenceNumber { get; set; }

        public int subsidiaryId { get; set; }

        public int customerId { get; set; }

        public int customerGlobalId { get; set; }

        public DateTime? applicationDate { get; set; }

        public double interestRate { get; set; }

        public int applicationTenor { get; set; }

        public int approvalStatusId { get; set; }

        public int? approvalLevelId { get; set; }

        public int approvalLevelGlobalCode { get; set; }

        public int? toStaffId { get; set; }

        public float applicationStatusId { get; set; }

        public string operationName { get; set; }

        public string productClassName { get; set; }

        public string productName { get; set; }

        public string productClassProcess { get; set; }

        public string loanApplicationTypeName { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string middleName { get; set; }

        public string businessUnitShortCode { get; set; }

        public decimal applicationAmount { get; set; }

        public decimal totalExposureAmount { get; set; }

        public DateTime systemDateTime { get; set; }

        public int createdBy { get; set; }

        public int lastUpdatedBy { get; set; }

        public DateTime dateTimeCreated { get; set; }

        public DateTime? dateTimeUpdated { get; set; }

        public bool deleted { get; set; }

        public int? deletedBy { get; set; }

        public DateTime? dateTimeDeleted { get; set; }



    }

}
