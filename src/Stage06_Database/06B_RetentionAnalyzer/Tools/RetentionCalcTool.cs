namespace _06B_RetentionAnalyzer.Tools;

using Microsoft.Data.Sqlite;

/// <summary>
/// 리텐션 분석을 위한 SQL Tool
/// </summary>
public class RetentionCalcTool
{
    private readonly string _connectionString;

    public RetentionCalcTool(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }

    /// <summary>
    /// 일별 리텐션률을 계산합니다
    /// </summary>
    public string CalculateDailyRetention(int days = 7)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = """
            WITH DailyLogins AS (
                SELECT 
                    DATE(LastLoginDate) as LoginDate,
                    COUNT(DISTINCT PlayerId) as DailyUsers
                FROM Players
                GROUP BY DATE(LastLoginDate)
                ORDER BY LoginDate DESC
                LIMIT @Days
            )
            SELECT LoginDate, DailyUsers FROM DailyLogins
            """;
        
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Days", days);

        using var reader = cmd.ExecuteReader();
        var results = new List<string>();
        
        while (reader.Read())
        {
            results.Add($"{reader["LoginDate"]}: {reader["DailyUsers"]}명");
        }

        return $"""
            일별 로그인 통계 (최근 {days}일):
            
            {string.Join("\n", results)}
            
            리텐션 분석:
            - 등락 패턴을 확인하세요
            - 주말/평일 비교
            - 이벤트 기간과 비교
            """;
    }

    /// <summary>
    /// 주별 리텐션률을 계산합니다
    /// </summary>
    public string CalculateWeeklyRetention(int weeks = 4)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = """
            SELECT 
                STRFTIME('%Y-W%W', LastLoginDate) as Week,
                COUNT(DISTINCT PlayerId) as WeeklyUsers
            FROM Players
            GROUP BY STRFTIME('%Y-W%W', LastLoginDate)
            ORDER BY Week DESC
            LIMIT @Weeks
            """;
        
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Weeks", weeks);

        using var reader = cmd.ExecuteReader();
        var results = new List<string>();
        
        while (reader.Read())
        {
            results.Add($"{reader["Week"]}: {reader["WeeklyUsers"]}명");
        }

        return $"""
            주별 로그인 통계 (최근 {weeks}주):
            
            {string.Join("\n", results)}
            """;
    }

    /// <summary>
    /// 활성/비활성 플레이어를 분류합니다
    /// </summary>
    public string GetActiveInactivePlayers(int inactiveDays = 30)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var activeSql = """
            SELECT COUNT(*) FROM Players 
            WHERE DATE(LastLoginDate) >= DATE('now', ?)
            """;
        
        using var activeCmd = new SqliteCommand(activeSql, connection);
        activeCmd.Parameters.AddWithValue("@p1", $"-{inactiveDays} days");
        var activeCount = activeCmd.ExecuteScalar();

        var totalSql = "SELECT COUNT(*) FROM Players";
        using var totalCmd = new SqliteCommand(totalSql, connection);
        var totalCount = totalCmd.ExecuteScalar();

        var inactiveCount = (long)totalCount - (long)activeCount;
        var retentionRate = (double)(long)activeCount / (long)totalCount * 100;

        return $"""
            플레이어 활성도 분석:
            
            - 활성 플레이어 ({inactiveDays}일이내): {activeCount}명
            - 비활성 플레이어: {inactiveCount}명
            - 총 플레이어: {totalCount}명
            
            리텐션율: {retentionRate:F1}%
            
            {(retentionRate < 50 ? "⚠️ 리텐션율이 낮습니다. 개선이 필요합니다." : "✅ 양호한 리텐션율입니다.")}
            """;
    }
}
