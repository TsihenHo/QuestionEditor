using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using HandyControl.Tools.Extension;
using QuestionEditor.Data.Question;

namespace QuestionEditor.Data.Manager;

public abstract class BaseManager<TQuestionType>
    where TQuestionType : Question.Question
{
    protected SuperManager.SqlApi SqlApi { get; }
    
    public abstract QuestionType Type { get; }

    protected BaseManager(SuperManager.SqlApi api)
    {
        SqlApi = api;
    }

    /// <summary>
    /// 添加新的题目。
    /// </summary>
    /// <param name="question"></param>
    public abstract void Add(TQuestionType question);

    /// <summary>
    /// 根据序号删除题目。
    /// </summary>
    /// <param name="id"></param>
    public abstract void RemoveById(long id);

    /// <summary>
    /// 根据序号更新题目。
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newQuestion"></param>
    public abstract void UpdateById(long id, TQuestionType newQuestion);

    /// <summary>
    /// 获取数量。
    /// </summary>
    /// <returns></returns>
    public abstract int GetCount();

    public abstract TQuestionType LoadById(long id);

    /// <summary>
    /// 获取包含指定文本的全部数据。
    /// </summary>
    /// <param name="contains"></param>
    /// <returns></returns>
    public abstract DataTable ReadAll(string contains);

    public abstract void Export(StreamWriter writer);
}