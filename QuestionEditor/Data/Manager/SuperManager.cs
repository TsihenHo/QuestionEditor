using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using QuestionEditor.Data.Question;
using MessageBox = HandyControl.Controls.MessageBox;

namespace QuestionEditor.Data.Manager;

public class SuperManager
{
    #region SQL 语句

    // ReSharper disable RedundantStringInterpolation
    private const string FieldAtTemp = "@1";

    private const string CmdAttach = $"ATTACH {FieldAtTemp} AS OTHER;";
    private const string CmdDetach = "DETACH OTHER;";

    private const string CmdMergeLegacyCompletion =
        $"INSERT INTO {CompletionManager.TableName} " +
        $"SELECT NULL , question, answer, '' " +
        $"FROM OTHER.questions " +
        $"WHERE a = '';";

    // 因为“选择题答案只能是‘A’‘B’‘C’‘D’中的某一个。”，所以这里暂且把选择题的答案搁置到解析里。
    private const string CmdMergeLegacyMultipleChoice =
        $"INSERT INTO {MultipleChoiceManager.TableName} " +
        $"SELECT NULL , question, GetAnswer(answer), answer, a, b, c, d " +
        $"FROM OTHER.questions " +
        $"WHERE NOT a = '';";

    private const string CmdMerge =
        $"INSERT INTO {CompletionManager.TableName} " +
        $"SELECT NULL, {CompletionManager.FieldQuestion}, {CompletionManager.FieldAnswer}, {CompletionManager.FieldAnalysis} " +
        $"FROM OTHER.{CompletionManager.TableName}; " +
        $"" +
        $"INSERT INTO {MultipleChoiceManager.TableName} " +
        $"SELECT NULL, {MultipleChoiceManager.FieldQuestion}, {MultipleChoiceManager.FieldAnswer}, {MultipleChoiceManager.FieldAnalysis}, " +
        $"{MultipleChoiceManager.FieldOptionA}, {MultipleChoiceManager.FieldOptionB}, {MultipleChoiceManager.FieldOptionC}, {MultipleChoiceManager.FieldOptionD} " +
        $"FROM OTHER.{MultipleChoiceManager.TableName}; " +
        $"" +
        $"INSERT INTO {StepByStepManager.TableName} " +
        $"SELECT NULL, {StepByStepManager.FieldQuestion}, {StepByStepManager.FieldAnswer}, {StepByStepManager.FieldAnalysis} " +
        $"FROM OTHER.{StepByStepManager.TableName} " +
        $"ORDER BY {StepByStepManager.FieldId};" +
        $"" +
        $"INSERT INTO {StepByStepManager.TableNameClues} " +
        $"SELECT {StepByStepManager.FieldClueId} + {FieldAtTemp}, {StepByStepManager.FieldClueIndex}, {StepByStepManager.FieldClueContent} " +
        $"FROM OTHER.{StepByStepManager.TableNameClues} " +
        $"ORDER BY {StepByStepManager.FieldClueId};";
    // ReSharper restore RedundantStringInterpolation

    #endregion

    private SqlApi Api { get; }
    private CompletionManager CompletionMgr { get; }
    private MultipleChoiceManager MultipleChoiceMgr { get; }
    private StepByStepManager StepByStepMgr { get; }

    public SuperManager(string dbName)
    {
        Api = new SqlApi(dbName);
        CompletionMgr = new CompletionManager(Api);
        MultipleChoiceMgr = new MultipleChoiceManager(Api);
        StepByStepMgr = new StepByStepManager(Api);
    }

    private static readonly Regex ReMatchAnswer = new("[ABCDabcd]", RegexOptions.Compiled);

    public void Export(string path)
    {
        if (File.Exists(path)) File.Delete(path);
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var enc = Encoding.GetEncoding("GBK");
        using var writer = new StreamWriter(path, true, enc);
        writer.AutoFlush = true;
        CompletionMgr.Export(writer);
        MultipleChoiceMgr.Export(writer);
        StepByStepMgr.Export(writer);
    }

