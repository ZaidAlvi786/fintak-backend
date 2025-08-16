using System;
using FintrakBanking.Entities.Models;
using FintrakBanking.ViewModels.Setups.General;
using System.Collections.Generic;
using System.Threading.Tasks;
using FintrakBanking.ViewModels;
using System.Linq;

namespace FintrakBanking.Interfaces.Setups.General
{
    public interface IAuthenticationRepository
    {
        Task<IList<UserViewModel>> GetAllUsers();

        Task<UserViewModel> GetSingleUser(int userId);

        Task<UserViewModel> GetSingleUserByUserName(string userName);

        Task<bool> CreateUser(UserViewModel user);

        Task<bool> DeleteUser(int userId);

        Task<bool> IsAccountLocked(string userName);

        Task<SessionStatusInfo> CheckSessionState(string username, string ipAddress);

        SessionStatusInfo SessionInfo { get; set; }

        Task<bool> UpdateUser(int userId, UserViewModel user);

        Task<bool> IsAccountActive(string userName);

        Task<bool> ResumptionClosignTime(string userName, bool log);

        Task<UserViewModel> FindUserByUserNameAndPassword(string username, string password);

        Task<UserViewModel> FindUserByUserNameAsync(string username);

        Task<bool> IsUserExisting(string username);

        Task<bool> IsUserAccountValid(string username);

        List<string> GetUserActivitiesByUser(int userId);

        // Groups

        IEnumerable<TBL_PROFILE_GROUP> GetAllGroups();

        Task<bool> ClearLoginToken(string userName);

        Task<LookupViewModel> GetDashboardStaffRole(int staffId);

        Task<bool> PasswordChange(PasswordChangeViewModel pwdChange);

        bool ValidatePasswordPolicy(string password);

        Task<bool> ValidateOldPassword(string username, string oldPassword);

        Task<UserViewModel> GetUserLoginInfoByUserName(string userName);

    }
}