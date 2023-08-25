using System;
using System.Threading.Tasks;
using AvaloniaSyncer.Plugins.Local;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace AvaloniaSyncer.Plugins;

public interface ISession
{
    //public Task<Result<IFileSystem>> FileSystem(ISessionConfiguration configuration);
    public IObservable<IZafiroDirectory> Directory { get; }
    IObservable<bool> IsValid { get; }
}