using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using static Nuke.Common.Tooling.ProcessTasks;
using Nuke.Common.Utilities.Collections;
using Nuke.GitHub;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.GitHub.GitHubTasks;

[AzurePipelines(AzurePipelinesImage.WindowsLatest, ImportSecrets = new[]{ nameof(GitHubAuthenticationToken)}, AutoGenerate = false)]
class Build : NukeBuild
{
    //public static int Main() => Execute<Build>(x => x.Publish);

    public AbsolutePath OutputDirectory = RootDirectory / "output";
    public AbsolutePath PublishDirectory => OutputDirectory / "publish";
    public AbsolutePath PackagesDirectory => OutputDirectory / "packages";

    [Parameter("authtoken")] [Secret] readonly string GitHubAuthenticationToken;
    
    [GitRepository] readonly GitRepository Repository;
    
    [GitVersion] readonly GitVersion GitVersion;

    [Solution] Solution Solution;

    [Parameter("publish-framework")] public string PublishFramework { get; set; }

    [Parameter("publish-runtime")] public string PublishRuntime { get; set; }

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Executes(() =>
        {
            OutputDirectory.CreateOrCleanDirectory();
            var absolutePaths = RootDirectory.GlobDirectories("**/bin", "**/obj").Where(a => !((string)a).Contains("build")).ToList();
            Log.Information("Deleting {Dirs}", absolutePaths);
            absolutePaths.DeleteDirectories();
        });

    Target Publish => _ => _
        .DependsOn(PublishAndroid)
        .DependsOn(PublishDesktop);

    Target RestoreWorkloads => _ => _
        .Executes(() =>
        {
            StartShell($"dotnet workload restore \"{Solution.Path}\"").AssertZeroExitCode();
        });

    Target PackDebian => _ => _
        .DependsOn(Clean)
        .DependsOn(RestoreWorkloads)
        .Executes(() =>
        {
            
        });

    Target PublishDesktop => _ => _
        .DependsOn(Clean)
        .DependsOn(RestoreWorkloads)
        .Executes(() =>
        {
            var desktopProject = Solution.AllProjects.First(project => project.Name.EndsWith("Desktop"));
            var runtimes = new[] { "win-x64", "linux-x64", "linux-arm64" };

            DotNetPublish(settings => settings
                .SetConfiguration(Configuration)
                .SetProject(desktopProject)
                .CombineWith(runtimes, (c, runtime) =>
                    c.SetRuntime(runtime)
                        .SetOutput(PublishDirectory / runtime)));

            runtimes.ForEach(rt =>
            {
                var src = PublishDirectory / rt;
                var dest = PackagesDirectory / rt + ".zip";
                Log.Information("Zipping {Input} to {Output}", src, dest);
                src.ZipTo(dest);
            });
        });

    Target PublishAndroid => _ => _
        .DependsOn(Clean)
        .DependsOn(RestoreWorkloads)
        .Executes(() =>
    {
        var androidProject = Solution.AllProjects.First(project => project.Name.EndsWith("Android"));
        
        DotNetPublish(settings => settings
            .SetProperty("ApplicationVersion", GitVersion.CommitsSinceVersionSource)
            .SetProperty("ApplicationDisplayVersion", GitVersion.MajorMinorPatch)
            .SetConfiguration(Configuration)
            .SetProject(androidProject)
            .SetOutput(PackagesDirectory));
    });

    Target PublishGitHubRelease => _ => _
        .OnlyWhenStatic(() => GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
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
