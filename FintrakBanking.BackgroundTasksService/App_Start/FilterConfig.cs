using System.Web;
using System.Web.Mvc;

namespace FintrakBanking.BackgroundTasksService
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

<!-- Auto-push timestamp: 2026-04-17 20:32:53 -->