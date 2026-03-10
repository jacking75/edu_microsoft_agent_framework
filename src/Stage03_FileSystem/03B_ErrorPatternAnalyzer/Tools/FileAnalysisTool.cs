namespace _03B_ErrorPatternAnalyzer.Tools;

/// <summary>
/// 에러 패턴 분석을 위한 Tool
/// </summary>
public class FileAnalysisTool
{
    /// <summary>
    /// 파일에서 에러 라인을 추출합니다
    /// </summary>
    public List<string> ExtractErrors(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<string> { $"Error: File not found - {filePath}" };
        }

        var lines = File.ReadAllLines(filePath);
        return lines.Where(l => l.Contains("[ERROR]", StringComparison.OrdinalIgnoreCase)).ToList();
    }

    /// <summary>
    /// 에러 패턴을 분석하여 빈도를 계산합니다
    /// </summary>
    public Dictionary<string, int> AnalyzeErrorPatterns(string filePath)
    {
        var errors = ExtractErrors(filePath);
        var patternCount = new Dictionary<string, int>();

        foreach (var error in errors)
        {
            // 에러 타입 추출 (예: NullReferenceException, TimeoutException)
            var errorType = ExtractErrorType(error);
            if (!string.IsNullOrEmpty(errorType))
            {
                if (!patternCount.ContainsKey(errorType))
                {
                    patternCount[errorType] = 0;
                }
                patternCount[errorType]++;
            }
        }

        return patternCount;
    }

    /// <summary>
    /// 에러 메시지에서 에러 타입을 추출합니다
    /// </summary>
    private string ExtractErrorType(string errorLine)
    {
        var exceptionTypes = new[] { "NullReferenceException", "TimeoutException", 
            "InvalidOperationException", "ArgumentException", "IOException" };
        
        foreach (var type in exceptionTypes)
        {
            if (errorLine.Contains(type, StringComparison.OrdinalIgnoreCase))
            {
                return type;
            }
        }
        
        // 일반 에러 패턴
        if (errorLine.Contains("Failed", StringComparison.OrdinalIgnoreCase))
            return "OperationFailed";
        if (errorLine.Contains("Connection", StringComparison.OrdinalIgnoreCase))
            return "ConnectionError";
        
        return "Unknown";
    }
}
