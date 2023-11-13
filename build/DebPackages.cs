using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DotnetPackaging.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;

class DebPackages
{
    public static async Task Create(Solution solution,
        Configuration configuration,
        AbsolutePath publishDirectory,
        AbsolutePath outputDirectory,
        string version)
    {
        var desktopProject = solution.AllProjects.First(project => project.Name.EndsWith("Desktop"));
        var runtimes = new[] { "linux-x64", "linux-arm64" };

        DotNetTasks.DotNetPublish(settings => DotNetPublishSettingsExtensions
            .SetConfiguration(settings, configuration)
            .SetProject(desktopProject)
            .CombineWith(runtimes, (c, runtime) =>
                c.SetRuntime(runtime)
                    .SetOutput(publishDirectory / runtime)));

        var results = await runtimes
            .Select(runtime => Observable.FromAsync(async () =>
            {
                string projectName = solution.Name;
                string architecture = runtime.Split("-")[1];

                string packageName = $"{projectName!.Replace(" ", "").ToLower()}_{version}_{architecture}.deb";

                var packageDefinition = await new FileInfo(solution.Directory / "metadata.deb.json").ToPackageDefinition();
                
                Log.Information("Creating {Package}", packageName);
                var result = await DotnetPackaging.Create.Deb(packageDefinition, publishDirectory / runtime, outputDirectory / packageName);
                return result
                    .Finally(r =>
                    {
                        Log.Information("{Package} {Result}", packageName, r.Match(() => $"{packageName} created successfully!", error => $"{packageName} creation failed: {error}"));
                        return r;
                    });
            }))
            .Merge(1)
            .ToList();

        if (results.Combine().IsFailure)
        {
            throw new Exception(".deb creation failed");
        }
    }
}