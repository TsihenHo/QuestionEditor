using System;
using System.Windows.Input;

namespace QuestionEditor.Utils;

/// <summary>
/// Command 类的简单实例。
/// </summary>
public class CommandBase : ICommand
{
    public Func<object?, bool>? DoCanExecute { get; init; }
    public Action<object?>? DoExecute { get; init; }

    public bool CanExecute(object? parameter)
    {
        return DoCanExecute?.Invoke(parameter) ?? throw new NullReferenceException(nameof(DoCanExecute));
    }

    public void Execute(object? parameter)
    {
        if (DoExecute is null)
        {
            throw new NullReferenceException(nameof(DoExecute));
        }

        DoExecute(parameter);
    }

    public event EventHandler? CanExecuteChanged;
}