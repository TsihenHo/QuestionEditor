using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HandyControl.Controls;
using QuestionEditor.Annotations;
using QuestionEditor.Data.Manager;
using QuestionEditor.Data.Question;
using QuestionEditor.Dialogs;
using QuestionEditor.ViewModels.EditControls;
using TextBox = System.Windows.Controls.TextBox;

namespace QuestionEditor.Pages;

public partial class AddNewControl : INotifyPropertyChanged
{
    private bool _isSubmitClickable = true;
    private bool _isClearClickable = true;
    private string _submitBtnText = StrSubmit;
    private string _clearBtnText = StrClear;
    private const string StrSubmit = "提交";
    private const string StrClear = "清空";
    private const string StrSubmitSuccess = "成功";
    private const string StrClearSuccess = "如你所愿";
    private const string StrClearMakeSure = "确认清空？";
    private static SuperManager Mgr => ((App)Application.Current).Mgr;

    private QuestionType NowFocus { get; set; } = QuestionType.Completion;
    public EditCompletionViewModel VmCompletion { get; } = new();
    public EditMultipleChoiceViewModel VmMultipleChoice { get; } = new();
    public EditStepByStepViewModel VmStepByStep { get; } = new();

    public bool IsSubmitClickable
    {
        get => _isSubmitClickable;
        set
        {
            if (value == _isSubmitClickable) return;
            _isSubmitClickable = value;
            OnPropertyChanged();
        }
    }

    public bool IsClearClickable
    {
        get => _isClearClickable;
        set
        {
            if (value == _isClearClickable) return;
            _isClearClickable = value;
            OnPropertyChanged();
        }
    }

    public string SubmitBtnText
    {
        get => _submitBtnText;
        private set
        {
            if (value == _submitBtnText) return;
            _submitBtnText = value;
            OnPropertyChanged();
        }
    }

    public string ClearBtnText
    {
        get => _clearBtnText;
        private set
        {
            if (value == _clearBtnText) return;
            _clearBtnText = value;
            OnPropertyChanged();
        }
    }

    private bool IsSureClear { get; set; }


    public AddNewControl()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void OnGotFocus(object sender, RoutedEventArgs e)
    {
        NowFocus = ((FrameworkElement)sender).DataContext switch
        {
            EditViewModel<StepByStep> => QuestionType.StepByStep,
            EditViewModel<Completion> => QuestionType.Completion,
            EditViewModel<MultipleChoice> => QuestionType.MultipleChoice,
            _ => NowFocus
        };
    }

    private void Submit()
    {
        try
        {
            // 让文本框失去焦点
            SubmitBtn.Focus();
            switch (NowFocus)
            {
                case QuestionType.Completion:
                    Mgr.Add(VmCompletion.Generate());
                    break;
                case QuestionType.MultipleChoice:
                    Mgr.Add(VmMultipleChoice.Generate());
                    break;
                case QuestionType.StepByStep:
                    Mgr.Add(VmStepByStep.Generate());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(null);
            }

            Clear();
        }
        catch (IncompleteInfoException e)
        {
            Dialog.Show(new TextDialog($"错误：{e.Message}"));
        }
    }

    private void SetEditQuestionFocus()
    {
        // 我知道这么做很不优雅，但是着实没办法。
        var ctrl = (UserControl)TabControl.SelectedContent;
        var question = (TextBox)ctrl.GetType().GetField("Question", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(ctrl)!;
        question.Focus();
    }

    private void Clear()
    {
        // 让文本框失去焦点
        SubmitBtn.Focus();

        switch (NowFocus)
        {
            case QuestionType.Completion:
                VmCompletion.Clear();
                break;
            case QuestionType.MultipleChoice:
                VmMultipleChoice.Clear();
                break;
            case QuestionType.StepByStep:
                VmStepByStep.Clear();
                break;
            default:
                throw new ArgumentOutOfRangeException(null);
        }

        SetEditQuestionFocus();
    }

    private void SubmitClick(object? sender, RoutedEventArgs? e)
    {
        Submit();
        IsSubmitClickable = false;
        SubmitBtnText = StrSubmitSuccess;
        new Thread(() =>
        {
            Thread.Sleep(2000);
            SubmitBtnText = StrSubmit;
            IsSubmitClickable = true;
        }).Start();
    }

    private void ClearClick(object sender, RoutedEventArgs e)
    {
        if (IsSureClear)
        {
            Clear();
            IsSureClear = false;
            ClearBtnText = StrClearSuccess;
            IsClearClickable = false;
            new Thread(() =>
            {
                Thread.Sleep(2000);
                ClearBtnText = StrClear;
                IsClearClickable = true;
            }).Start();
        }
        else
        {
            IsSureClear = true;
            ClearBtnText = StrClearMakeSure;
            IsClearClickable = false;
            new Thread(() =>
            {
                Thread.Sleep(1000);
                IsClearClickable = true;

                Thread.Sleep(5000);
                if (!IsSureClear) return;
                IsSureClear = false;
                ClearBtnText = StrClear;
            }).Start();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void AddNewControl_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        if (Keyboard.Modifiers != ModifierKeys.Control) return;
        if (IsSubmitClickable)
        {
            SubmitClick(null, null);
        }
    }
}