using System.Data;
using System.Data.SQLite;
using System.IO;
using QuestionEditor.Data.Question;

namespace QuestionEditor.Data.Manager;

public class MultipleChoiceManager : BaseManager<MultipleChoice>
{
    #region SQL 常量

    public const string TableName = SuperManager.SqlApi.TableNameMultipleChoice;

    public const string FieldId = "id";
    public const string FieldQuestion = "question";
    public const string FieldAnswer = "answer";
    public const string FieldAnalysis = "analysis";
    public const string FieldOptionA = "op_a";
    public const string FieldOptionB = "op_b";
    public const string FieldOptionC = "op_c";
    public const string FieldOptionD = "op_d";
    
    private const string FieldAtId = $"@{FieldId}";
    private const string FieldAtQuestion = $"@{FieldQuestion}";
    private const string FieldAtAnswer = $"@{FieldAnswer}";
    private const string FieldAtAnalysis = $"@{FieldAnalysis}";
    private const string FieldAtOptionA = $"@{FieldOptionA}";
    private const string FieldAtOptionB = $"@{FieldOptionB}";
    private const string FieldAtOptionC = $"@{FieldOptionC}";
    private const string FieldAtOptionD = $"@{FieldOptionD}";

    private const string FieldAtTemp = "@1";

    #endregion

    #region SQL 命令
    
    // ReSharper disable RedundantStringInterpolation

    private const string CmdCreateTableMultipleChoice =
        $"CREATE TABLE IF NOT EXISTS {TableName} " +
        $"( " +
        $"{FieldId} INTEGER PRIMARY KEY AUTOINCREMENT , " +
        $"{FieldQuestion} TEXT, " +
        $"{FieldAnswer} TEXT, " +
        $"{FieldAnalysis} TEXT, " +
        $"{FieldOptionA} TEXT, " +
        $"{FieldOptionB} TEXT, " +
        $"{FieldOptionC} TEXT, " +
        $"{FieldOptionD} TEXT" +
        $");";

    private const string CmdGetCount =
        $"SELECT COUNT({FieldId}) FROM {TableName};";

    private const string CmdAdd =
        $"INSERT INTO {TableName} " +
        $"( " +
        $"{FieldId}, " +
        $"{FieldQuestion}, " +
        $"{FieldAnswer}, " +
        $"{FieldAnalysis}, " +
        $"{FieldOptionA}, " +
        $"{FieldOptionB}, " +
        $"{FieldOptionC}, " +
        $"{FieldOptionD}" +
        $") " +
        $"VALUES " +
        $"( " +
        $"{FieldAtId}, " +
        $"{FieldAtQuestion}, " +
        $"{FieldAtAnswer}, " +
        $"{FieldAtAnalysis}, " +
        $"{FieldAtOptionA}, " +
        $"{FieldAtOptionB}, " +
        $"{FieldAtOptionC}, " +
        $"{FieldAtOptionD}" +
        $");";
    
    private const string CmdRemoveById =
        $"DELETE FROM {TableName} WHERE {FieldId} = {FieldAtId};";

    private const string CmdDecreaseId =
        $"UPDATE {TableName} SET {FieldId} = {FieldId} - 1 WHERE {FieldId} > {FieldAtId};";

    private const string CmdUpdate =
        $"UPDATE {TableName} " +
        $"SET " +
        $"{FieldQuestion} = {FieldAtQuestion}, " +
        $"{FieldAnswer} = {FieldAtAnswer}, " +
        $"{FieldAnalysis} = {FieldAtAnalysis}, " +
        $"{FieldOptionA} = {FieldAtOptionA}, " +
        $"{FieldOptionB} = {FieldAtOptionB}, " +
        $"{FieldOptionC} = {FieldAtOptionC}, " +
        $"{FieldOptionD} = {FieldAtOptionD} " +
        $"WHERE " +
        $"{FieldId} = {FieldAtId};";

    private const string CmdReadAll =
        $"SELECT * FROM {TableName} " +
        $"WHERE {FieldQuestion} LIKE '%' || {FieldAtTemp} || '%' " +
        $"OR {FieldAnswer} LIKE '%' || {FieldAtTemp} || '%' " +
        $"OR {FieldAnalysis}  LIKE '%' || {FieldAtTemp} || '%'" +
        $"OR {FieldOptionA}  LIKE '%' || {FieldAtTemp} || '%'" +
        $"OR {FieldOptionB}  LIKE '%' || {FieldAtTemp} || '%'" +
        $"OR {FieldOptionC}  LIKE '%' || {FieldAtTemp} || '%'" +
        $"OR {FieldOptionD}  LIKE '%' || {FieldAtTemp} || '%';";
    
    private const string CmdLoadById = $"SELECT * FROM {TableName} WHERE {FieldId} = {FieldAtId}";

