using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using QuestionEditor.Data.Manager;
using MessageBox = System.Windows.MessageBox;

namespace QuestionEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string DbFileName = "QuestionEditor.db";

        public static readonly string DbFullPath =
            Path.Combine(Environment.GetEnvironmentVariable("UserProfile")!, "Documents", DbFileName);

        public SuperManager Mgr { get; }

        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            Mgr = new SuperManager(DbFullPath);

            var oldPath = Path.Combine(Environment.GetEnvironmentVariable("UserProfile")!, "Documents",
                "QuestionEditor.sqlite");

            if (!File.Exists(oldPath)) return;
            Mgr.MergeFromLegacy(oldPath);
            MessageBox.Show(
                $"从旧数据库导入数据成功，请进入“管理”界面查看情况。" +
                $"\n由于选择题的特殊性，它们的答案被放在了“解析”项目里面，并且程序将答案中首次出现的ABCD作为答案，请务必去检查 ！！！" +
                $"\n旧的数据库已经重命名为{DbFileName}.legacy.backup。" +
                $"\n点击确认以关闭应用，然后请自行重启。",
                "提示",
                MessageBoxButton.OK, MessageBoxImage.Information
            );
            Environment.Exit(0);
        }

        private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), e.Exception.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            e.SetObserved();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString(), "糟糕", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void App_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            MessageBox.Show(e.Exception.ToString(), e.Exception.Message, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}