namespace _04B_BuildAnalyzer.Tools;

/// <summary>
/// 경고 분류 Tool
/// </summary>
public class WarningClassifierTool
{
    public Dictionary<string, int> ClassifyWarnings(string logPath)
    {
        if (!File.Exists(logPath))
        {
            return new Dictionary<string, int>();
        }

        var lines = File.ReadAllLines(logPath);
        var categories = new Dictionary<string, int>();

        foreach (var line in lines.Where(l => 
            l.Contains("warning", StringComparison.OrdinalIgnoreCase)))
        {
            var category = CategorizeWarning(line);
            if (!categories.ContainsKey(category))
                categories[category] = 0;
            categories[category]++;
        }

        return categories;
    }

    private string CategorizeWarning(string line)
    {
        if (line.Contains("unused", StringComparison.OrdinalIgnoreCase))
            return "사용하지 않는 코드";
        if (line.Contains("deprecated", StringComparison.OrdinalIgnoreCase))
            return "사용 중단된 API";
        if (line.Contains("null", StringComparison.OrdinalIgnoreCase))
            return "Null 참조";
        if (line.Contains("async", StringComparison.OrdinalIgnoreCase))
            return "비동기 코드";
        
        return "기타";
    }

    public string GetSummary(string logPath)
    {
        var categories = ClassifyWarnings(logPath);
        
        if (!categories.Any())
        {
            return "경고가 없습니다.";
        }

        var sorted = categories.OrderByDescending(x => x.Value);
        
        return $"""
            경고 분류 요약:
            
            {string.Join("\n", sorted.Select(c => $"  {c.Key}: {c.Value}개"))}
            
            우선순위:
            1. null 참조 경고 (런타임 에러 가능성)
            2. 사용하지 않는 코드 (코드 정리 필요)
            3. 사용 중단된 API (호환성 문제)
            """;
    }
}
