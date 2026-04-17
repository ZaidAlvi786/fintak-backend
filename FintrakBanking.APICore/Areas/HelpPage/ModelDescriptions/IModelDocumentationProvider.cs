using System;
using System.Reflection;

namespace FintrakBanking.APICore.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}

<!-- Auto-push timestamp: 2026-04-17 12:51:07 -->