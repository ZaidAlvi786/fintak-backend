using FintrakBanking.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace FintrakBanking.Interfaces.Admin
{
    public interface ICanAuthorizationRepository
    {
        bool CanPerformActionOnResource(int userId, int activityId, UserActions action);
    }
}

<!-- Auto-push timestamp: 2026-02-10 22:27:45 -->