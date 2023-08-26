using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AvaloniaSyncer.Settings;
using CSharpFunctionalExtensions;
using Serilog;

namespace AvaloniaSyncer.Plugins.SeaweedFS.Settings;

public class SeaweedProfiles
{
    public SeaweedProfiles(Maybe<ILogger> logger)
    {
        var profilesManager = new ProfilesViewModel<SeaweedProfile>(
            logger, 
            () => new SeaweedProfile(Guid.NewGuid()), toSave => Task.FromResult(Result.Success()),
            () => Task.FromResult(Result.Success<IEnumerable<SeaweedProfile>>(new List<SeaweedProfile>())));
    }
}