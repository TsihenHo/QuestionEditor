using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HandyControl.Controls;
using HandyControl.Tools;
using QuestionEditor.Dialogs;
using QuestionEditor.ViewModels.EditControls;

namespace QuestionEditor.Pages.EditControls;

public partial class EditStepByStepControl
{
    private EditStepByStepViewModel ViewModel => (EditStepByStepViewModel)DataContext;
    public EditStepByStepControl()
    {
        InitializeComponent();
    }

    private void AddClueClick(object? sender, RoutedEventArgs? e)
    {
        ViewModel.DoAddClue();
    }
    private void DataGrid_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        var dataGrid = (DataGrid)sender;
        var pos = e.GetPosition(dataGrid);
        var result = VisualTreeHelper.HitTest(dataGrid, pos);
        if (result == null)
        {
            return;
        }

        var rowItem = VisualHelper.GetParent<DataGridRow>(result.VisualHit);
        if (rowItem == null)
        {
            return;
        }

        var clue = rowItem.Item.ToString()!;
        var index = ViewModel.Clues.IndexOf(clue);
        ShowEditDialog(index);
    }

    private void ShowEditDialog(int index)
    {
        var clues = ViewModel.Clues;
        var clue = clues[index];

        var dialog = new InputDialog(str =>
        {
            clues[index] = str;
        }, clue);
        
        if (index != 0)
        {
            dialog.AddButton("上移", () =>
            {
                (clues[index - 1], clues[index]) = (clues[index], clues[index - 1]);
            });
        }

        if (index != ViewModel.Clues.Count - 1)
        {
            dialog.AddButton("下移", () =>
            {
                (clues[index + 1], clues[index]) = (clues[index], clues[index + 1]);
            });
        }

        dialog.AddButton("删除", () =>
        {
            clues.RemoveAt(index);
        });
        
        Dialog.Show(dialog);
    }


    private void DataGrid_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - e.Delta);
    }

    private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;

        // 该死的文本框只有在失去焦点的时候才会保存文本到 Binding 里面去
        AddClueBtn.Focus();
        AddClueClick(null, null);
        EnterClue.Focus();
    }
}