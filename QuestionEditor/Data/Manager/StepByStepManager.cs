using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using QuestionEditor.Data.Question;

namespace QuestionEditor.Data.Manager;

public class StepByStepManager : BaseManager<StepByStep>
{
    /*
     * 说在前面，由于这个题目类型比较特殊，所以需要两个表来存储，
     * 第一个表储存题目的除了 Clues 的所有信息；
     * 第二个表存储每个题目的 Clues，列如下：
     * 题目id ClueId Clue内容
     */

    #region SQL 常量

    public const string TableName = SuperManager.SqlApi.TableNameStepByStep;

    public const string FieldId = "id";
    public const string FieldQuestion = "question";
    public const string FieldAnswer = "answer";
    public const string FieldAnalysis = "analysis";

    private const string FieldAtId = $"@{FieldId}";
    private const string FieldAtQuestion = $"@{FieldQuestion}";
    private const string FieldAtAnswer = $"@{FieldAnswer}";
    private const string FieldAtAnalysis = $"@{FieldAnalysis}";

    public const string TableNameClues = SuperManager.SqlApi.TableNameClues;

    public const string FieldClueId = "id";
    public const string FieldClueIndex = "_index";
    public const string FieldClueContent = "content";

    private const string FieldAtClueId = $"@{FieldClueId}";
    private const string FieldAtClueIndex = $"@{FieldClueIndex}";
    private const string FieldAtClueContent = $"@{FieldClueContent}";

    private const string FieldAtTemp = "@1";

    #endregion

    #region SQL 命令

    // ReSharper disable RedundantStringInterpolation

    private const string CmdCreateTableStepByStep =
        $"CREATE TABLE IF NOT EXISTS {TableName} " +
        $"( " +
        $"{FieldId} INTEGER PRIMARY KEY AUTOINCREMENT , " +
        $"{FieldQuestion} TEXT, " +
        $"{FieldAnswer} TEXT, " +
        $"{FieldAnalysis} TEXT" +
        $");";

    private const string CmdGetQuestionsCount =
        $"SELECT COUNT({FieldId}) FROM {TableName};";

    private const string CmdAddQuestion =
        $"INSERT INTO {TableName} " +
        $"(" +
        $"{FieldId}, " +
        $"{FieldQuestion}, " +
        $"{FieldAnswer}, " +
        $"{FieldAnalysis}" +
        $") " +
        $"VALUES " +
        $"(" +
        $"{FieldAtId}, " +
        $"{FieldAtQuestion}, " +
        $"{FieldAtAnswer}, " +
        $"{FieldAtAnalysis}" +
        $");";

    private const string CmdRemoveQuestionById =
        $"DELETE FROM {TableName} WHERE {FieldId} = {FieldAtId};";

    private const string CmdRemoveCluesById =
        $"DELETE FROM {TableNameClues} WHERE {FieldClueId} = {FieldAtClueId};";

    private const string CmdDecreaseQuestionId =
        $"UPDATE {TableName} SET {FieldId} = {FieldId} - 1 WHERE {FieldId} > {FieldAtId};";

    private const string CmdDecreaseClueId =
        $"UPDATE {TableNameClues} SET {FieldId} = {FieldId} - 1 WHERE {FieldClueId} > {FieldAtClueId};";

    private const string CmdUpdateQuestion =
        $"UPDATE {TableName} " +
        $"SET " +
        $"{FieldQuestion} = {FieldAtQuestion}, " +
        $"{FieldAnswer} = {FieldAtAnswer}, " +
        $"{FieldAnalysis} = {FieldAtAnalysis} " +
        $"WHERE " +
        $"{FieldId} = {FieldAtId};";

    private const string CmdCreateTableClues =
        $"CREATE TABLE IF NOT EXISTS {TableNameClues} " +
        $"( " +
        $"{FieldClueId} INTEGER, " +
        $"{FieldClueIndex} INTEGER, " +
        $"{FieldClueContent} TEXT" +
        $");";

    private const string CmdLoadById = $"SELECT * FROM {TableName} WHERE {FieldId} = {FieldAtId}";

    private const string CmdLoadCluesById =
        $"SELECT {FieldClueContent} FROM {TableNameClues} WHERE {FieldClueId} = {FieldAtId} ORDER BY {FieldClueIndex}";

    // private const string CmdGetCluesCount =
    // $"SELECT COUNT({FieldId}) FROM {TableNameClues} WHERE {FieldClueId} = {FieldAtClueId};";

