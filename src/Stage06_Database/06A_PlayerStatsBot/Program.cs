// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _06A_PlayerStatsBot.Tools;
using Microsoft.Data.Sqlite;

// ==========================================
// 6 단계 A: 플레이어 통계 조회 봇
// ==========================================
// 학습 목표:
// 1. 안전한 SQL 쿼리 실행
// 2. 파라미터화된 쿼리로 SQL 인젝션 방지
// 3. SQLite 데이터베이스 연동
// ==========================================

Console.WriteLine("📊 플레이어 통계 조회 봇에 오신 것을 환영합니다!");
Console.WriteLine("플레이어 정보를 조회합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 데이터베이스 경로 (샘플 DB 생성 필요)
var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "game.db");

if (!File.Exists(dbPath))
{
    Console.WriteLine("⚠️ 데이터베이스 파일이 없습니다. 샘플 DB 를 생성합니다...");
    CreateSampleDatabase(dbPath);
}

var sqlTool = new SqlQueryTool(dbPath);

var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 게임 플레이어 통계 어시스턴트입니다.
        
        당신의 역할:
        1. 플레이어 ID 로 통계를 조회합니다
        2. 상위 플레이어 랭킹을 보여줍니다
        3. 전체 플레이어 수를 보고합니다
        
        사용 가능한 도구:
        - GetPlayerStats(playerId): 특정 플레이어 통계
        - GetTopPlayers(limit): 상위 N 명 플레이어
        - GetPlayerCount(): 총 플레이어 수
        
        보안 주의사항:
        - SQL 인젝션 방지를 위해 파라미터화된 쿼리 사용
        - 사용자 입력 직접 SQL 에 사용 금지
        """,
    name: "PlayerStatsBot",
    tools: [
        AIFunctionFactory.Create(sqlTool.GetPlayerStats),
        AIFunctionFactory.Create(sqlTool.GetTopPlayers),
        AIFunctionFactory.Create(sqlTool.GetPlayerCount)
    ]
);

Console.WriteLine("✅ 플레이어 통계 봇이 초기화되었습니다.\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"플레이어 1 의 통계를 보여줘\"");
Console.WriteLine("  - \"상위 5 명 플레이어가 누구야?\"");
Console.WriteLine("  - \"전체 플레이어 수가 얼마나 돼?\"\n");

Console.WriteLine("명령을 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput) || 
        userInput.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    try
    {
        var response = await agent.RunAsync(userInput);
        
        Console.WriteLine($"\n🤖 에이전트: {response}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}

// 샘플 데이터베이스 생성
static void CreateSampleDatabase(string dbPath)
{
    Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
    
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    var createTable = """
        CREATE TABLE Players (
            PlayerId TEXT PRIMARY KEY,
            PlayerName TEXT NOT NULL,
            Level INTEGER NOT NULL,
            TotalPlayTime REAL NOT NULL,
            LastLoginDate TEXT NOT NULL
        )
        """;
    
    using var cmd = new SqliteCommand(createTable, connection);
    cmd.ExecuteNonQuery();

    // 샘플 데이터 삽입
    var players = new[]
    {
        ("P001", "Hero123", 50, 120.5, "2025-01-15"),
        ("P002", "Mage456", 45, 98.3, "2025-01-14"),
        ("P003", "Warrior789", 55, 150.2, "2025-01-15"),
        ("P004", "Archer321", 42, 85.7, "2025-01-13"),
        ("P005", "Healer654", 48, 110.4, "2025-01-15"),
    };

    foreach (var (id, name, level, time, date) in players)
    {
        var insert = "INSERT INTO Players VALUES (@Id, @Name, @Level, @Time, @Date)";
        using var insertCmd = new SqliteCommand(insert, connection);
        insertCmd.Parameters.AddWithValue("@Id", id);
        insertCmd.Parameters.AddWithValue("@Name", name);
        insertCmd.Parameters.AddWithValue("@Level", level);
        insertCmd.Parameters.AddWithValue("@Time", time);
        insertCmd.Parameters.AddWithValue("@Date", date);
        insertCmd.ExecuteNonQuery();
    }

    Console.WriteLine("✅ 샘플 데이터베이스가 생성되었습니다.\n");
}
