using System;
using System.Linq;

namespace QuestionEditor.Data.Question;

public class MultipleChoice : Question
{
    private static readonly string[] AllowedAnswers = { "A", "B", "C", "D" };
    private string _answerContent = "";
    
    /// <summary>
    /// A 选项内容，以此类推。
    /// </summary>
    public string OptionA { get; set; } = "";
    public string OptionB { get; set; } = "";
    public string OptionC { get; set; } = "";
    public string OptionD { get; set; } = "";

    public override QuestionType Type => QuestionType.MultipleChoice;

    public override string AnswerContent
    {
        get => _answerContent;
        set
        {
            if (!AllowedAnswers.Contains(value))
            {
                throw new ArgumentException("选择题答案只能是“A”“B”“C”“D”中的某一个。");
            }

            _answerContent = value;
        }
    }
}