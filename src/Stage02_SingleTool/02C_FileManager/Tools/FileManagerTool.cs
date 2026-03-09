using System.ComponentModel;

namespace _02C_FileManager.Tools;

/// <summary>
/// 파일 관리 작업을 위한 Tool
/// </summary>
public class FileManagerTool
{
    [Description("텍스트 파일의 내용을 읽습니다.")]
    public string ReadFile(
        [Description("읽을 파일의 경로")] string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return $"Error: File not found - {filePath}";
            }
            return File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    [Description("텍스트 파일에 내용을 작성합니다. 기존 내용은 삭제됩니다.")]
    public string WriteFile(
        [Description("작성할 파일의 경로")] string filePath,
        [Description("파일에 작성할 내용")] string content)
    {
        try
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(filePath, content);
            return $"Success: File written - {filePath} ({content.Length} bytes)";
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    [Description("파일의 내용을 기존 내용에 추가합니다.")]
    public string AppendToFile(
        [Description("추가할 파일의 경로")] string filePath,
        [Description("추가할 내용")] string content)
    {
        try
        {
            File.AppendAllText(filePath, content);
            return $"Success: Content appended - {filePath}";
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    [Description("지정된 디렉토리의 파일과 폴더 목록을 조회합니다.")]
    public string ListDirectory(
        [Description("목록을 조회할 디렉토리 경로 (기본값: 현재 디렉토리)")] string? path = null)
    {
        try
        {
            var targetPath = path ?? Directory.GetCurrentDirectory();
            
            if (!Directory.Exists(targetPath))
            {
                return $"Error: Directory not found - {targetPath}";
            }

            var directories = Directory.GetDirectories(targetPath);
            var files = Directory.GetFiles(targetPath);

            var result = new System.Text.StringBuilder();
            result.AppendLine($"Directory: {targetPath}");
            result.AppendLine();
            result.AppendLine("Folders:");
            foreach (var dir in directories)
            {
                result.AppendLine($"  [DIR]  {Path.GetFileName(dir)}");
            }
            result.AppendLine();
            result.AppendLine("Files:");
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                result.AppendLine($"  [FILE] {Path.GetFileName(file)} ({fileInfo.Length} bytes)");
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    [Description("파일을 삭제합니다. 신중하게 사용하세요.")]
    public string DeleteFile(
        [Description("삭제할 파일의 경로")] string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return $"Error: File not found - {filePath}";
            }
            
            File.Delete(filePath);
            return $"Success: File deleted - {filePath}";
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    [Description("파일을 다른 위치로 복사합니다.")]
    public string CopyFile(
        [Description("소스 파일 경로")] string sourcePath,
        [Description("대상 파일 경로")] string destPath)
    {
        try
        {
            if (!File.Exists(sourcePath))
            {
                return $"Error: Source file not found - {sourcePath}";
            }
            
            var destDirectory = Path.GetDirectoryName(destPath);
            if (!string.IsNullOrEmpty(destDirectory) && !Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            
            File.Copy(sourcePath, destPath, overwrite: true);
            return $"Success: File copied - {sourcePath} -> {destPath}";
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
}
