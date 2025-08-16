using FintrakBanking.ViewModels.Credit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Credit
{
    public interface IDashboardRepository
    {
        Task<List<DashboardViewModel>> LoanApplicationsBySector(DateTime startDate, DateTime endDate,int companyId, int staffId);
        Task<List<DashboardReportItem>> LoanPerformance(DateTime startDate, DateTime endDate,int companyId, int staffId);
        Task<List<DashboardViewModel>> LoanOnThePipeline(DateTime startDate, DateTime endDate, int companyId, int staffId);
        Task<List<DashboardViewModel>> ExpotureByRiskRating(DateTime startDate, DateTime endDate, int companyId, int staffId);
        Task<List<DashboardViewModel>> CollateralCoverage(DateTime startDate, DateTime endDate, int companyId, int staffId);
        Task<List<DashboardViewModel>> ApprovedLoan(DateTime startDate, DateTime endDate, int companyId, int staffId);
        Task<List<DashboardViewModel>> TotalRiskExposure(DateTime startDate, DateTime endDate, int companyId, int staffId);
        Task<List<LoanDisburseByType>> LoanDisbursedByType(DateTime startDate, DateTime endDate, int companyId, int staffId);
        Task <DashboardViewModel> GetLoanInThePipelineLms(int operationId, int staffId, int companyId, int branchId, int? classId);
        Task <DashboardViewModel> GetApprovedLoansLms(int companyId, int staffId);
        Task<DashboardViewModel> GetCountryCurrency(int companyId);
    }
}
