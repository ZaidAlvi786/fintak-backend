using FintrakBanking.ViewModels.Setups.General;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.General
{
    public interface IJobTitleRepository
    {
        IEnumerable<JobTitleViewModel> JobTitle();

        JobTitleViewModel GetJobTitle(int jobTitleId);

        IEnumerable<JobTitleViewModel> GetJobTitleByCompanyId(int companyId);

        Task<bool> AddUpdateJobTitle(JobTitleViewModel entity);

        bool ValidateJobTitle(string jobTitleName);
    }
}