namespace _04B_BuildAnalyzer.Tools;

/// <summary>
/// 빌드 로그 파싱 Tool
/// </summary>
public class BuildLogParserTool
{
    public string ParseBuildLog(string logPath)
    {
        if (!File.Exists(logPath))
        {
            return $"Error: File not found - {logPath}";
        }

        var lines = File.ReadAllLines(logPath);
        var errors = new List<string>();
        var warnings = new List<string>();
        var info = new List<string>();

        foreach (var line in lines)
        {
            if (line.Contains("error", StringComparison.OrdinalIgnoreCase))
                errors.Add(line);
            else if (line.Contains("warning", StringComparison.OrdinalIgnoreCase))
                warnings.Add(line);
            else if (line.Contains("info", StringComparison.OrdinalIgnoreCase))
                info.Add(line);
        }

        return $"""
            빌드 로그 분석:
            
            📊 요약:
            - 에러: {errors.Count}개
            - 경고: {warnings.Count}개
            - 정보: {info.Count}개
            
            {(errors.Count > 0 ? "❌ 빌드 실패 - 에러를 수정하세요." : "✅ 빌드 성공")}
            
            {(warnings.Count > 5 ? $"⚠️ 경고가 많습니다 ({warnings.Count}개)" : "")}
            """;
    }

    public string ExtractErrors(string logPath)
    {
        if (!File.Exists(logPath))
        {
            return $"Error: File not found - {logPath}";
        }

        var lines = File.ReadAllLines(logPath);
        var errors = lines.Where(l => 
            l.Contains("error", StringComparison.OrdinalIgnoreCase)).ToList();

        if (!errors.Any())
        {
            return "에러가 없습니다.";
        }

        return $"""
            발견된 에러 ({errors.Count}개):
            
            {string.Join("\n", errors.Take(10))}
            
            {(errors.Count > 10 ? $"...외 {errors.Count - 10}개" : "")}
            """;
    }
}
