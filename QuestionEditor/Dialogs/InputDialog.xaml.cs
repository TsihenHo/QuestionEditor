using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Interactivity;
using QuestionEditor.Annotations;

namespace QuestionEditor.Dialogs;

public partial class InputDialog : INotifyPropertyChanged
{
    private string _input;
    private Action<string>? OnOkBtnClick { get; }

    public string Input
    {
        get => _input;
        set
        {
            if (value == _input) return;
            _input = value;
            OnPropertyChanged();
        }
    }

    public InputDialog(Action<string>? onOkClick = null, string defaultText = "")
    {
        OnOkBtnClick = onOkClick;
        _input = defaultText;
        
        InitializeComponent();

        DataContext = this;
    }

    public InputDialog AddButton(string name, Action onClick)
    {
        var btn =
            new Button
            {
                Style = FindResource("ButtonPrimary") as Style,
                Content = name,
                Margin = new Thickness(10d),
                HorizontalAlignment = HorizontalAlignment.Center
            };
        btn.Click += (_, _) => onClick();
        btn.Click += (_, _) => ControlCommands.Close.Execute(this, this);
        StackPanel.Children.Add(btn);

        return this;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        OnOkBtnClick?.Invoke(Input);
        ControlCommands.Close.Execute(this, this);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}