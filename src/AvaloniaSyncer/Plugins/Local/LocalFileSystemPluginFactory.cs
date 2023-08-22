﻿using System;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.Local;

class LocalFileSystemPluginFactory : IFileSystemPluginFactory
{
    private readonly Maybe<ILogger> logger;

    public LocalFileSystemPluginFactory(Maybe<ILogger> logger)
    {
        this.logger = logger;
        Settings = Maybe<IPluginSettings>.None;
    }

    public string Name => "Local";
    public Uri Icon => new Uri("avares://AvaloniaSyncer/Assets/sftp.png");

    public IFileSystemPlugin Create()
    {
        return new LocalPluginViewModel(logger);
    }

    public Maybe<IPluginSettings> Settings { get; }
}