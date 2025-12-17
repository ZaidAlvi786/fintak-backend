using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels.Setups.General; 
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using static FinTrakBanking.ThirdPartyIntegration.TwoFactorAuthIntegration.TwoFactorAuthIntegrationService;

namespace FintrakBanking.Repositories.Setups.General
{
    public class AuthorizationRepository : IAuthorizationRepository
    { 
        private FinTrakBankingContext context;
        private ITwoFactorAuthIntegrationService auth;

        public AuthorizationRepository( FinTrakBankingContext _context,
            ITwoFactorAuthIntegrationService _auth)
        { 
            this.context = _context;
            this.auth = _auth;
        }

        public async Task<bool> AddGroup(GroupModel groupModel)
        {
            var newGroup = new TBL_PROFILE_GROUP()
            {
                GROUPNAME = groupModel.groupName,
                CREATEDBY = groupModel.createdBy,
                DATETIMECREATED = DateTime.Now
            };

              context.TBL_PROFILE_GROUP.Add(newGroup);

            var response = await context.SaveChangesAsync();

            return response > 0;
        }

        public Task<bool> DeleteGroup(short groupId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateGroup(short groupId, GroupViewModel groupModel)
        {
            var targetGroup = context.TBL_PROFILE_GROUP.Find(groupId);

            targetGroup.GROUPNAME = groupModel.groupName;
            targetGroup.DATETIMEUPDATED = DateTime.Now;
            targetGroup.LASTUPDATEDBY = groupModel.createdBy;

            var response = await context.SaveChangesAsync();
            return response != 0;
        }

        public async Task<List<GroupViewModel>> GetGroups()
        {
          

            var result = await context.TBL_PROFILE_GROUP.Select(g => new GroupViewModel()
            {
                groupId = g.GROUPID,
                groupName = g.GROUPNAME,
                Activities = g.TBL_PROFILE_GROUP_ACTIVITY.Select(x => new ActivityViewModel()
                {
                    activityId = x.ACTIVITYID,
                    activityName = x.TBL_PROFILE_ACTIVITY.ACTIVITYNAME
                }).ToList()
            }).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<ActivityViewModel>> GetActivities()
        {
           var data =  await context.TBL_PROFILE_ACTIVITY.Select(x => new ActivityViewModel()
            {
                activityId = x.ACTIVITYID,
                activityName = x.ACTIVITYNAME
            }).ToListAsync();
            return data;
        }

        public async Task<bool> AddActivitiesToGroup(GroupViewModel model)
        {
            if (model.Activities.Any())
            {
                var existingActivities = context.TBL_PROFILE_GROUP_ACTIVITY.Where(x => x.GROUPID == model.groupId).ToList();
                bool isExist = existingActivities.Count() > 0;
                if (existingActivities.Count > 0)
                {
                    foreach (var item in existingActivities)
                    {
                        context.TBL_PROFILE_GROUP_ACTIVITY.Remove(item);
                    }
                }

                foreach (var activity in model.Activities)
                {
                    var newActivity = new TBL_PROFILE_GROUP_ACTIVITY()
                    {
                        ACTIVITYID = activity.activityId,
                        GROUPID = model.groupId,
                        CANADD = false,
                        CANAPPROVE = false,
                        CANEDIT = false,
                        CANDELETE = false,
                        CANVIEW = false,
                        CREATEDBY = model.createdBy,
                        DATETIMECREATED = DateTime.Now
                    };
                    if (isExist)
                    {
                        newActivity.DATETIMEUPDATED = DateTime.Now;
                    }

                    context.TBL_PROFILE_GROUP_ACTIVITY.Add(newActivity);
                }
            }
            var response = await context.SaveChangesAsync() > 0;

            return response;
        }

        //public bool AddActivitiesToGroup(GroupViewModel model)
        //{
        //    if (model.Activities.Any())
        //    {
        //        var existingActivities = context.tbl_Profile_Group_Activity.Where(x => x.GroupId == model.groupId).ToList();
        //        bool isExist = existingActivities.Count() > 0;
        //        if (existingActivities.Count > 0)
        //        {
        //            foreach (var item in existingActivities)
        //            {
        //                context.tbl_Profile_Group_Activity.Remove(item);
        //            }
        //        }

        //        foreach (var activity in model.Activities)
        //        {
        //            var newActivity = new tbl_Profile_Group_Activity()
        //            {
        //                ActivityId = activity.activityId,
        //                GroupId = model.groupId,
        //                CreatedBy = model.createdBy,
        //                DateTimeCreated = DateTime.Now
        //            };
        //            if (isExist)
        //            {
        //                newActivity.DateTimeUpdated = DateTime.Now;
        //            }

        //            context.tbl_Profile_Group_Activity.Add(newActivity);
        //        }
        //    }
        //    var response = context.SaveChanges();

        //    return response > 0;
        //}

        public async Task<IEnumerable<Object>> GetActivitiesByGroupId(int grpId)
        {
            var data =  await context.TBL_PROFILE_GROUP_ACTIVITY.Where(a => a.GROUPID == grpId)
                .Select(x => new
                {
                    activityId = x.ACTIVITYID,
                    groupId = x.GROUPID
                }).ToListAsync();

            return data;
        }

        public async Task<IEnumerable<Object>> GetActivitiesByRoleId(int roleId)
        {
            var data = await context.TBL_PROFILE_STAFF_ROLE_ADT_ACT.Where(a => a.STAFFROLEID == roleId).Select(x => new
            {
                activityId = x.ACTIVITYID,
                roleId = x.STAFFROLEID
            }).ToListAsync();

            return data;
        }

        public async Task<IEnumerable<Object>> GetGroupsByRoleId(int roleId)
        {
            var data = await context.TBL_PROFILE_STAFF_ROLE_GROUP.Where(a => a.STAFFROLEID == roleId).Select(x => new
            {
                groupId = x.GROUPID,
                roleId = x.STAFFROLEID
            }).ToListAsync();
            return data;
        }

        public async Task<int> GetLoggedInUsersNumber(int? userId = null)
        {
            int data = 0;
            if (userId == null)
            {
               data =  await context.TBL_PROFILE_USER.Where(x => x.LOGINCODE != null && x.ISACTIVE == true).CountAsync();
            }
            else
            {
                data = await context.TBL_PROFILE_USER.Where(x => x.LOGINCODE != null && x.ISACTIVE == true && x.USERID != userId).CountAsync();
            }

            return data;
          
        }

        public async Task<bool> LogOutAllUsers(int userId)
        {
            var activeUsers = await context.TBL_PROFILE_USER.Where(x => x.LOGINCODE != null && x.ISACTIVE == true && x.USERID != userId).ToListAsync();
            foreach (var user in activeUsers) user.LOGINCODE = null;
            if (!activeUsers.Any()) return true;
            var result =  await context.SaveChangesAsync();

            return result > 0;
        }
    }
}

<!-- Auto-push timestamp: 2025-12-17 12:20:05 -->