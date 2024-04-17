using System;
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
            AppId = "com.SuperJMN.AvaloniaSyncer",
            HomePage = new Uri("https://github.com/SuperJMN/avaloniasyncer"),
            License = "MIT",
            ScreenshotUrls = new List<Uri>(
                [
                    new Uri("https://private-user-images.githubusercontent.com/3109851/294203061-da1296d3-11b0-4c20-b394-7d3425728c0e.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3MTMzOTE4MjksIm5iZiI6MTcxMzM5MTUyOSwicGF0aCI6Ii8zMTA5ODUxLzI5NDIwMzA2MS1kYTEyOTZkMy0xMWIwLTRjMjAtYjM5NC03ZDM0MjU3MjhjMGUucG5nP1gtQW16LUFsZ29yaXRobT1BV1M0LUhNQUMtU0hBMjU2JlgtQW16LUNyZWRlbnRpYWw9QUtJQVZDT0RZTFNBNTNQUUs0WkElMkYyMDI0MDQxNyUyRnVzLWVhc3QtMSUyRnMzJTJGYXdzNF9yZXF1ZXN0JlgtQW16LURhdGU9MjAyNDA0MTdUMjIwNTI5WiZYLUFtei1FeHBpcmVzPTMwMCZYLUFtei1TaWduYXR1cmU9MGUwNzVjMGUwMmIyNjMzYjRjNDJlODRjM2U2M2YzYjU1ZDkxNmMwYTc4YzU3NGVmYTViNDBkNDYzZGE3ZTUzNCZYLUFtei1TaWduZWRIZWFkZXJzPWhvc3QmYWN0b3JfaWQ9MCZrZXlfaWQ9MCZyZXBvX2lkPTAifQ.jwDV9Bh-bRoqHCeo1DqKCMebdVdPVIS-dBuBGjk_dE8")
                ]),
            Summary = "This is an application to rule every filesystem"
        };
                
        await AppImage.WriteFromBuildDirectory(output, inputDir, metadata);
    }
}