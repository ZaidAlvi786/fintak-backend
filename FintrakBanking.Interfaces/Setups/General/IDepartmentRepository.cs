using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.General;
using FintrakBanking.ViewModels.WorkFlow;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.General
{
    public interface IDepartmentRepository
    {
        IEnumerable<DepartmentViewModel> GetAllDepartment();
        IEnumerable<DepartmentViewModel> GetAllDepartmentUnits(short departmentId);

        IEnumerable<DepartmentViewModel> GetJobDepartmentByJobTypeId(short jobTypeId);
        IEnumerable<DepartmentViewModel> GetAllUnits(int companyId);
        Task<bool> AddUnit(DepartmentViewModel entity);
        Task<bool> UpdateUnit(short unitId, DepartmentViewModel entity);


        IEnumerable<OperationStaffViewModel> GetAllDepartmentStaff(int departmentId);

        IQueryable<DepartmentCustomersViewModel> SearchForDepartmentStaffByUnitId(int companyId, string searchQuery, int departmentUnitId);
        IQueryable<DepartmentCustomersViewModel> SearchForDepartmentStaff(int companyId, string searchQuery);
        IQueryable<DepartmentCustomersViewModel> SearchDepartment(int departmentId, int companyId, string searchQuery);


        Task<bool> AddDepartment(DepartmentViewModel entity);

        Task<bool> UpdateDepartment(int departmentId, DepartmentViewModel entity);

        Task<bool> DeleteDepartment(int departmentId);

        Task<bool> DeleteUnit(UserInfo user, int unitId);
    }
}