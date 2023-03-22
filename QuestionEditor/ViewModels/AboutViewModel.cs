using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Input;
using QuestionEditor.Utils;

namespace QuestionEditor.ViewModels;

public class AboutViewModel
{
    private ICommand? _githubCommand;

    public ICommand GithubCommand
    {
        get
        {
            return _githubCommand ??= new CommandBase
            {
                DoCanExecute = _ => true,
                DoExecute = obj =>
                {
                    var link = obj as string;
                    OpenInBrowser(link ?? "");
                }
            };
        }
    }
    
    
    public static void OpenInBrowser(string url)
    {
        // 判断操作系统是否为 Windows
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        // 修正格式
        url = url.Replace("&", "^&");
        // ProcessStartInfo 的使用是必需的，因为我们需要用cmd命令打开目标网站
        // 设置它的 CreateNoWindow = true 以防止 cmd 窗口出现
        // ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true };
        Process.Start(
            new ProcessStartInfo(
                "cmd",
                $"/c start {url}"
            ) { CreateNoWindow = true }
        );
    }
    
}