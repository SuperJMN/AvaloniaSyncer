using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaSyncer.Sections.Connections.Configuration.Android;
using AvaloniaSyncer.Sections.Connections.Configuration.Local;
using AvaloniaSyncer.Sections.Connections.Configuration.SeaweedFS;
using AvaloniaSyncer.Sections.Connections.Configuration.Sftp;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI;

namespace AvaloniaSyncer.Sections.Connections;

public class CreateNewConnectionDialogViewModel : ReactiveValidationObject, IResult<IConfiguration>
{
    private readonly IEnumerable<string> existingConfigurationNames;
    private readonly TaskCompletionSource<IConfiguration> tcs;

    public CreateNewConnectionDialogViewModel(IEnumerable<string> existingConfigurationNames)
    {
        this.existingConfigurationNames = existingConfigurationNames;
        tcs = new TaskCompletionSource<IConfiguration>();
        Name = "";
        this.ValidationRule(x => x.Name, s => !string.IsNullOrEmpty(s), "Invalid name");
        this.ValidationRule(x => x.Name, s => !IsTaken(s), "The name is taken");
        this.ValidationRule(x => x.SelectedPlugin, s => s is not null, "Required");

        Create = ReactiveCommand.Create(OnCreate, this.IsValid());
        Plugins = new IPlugin[]
        {
            OperatingSystem.IsAndroid() ? new AndroidPlugin() : new LocalPlugin(),       
            new SeaweedFSPlugin(),
            new SftpPlugin(),
        };
    }

    public ReactiveCommand<Unit, Unit> Create { get; set; }

    [Reactive] public string Name { get; set; }
    public IList<IPlugin> Plugins { get; }
    [Reactive] public IPlugin? SelectedPlugin { get; set; }

    public Task<IConfiguration> Result => tcs.Task;

    private void OnCreate()
    {
        tcs.SetResult(SelectedPlugin!.CreateConfig(Name));
    }

    private bool IsTaken(string? name)
    {
        return existingConfigurationNames.ToList().Contains(name, comparer: StringComparer.InvariantCultureIgnoreCase);
    }
}