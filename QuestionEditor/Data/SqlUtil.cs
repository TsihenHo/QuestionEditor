using System.Data.SQLite;

namespace QuestionEditor.Data;

public static class SqlUtil
{
    public static SQLiteParameter To(this string parameterName, object? value) => new(parameterName, value);

    public static void BindArgs(this SQLiteCommand cmd, params SQLiteParameter[] args)
    {
        cmd.Parameters.AddRange(args);
    }

    public static string Encode(this string str)
        => str.Replace(@"\", @"\\").Replace("\r\n", @"\n");
}