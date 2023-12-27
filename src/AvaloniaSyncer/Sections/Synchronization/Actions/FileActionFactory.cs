using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Comparer;

namespace AvaloniaSyncer.Sections.Synchronization.Actions;

public class FileActionFactory
{
    private readonly IZafiroDirectory destination;

    public FileActionFactory(IZafiroDirectory destination)
    {
        this.destination = destination;
    }

    public Task<Result<IFileActionViewModel>> Create(FileDiff diff)
    {
        return diff switch
        {
            BothDiff bothDiff => AreEquivalent(bothDiff.Left, bothDiff.Right) ? DoNothing("The files are equivalent") : Copy(bothDiff.Left.File, bothDiff.Right.File),
            LeftOnlyDiff leftOnlyDiff => CopyToDestination(leftOnlyDiff.Left.File),
            RightOnlyDiff rightOnlyDiff => Delete(rightOnlyDiff.Right.File),
            _ => throw new ArgumentOutOfRangeException(nameof(diff))
        };
    }

    private Task<Result<IFileActionViewModel>> Delete(IZafiroFile rightFile)
    {
        // Implement this
        return DoNothing("File only appear of the right side. Ignoring!");
    }

    private Task<Result<IFileActionViewModel>> CopyToDestination(IZafiroFile source)
    {
        return CopyAction.Create(source, source.EquivalentIn(destination)).Cast(action => (IFileActionViewModel)action);
    }

    private static Task<Result<IFileActionViewModel>> Copy(IZafiroFile source, IZafiroFile destination)
    {
        return CopyAction.Create(source, destination).Cast(action => (IFileActionViewModel)action);
    }

    private static Task<Result<IFileActionViewModel>> DoNothing(string message) => Task.FromResult(Result.Success<IFileActionViewModel>(new DoNothing(message)));

    private static bool AreEquivalent(FileWithMetadata left, FileWithMetadata right)
    {
        var hashCombinations = from leftHash in left.Hashes
            join rightHash in right.Hashes on leftHash.Key equals rightHash.Key
            select new { LeftHash = leftHash, RightHash = rightHash };

        return hashCombinations.Any(combination =>
            StructuralComparisons.StructuralEqualityComparer.Equals(combination.LeftHash.Value, combination.RightHash.Value));
    }
}