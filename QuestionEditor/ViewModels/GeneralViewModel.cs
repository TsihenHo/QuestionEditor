using System.Windows;

namespace QuestionEditor.ViewModels;

public class GeneralViewModel
{
    public string DbPath => $"数据库路径：{App.DbFullPath}";
    public string Version => $"版本号：{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Null"}";
}