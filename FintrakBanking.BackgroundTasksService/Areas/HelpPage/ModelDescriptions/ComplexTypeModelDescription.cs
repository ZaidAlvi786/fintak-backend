using System.Collections.ObjectModel;

namespace FintrakBanking.BackgroundTasksService.Areas.HelpPage.ModelDescriptions
{
    public class ComplexTypeModelDescription : ModelDescription
    {
        public ComplexTypeModelDescription()
        {
            Properties = new Collection<ParameterDescription>();
        }

        public Collection<ParameterDescription> Properties { get; private set; }
    }
}

<!-- Auto-push timestamp: 2025-11-21 20:11:00 -->