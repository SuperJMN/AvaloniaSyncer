﻿using System;
using System.Reactive;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI;

namespace AvaloniaSyncer.ViewModels;

public static class ShowDialogHelper
{
    public static async Task<Maybe<TReturn>> Prompt<TViewModel, TReturn>(this IDialogService dialogService, string title, TViewModel instance, string okTitle, Func<TViewModel, ReactiveCommand<Unit, TReturn>> commandFactory)
    {
        Maybe<TReturn> result = Maybe<TReturn>.None;
        IDisposable? subscription = null;
        await dialogService.ShowDialog(instance, title, new OptionConfiguration<TViewModel>(okTitle, context =>
        {
            var command = commandFactory(instance);
            subscription = command.Subscribe(x => result = x);
            return command.Extend(_ => context.Window.Close());
        }));

        subscription!.Dispose();
        return result;
    }
}