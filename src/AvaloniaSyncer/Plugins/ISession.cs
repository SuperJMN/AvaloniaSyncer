﻿using System;
using System.Threading.Tasks;
using AvaloniaSyncer.Plugins.Local;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Plugins;

public interface ISession
{
    public Task<Result<IFileSystem>> FileSystem();
    public string Path { get; set; }
    public IObservable<IZafiroDirectory> Directory { get; }
    IObservable<bool> IsValid { get; }
}