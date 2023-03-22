using System;
using System.Windows;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Interactivity;
using QuestionEditor.Data.Manager;
using QuestionEditor.Data.Question;
using QuestionEditor.Pages.EditControls;
using QuestionEditor.ViewModels;
using QuestionEditor.ViewModels.EditControls;

namespace QuestionEditor.Dialogs;

public partial class EditDialog
{
    private static SuperManager Mgr => ((App)Application.Current).Mgr;
    private readonly QuestionType _type;
    private ViewModelBase _vm;

    public delegate void OnSave();

    public event OnSave? DoOnSave;

    public EditDialog(long id, QuestionType type)
    {
        _type = type;
        InitializeComponent();
        var ques = Mgr.LoadQuestionById(id, type);
        switch (type)
        {
            case QuestionType.Completion:
            {
                _vm = new EditCompletionViewModel();
                ((EditCompletionViewModel)_vm).LoadFrom((Completion)ques);
                var content = new EditCompletionControl
                {
                    DataContext = _vm
                };
                StackPanel.Children.Insert(0, content);
                break;
            }
            case QuestionType.MultipleChoice:
            {
                _vm = new EditMultipleChoiceViewModel();
                ((EditMultipleChoiceViewModel)_vm).LoadFrom((MultipleChoice)ques);
                var content = new EditMultipleChoiceControl
                {
                    DataContext = _vm
                };
                StackPanel.Children.Insert(0, content);
                break;
            }
            case QuestionType.StepByStep:
            {
                _vm = new EditStepByStepViewModel();
                ((EditStepByStepViewModel)_vm).LoadFrom((StepByStep)ques);
                var content = new EditStepByStepControl
                {
                    DataContext = _vm
                };
                StackPanel.Children.Insert(0, content);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private void SaveClick(object sender, RoutedEventArgs e)
    {
        try
        {
            switch (_type)
            {
                case QuestionType.Completion:
                {
                    var v = (EditCompletionViewModel)_vm;
                    Mgr.UpdateById(v.Id, v.Generate());
                    break;
                }
                case QuestionType.MultipleChoice:
                {
                    var v = (EditMultipleChoiceViewModel)_vm;
                    Mgr.UpdateById(v.Id, v.Generate());
                    break;
                }
                case QuestionType.StepByStep:
                {
                    var v = (EditStepByStepViewModel)_vm;
                    Mgr.UpdateById(v.Id, v.Generate());
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(null);
            }

            OnDoOnSave();
            ControlCommands.Close.Execute(this, this);
        }
        catch (IncompleteInfoException ex)
        {
            Dialog.Show(new TextDialog($"错误：{ex.Message}"));
        }
    }

    private void NotSaveClick(object sender, RoutedEventArgs e)
    {
        ControlCommands.Close.Execute(this, this);
    }

    protected virtual void OnDoOnSave()
    {
        DoOnSave?.Invoke();
    }

    private void StackPanel_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - e.Delta);
    }
}