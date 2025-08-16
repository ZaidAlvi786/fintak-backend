using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.General
{
    public interface IPublicHolidayRepository
    {
        bool DoesHolidayExist(DateTime date, int countryId);
        PublicHolidayViewModel GetPublicHoliday(int id);
        Task<IEnumerable<PublicHolidayViewModel>> GetAllPublicHoliday();
        IEnumerable<PublicHolidayViewModel> GetAllPublicHolidayByCompanyId(int id);
        Task<bool> AddPublicHoliday(PublicHolidayViewModel model);
        Task<bool> UpdatePublicHoliday(PublicHolidayViewModel model, int id);

        Task<bool> AddWeekendsInTheYear(PublicHolidayViewModel model);

        DateTime GetNextWorkDay(DateTime date, int countryId);

        Task<bool> DeletePublicHoliday(UserInfo model,int id);
    }
}
