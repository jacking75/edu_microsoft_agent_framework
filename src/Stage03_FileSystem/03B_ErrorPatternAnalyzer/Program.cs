// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _03B_ErrorPatternAnalyzer.Tools;

// ==========================================
// 3 단계 B: 에러 패턴 분석기
// ==========================================

Console.WriteLine("🔍 에러 패턴 분석기에 오신 것을 환영합니다!");
Console.WriteLine("크래시 리포트에서 공통 패턴을 발견합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

var analysisTool = new FileAnalysisTool();

var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 에러 로그 분석 전문가입니다.
        
        당신의 역할:
        1. 크래시 리포트를 읽고 에러 패턴을 분석합니다
        2. 빈번하게 발생하는 에러 타입을 식별합니다
        3. 에러의 근본 원인을 추론합니다
        4. 수정 우선순위를 제안합니다
        
        분석 시 다음을 고려하세요:
        - NullReferenceException: null 체크 누락
        - TimeoutException: 네트워크/DB 지연
        - IOException: 파일/디스크 문제
        - InvalidOperationException: 컬렉션 수정 문제
        
        사용 가능한 도구:
        - ExtractErrors(filePath): 에러 라인 추출
        - AnalyzeErrorPatterns(filePath): 에러 빈도 분석
        """,
    name: "ErrorPatternAnalyzer",
    tools: [
        AIFunctionFactory.Create(analysisTool.ExtractErrors),
        AIFunctionFactory.Create(analysisTool.AnalyzeErrorPatterns)
    ]
);

var defaultCrashPath = Path.Combine(AppContext.BaseDirectory, "samples", "crash_report.txt");

if (!File.Exists(defaultCrashPath))
{
    var projectDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.FullName 
        ?? AppContext.BaseDirectory;
    defaultCrashPath = Path.Combine(projectDir, "samples", "crash_report.txt");
}

Console.WriteLine($"📁 기본 크래시 리포트: {defaultCrashPath}\n");

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
        var enhancedPrompt = $"""
            다음 요청을 처리해주세요. 크래시 리포트 파일 경로는 아래와 같습니다.
            
            파일 경로: {defaultCrashPath}
            
            사용자 요청: {userInput}
            
            파일을 분석하여 에러 패턴과 개선 사항을 제안하세요.
            """;

        var response = await agent.RunAsync(enhancedPrompt);
        
        Console.WriteLine($"\n🤖 에이전트: {response}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
