namespace _03A_LogSummarizer.Tools;

/// <summary>
/// 로그 파일 처리를 위한 Tool
/// </summary>
public class FileReadTool
{
    /// <summary>
    /// 텍스트 파일을 읽어서 내용을 반환합니다
    /// </summary>
    public string ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return $"Error: File not found - {filePath}";
        }
        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// 파일의 마지막 N 줄을 읽습니다
    /// </summary>
    public string ReadLastLines(string filePath, int lineCount = 50)
    {
        if (!File.Exists(filePath))
        {
            return $"Error: File not found - {filePath}";
        }

        var lines = File.ReadAllLines(filePath);
        var takeCount = Math.Min(lineCount, lines.Length);
        var lastLines = lines.Skip(Math.Max(0, lines.Length - takeCount));
        
        return string.Join("\n", lastLines);
    }

    /// <summary>
    /// 파일에서 특정 패턴을 검색합니다
    /// </summary>
    public string SearchPattern(string filePath, string pattern)
    {
        if (!File.Exists(filePath))
        {
            return $"Error: File not found - {filePath}";
        }

        var lines = File.ReadAllLines(filePath);
        var matches = lines.Where(l => l.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        
        if (!matches.Any())
        {
            return $"No matches found for pattern: {pattern}";
        }

        return $"Found {matches.Count()} matches:\n" + string.Join("\n", matches.Take(20));
    }
}
