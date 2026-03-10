namespace _06A_PlayerStatsBot.Tools;

using Microsoft.Data.Sqlite;

/// <summary>
/// 안전한 SQL 쿼리 실행 Tool
/// </summary>
public class SqlQueryTool
{
    private readonly string _connectionString;

    public SqlQueryTool(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }

    /// <summary>
    /// 플레이어 통계를 조회합니다 (파라미터화 된 쿼리로 SQL 인젝션 방지)
    /// </summary>
    public string GetPlayerStats(string playerId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = "SELECT PlayerId, PlayerName, Level, TotalPlayTime, LastLoginDate FROM Players WHERE PlayerId = @PlayerId";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@PlayerId", playerId);

        using var reader = cmd.ExecuteReader();
        if (!reader.HasRows)
        {
            return $"플레이어 {playerId} 를 찾을 수 없습니다.";
        }

        reader.Read();
        return $"""
            플레이어 통계:
            - ID: {reader["PlayerId"]}
            - 이름: {reader["PlayerName"]}
            - 레벨: {reader["Level"]}
            - 총 플레이 시간: {reader["TotalPlayTime"]}시간
            - 마지막 로그인: {reader["LastLoginDate"]}
            """;
    }

    /// <summary>
    /// 상위 N 명 플레이어를 조회합니다
    /// </summary>
    public string GetTopPlayers(int limit = 10)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = $"SELECT PlayerName, Level FROM Players ORDER BY Level DESC LIMIT @Limit";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Limit", limit);

        using var reader = cmd.ExecuteReader();
        var results = new List<string>();
        
        while (reader.Read())
        {
            results.Add($"{reader["PlayerName"]} (Lv.{reader["Level"]})");
        }

        return $"""
            상위 {limit}명 플레이어:
            
            {string.Join("\n", results)}
            """;
    }

    /// <summary>
    /// 플레이어 수를 조회합니다
    /// </summary>
    public string GetPlayerCount()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = "SELECT COUNT(*) FROM Players";
        using var cmd = new SqliteCommand(sql, connection);
        var result = cmd.ExecuteScalar();

        return $"총 플레이어 수: {result}명";
    }
}
