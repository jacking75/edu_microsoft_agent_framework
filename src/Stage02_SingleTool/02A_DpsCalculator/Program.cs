// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _02A_DpsCalculator.Tools;

// ==========================================
// 2 단계 A: DPS 계산기
// ==========================================
// 학습 목표:
// 1. Function Tool 정의
// 2. Tool 을 사용한 계산
// 3. Agent 가 Tool 결과를 활용하는 방식 이해
// ==========================================

Console.WriteLine("⚔️ DPS 계산기에 오신 것을 환영합니다!");
Console.WriteLine("무기의 DPS 를 계산해드립니다.\n");

// 1. 환경 변수에서 API 키와 베이스 URL 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 2. DPS 계산 Tool 생성
var dpsTool = new DpsCalculationTool();

// 3. OpenAI 클라이언트 생성
var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// 4. AIAgent 생성 - Tool 등록
AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 게임 DPS 계산 전문가입니다.
        
        당신의 역할:
        1. 사용자가 무기 스탯을 입력하면 DPS 를 계산합니다
        2. DPS = 공격력 × 초당공격속도 공식으로 계산합니다
        3. 크리티컬 확률과 배율이 주어지면 기대 DPS 도 계산합니다
        4. 계산 과정과 결과를 자세히 설명합니다
        
        계산이 필요한 경우 반드시 제공된 도구를 사용하세요.
        """,
    name: "DpsCalculator",
    tools: [
        AIFunctionFactory.Create(dpsTool.CalculateDps),
        AIFunctionFactory.Create(dpsTool.CalculateExpectedDps)
    ]
);

Console.WriteLine("✅ DPS 계산기가 초기화되었습니다.\n");

// 5. 사용 예시 안내
Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"공격력 100, 초당 공격 속도 1.5 인 무기의 DPS 를 계산해줘\"");
Console.WriteLine("  - \"DPS 가 150 이고 크리티컬 확률 20%, 배율 2 배일 때 기대 DPS 는?\"");
Console.WriteLine("  - \"공격력 80, 공격 속도 2.0, 크리티컬 30%, 배율 2.5 배 계산해줘\"\n");

// 6. 대화 루프 시작
Console.WriteLine("계산할 무기 스탯을 입력하세요 (종료: 'quit' 또는 'exit')\n");

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
