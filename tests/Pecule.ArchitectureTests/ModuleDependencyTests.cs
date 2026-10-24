using System;
using System.Collections.Generic;
using System.Text;

namespace Pecule.ArchitectureTests;

public class ModuleDependencyTests
{
    private static readonly string[] Modules =
        ["Pecule.Accounts", "Pecule.Imports", "Pecule.Insights", "Pecule.Notifications"];

    [Theory]
    [InlineData(typeof(Pecule.Accounts.AccountsModule))]
    [InlineData(typeof(Pecule.Imports.ImportsModule))]
    [InlineData(typeof(Pecule.Insights.InsightsModule))]
    [InlineData(typeof(Pecule.Notifications.NotificationsModule))]
    public void ModuleDoesntReferenceOtherModules(Type pointEntree)
    {
        var assembly = pointEntree.Assembly;
        var references = assembly.GetReferencedAssemblies().Select(a => a.Name);
        var autresModules = Modules.Where(m => m != assembly.GetName().Name);

        Assert.Empty(references.Intersect(autresModules));
    }
}