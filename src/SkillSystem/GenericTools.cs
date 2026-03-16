using System.Diagnostics;

namespace SkillSystem;

public class GenericTools
{
    public string ExecuteCommand(string command)
    {
        var blockedCommands = new[] { "del", "format", "fdisk", "shutdown", "taskkill" };
        foreach (var blocked in blockedCommands)
        {
            if (command.Contains(blocked, StringComparison.OrdinalIgnoreCase))
            {
                return $"❌ 위험한 명령어는 실행할 수 없습니다: {blocked}";
            }
        }

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return "❌ 프로세스를 시작할 수 없습니다.";
            }

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(error))
            {
                return $"❌ 오류: {error}";
            }

            return string.IsNullOrWhiteSpace(output) 
                ? "명령어가 실행되었으나 출력이 없습니다." 
                : output;
        }
        catch (Exception ex)
        {
            return $"❌ 실행 오류: {ex.Message}";
        }
    }

    public string DownloadFile(string url, string? savePath = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "❌ URL 이 필요합니다.";
            }

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                return "❌ URL 은 http:// 또는 https:// 로 시작해야 합니다.";
            }

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);
            
            var data = client.GetByteArrayAsync(url).Result;
            
            var fileName = savePath ?? Path.GetFileName(new Uri(url).LocalPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = $"downloaded_{DateTime.Now:yyyyMMdd_HHmmss}";
            }

            var directory = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(fileName, data);
            
            return $"✅ 파일 다운로드 완료: {fileName} ({data.Length} bytes)";
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
        {
            return "❌ 다운로드 시간이 초과되었습니다 (30 초).";
        }
        catch (Exception ex)
        {
            return $"❌ 다운로드 오류: {ex.Message}";
        }
    }

    public string InstallPackage(string packageName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                return "❌ 패키지 이름이 필요합니다.";
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = $"install --id {packageName} --silent --accept-package-agreements --accept-source-agreements",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return "❌ winget 을 실행할 수 없습니다. Windows 10/11 에서만 사용 가능합니다.";
            }

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                return $"✅ 패키지 설치 완료: {packageName}\n{output}";
            }
            else if (error.Contains("Found no matching package"))
            {
                return $"❌ 패키지를 찾을 수 없습니다: {packageName}";
            }
            else
            {
                return $"❌ 설치 오류 (종료 코드: {process.ExitCode})\n{error}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ winget 실행 오류: {ex.Message}";
        }
    }

    public string SearchPackage(string packageName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                return "❌ 검색어가 필요합니다.";
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = $"search {packageName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return "❌ winget 을 실행할 수 없습니다.";
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return $"winget 검색 결과:\n{output}";
        }
        catch (Exception ex)
        {
            return $"❌ 검색 오류: {ex.Message}";
        }
    }

    public string GetSystemInfo()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c systeminfo | findstr /B /C:\"OS 이름\" /C:\"OS 버전\" /C:\"시스템 종류\" /C:\"총 물리적 메모리\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return "❌ 시스템 정보를 가져올 수 없습니다.";
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return $"시스템 정보:\n{output}";
        }
        catch (Exception ex)
        {
            return $"❌ 오류: {ex.Message}";
        }
    }

    public string GetProcessList()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "tasklist",
                Arguments = "/FO TABLE /NH",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return "❌ 프로세스 목록을 가져올 수 없습니다.";
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var lines = output.Split('\n').Take(20).ToArray();
            return $"실행 중인 프로세스 (상위 20 개):\n{string.Join("\n", lines)}";
        }
        catch (Exception ex)
        {
            return $"❌ 오류: {ex.Message}";
        }
    }

    public string SearchWeb(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return "❌ 검색어가 필요합니다.";
            }

            var encodedQuery = Uri.EscapeDataString(query);
            var searchUrl = $"https://www.google.com/search?q={encodedQuery}";
            
            Process.Start(new ProcessStartInfo
            {
                FileName = searchUrl,
                UseShellExecute = true
            });

            return $"🌐 '{query}'(으) 로 웹 검색을 시작합니다.\n브라우저가 열립니다: {searchUrl}";
        }
        catch (Exception ex)
        {
            return $"❌ 검색 오류: {ex.Message}";
        }
    }

    public string GetUrlContent(string url)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "❌ URL 이 필요합니다.";
            }

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            
            var content = client.GetStringAsync(url).Result;
            
            var preview = content.Length > 500 
                ? content.Substring(0, 500) + "... (내용이 잘렸습니다)" 
                : content;

            return $"URL 내용 ({url}):\n\n{preview}";
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
        {
            return "❌ 요청 시간이 초과되었습니다 (10 초).";
        }
        catch (Exception ex)
        {
            return $"❌ 가져오기 오류: {ex.Message}";
        }
    }

    public string ReadFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return $"❌ 파일을 찾을 수 없습니다: {filePath}";
            }

            var content = File.ReadAllText(filePath);
            var lines = content.Split('\n');
            
            if (lines.Length > 100)
            {
                var preview = string.Join("\n", lines.Take(100));
                return $"파일 내용 (상위 100 줄):\n\n{preview}\n\n... (총 {lines.Length} 줄)";
            }

            return $"파일 내용 ({filePath}):\n\n{content}";
        }
        catch (Exception ex)
        {
            return $"❌ 읽기 오류: {ex.Message}";
        }
    }

    public string WriteFile(string filePath, string content)
    {
        try
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, content);
            return $"✅ 파일 작성 완료: {filePath} ({content.Length} bytes)";
        }
        catch (Exception ex)
        {
            return $"❌ 쓰기 오류: {ex.Message}";
        }
    }

    public string DeleteFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return $"❌ 파일을 찾을 수 없습니다: {filePath}";
            }

            File.Delete(filePath);
            return $"✅ 파일 삭제 완료: {filePath}";
        }
        catch (Exception ex)
        {
            return $"❌ 삭제 오류: {ex.Message}";
        }
    }

    public string ListDirectory(string? path = null)
    {
        try
        {
            var targetPath = string.IsNullOrWhiteSpace(path) 
                ? Directory.GetCurrentDirectory() 
                : path;

            if (!Directory.Exists(targetPath))
            {
                return $"❌ 디렉토리를 찾을 수 없습니다: {targetPath}";
            }

            var directories = Directory.GetDirectories(targetPath)
                .Select(Path.GetFileName)
                .OrderBy(d => d)
                .ToArray();

            var files = Directory.GetFiles(targetPath)
                .Select(Path.GetFileName)
                .OrderBy(f => f)
                .ToArray();

            var result = new System.Text.StringBuilder();
            result.AppendLine($"📁 디렉토리: {targetPath}");
            result.AppendLine();
            result.AppendLine($"📂 폴더 ({directories.Length}개):");
            foreach (var dir in directories.Take(20))
            {
                result.AppendLine($"  {dir}/");
            }
            result.AppendLine();
            result.AppendLine($"📄 파일 ({files.Length}개):");
            foreach (var file in files.Take(20))
            {
                result.AppendLine($"  {file}");
            }

            if (directories.Length > 20 || files.Length > 20)
            {
                result.AppendLine($"\n... (일부만 표시됨)");
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"❌ 오류: {ex.Message}";
        }
    }
}
