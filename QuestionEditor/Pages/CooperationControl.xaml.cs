using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using QuestionEditor.Dialogs;

namespace QuestionEditor.Pages;

public partial class CooperationControl : UserControl
{
    public CooperationControl()
    {
        InitializeComponent();
    }

    private void CooperationControl_OnDragDrop(object sender, DragEventArgs e)
    {
        try
        {
            var arr = e.Data.GetData(DataFormats.FileDrop) as Array;
            var fileName = arr?.GetValue(0)?.ToString();
            if (fileName is null)
            {
                Dialog.Show(new TextDialog("你拖了个啥？"), "MainWindow");
                // 没拖呢，不管
                return;
            }

            if (Directory.Exists(fileName))
            {
                Dialog.Show(new TextDialog("我不要文件夹"), "MainWindow");
                return;
            }

            if (!File.Exists(fileName))
            {
                // 什么样的大聪明能够拖进来个不存在的文件？
                Dialog.Show(new TextDialog("文件不存在"), "MainWindow");
                return;
            }

            if (Path.GetFullPath(fileName) == Path.GetFullPath(App.DbFullPath))
            {
                Dialog.Show(new TextDialog("这样会卡死的，收手吧"), "MainWindow");
                return;
            }

            ((App)Application.Current).Mgr.Merge(fileName);
            Block.Text = "成功";
            new Thread(() =>
            {
                Thread.Sleep(500);
                Dispatcher.BeginInvoke(() =>
                {
                    Block.Text = "";
                });
            })
                .Start();
            Console.WriteLine("成功");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            Dialog.Show(new ExceptionDialog(exception.ToString()), "MainWindow");
        }
    }
}