// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _06C_NaturalLanguageSql.Tools;
using _06C_NaturalLanguageSql.Services;

// ==========================================
// 6 단계 C: 자연어 SQL 변환기
// ==========================================

Console.WriteLine("🔤 자연어 SQL 변환기에 오신 것을 환영합니다!");
Console.WriteLine("자연어를 SQL 로 변환하여 실행합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

var sqlGenerator = new SqlGeneratorTool();
var queryValidator = new QueryValidator();

var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 자연어 SQL 변환 전문가입니다.
        
        당신의 역할:
        1. 사용자의 자연어 질문을 SQL 로 변환합니다
        2. SQL 의 안전성을 검증합니다
        3. 쿼리 결과를 해석합니다
        
        사용 가능한 도구:
        - GenerateSql(naturalLanguage): SQL 생성
        - ValidateSql(sql): 안전성 검증
        
        보안 가이드라인:
        - SELECT 쿼리만 허용
        - DROP, DELETE, UPDATE, INSERT 금지
        - LIMIT 필수 (최대 1000 행)
        - 파라미터화된 쿼리 사용
        
        Players 테이블 스키마:
        - PlayerId (TEXT)
        - PlayerName (TEXT)
        - Level (INTEGER)
        - TotalPlayTime (REAL)
        - LastLoginDate (TEXT)
        """,
    name: "NaturalLanguageSql",
    tools: [
        AIFunctionFactory.Create(sqlGenerator.GenerateSql),
        AIFunctionFactory.Create(sqlGenerator.ValidateSql)
    ]
);

Console.WriteLine("✅ NL-to-SQL 도구가 초기화되었습니다.\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"플레이어가 총 몇 명이야?\"");
Console.WriteLine("  - \"레벨이 가장 높은 플레이어 5 명을 보여줘\"");
Console.WriteLine("  - \"플레이어들의 평균 레벨은?\"\n");

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
        // Agent 가 도구 사용해서 SQL 생성 및 검증
        var response = await agent.RunAsync(userInput);
        
        Console.WriteLine($"\n🤖 에이전트: {response}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
