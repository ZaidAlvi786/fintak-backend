using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.General
{
    public interface IAuthorizationRepository
    {
        Task<bool> AddGroup(GroupModel groupModel);

        Task<bool> DeleteGroup(short groupId);

        Task<bool> UpdateGroup(short groupId, GroupViewModel groupModel);

        Task<List<GroupViewModel>> GetGroups();

        Task<IEnumerable<ActivityViewModel>> GetActivities();

        Task<IEnumerable<Object>> GetActivitiesByGroupId(int grpId);

        Task<IEnumerable<Object>> GetActivitiesByRoleId(int roleId);
        Task<IEnumerable<Object>> GetGroupsByRoleId(int roleId);

        Task<bool> AddActivitiesToGroup(GroupViewModel model);

        //bool AddActivitiesToGroup(GroupViewModel model);

        Task<int> GetLoggedInUsersNumber(int? userId = null);

        Task<bool> LogOutAllUsers(int userId);
    }
}