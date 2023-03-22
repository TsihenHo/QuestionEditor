using System;

namespace QuestionEditor.Data.Question;

/// <summary>
/// 所有题目均继承自本类。
/// </summary>
[Serializable]
public abstract class Question
{
    /// <summary>
    /// 题目类型。
    /// </summary>
    public abstract QuestionType Type { get; }
    
    /// <summary>
    /// 题目 id，用于识别题目。之所以不是 get-only，是因为有些场景需要更改题目的 id 以保证 id 连续。
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 题目内容。
    /// </summary>
    public virtual string QuestionContent { get; set; } = "";
    
    /// <summary>
    /// 题目答案。
    /// </summary>
    public virtual string AnswerContent { get; set; } = "";

    /// <summary>
    /// 解析。
    /// </summary>
    public string Analysis { get; set; } = "";
}

public enum QuestionType
{
    MultipleChoice,
    Completion,
    StepByStep
}