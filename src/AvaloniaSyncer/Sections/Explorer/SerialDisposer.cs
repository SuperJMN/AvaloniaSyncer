using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace AvaloniaSyncer.Sections.Explorer;

public class SerialDisposer<T> : IDisposable
{
    private readonly SerialDisposable serialDisposable = new();
    private readonly IDisposable disposable;

    public SerialDisposer(IObservable<T> observable)
    {
        disposable = observable
            .Select(arg => arg as IDisposable)
            .WhereNotNull()
            .Do(d => serialDisposable.Disposable = d).Subscribe();
    }
    
    public void Dispose()
    {
        serialDisposable.Dispose();
        disposable.Dispose();
    }
}