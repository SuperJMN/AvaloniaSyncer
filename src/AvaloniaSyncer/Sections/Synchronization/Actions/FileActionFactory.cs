using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Comparer;

namespace AvaloniaSyncer.Sections.Synchronization.Actions;

public class FileActionFactory
{
    private readonly IZafiroDirectory source;
    private readonly IZafiroDirectory destination;

    public FileActionFactory(IZafiroDirectory source, IZafiroDirectory destination)
    {
        this.source = source;
        this.destination = destination;
    }

    public Task<Result<IFileActionViewModel>> Create(FileDiff diff, IFileCompareStrategy strategy)
    {
       
        return diff switch
        {
            BothDiff bothDiff => GenerateBothAction(bothDiff, strategy),
            LeftOnlyDiff leftOnlyDiff => CopyToDestination(leftOnlyDiff.Left),
            RightOnlyDiff rightOnlyDiff => Delete(rightOnlyDiff.Right),
            _ => throw new ArgumentOutOfRangeException(nameof(diff))
        };
    }

    private Task<Result<IFileActionViewModel>> GenerateBothAction(BothDiff bothDiff, IFileCompareStrategy compareStrategy)
    {
        return bothDiff.Left.AreEqual(bothDiff.Right, compareStrategy)
            .Bind(areEqual =>
            {
                if (areEqual)
                {
                    return Task.FromResult(Result.Success((IFileActionViewModel)new DoNothing("Skip", "Files are considered equal", Maybe<IZafiroFile>.From(bothDiff.Left), Maybe<IZafiroFile>.From(bothDiff.Right))));
                }

                return CopyAction.Create(bothDiff.Right, bothDiff.Left, "Files are different").Cast(action => (IFileActionViewModel)action);
            });
    }

    private Task<Result<IFileActionViewModel>> Delete(IZafiroFile rightFile)
    {
        return DeleteDestinationAction.Create(rightFile, $"File {rightFile} exists in {destination}, but it doesn't exist in {source}, so we will delete it").Cast(x => (IFileActionViewModel)x);
    }

    private Task<Result<IFileActionViewModel>> CopyToDestination(IZafiroFile toCopy)
    {
        var translatedToDestination = destination.FileSystem.GetFile(destination.Path.Combine(toCopy.Path.MakeRelativeTo(source.Path)));
        return CopyAction.Create(toCopy, translatedToDestination, $"File {toCopy} does not exist in {destination}").Cast(action => (IFileActionViewModel)action);
    }
}