    public void MergeFromLegacy(string path)
    {
        // 备份现有数据库，然后合并
        var backup = App.DbFullPath + ".mergefromlegacy.backup";
        if (File.Exists(backup))
        {
            File.Delete(backup);
        }

        File.Copy(App.DbFullPath, backup);

        using var conn = Api.CreateConn();

        conn.BindFunction(new SQLiteFunctionAttribute("GetAnswer", 1, FunctionType.Scalar),
            new SQLiteInvokeDelegate((_, args) =>
            {
                try
                {
                    return ReMatchAnswer.Match(args[0]!.ToString()!).Captures[0];
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show(
                        $"合并答案{args[0]}时出错：在其中找不到ABCD。",
                        "错误",
                        MessageBoxButton.OK, MessageBoxImage.Error
                    );
                    Environment.Exit(1);
                    return 0;
                }
            }), null);

        using var cmdAttach = new SQLiteCommand(CmdAttach, conn);
        cmdAttach.BindArgs(FieldAtTemp.To(path));
        cmdAttach.ExecuteNonQuery();

        using var cmdCom = new SQLiteCommand(CmdMergeLegacyCompletion, conn);
        cmdCom.ExecuteNonQuery();

        using var cmdChoice = new SQLiteCommand(CmdMergeLegacyMultipleChoice, conn);
        cmdChoice.ExecuteNonQuery();

        using var cmdDetach = new SQLiteCommand(CmdDetach, conn);
        cmdDetach.ExecuteNonQuery();

        File.Move(path, path + ".legacy.backup");
    }

    public void Merge(string path)
    {
        // 备份现有数据库，然后合并
        var backup = App.DbFullPath + ".merge.backup";
        if (File.Exists(backup))
        {
            File.Delete(backup);
        }

        File.Copy(App.DbFullPath, backup);
        using var conn = Api.CreateConn();

        using var cmdAttach = new SQLiteCommand(CmdAttach, conn);
        cmdAttach.BindArgs(FieldAtTemp.To(path));
        cmdAttach.ExecuteNonQuery();

        using var cmdMerge = new SQLiteCommand(CmdMerge, conn);
        cmdMerge.BindArgs(FieldAtTemp.To(StepByStepMgr.GetCount()));
        cmdMerge.ExecuteNonQuery();

        using var cmdDetach = new SQLiteCommand(CmdDetach, conn);
        cmdDetach.ExecuteNonQuery();
    }

    public void Add(Question.Question question)
    {
        switch (question)
        {
            case Completion com:
                CompletionMgr.Add(com);
                break;
            case MultipleChoice mc:
                MultipleChoiceMgr.Add(mc);
                break;
            case StepByStep sbs:
                StepByStepMgr.Add(sbs);
                break;
        }
    }

    public void RemoveById(long id, QuestionType type)
    {
        switch (type)
        {
            case QuestionType.Completion:
                CompletionMgr.RemoveById(id);
                break;
            case QuestionType.MultipleChoice:
                MultipleChoiceMgr.RemoveById(id);
                break;
            case QuestionType.StepByStep:
                StepByStepMgr.RemoveById(id);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void UpdateById(long id, Question.Question newQuestion)
    {
        switch (newQuestion)
        {
            case Completion com:
                CompletionMgr.UpdateById(id, com);
                break;
            case MultipleChoice mc:
                MultipleChoiceMgr.UpdateById(id, mc);
                break;
            case StepByStep sbs:
                StepByStepMgr.UpdateById(id, sbs);
                break;
        }
    }

    public Question.Question LoadQuestionById(long id, QuestionType type) =>
        type switch
        {
            QuestionType.Completion => CompletionMgr.LoadById(id),
            QuestionType.MultipleChoice => MultipleChoiceMgr.LoadById(id),
            QuestionType.StepByStep => StepByStepMgr.LoadById(id),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

    public DataTable ReadAll(string contains, QuestionType type) =>
        type switch
        {
            QuestionType.Completion => CompletionMgr.ReadAll(contains),
            QuestionType.MultipleChoice => MultipleChoiceMgr.ReadAll(contains),
            QuestionType.StepByStep => StepByStepMgr.ReadAll(contains),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

    public class SqlApi
    {
        // 'Q' for 'Question', 'E' for 'Extra', 'M' for 'Metadata'
        public const string TableNameCompletion = "Q_Completion";
        public const string TableNameMultipleChoice = "Q_MultipleChoice";
        public const string TableNameStepByStep = "Q_StepByStep";
        public const string TableNameClues = "E_StepByStep_Clues";

        /// <summary>
        /// 数据库名称。
        /// </summary>
        private string DatabaseName { get; }

        public SqlApi(string dbName)
        {
            DatabaseName = dbName;

            if (!File.Exists(DatabaseName))
            {
                File.Create(DatabaseName).Close();
            }
        }

        /// <summary>
        /// 获取数据库的连接。使用完毕后请自行关闭。
        /// </summary>
        /// <returns></returns>
        public SQLiteConnection CreateConn()
        {
            var conn = new SQLiteConnection($"Data Source={DatabaseName};");
            conn.Open();
            return conn;
        }
    }
}