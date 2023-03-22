using System.Collections.Generic;

namespace QuestionEditor.Data.Question;

public class StepByStep : Question
{
    /// <summary>
    /// 线索列表。
    /// </summary>
    public IList<string> Clues { get; init; } = new List<string>();

    public override QuestionType Type => QuestionType.StepByStep;
}