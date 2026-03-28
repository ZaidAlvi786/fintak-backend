using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FintrakBanking.BackgroundTasksService.Areas.HelpPage.ModelDescriptions
{
    public class EnumTypeModelDescription : ModelDescription
    {
        public EnumTypeModelDescription()
        {
            Values = new Collection<EnumValueDescription>();
        }

        public Collection<EnumValueDescription> Values { get; private set; }
    }
}

<!-- Auto-push timestamp: 2026-03-28 14:22:45 -->