    // ReSharper restore RedundantStringInterpolation
    
    #endregion

    public override QuestionType Type => QuestionType.MultipleChoice;
    
    public override void Add(MultipleChoice question)
    {
        question.Id = GetCount();

        using var conn = SqlApi.CreateConn();
        using var cmdAdd = new SQLiteCommand(CmdAdd, conn);

        cmdAdd.BindArgs(
            FieldAtId.To(question.Id),
            FieldAtQuestion.To(question.QuestionContent),
            FieldAtAnswer.To(question.AnswerContent),
            FieldAtAnalysis.To(question.Analysis),
            FieldAtOptionA.To(question.OptionA),
            FieldAtOptionB.To(question.OptionB),
            FieldAtOptionC.To(question.OptionC),
            FieldAtOptionD.To(question.OptionD)
        );

        cmdAdd.ExecuteNonQuery();
    }

    public override void RemoveById(long id)
    {
        using var conn = SqlApi.CreateConn();

        // 1. 删除    2. 更新 id
        using var cmdRm = new SQLiteCommand(CmdRemoveById, conn);
        cmdRm.BindArgs(FieldAtId.To(id));
        cmdRm.ExecuteNonQuery();

        using var cmdDecreaseId = new SQLiteCommand(CmdDecreaseId, conn);
        cmdDecreaseId.BindArgs(FieldAtId.To(id));
        cmdDecreaseId.ExecuteNonQuery();
    }

    public override void UpdateById(long id, MultipleChoice newQuestion)
    {
        using var conn = SqlApi.CreateConn();
        using var cmdUpdate = new SQLiteCommand(CmdUpdate, conn);
        cmdUpdate.BindArgs(
            FieldAtId.To(id),
            FieldAtQuestion.To(newQuestion.QuestionContent),
            FieldAtAnswer.To(newQuestion.AnswerContent),
            FieldAtAnalysis.To(newQuestion.Analysis),
            FieldAtOptionA.To(newQuestion.OptionA),
            FieldAtOptionB.To(newQuestion.OptionB),
            FieldAtOptionC.To(newQuestion.OptionC),
            FieldAtOptionD.To(newQuestion.OptionD)
        );
        cmdUpdate.ExecuteNonQuery();
    }

    public override int GetCount()
    {
        using var conn = SqlApi.CreateConn();
        using var cmdGetCount = new SQLiteCommand(CmdGetCount, conn);
        using var readerCount = cmdGetCount.ExecuteReader();
        readerCount.Read();
        return readerCount.GetInt32(0);
    }

    public override MultipleChoice LoadById(long id)
    {
        
        using var conn = SqlApi.CreateConn();

        using var cmdLoadQues = new SQLiteCommand(CmdLoadById, conn);
        cmdLoadQues.BindArgs(FieldAtId.To(id));
        using var readerQues = cmdLoadQues.ExecuteReader();
        readerQues.Read();

        return new MultipleChoice
        {
            Id = readerQues.GetInt64(0),
            QuestionContent = readerQues.GetString(1),
            AnswerContent = readerQues.GetString(2),
            Analysis = readerQues.GetString(3),
            OptionA = readerQues.GetString(4),
            OptionB = readerQues.GetString(5),
            OptionC = readerQues.GetString(6),
            OptionD = readerQues.GetString(7),
        };
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
        dt.Columns[4].ColumnName = "A";
        dt.Columns[5].ColumnName = "B";
        dt.Columns[6].ColumnName = "C";
        dt.Columns[7].ColumnName = "D";
        
        return dt;
    }

    public override void Export(StreamWriter writer)
    {
        var dt = ReadAll("");
        foreach (DataRow row in dt.Rows)
        {
            var obj = row.ItemArray;
            var id = obj[0]!.ToString();
            var ques = obj[1]!.ToString()!.Encode();
            var ans = obj[2]!.ToString()!.Encode();
            var anal = obj[3]!.ToString()!.Encode();
            var a = obj[4]!.ToString()!.Encode();
            var b = obj[5]!.ToString()!.Encode();
            var c = obj[6]!.ToString()!.Encode();
            var d = obj[7]!.ToString()!.Encode();
            writer.WriteLine($"{id} 0");
            writer.WriteLine(ques);
            writer.WriteLine(a);
            writer.WriteLine(b);
            writer.WriteLine(c);
            writer.WriteLine(d);
            writer.WriteLine(ans);
            writer.WriteLine(anal);
        }
    }

    public MultipleChoiceManager(SuperManager.SqlApi api) : base(api)
    {
        using var conn = SqlApi.CreateConn();
        using var cmdCreateTable = new SQLiteCommand(CmdCreateTableMultipleChoice, conn);
        cmdCreateTable.ExecuteNonQuery();
    }
}