using System;
using QuestionEditor.Data.Question;

namespace QuestionEditor.ViewModels.EditControls;

public abstract class EditViewModel<TQuestionType> : ViewModelBase where TQuestionType : Question
{
    public abstract void Clear();

    /// <summary>
    /// 生成 Question
    /// </summary>
    /// <returns></returns>
    /// <exception cref="IncompleteInfoException"></exception>
    public abstract TQuestionType Generate();

    public abstract QuestionType Type { get; }

    public abstract void LoadFrom(TQuestionType question);

    public long Id { get; set; } = -1;
}

/// <summary>
/// 填写的信息不完整时抛出
/// </summary>
public class IncompleteInfoException : Exception
{
    public IncompleteInfoException(string message) : base(message)
    {
    }
}