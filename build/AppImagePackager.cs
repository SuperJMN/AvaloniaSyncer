using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DotnetPackaging;
using DotnetPackaging.AppImage;
using DotnetPackaging.AppImage.Core;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Zafiro.FileSystem.Lightweight;

public static class AppImagePackager
{
    static readonly Dictionary<Architecture, (string Runtime, string RuntimeLinux)> Mapping = new()
    {
        [Architecture.X64] = ("linux-x64", "x86_64"),
        [Architecture.Arm64] = ("linux-arm64", "arm64"),
    };

    public static Task PackAppImage(Project project, Architecture architecture, Configuration configuration, AbsolutePath output, string version)
    {
        var runtime = Mapping[architecture].Runtime;

        var publishDirectory = project.Directory / "bin" / "publish" / runtime;
        
        DotNetTasks.DotNetPublish(settings => settings
            .SetProject(project)
            .SetConfiguration(configuration)
            .SetSelfContained(true)
            .SetRuntime(Mapping[architecture].Runtime)
            .SetOutput(publishDirectory));
        
        return Pack(project, publishDirectory, version, architecture, output);
    }

    static async Task Pack(Project project, AbsolutePath publishDirectory, string version, Architecture architecture, AbsolutePath outputDir)
    {
        var fs = new FileSystem();
        var inputDir = new DirectorioIODirectory(Maybe<string>.None, fs.DirectoryInfo.New(publishDirectory));
        var appName = project.Name.Replace(".Desktop", "");
        var packagePath = outputDir / appName + "-" + version + "-" + Mapping[architecture].RuntimeLinux + ".AppImage";
        packagePath.Parent.CreateDirectory();
        await using var output = fs.File.Open(packagePath, FileMode.Create);
        IEnumerable<AdditionalCategory> categories = [AdditionalCategory.FileManager, AdditionalCategory.FileTools, AdditionalCategory.FileTransfer, AdditionalCategory.Filesystem];
        var metadata = new Options()
        {
            Icon = Maybe<IIcon>.None,
            MainCategory = Maybe<MainCategory>.From(MainCategory.Utility),
            AdditionalCategories = Maybe.From(categories),
            AppName = appName,
            Comment = "Cross-Platform File Synchronization Powered by AvaloniaUI",
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
            StartupWmClass = appName,
            Version = version,
        };
                
        await AppImage.WriteFromBuildDirectory(output, inputDir, metadata);
    }
}