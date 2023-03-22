using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HandyControl.Controls;
using QuestionEditor.Data.Manager;
using QuestionEditor.Data.Question;
using QuestionEditor.Dialogs;
using QuestionEditor.Utils;
using QuestionEditor.ViewModels;

namespace QuestionEditor.Pages;

public partial class GeneralControl : UserControl
{
    private static SuperManager Mgr => ((App)Application.Current).Mgr;

    public GeneralControl()
    {
        InitializeComponent();
    }

    private void GeneralControl_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        Process.Start(
            new ProcessStartInfo(
                "explorer.exe",
                $"/select, {App.DbFullPath}"
            )
        );
    }

    private void ImportTxtStepByStep(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "题目 (*.txt)|*.txt",
            Multiselect = false,
            CheckFileExists = true
        };
        var result = openFileDialog.ShowDialog();
        if (result != true) return;

        var path = openFileDialog.FileName;
        var lines = File.ReadAllLines(path);

        if (lines.Length % 6 != 0)
        {
            Dialog.Show(new TextDialog("文件行数不是6的倍数：可以试着在末尾添加空行"));
            return;
        }

        var i = 0;
        var q = new StepByStep();

        foreach (var line in lines)
        {
            switch (i % 6)
            {
                case 0:
                    q.QuestionContent = line;
                    break;
                case 1 or 2 or 3:
                    q.Clues.Add(line);
                    break;
                case 4:
                    q.AnswerContent = line;
                    break;
                case 5:
                    ((App)Application.Current).Mgr.Add(q);
                    q = new StepByStep();
                    break;
            }

            i += 1;
        }

        Dialog.Show(new TextDialog($"成功，导入了{i / 6}条题目"));
    }
}