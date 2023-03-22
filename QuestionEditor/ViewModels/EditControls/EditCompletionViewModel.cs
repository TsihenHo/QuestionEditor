using QuestionEditor.Data.Question;

namespace QuestionEditor.ViewModels.EditControls;

public class EditCompletionViewModel : EditViewModel<Completion>
{
    private string _questionContent = "";
    private string _answerContent = "";
    private string _analysisContent = "";

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

    public override void Clear()
    {
        AnalysisContent = "";
        QuestionContent = "";
        AnswerContent = "";
    }

    public override Completion Generate()
    {
        if (QuestionContent == "") throw new IncompleteInfoException("题目未填写。");
        if (AnswerContent == "") throw new IncompleteInfoException("答案未填写。");
        
        return new Completion
            { QuestionContent = QuestionContent, AnswerContent = AnswerContent, Analysis = AnalysisContent };
    }

    public override QuestionType Type => QuestionType.Completion;
    public override void LoadFrom(Completion question)
    {
        Id = question.Id;
        AnalysisContent = question.Analysis;
        AnswerContent = question.AnswerContent;
        QuestionContent = question.QuestionContent;
    }
}