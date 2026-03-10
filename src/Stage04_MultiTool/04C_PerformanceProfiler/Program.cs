// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _04C_PerformanceProfiler.Tools;

// ==========================================
// 4 단계 C: 성능 프로파일러
// ==========================================

Console.WriteLine("⚡ 성능 프로파일러에 오신 것을 환영합니다!");
Console.WriteLine("게임 성능을 분석하고 병목을 찾습니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

var fpsTool = new FpsAnalyzerTool();
var bottleneckTool = new BottleneckDetectorTool();

var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 게임 성능 분석 전문가입니다.
        
        당신의 역할:
        1. FPS 데이터를 분석하고 성능을 평가합니다
        2. 병목 현상을 감지하고 원인을 찾습니다
        3. 최적화 방안을 제안합니다
        
        사용 가능한 도구:
        - AnalyzeFpsData(fpsValues): FPS 통계 분석
        - DetectDrops(fpsValues, threshold): FPS 드롭 감지
        - DetectBottleneck(metrics): 병목 현상 분석
        - SuggestOptimizations(type): 최적화 제안
        
        성능 기준:
        - 60 FPS 이상: 원활
        - 30-60 FPS: 일반
        - 30 FPS 미만: 개선 필요
        - 1% Low < 30: 스터터링 가능성
        """,
    name: "PerformanceProfiler",
    tools: [
        AIFunctionFactory.Create(fpsTool.AnalyzeFpsData),
        AIFunctionFactory.Create(fpsTool.DetectDrops),
        AIFunctionFactory.Create(bottleneckTool.DetectBottleneck),
        AIFunctionFactory.Create(bottleneckTool.SuggestOptimizations)
    ]
);

Console.WriteLine("✅ 성능 분석 도구가 초기화되었습니다.\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"FPS 55, 58, 60, 45, 52, 59 의 평균을 구해줘\"");
Console.WriteLine("  - \"CPU 92%, GPU 85%, 메모리 78% 일 때 병목은?\"");
Console.WriteLine("  - \"최적화 방안을 제안해줘\"\n");

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
