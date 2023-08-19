using Zafiro.FileSystem;

namespace AvaloniaSyncer.ViewModels;

public record SyncSession(IZafiroDirectory Source, IZafiroDirectory Destination);