

using System.Data.Entity;

namespace FintrakBanking.Repositories
{
    public class GenericSaveChanges
    {
        public bool SaveChanges(DbContext context)
        {
            return context.SaveChanges() > 0;
        }

        public bool SaveChanges(DbContext context, string columnNames)
        {
            return context.SaveChanges() > 0;
        }
    }
}

<!-- Auto-push timestamp: 2026-04-20 22:31:53 -->