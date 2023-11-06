using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using DotnetPackaging;
using DotnetPackaging.Deb;
using Microsoft.VisualBasic;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using static Nuke.Common.Tooling.ProcessTasks;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.IO;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class NewBuild : NukeBuild
{
    public static int Main() => Execute<NewBuild>(x => x.PackDebian);

    public AbsolutePath OutputDirectory = RootDirectory / "output";
    public AbsolutePath PublishDirectory => OutputDirectory / "publish";
    public AbsolutePath PackagesDirectory => OutputDirectory / "packages";

    [Solution] Solution Solution;
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    [GitVersion] readonly GitVersion GitVersion;

    Target Clean => td => td
        .Executes(() =>
        {
            OutputDirectory.CreateOrCleanDirectory();
            var absolutePaths = RootDirectory.GlobDirectories("**/bin", "**/obj").Where(a => !((string)a).Contains("build")).ToList();
            Log.Information("Deleting {Dirs}", absolutePaths);
            absolutePaths.DeleteDirectories();
        });

    Target RestoreWorkloads => td => td
        .Executes(() =>
        {
            StartShell(@$"dotnet workload restore {Solution.Path}").AssertZeroExitCode();
        });

    Target PackDebian => td => td
        .DependsOn(Clean)
        .DependsOn(RestoreWorkloads)
        .Executes(async () =>
        {
            var desktopProject = Solution.AllProjects.First(project => project.Name.EndsWith("Desktop"));
            var runtimes = new[] { "linux-x64", "linux-arm64" };

            DotNetPublish(settings => settings
                .SetConfiguration(Configuration)
                .SetProject(desktopProject)
                .CombineWith(runtimes, (c, runtime) =>
                    c.SetRuntime(runtime)
                        .SetOutput(PublishDirectory / runtime)));

            var results = await runtimes
                .Select(runtime => Observable.FromAsync(async () =>
                {
                    string projectName = Solution.Name;
                    string version = GitVersion.MajorMinorPatch;
                    string architecture = runtime.Split("-")[1];

                    string packageName = $"{projectName.Replace(" ", "").ToLower()}_{version}_{architecture}.deb";

                    Log.Information("Creating {Package}", packageName);
                    var result = await Create.Deb(PublishDirectory / runtime, PackagesDirectory / packageName, Metadata, ExecutableFiles);
                    return result
                        .Finally(r =>
                        {
                            Log.Information("{Package} {Result}", packageName, r.Match(() => "Succeeded", s => "Failed: {s}"));
                            return r;
                        });
                }))
                .Merge(1)
                .ToList();

            if (results.Combine().IsFailure)
            {
                throw new Exception(".deb creation failed");
            }
        });

    public DesktopEntry DesktopEntry => new()
    {
        Name = "Avalonia Syncer",
        Icons = IconResources.Create(new IconData(32, () => Observable.Using(() => File.OpenRead(Solution.Path!.Parent / "icon.png"), stream => stream.ToObservable()))).Value,
        StartupWmClass = "AvaloniaSyncer",
        Keywords = new[] { "file manager" },
        Comment = "The best file explorer ever",
        Categories = new[] { "FileManager", "Filesystem", "Utility", "FileTransfer", "Archiving" }
    };

    public Metadata Metadata => new()
    {
        PackageName = "AvaloniaSyncer",
        Description = "Best file explorer you'll ever find",
        ApplicationName = "Avalonia Syncer",
        Architecture = "amd64",
        Homepage = "https://www.something.com",
        License = "MIT",
        Maintainer = "SuperJMN@outlook.com",
        Version = "0.1.33"
    };

    public Dictionary<ZafiroPath, ExecutableMetadata> ExecutableFiles => new()
    {
        ["AvaloniaSyncer.Desktop"] = new("avaloniasyncer", DesktopEntry),
    };
}
