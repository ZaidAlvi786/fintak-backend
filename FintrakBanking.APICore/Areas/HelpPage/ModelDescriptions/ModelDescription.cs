using System;

namespace FintrakBanking.APICore.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// Describes a type model.
    /// </summary>
    public abstract class ModelDescription
    {
        public string Documentation { get; set; }

        public Type ModelType { get; set; }

        public string Name { get; set; }
    }
}

<!-- Auto-push timestamp: 2026-04-19 16:25:40 -->