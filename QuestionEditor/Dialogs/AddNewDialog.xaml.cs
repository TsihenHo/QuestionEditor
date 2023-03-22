using System;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;

namespace QuestionEditor.Dialogs;

public partial class AddNewDialog : UserControl
{
    private readonly Action<string> _action;
    private readonly string _type;

    public AddNewDialog(Action<string> action, string type)
    {
        _action = action;
        _type = type;

        InitializeComponent();
        SetValue(InfoElement.PlaceholderProperty, $"添加{_type}...");
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var name = MainTextBox.Text;

        if (string.IsNullOrEmpty(name))
        {
            Dialog.Show(new TextDialog($"请输入新{_type}名。"));
            return;
        }

        try
        {
            _action(name);
            Dialog.Show(new TextDialog("成功，请关闭。"));
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            Dialog.Show(new TextDialog(exception.ToString()));
        }
    }
}