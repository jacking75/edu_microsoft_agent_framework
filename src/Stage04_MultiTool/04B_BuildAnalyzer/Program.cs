// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _04B_BuildAnalyzer.Tools;

// ==========================================
// 4 단계 B: 빌드 분석기
// ==========================================

Console.WriteLine("🔧 빌드 분석기에 오신 것을 환영합니다!");
Console.WriteLine("빌드 로그를 분석하고 문제를 진단합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

var parserTool = new BuildLogParserTool();
var classifierTool = new WarningClassifierTool();

var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 빌드 로그 분석 전문가입니다.
        
        당신의 역할:
        1. 빌드 로그를 파싱하고 에러/경고를 추출합니다
        2. 경고를 분류하고 우선순위를 매깁니다
        3. 수정 방안을 제안합니다
        
        사용 가능한 도구:
        - ParseBuildLog(logPath): 빌드 로그 분석
        - ExtractErrors(logPath): 에러 추출
        - ClassifyWarnings(logPath): 경고 분류
        - GetSummary(logPath): 경고 요약
        
        우선순위:
        1. 에러 - 즉시 수정 필요
        2. Null 참조 경고 - 런타임 에러 가능성
        3. 사용하지 않는 코드 - 코드 정리
        4. 사용 중단된 API - 호환성 문제
        """,
    name: "BuildAnalyzer",
    tools: [
        AIFunctionFactory.Create(parserTool.ParseBuildLog),
        AIFunctionFactory.Create(parserTool.ExtractErrors),
        AIFunctionFactory.Create(classifierTool.ClassifyWarnings),
        AIFunctionFactory.Create(classifierTool.GetSummary)
    ]
);

Console.WriteLine("✅ 빌드 분석 도구가 초기화되었습니다.\n");

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
