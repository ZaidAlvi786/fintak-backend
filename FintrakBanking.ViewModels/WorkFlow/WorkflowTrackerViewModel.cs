using System;
using System.Collections.Generic;

namespace FintrakBanking.ViewModels.WorkFlow
{
    public class WorkflowTrackerViewModel
    {
        public string dueHours { get; set; }
        //public int dueDate;
        public int dueDay { get; set; }
        public string divisionCode { get; set; }
        public string divisionName { get; set; }
        public string customerDivisionShortCode { get; set; }
        public int approvalTrailId { get; set; }
        public List<ApprovalTrailViewModel> comments { get; set; }
        public DateTime reportDate { get { return DateTime.Now; } }
        public string groupName { get; set; }
        public string operationName { get; set; }
        public string requestStaffName { get; set; }
        public string requestStaffCode { get; set; }
        public string requestApprovalLevel { get; set; }
        public DateTime arrivalDate { get; set; }
        public string approvalStatus { get; set; }
        public string companyName { get; set; }
        public int sla { get; set; }
        public DateTime responseDate { get; set; }
        public int? requestStaffId { get; set; }
        public bool isSkipAppraisalEnabled { get; set; }
        public string responseStaffName { get; set; }
        public string responsibleStaffName { get; set; }
        public string reliefStaffName { get; set; }
        public string responseStaffCode { get; set; }
        public string responseApprovalLevel { get; set; }
        public int TargetId { get; set; }
        public string comment { get; set; }
        private TimeSpan de { get { return (responseDate - arrivalDate); } }
        public double slaDifferenceMinutes { get { return (de).TotalMinutes; } }
        public string slaDifferenceMinute { get { return $"{de.Days}day(s) {de.Hours}h {de.Minutes}m {de.Seconds}s"; } }
        public string timespan { get { return (de).ToString(@"dd\.hh\:mm\:ss"); } }
        public int approvalStatusId { get; set; }
        public int loanBookingRequestId { get; set; }
        public DateTime? systemResponseDate { get; set; }
        public DateTime? systemArrivalDate { get; set; }
        public int operationId { get; set; }
        public string currentLevel { get; set; }
        public string applicationReferenceNumber { get; set; }
        public string approvalState { get; set; }
        public string customerName { get; set; }
        public DateTime applicationDate { get; set; }
        public string branchName { get; set; }
        public int loanApplicationId { get; set; }
        public string productNames { get; set; }
        public decimal amount { get; set; }
        public string baseCurrencyCode { get; set; }
        public int serial { get; set; }
        public int loanApplicationDetailId { get; set; }
        public byte[] reportData { get; set; }
        public string templateTypeName { get; set; }
        public string apiRequestId { get; set; }
        public int? loopedStaffId { get; set; }
        public int? toStaffId { get; set; }
        public int? fromApprovalLevelId { get; set; }
        public int? toApprovalLevelId { get; set; }
        public int? currentApprovalLevelId { get; set; }
        public DateTime? slaTime { get; set; }
        public int? turnAroundTime { get; set; }
        public short productClassProcessId { get; set; }
        public string expectedTAT { get; set; }
        public DateTime? firstApprovertime { get; set; }
        public DateTime? lastApprovertime { get; set; }
        public bool isCreditProgramme { get; set; }
        public string maxIndividualSLA { get; set; }
        public string applicationDate2 { get; set; }
        public int countryId { get; set; }
        public int excludedDays { get; set; }
        public string slaDays { get; set; }
        public bool useHolidays { get; set; }
    }

    public class TurnAroundTimeViewModel
    {
        public int tatId { get; set; }
        public short productClassProcessId { get; set; }
        public string productClassProcessName { get; set; }
        public int turnAroundTime { get; set; }
        public bool excludePublicHolidays { get; set; }
        public DateTime dateTimeCreated { get; set; }
        public int createdBy { get; set; }
        public DateTime dateTimeUpdated { get; set; }
        public int updatedBy { get; set; }
        public bool deleted { get; set; }
        public int deletedBy { get; set; }
        public DateTime dateTimeDeleted { get; set; }
        public short userBranchId { get; set; }
        public string userIPAddress { get; set; }
        public string applicationUrl { get; set; }
        public int companyId { get; set; }
    }

}