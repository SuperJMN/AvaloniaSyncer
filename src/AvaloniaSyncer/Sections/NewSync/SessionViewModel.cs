using Zafiro.FileSystem;

namespace AvaloniaSyncer.Sections.NewSync;

public class SessionViewModel
{
    public IZafiroDirectory Source { get; }
    public IZafiroDirectory Destination { get; }
    public string Description { get; }

    public SessionViewModel(IZafiroDirectory source, IZafiroDirectory destination)
    {
        Source = source;
        Destination = destination;
        Description = $"{source} => {destination}";
    }
}