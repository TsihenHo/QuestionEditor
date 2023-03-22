using System;
using System.Data;
using System.Windows;
using HandyControl.Controls;
using QuestionEditor.Data.Manager;
using QuestionEditor.Data.Question;
using QuestionEditor.Dialogs;
using QuestionEditor.Pages;

namespace QuestionEditor.ViewModels;

public class ManageViewModel : ViewModelBase
{
    private static SuperManager Mgr => ((App)Application.Current).Mgr;
    private readonly ManageControl _ctrl;
    private string _searchText = "";
    private QuestionType _nowType = QuestionType.Completion;

    public DataView GridDataView =>
        Mgr.ReadAll(SearchText, NowType).DefaultView;

    public QuestionType NowType
    {
        get => _nowType;
        set
        {
            if (value == _nowType) return;
            _nowType = value;
            OnPropertyChanged();
            UpdateDataGrid();
            UpdateSelectedId();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (value == _searchText) return;
            _searchText = value;
            OnPropertyChanged();
            UpdateDataGrid();
        }
    }

    public string NowCheckedString => $"选中的题目Id：{SelectedId?.ToString() ?? "无"}";

    private long? SelectedId
    {
        get
        {
            try
            {
                var id = (long?)(_ctrl.DataGrid.SelectedCells?[0].Item as DataRowView)?[0];
                return id;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }
    }

    public void Edit()
    {
        var id = SelectedId;
        if (id is null)
        {
            Dialog.Show(new TextDialog("未选中题目"));
        }
        else
        {
            var dialog = new EditDialog(id.Value, NowType);
            dialog.DoOnSave += UpdateDataGrid;
            Dialog.Show(dialog);
        }
    }

    public void Delete()
    {
        var id = SelectedId;
        if (id is null)
        {
            Dialog.Show(new TextDialog("未选中题目"));
        }
        else
        {
            Mgr.RemoveById(id.Value, NowType);
            UpdateDataGrid();
            Dialog.Show(new TextDialog("成功"));
        }
    }

    public void UpdateSelectedId()
    {
        OnPropertyChanged(nameof(NowCheckedString));
    }

    public void UpdateDataGrid()
    {
        OnPropertyChanged(nameof(GridDataView));
    }

    public ManageViewModel(ManageControl ctrl)
    {
        _ctrl = ctrl;
    }
}