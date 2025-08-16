using FintrakBanking.Interfaces.WorkFlow;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.General
{
    public interface IEmployerRepository
    {
        Task<IEnumerable<EmployerViewModel>> getEmployer(int companyId);
        Task<IEnumerable<EmployerViewModel>> getAllPendingEmployers(int companyId);
        Task<IEnumerable<EmployerViewModel>> getAllApprovedEmployers(int companyId);
        String ResponseMessage(WorkflowResponse response, string itemHeading);
        Task<WorkflowResponse> ForwardRelatedEmployerForApproval(EmployerViewModel model);
        Task<IEnumerable<EmployerViewModel>> GetRelatedEmployersWaitingForApproval(int staffId);

        Task<EmployerViewModel> getEmployer(int employerId, int companyId);
        Task<string> addEmployer(EmployerViewModel employer);
        Task<string> updateEmployer(int employerId, EmployerViewModel employer);
        Task<string> deleteEmployer(int employerId, EmployerViewModel employer);
        Task<IEnumerable<EmployerType>> getEmployerType();
        Task<IEnumerable<EmployerSubType>> getEmployerSunType(int employerTypeId);
        Task<IEnumerable<EmployerSubType>> getAllEmployerSubTypes();


        Task<string> addEmployerType(EmployerViewModel employerType);
        Task<string> deleteEmployerType(int employerId, EmployerViewModel employerType);
        Task<string> updateEmployerType(int employerTypeId, EmployerViewModel employerType);
        IEnumerable<EmployerType> getEmployerType(int employerTypeId);
        Task<string> addEmployerSubType(EmployerViewModel employerSubType);
        Task<string> deleteEmployerSubType(int employerId, EmployerViewModel employerSubType);
        Task<string> updateEmployerSubType(int employerTypeId, EmployerViewModel employerSubType);
        Task<IEnumerable<EmployerSubType>> getEmployerSubType(int employerSubTypeId);
    }
}
