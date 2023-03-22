using System.Collections.ObjectModel;
using System.Linq;
using QuestionEditor.Data.Question;

namespace QuestionEditor.ViewModels.EditControls;

public class EditStepByStepViewModel : EditViewModel<StepByStep>
{
    private string _questionContent = "";
    private string _answerContent = "";
    private string _analysisContent = "";
    private string _newClue = "";

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

    public string AnswerContent
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

    public ObservableCollection<string> Clues { get; } = new();

    public string NewClue
    {
        get => _newClue;
        set
        {
            if (value == _newClue) return;
            _newClue = value;
            OnPropertyChanged();
        }
    }

    public void DoAddClue()
    {
        Clues.Add(NewClue);
        NewClue = "";
    }

    public override void Clear()
    {
        AnalysisContent = "";
        QuestionContent = "";
        AnswerContent = "";
        NewClue = "";
        Clues.Clear();
    }

    public override StepByStep Generate()
    {
        if (QuestionContent == "") throw new IncompleteInfoException("题目未填写。");
        if (AnswerContent == "") throw new IncompleteInfoException("答案未填写。");
        if (Clues.Count == 0) throw new IncompleteInfoException("至少添加一条线索。");
        
        return new StepByStep
        {
            QuestionContent = QuestionContent, AnswerContent = AnswerContent,
            Analysis = AnalysisContent, Clues = Clues.ToList()
        };
    }

    public override QuestionType Type => QuestionType.StepByStep;
    public override void LoadFrom(StepByStep question)
    {
        Id = question.Id;
        QuestionContent = question.QuestionContent;
        AnswerContent = question.AnswerContent;
        AnalysisContent = question.Analysis;
        Clues.Clear();
        foreach (var clue in question.Clues)
        {
            Clues.Add(clue);
        }
    }
}