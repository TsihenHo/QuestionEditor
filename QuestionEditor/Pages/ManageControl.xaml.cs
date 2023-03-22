using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuestionEditor.ViewModels;

namespace QuestionEditor.Pages;

public partial class ManageControl : UserControl
{
    private ManageViewModel ViewModel => (ManageViewModel)DataContext;

    public ManageControl()
    {
        InitializeComponent();
        DataContext = new ManageViewModel(this);
    }

    private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;

        ViewModel.SearchText = SearchBox.Text;
    }

    private void DeleteCompletion(object sender, RoutedEventArgs e)
    {
        ViewModel.Delete();
    }

    private void EditCompletion(object sender, RoutedEventArgs e)
    {
        ViewModel.Edit();
    }

    private void CompletionGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        ViewModel.UpdateSelectedId();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel.UpdateDataGrid();
    }
}