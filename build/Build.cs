using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using DotnetPackaging;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Git;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Nuke.GitHub;
using Serilog;
using Zafiro.Nuke;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.GitHub.GitHubTasks;
using static Nuke.Common.Tooling.ProcessTasks;
using Maybe = CSharpFunctionalExtensions.Maybe;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.PublishGitHubRelease);

    public AbsolutePath OutputDirectory = RootDirectory / "output";
    public AbsolutePath PublishDirectory => OutputDirectory / "publish";
    public AbsolutePath PackagesDirectory => OutputDirectory / "packages";

    [Solution] Solution Solution;
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    [GitVersion] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository Repository;

    [Parameter("GitHub Authentication Token")][Secret] readonly string GitHubAuthenticationToken;
    [Parameter("Contents of the keystore encoded as Base64.")] readonly string Base64Keystore;
    [Parameter("The alias for the key in the keystore.")] readonly string AndroidSigningKeyAlias;
    [Parameter("The password for the keystore file.")][Secret] readonly string AndroidSigningStorePass;
    [Parameter("The password of the key within the keystore file.")][Secret] readonly string AndroidSigningKeyPass;
    
    private Actions Actions { get; }

    public Build()
    {
        Actions = new Actions(Solution, RootDirectory, GitVersion, Configuration);
    }

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
            DotNetWorkloadRestore(x => x.SetProject(Solution));
        });

    Target PackLinuxAppImages => td => td
        .Executes(async () =>
        {
            IEnumerable<AdditionalCategory> additionalCategories = [AdditionalCategory.FileTransfer, AdditionalCategory.FileTools, AdditionalCategory.FileManager, AdditionalCategory.Filesystem];

            IEnumerable<Uri> screenShots =
            [
                new Uri("https://private-user-images.githubusercontent.com/3109851/294203061-da1296d3-11b0-4c20-b394-7d3425728c0e.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3MTU3NjEyMzQsIm5iZiI6MTcxNTc2MDkzNCwicGF0aCI6Ii8zMTA5ODUxLzI5NDIwMzA2MS1kYTEyOTZkMy0xMWIwLTRjMjAtYjM5NC03ZDM0MjU3MjhjMGUucG5nP1gtQW16LUFsZ29yaXRobT1BV1M0LUhNQUMtU0hBMjU2JlgtQW16LUNyZWRlbnRpYWw9QUtJQVZDT0RZTFNBNTNQUUs0WkElMkYyMDI0MDUxNSUyRnVzLWVhc3QtMSUyRnMzJTJGYXdzNF9yZXF1ZXN0JlgtQW16LURhdGU9MjAyNDA1MTVUMDgxNTM0WiZYLUFtei1FeHBpcmVzPTMwMCZYLUFtei1TaWduYXR1cmU9MjBhZDc5YzlhM2ZlMTBmZDc4ODcwZjBjMmJiYzU0Zjc2ZTMyMjAwYjgwZmE1NmZlYzBhYmM2MjIwOTBjMWVmZSZYLUFtei1TaWduZWRIZWFkZXJzPWhvc3QmYWN0b3JfaWQ9MCZrZXlfaWQ9MCZyZXBvX2lkPTAifQ.3V2PtKWDyqZqw5AUZMxkt5Dh6k7xa8-eNre7hzjR-lI")
            ];

            await Actions.CreateLinuxAppImages(new Options()
                {
                    MainCategory = MainCategory.Utility,
                    AdditionalCategories = Maybe.From(additionalCategories),
                    AppName = "AvaloniaSyncer",
                    Version = GitVersion.MajorMinorPatch,
                    Comment = "Cross-Platform File Synchronization Powered by AvaloniaUI",
                    AppId = "com.SuperJMN.AvaloniaSyncer",
                    StartupWmClass = "AvaloniaSyncer",
                    HomePage = new Uri("https://github.com/SuperJMN/avaloniasyncer"),
                    Keywords = new List<string>
                    {
                        "File Synchronization",
                        "Cross-Platform",
                        "AvaloniaUI",
                        "Avalonia",
                        "File Management",
                        "Folder Sync",
                        "UI Design",
                        "Open Source",
                        "Reactive Programming"
                    },
                    License = "MIT",
                    ScreenshotUrls = Maybe.From(screenShots),
                    Summary = "This is an application to rule every filesystem",
                })
                .TapError(Log.Error);
        });

    Target PackWindows => td => td
        .DependsOn(Clean)
        .DependsOn(RestoreWorkloads)
        .Executes(() =>
        {
            Actions.CreateWindowsPack();
        });

    Target PackAndroid => td => td
        .DependsOn(Clean)
        .DependsOn(RestoreWorkloads)
        .Executes(() =>
        {
            var androidProject = Solution.AllProjects.First(project => project.Name.EndsWith("Android"));
            var keystore = OutputDirectory / "temp.keystore";
            keystore.WriteAllBytes(Convert.FromBase64String(Base64Keystore));

            DotNetPublish(settings => settings
                .SetProperty("ApplicationVersion", GitVersion.CommitsSinceVersionSource)
                .SetProperty("ApplicationDisplayVersion", GitVersion.MajorMinorPatch)
                .SetProperty("AndroidKeyStore", "true")
                .SetProperty("AndroidSigningKeyStore", keystore)
                .SetProperty("AndroidSigningKeyAlias", AndroidSigningKeyAlias)
                .SetProperty("AndroidSigningStorePass", AndroidSigningStorePass)
                .SetProperty("AndroidSigningKeyPass", AndroidSigningKeyPass)
                .SetConfiguration(Configuration)
                .SetProject(androidProject)
                .SetOutput(PackagesDirectory));

            keystore.DeleteFile();
        });


    Target Publish => td => td.DependsOn(PackWindows, PackAndroid, PackLinuxAppImages);

    Target PublishGitHubRelease => td => td
        .OnlyWhenStatic(() => Repository.IsOnMainOrMasterBranch())
        .DependsOn(Publish)
        .Requires(() => GitHubAuthenticationToken)
        .Executes(async () =>
        {
            var releaseTag = $"v{GitVersion.MajorMinorPatch}";

            var repositoryInfo = GetGitHubRepositoryInfo(Repository);

            Log.Information("Commit for the release: {GitVersionSha}", GitVersion.Sha);

            Log.Information("Getting list of files in {Path}", PackagesDirectory);
            var artifacts = PackagesDirectory.GetFiles().ToList();
            Log.Information("List of files obtained successfully");

            Assert.NotEmpty(artifacts,
                "Could not find any package to upload to the release");

            await PublishRelease(x => x
                .SetArtifactPaths(artifacts.Select(path => (string)path).ToArray())
                .SetCommitSha(GitVersion.Sha)
                .SetRepositoryName(repositoryInfo.repositoryName)
                .SetRepositoryOwner(repositoryInfo.gitHubOwner)
                .SetTag(releaseTag)
                .SetToken(GitHubAuthenticationToken)
            );
        });
}