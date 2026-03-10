// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _06B_RetentionAnalyzer.Tools;

// ==========================================
// 6 단계 B: 리텐션 분석 에이전트
// ==========================================

Console.WriteLine("📈 리텐션 분석 에이전트에 오신 것을 환영합니다!");
Console.WriteLine("플레이어 리텐션을 분석합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "game.db");

if (!File.Exists(dbPath))
{
    Console.WriteLine("❌ 데이터베이스 파일이 없습니다.");
    Console.WriteLine("06A_PlayerStatsBot 을 먼저 실행하여 DB 를 생성하세요.");
    return;
}

var retentionTool = new RetentionCalcTool(dbPath);

var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 게임 리텐션 분석 전문가입니다.
        
        당신의 역할:
        1. 일별/주별 리텐션률을 계산합니다
        2. 활성/비활성 플레이어를 분류합니다
        3. 리텐션 개선 방안을 제안합니다
        
        사용 가능한 도구:
        - CalculateDailyRetention(days): 일별 리텐션
        - CalculateWeeklyRetention(weeks): 주별 리텐션
        - GetActiveInactivePlayers(inactiveDays): 활성도 분석
        
        분석 기준:
        - 활성: 30 일 이내 로그인
        - 비활성: 30 일 이상 로그인 없음
        - 리텐션율: 50% 이상 양호
        """,
    name: "RetentionAnalyzer",
    tools: [
        AIFunctionFactory.Create(retentionTool.CalculateDailyRetention),
        AIFunctionFactory.Create(retentionTool.CalculateWeeklyRetention),
        AIFunctionFactory.Create(retentionTool.GetActiveInactivePlayers)
    ]
);

Console.WriteLine("✅ 리텐션 분석 도구가 초기화되었습니다.\n");

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
