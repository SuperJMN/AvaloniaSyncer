using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DotnetPackaging;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Serilog;
using Zafiro.Mixins;
using Zafiro.Nuke;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Maybe = CSharpFunctionalExtensions.Maybe;

class Build : NukeBuild
{
    [Parameter("The alias for the key in the keystore.")] readonly string AndroidSigningKeyAlias;
    [Parameter("The password of the key within the keystore file.")] [Secret] readonly string AndroidSigningKeyPass;
    [Parameter("The password for the keystore file.")] [Secret] readonly string AndroidSigningStorePass;
    [Parameter("Contents of the keystore encoded as Base64.")] readonly string Base64Keystore;
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("GitHub Authentication Token")] [Secret] readonly string GitHubAuthenticationToken;
    [GitVersion] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository Repository;

    public AbsolutePath OutputDirectory = RootDirectory / "output";

    [Solution] Solution Solution;

    public Build()
    {
        Debugger.Launch();
    }

    protected override void OnBuildInitialized()
    {
        Actions = new Actions(Solution, Repository, RootDirectory, GitVersion, Configuration);
    }

    Actions Actions { get; set; }

    Target Clean => td => td
        .Executes(() =>
        {
            OutputDirectory.CreateOrCleanDirectory();
            var absolutePaths = RootDirectory.GlobDirectories("**/bin", "**/obj").Where(a => !((string) a).Contains("build")).ToList();
            Log.Information("Deleting {Dirs}", absolutePaths);
            absolutePaths.DeleteDirectories();
        });

    Target RestoreWorkloads => td => td
        .Executes(() =>
        {
            DotNetWorkloadRestore(x => x.SetProject(Solution));
        });
    
    Target PackAll => td => td
        .DependsOn(RestoreWorkloads)
        .OnlyWhenStatic(() => Repository.IsOnMainOrMasterBranch())
        .Executes(async () =>
        {
            var windowsFiles = Task.FromResult(Actions.CreateWindowsPacks());
            var options = Options();
            Debugger.Launch();
            var linuxAppImageFiles = Actions.CreateLinuxAppImages(options);
            var androidFiles = Task.FromResult(Actions.CreateAndroidPacks(Base64Keystore, AndroidSigningKeyAlias, AndroidSigningKeyPass, AndroidSigningStorePass));
            
            var allFiles = new[] { windowsFiles, linuxAppImageFiles, androidFiles }.Combine();
            await allFiles
                .Tap(allFiles => Log.Information("Published @{AllFiles}", allFiles))
                .TapError(e => throw new ApplicationException(e));
        });

    Target PublishGitHubRelease => td => td
        .DependsOn(RestoreWorkloads)
        .OnlyWhenStatic(() => Repository.IsOnMainOrMasterBranch())
        .Requires(() => GitHubAuthenticationToken)
        .Executes(async () =>
        {
            var windowsFiles = Task.FromResult(Actions.CreateWindowsPacks());
            var options = Options();
            var linuxAppImageFiles = Actions.CreateLinuxAppImages(options);
            var androidFiles = Task.FromResult(Actions.CreateAndroidPacks(Base64Keystore, AndroidSigningKeyAlias, AndroidSigningKeyPass, AndroidSigningStorePass));
            
            var allFiles = new[] { windowsFiles, linuxAppImageFiles, androidFiles }.Combine();
            await allFiles
                .Bind(paths => Actions.CreateGitHubRelease(GitHubAuthenticationToken, paths.Flatten().ToArray()))
                .TapError(e => throw new ApplicationException(e));
        });

    public static int Main() => Execute<Build>(x => x.PublishGitHubRelease);

    Options Options()
    {
        IEnumerable<AdditionalCategory> additionalCategories = [AdditionalCategory.FileTransfer, AdditionalCategory.FileTools, AdditionalCategory.FileManager, AdditionalCategory.Filesystem];

        IEnumerable<Uri> screenShots =
        [
            new Uri("https://private-user-images.githubusercontent.com/3109851/294203061-da1296d3-11b0-4c20-b394-7d3425728c0e.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3MTU3NjEyMzQsIm5iZiI6MTcxNTc2MDkzNCwicGF0aCI6Ii8zMTA5ODUxLzI5NDIwMzA2MS1kYTEyOTZkMy0xMWIwLTRjMjAtYjM5NC03ZDM0MjU3MjhjMGUucG5nP1gtQW16LUFsZ29yaXRobT1BV1M0LUhNQUMtU0hBMjU2JlgtQW16LUNyZWRlbnRpYWw9QUtJQVZDT0RZTFNBNTNQUUs0WkElMkYyMDI0MDUxNSUyRnVzLWVhc3QtMSUyRnMzJTJGYXdzNF9yZXF1ZXN0JlgtQW16LURhdGU9MjAyNDA1MTVUMDgxNTM0WiZYLUFtei1FeHBpcmVzPTMwMCZYLUFtei1TaWduYXR1cmU9MjBhZDc5YzlhM2ZlMTBmZDc4ODcwZjBjMmJiYzU0Zjc2ZTMyMjAwYjgwZmE1NmZlYzBhYmM2MjIwOTBjMWVmZSZYLUFtei1TaWduZWRIZWFkZXJzPWhvc3QmYWN0b3JfaWQ9MCZrZXlfaWQ9MCZyZXBvX2lkPTAifQ.3V2PtKWDyqZqw5AUZMxkt5Dh6k7xa8-eNre7hzjR-lI")
        ];

        return new Options
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
            Summary = "This is an application to rule every filesystem"
        };
    }
}