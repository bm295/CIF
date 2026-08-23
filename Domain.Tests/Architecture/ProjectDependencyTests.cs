using System.Xml.Linq;

namespace CIF.Domain.Tests.Architecture;

public sealed class ProjectDependencyTests
{
    [Fact]
    public void ProductionProjects_RespectDocumentedDependencyDirection()
    {
        var repositoryRoot = FindRepositoryRoot();
        var allowedReferences = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["Domain"] = [],
            ["Application"] = ["Domain"],
            ["Infrastructure"] = ["Application", "Domain"],
            ["Api"] = ["Application", "Infrastructure"]
        };

        foreach (var (project, allowed) in allowedReferences)
        {
            var projectFile = Path.Combine(repositoryRoot, project, $"{project}.csproj");
            var references = XDocument.Load(projectFile)
                .Descendants("ProjectReference")
                .Select(reference => Path.GetFileNameWithoutExtension(
                    reference.Attribute("Include")!.Value.Replace('\\', '/')))
                .ToArray();

            Assert.True(
                references.All(allowed.Contains),
                $"{project} has an outward project reference. Actual: [{string.Join(", ", references)}]; allowed: [{string.Join(", ", allowed)}].");
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MultiThreadOddEven.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new DirectoryNotFoundException("Could not locate the repository root.");
    }
}
