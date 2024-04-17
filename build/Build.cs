using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DotnetPackaging;
using DotnetPackaging.AppImage;
using DotnetPackaging.AppImage.Core;
using NuGet.Common;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Git;
using Nuke.Common.Tools.DotNet;using Nuke.Common.Tools.NSwag;
using Nuke.Common.Utilities.Collections;
using Nuke.GitHub;
using Serilog;
using Zafiro.FileSystem.Lightweight;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.GitHub.GitHubTasks;
using static Nuke.Common.Tooling.ProcessTasks;
using Maybe = CSharpFunctionalExtensions.Maybe;
using static Nuke.Common.Tools.NSwag.NSwagTasks;

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

    //Target PackDeb => td => td
    //    .DependsOn(Clean)
    //    .DependsOn(RestoreWorkloads)
    //    .Executes(() => DebPackages.Create(Solution, Configuration, PublishDirectory, PackagesDirectory, GitVersion.MajorMinorPatch));

    Target PackAppImages => td => td
        .Executes(async () =>
        {
            IEnumerable<Architecture> supportedArchitectures = [Architecture.Arm64, Architecture.X64];
            var desktopProject = Solution.AllProjects.First(project => project.Name.EndsWith("Desktop"));
            
            foreach (var architecture in supportedArchitectures)
            {
                Log.Information("Publishing {Arch}", architecture);
                await AppImagePackager.PackAppImage(desktopProject, architecture, Configuration, PackagesDirectory, GitVersion.MajorMinorPatch);
            }
        });

    Target PackWindows => td => td
        .DependsOn(Clean)
        .DependsOn(RestoreWorkloads)
        .Executes(() =>
        {
            var desktopProject = Solution.AllProjects.First(project => project.Name.EndsWith("Desktop"));
            var runtimes = new[] { "win-x64", };

            DotNetPublish(settings => settings
                .SetConfiguration(Configuration)
                .SetProject(desktopProject)
                .CombineWith(runtimes, (c, runtime) =>
                    c.SetRuntime(runtime)
                        .SetOutput(PublishDirectory / runtime)));

            runtimes.ForEach(rt =>
            {
                var src = PublishDirectory / rt;
                var zipName = $"{Solution.Name}_{GitVersion.MajorMinorPatch}_{rt}.zip";
                var dest = PackagesDirectory / zipName;
                Log.Information("Zipping {Input} to {Output}", src, dest);
                src.ZipTo(dest);
            });
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


    Target Publish => td => td.DependsOn(PackWindows, PackAndroid, PackAppImages);

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