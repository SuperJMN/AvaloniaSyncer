using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Comparer;
using Zafiro.Mixins;

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
            BothDiff bothDiff => AreEquivalent(bothDiff.Left, bothDiff.Right) ? FileAreEqual(bothDiff) : FileAreDifferent(bothDiff.Left.File, bothDiff.Right.File),
            LeftOnlyDiff leftOnlyDiff => CopyToDestination(leftOnlyDiff.Left.File),
            RightOnlyDiff rightOnlyDiff => Delete(rightOnlyDiff.Right.File),
            _ => throw new ArgumentOutOfRangeException(nameof(diff))
        };
    }

    private static async Task<Result<IFileActionViewModel>> FileAreEqual(BothDiff bothDiff)
    {
        Maybe<string> comment = $"""
                                One of the checksums match:
                                · {bothDiff.Left.File}: 
                                    {FormatChecksums(bothDiff.Left.Hashes)}
                                · {bothDiff.Right.File}: 
                                    {FormatChecksums(bothDiff.Left.Hashes)}
                                """;

        var fileActionViewModel = new DoNothing(
            "Skip", 
            comment, 
            Maybe<IZafiroFile>.From(bothDiff.Left.File), 
            Maybe<IZafiroFile>.From(bothDiff.Right.File));

        return fileActionViewModel;
    }

    private static string FormatChecksums(IDictionary<ChecksumKind, byte[]> leftHashes)
    {
        return leftHashes.Select(pair => "\t" + pair.Key + "=" + Convert.ToHexString(pair.Value)).JoinWithLines();
    }

    private Task<Result<IFileActionViewModel>> Delete(IZafiroFile rightFile)
    {
        // Implement this
        return Task.FromResult(Result.Success<IFileActionViewModel>(new DoNothing("Skip", "File only appear of the right side. Ignoring!", Maybe<IZafiroFile>.None, Maybe.From(rightFile))));
    }

    private Task<Result<IFileActionViewModel>> CopyToDestination(IZafiroFile source)
    {
        return CopyAction.Create(source, source.EquivalentIn(destination), $"File {source} does not exist in {destination}").Cast(action => (IFileActionViewModel)action);
    }

    private static Task<Result<IFileActionViewModel>> FileAreDifferent(IZafiroFile source, IZafiroFile destination)
    {
        return CopyAction.Create(source, destination, "Files are different").Cast(action => (IFileActionViewModel)action);
    }

    private static bool AreEquivalent(FileWithMetadata left, FileWithMetadata right)
    {
        var hashCombinations = from leftHash in left.Hashes
            join rightHash in right.Hashes on leftHash.Key equals rightHash.Key
            select new { LeftHash = leftHash, RightHash = rightHash };

        return hashCombinations.Any(combination =>
            StructuralComparisons.StructuralEqualityComparer.Equals(combination.LeftHash.Value, combination.RightHash.Value));
    }
}