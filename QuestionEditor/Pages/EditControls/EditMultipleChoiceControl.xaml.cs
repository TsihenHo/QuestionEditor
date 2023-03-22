using System;
using System.Windows.Input;
using HandyControl.Controls;
using QuestionEditor.Data.Question;
using QuestionEditor.ViewModels.EditControls;
using TextBox = HandyControl.Controls.TextBox;

namespace QuestionEditor.Pages.EditControls;

public partial class EditMultipleChoiceControl
{
    public EditMultipleChoiceControl()
    {
        InitializeComponent();
    }

    private void OnEnterDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;

        var option = ((TextBox)sender).GetValue(InfoElement.PlaceholderProperty).ToString()!;
        ((EditMultipleChoiceViewModel)DataContext).AnswerContent = (Options)Enum.Parse(typeof(Options), option);
    }
}