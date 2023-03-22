using System;
using QuestionEditor.Data.Question;

namespace QuestionEditor.ViewModels.EditControls;

public class EditMultipleChoiceViewModel : EditViewModel<MultipleChoice>
{
    private string _questionContent = "";
    private Options _answerContent;
    private string _analysisContent = "";
    private string _optionA = "";
    private string _optionB = "";
    private string _optionC = "";
    private string _optionD = "";

    public string QuestionContent
    {
        get => _questionContent;
        set
        {
            if (value == _questionContent) return;
            _questionContent = value;
            OnPropertyChanged();
        }
    }

    public Options AnswerContent
    {
        get => _answerContent;
        set
        {
            if (value == _answerContent) return;
            _answerContent = value;
            OnPropertyChanged();
        }
    }

    public string AnalysisContent
    {
        get => _analysisContent;
        set
        {
            if (value == _analysisContent) return;
            _analysisContent = value;
            OnPropertyChanged();
        }
    }


    public string OptionA
    {
        get => _optionA;
        set
        {
            if (value == _optionA) return;
            _optionA = value;
            OnPropertyChanged();
        }
    }

    public string OptionB
    {
        get => _optionB;
        set
        {
            if (value == _optionB) return;
            _optionB = value;
            OnPropertyChanged();
        }
    }

    public string OptionC
    {
        get => _optionC;
        set
        {
            if (value == _optionC) return;
            _optionC = value;
            OnPropertyChanged();
        }
    }

    public string OptionD
    {
        get => _optionD;
        set
        {
            if (value == _optionD) return;
            _optionD = value;
            OnPropertyChanged();
        }
    }

    public override void Clear()
    {
        AnalysisContent = "";
        QuestionContent = "";
        AnswerContent = Options.A;
        OptionA = "";
        OptionB = "";
        OptionC = "";
        OptionD = "";
    }

    public override MultipleChoice Generate()
    {
        if (QuestionContent == "") throw new IncompleteInfoException("题目未填写。");
        if (OptionA == "") throw new IncompleteInfoException("选项A未填写。");
        if (OptionB == "") throw new IncompleteInfoException("选项B未填写。");
        if (AnswerContent == Options.C && OptionC == "") throw new IncompleteInfoException("答案是选项C，但选项C未填写。");
        if (AnswerContent == Options.D && OptionD == "") throw new IncompleteInfoException("答案是选项D，但选项D未填写。");
        if (OptionC == "" && OptionD != "") throw new IncompleteInfoException("选项D填写了，但选项C未填写。");

        return new MultipleChoice
        {
            QuestionContent = QuestionContent, AnswerContent = AnswerContent.ToString(), Analysis = AnalysisContent,
            OptionA = OptionA, OptionB = OptionB, OptionC = OptionC, OptionD = OptionD
        };
    }

    public override QuestionType Type => QuestionType.MultipleChoice;
    public override void LoadFrom(MultipleChoice question)
    {
        Id = question.Id;
        QuestionContent = question.QuestionContent;
        AnswerContent = (Options)Enum.Parse(typeof(Options), question.AnswerContent);
        AnalysisContent = question.Analysis;
        OptionA = question.OptionA;
        OptionB = question.OptionB;
        OptionC = question.OptionC;
        OptionD = question.OptionD;
    }
}

public enum Options
{
    A,
    B,
    C,
    D
}