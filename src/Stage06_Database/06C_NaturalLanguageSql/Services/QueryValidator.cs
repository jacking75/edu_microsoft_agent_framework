namespace _06C_NaturalLanguageSql.Services;

using Microsoft.Data.Sqlite;

/// <summary>
/// SQL 쿼리 검증 서비스
/// </summary>
public class QueryValidator
{
    public bool IsReadOnly(string sql)
    {
        var readOnlyKeywords = new[] { "SELECT" };
        var writeKeywords = new[] { "INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "CREATE" };

        var upperSql = sql.ToUpper().Trim();

        foreach (var keyword in writeKeywords)
        {
            if (upperSql.StartsWith(keyword))
            {
                return false;
            }
        }

        return readOnlyKeywords.Any(k => upperSql.StartsWith(k));
    }

    public int GetLimit(string sql)
    {
        var limitIndex = sql.ToUpper().IndexOf("LIMIT");
        if (limitIndex == -1) return 100; // 기본 제한

        var limitValue = sql.Substring(limitIndex + 5).Trim();
        if (int.TryParse(limitValue, out var limit))
        {
            return Math.Min(limit, 1000); // 최대 1000 행
        }

        return 100;
    }
}