    private const string CmdAddClue =
        $"INSERT INTO {TableNameClues} " +
        $"(" +
        $"{FieldClueId}, " +
        $"{FieldClueIndex}, " +
        $"{FieldClueContent}" +
        $") " +
        $"VALUES " +
        $"(" +
        $"{FieldAtClueId}, " +
        $"{FieldAtClueIndex}, " +
        $"{FieldAtClueContent}" +
        $");";

    private const string CmdReadAll =
        $"SELECT * FROM {TableName} " +
        $"WHERE {FieldQuestion} LIKE '%' || {FieldAtTemp} || '%' " +
        $"OR {FieldAnswer} LIKE '%' || {FieldAtTemp} || '%' " +
        $"OR {FieldAnalysis}  LIKE '%' || {FieldAtTemp} || '%';";

    // private const string CmdRemoveClueByIdAndIndex =
    // $"DELETE FROM {TableNameClues} WHERE {FieldClueId} = {FieldAtClueId} AND {FieldClueIndex} = {FieldAtClueIndex};";

    // private const string CmdDecreaseClueIndex =
    // $"UPDATE {TableNameClues} SET {FieldId} = {FieldId} - 1 WHERE {FieldClueId} = {FieldAtClueId} AND {FieldClueIndex} > {FieldAtClueIndex};";

    // ReSharper restore RedundantStringInterpolation

    #endregion

    public override QuestionType Type => QuestionType.StepByStep;

    public override void Add(StepByStep question)
    {
        question.Id = GetCount();
        using var conn = SqlApi.CreateConn();
        // 1.添加问题  2.添加线索
        using var cmdAddQues = new SQLiteCommand(CmdAddQuestion, conn);
        cmdAddQues.BindArgs(
            FieldAtId.To(question.Id),
            FieldAtQuestion.To(question.QuestionContent),
            FieldAtAnswer.To(question.AnswerContent),
            FieldAtAnalysis.To(question.Analysis)
        );
        cmdAddQues.ExecuteNonQuery();

        using var cmdAddClues = new SQLiteCommand(CmdAddClue, conn);
        var index = 0;
        foreach (var clue in question.Clues)
        {
            cmdAddClues.Reset();
            cmdAddClues.BindArgs(
                FieldAtClueId.To(question.Id),
                FieldAtClueIndex.To(index),
                FieldAtClueContent.To(clue)
            );
            cmdAddClues.ExecuteNonQuery();
            index += 1;
        }
    }

    public override void RemoveById(long id)
    {
        using var conn = SqlApi.CreateConn();
        // 1.删除问题 2.删除线索 3.问题递减id 4.线索递减id
        using var cmdRmQues = new SQLiteCommand(CmdRemoveQuestionById, conn);
        cmdRmQues.BindArgs(FieldAtId.To(id));
        cmdRmQues.ExecuteNonQuery();

        using var cmdRmClues = new SQLiteCommand(CmdRemoveCluesById, conn);
        cmdRmClues.BindArgs(FieldAtClueId.To(id));
        cmdRmClues.ExecuteNonQuery();

        using var cmdDecreaseQuesId = new SQLiteCommand(CmdDecreaseQuestionId, conn);
        cmdDecreaseQuesId.BindArgs(FieldAtId.To(id));
        cmdDecreaseQuesId.ExecuteNonQuery();

        using var cmdDecreaseClueId = new SQLiteCommand(CmdDecreaseClueId, conn);
        cmdDecreaseClueId.BindArgs(FieldAtClueId.To(id));
        cmdDecreaseClueId.ExecuteNonQuery();
    }

    public override void UpdateById(long id, StepByStep newQuestion)
    {
        using var conn = SqlApi.CreateConn();
        // 1.更新问题  2.删除线索  3.更新线索
        using var cmdUpdateQues = new SQLiteCommand(CmdUpdateQuestion, conn);
        cmdUpdateQues.BindArgs(
            FieldAtId.To(id),
            FieldAtQuestion.To(newQuestion.QuestionContent),
            FieldAtAnswer.To(newQuestion.AnswerContent),
            FieldAtAnalysis.To(newQuestion.Analysis)
        );
        cmdUpdateQues.ExecuteNonQuery();

        using var cmdRmClues = new SQLiteCommand(CmdRemoveCluesById, conn);
        cmdRmClues.BindArgs(FieldAtClueId.To(id));
        cmdRmClues.ExecuteNonQuery();

        using var cmdAddClues = new SQLiteCommand(CmdAddClue, conn);
        var index = 0;
        foreach (var clue in newQuestion.Clues)
        {
            cmdAddClues.Reset();
            cmdAddClues.BindArgs(
                FieldAtClueId.To(id),
                FieldAtClueIndex.To(index),
                FieldAtClueContent.To(clue)
            );
            cmdAddClues.ExecuteNonQuery();
            index += 1;
        }
    }

