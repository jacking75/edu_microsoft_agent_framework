namespace _06C_NaturalLanguageSql.Tools;

/// <summary>
/// NL-to-SQL 변환 및 검증 Tool
/// </summary>
public class SqlGeneratorTool
{
    /// <summary>
    /// 자연어를 SQL 쿼리로 변환합니다 (검증 필요)
    /// </summary>
    public string GenerateSql(string naturalLanguage)
    {
        // 실제로는 LLM 이 SQL 생성
        // 이 예제에서는 간단한 매핑만 구현
        return naturalLanguage.ToLower() switch
        {
            var q when q.Contains("플레이어 수") => "SELECT COUNT(*) FROM Players",
            var q when q.Contains("상위") && q.Contains("레벨") => 
                "SELECT PlayerName, Level FROM Players ORDER BY Level DESC LIMIT 10",
            var q when q.Contains("평균 레벨") => "SELECT AVG(Level) FROM Players",
            _ => "-- 지원하지 않는 쿼리입니다."
        };
    }

    /// <summary>
    /// SQL 쿼리의 안전성을 검증합니다
    /// </summary>
    public ValidationResult ValidateSql(string sql)
    {
        var dangerous = new[] { "DROP", "DELETE", "UPDATE", "INSERT", "ALTER", "TRUNCATE" };
        
        foreach (var keyword in dangerous)
        {
            if (sql.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return new ValidationResult 
                { 
                    IsValid = false, 
                    ErrorMessage = $"위험한 키워드 감지: {keyword}" 
                };
            }
        }

        if (!sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
        {
            return new ValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = "SELECT 쿼리만 허용됩니다." 
            };
        }

        return new ValidationResult { IsValid = true, Message = "안전한 쿼리입니다." };
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Message { get; set; }
}
