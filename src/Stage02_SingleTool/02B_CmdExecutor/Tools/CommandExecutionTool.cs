using System.ComponentModel;
using System.Diagnostics;

namespace _02B_CmdExecutor.Tools;

/// <summary>
/// CMD 명령어 실행을 위한 Tool
/// </summary>
public class CommandExecutionTool
{
    [Description("CMD 명령어를 실행하고 결과를 반환합니다.")]
    public string ExecuteCommand(
        [Description("실행할 명령어 (예: 'dir', 'systeminfo', 'tasklist')")] string command)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();
            
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                return $"Error: {error}";
            }

            return string.IsNullOrEmpty(output) ? "Command executed successfully (no output)" : output;
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    [Description("현재 시스템의 환경 변수 값을 가져옵니다.")]
    public string GetEnvironmentVariable(
        [Description("환경 변수 이름 (예: 'PATH', 'USERNAME', 'COMPUTERNAME')")] string variableName)
    {
        try
        {
            var value = Environment.GetEnvironmentVariable(variableName);
            return value ?? $"Environment variable '{variableName}' not found";
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    [Description("현재 날짜와 시간을 반환합니다.")]
    public string GetCurrentDateTime()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