    public override int GetCount()
    {
        using var conn = SqlApi.CreateConn();
        using var cmdGetCount = new SQLiteCommand(CmdGetQuestionsCount, conn);
        using var readerCount = cmdGetCount.ExecuteReader();
        readerCount.Read();
        return readerCount.GetInt32(0);
    }

    public override StepByStep LoadById(long id)
    {
        using var conn = SqlApi.CreateConn();

        using var cmdLoadQues = new SQLiteCommand(CmdLoadById, conn);
        cmdLoadQues.BindArgs(FieldAtId.To(id));
        using var readerQues = cmdLoadQues.ExecuteReader();
        readerQues.Read();

        var res = new StepByStep
        {
            Id = readerQues.GetInt64(0),
            QuestionContent = readerQues.GetString(1),
            AnswerContent = readerQues.GetString(2),
            Analysis = readerQues.GetString(3),
            Clues = GetCluesById(id)
        };

        return res;
    }

    public override DataTable ReadAll(string contains)
    {
        using var conn = SqlApi.CreateConn();
        using var cmdRead = new SQLiteCommand(CmdReadAll, conn);
        cmdRead.BindArgs(FieldAtTemp.To(contains));
        var adapter = new SQLiteDataAdapter(cmdRead);
        var dt = new DataTable();
        adapter.Fill(dt);

        foreach (DataColumn dtColumn in dt.Columns)
        {
            dtColumn.ReadOnly = true;
        }

        dt.Columns[0].ColumnName = "Id";
        dt.Columns[1].ColumnName = "题目";
        dt.Columns[2].ColumnName = "答案";
        dt.Columns[3].ColumnName = "解析";

        return dt;
    }

    public override void Export(StreamWriter writer)
    {
        var dt = ReadAll("");
        foreach (DataRow row in dt.Rows)
        {
            var obj = row.ItemArray;
            var id = (long)obj[0]!;
            var ques = obj[1]!.ToString()!.Encode();
            var ans = obj[2]!.ToString()!.Encode();
            var anal = obj[3]!.ToString()!.Encode();
            var clues = GetCluesById(id);
            writer.WriteLine($"{id} 2");
            writer.WriteLine(ques);
            ExportClues(writer, clues);
            writer.WriteLine(ans);
            writer.WriteLine(anal);
        }
    }

    private void ExportClues(StreamWriter writer, List<string> clues)
    {
        const int neededCluesNum = 3;
        var delta = clues.Count - neededCluesNum;
        IEnumerable<string> finalClues;
        if (delta >= 0)
        {
            finalClues = clues.Take(3);
        }
        else
        {
            finalClues = new List<string>(clues);
            for (var i = 0; i < -delta; i++)
            {
                ((List<string>)finalClues).Add("（没线索了）");
            }
        }
        
        foreach (var finalClue in finalClues)
        {
            writer.WriteLine(finalClue);
        }
    }

    private List<string> GetCluesById(long id)
    {
        using var conn = SqlApi.CreateConn();
        using var cmdLoadClues = new SQLiteCommand(CmdLoadCluesById, conn);
        cmdLoadClues.BindArgs(FieldAtId.To(id));
        using var readerClues = cmdLoadClues.ExecuteReader();
        var clues = new List<string>();

        while (readerClues.Read())
        {
            clues.Add(readerClues.GetString(0));
        }

        return clues;
    }

    public StepByStepManager(SuperManager.SqlApi api) : base(api)
    {
        using var conn = SqlApi.CreateConn();

        using var cmdCreateTable = new SQLiteCommand(CmdCreateTableStepByStep, conn);
        cmdCreateTable.ExecuteNonQuery();

        using var cmdCreateTableClues = new SQLiteCommand(CmdCreateTableClues, conn);
        cmdCreateTableClues.ExecuteNonQuery();
    }
}