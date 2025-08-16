
using FintrakBanking.ViewModels.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Notification
{
   
    public interface INotificationRepository
    {
      
        IEnumerable<NotificationViewModel> GetWorkflowNotifications(int staffId, int companyId);

        IEnumerable<NotificationViewModel> GetAllNotifications(int staffId, int companyId);

        Task<bool> UpdateNotificationState(int notificationId);

        Task<bool> AddNotification(NotificationViewModel model);
    }
}
