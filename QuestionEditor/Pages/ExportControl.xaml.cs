using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HandyControl.Controls;
using QuestionEditor.Dialogs;
using QuestionEditor.Utils;

namespace QuestionEditor.Pages;

public partial class ExportControl : UserControl
{
    public ExportControl()
    {
        InitializeComponent();
    }

    private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "题目 (*.txt)|*.txt",
            Multiselect = false,
            CheckFileExists = false
        };
        var result = openFileDialog.ShowDialog();
        if (result != true) return;
        var path = openFileDialog.FileName;

        try
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            ((App)Application.Current).Mgr.Export(path);
            Dialog.Show(new TextDialog("成功"));
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            Dialog.Show(new ExceptionDialog(exception.ToString()));
        }
    }
}