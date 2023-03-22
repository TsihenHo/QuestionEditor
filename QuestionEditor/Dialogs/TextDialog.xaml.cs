using System.Windows.Controls;

namespace QuestionEditor.Dialogs;

public partial class TextDialog : UserControl
{
    public TextDialog(string text)
    {
        InitializeComponent();

        Text.Text = text;
        Btn.Focus();
    }
}