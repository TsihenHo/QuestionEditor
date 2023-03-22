using System.Windows.Controls;

namespace QuestionEditor.Dialogs;

public partial class ExceptionDialog : UserControl
{
    public ExceptionDialog(string err)
    {
        InitializeComponent();

        Text.Text = err;
        Btn.Focus();
    }
}