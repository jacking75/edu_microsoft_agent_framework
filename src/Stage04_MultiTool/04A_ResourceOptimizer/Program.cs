// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _04A_ResourceOptimizer.Tools;

// ==========================================
// 4 단계 A: 리소스 최적화 어시스턴트
// ==========================================
// 학습 목표:
// 1. 여러 Tool 조합 사용
// 2. Tool 간 데이터 공유
// 3. 복잡한 워크플로우 구현
// ==========================================

Console.WriteLine("🎨 리소스 최적화 어시스턴트에 오신 것을 환영합니다!");
Console.WriteLine("게임 리소스를 분석하고 최적화 방안을 제안합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 2. 여러 Tool 생성
var textureTool = new TextureAnalyzerTool();
var memoryTool = new MemoryEstimatorTool();

var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// 3. AIAgent 생성 - 여러 Tool 등록
AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 게임 리소스 최적화 전문가입니다.
        
        당신의 역할:
        1. 텍스처 파일을 분석하고 최적화 방안을 제안합니다
        2. 메모리 사용량을 추정하고 개선점을 찾습니다
        3. 빌드 시간을 단축할 방안을 제안합니다
        
        사용 가능한 도구:
        - AnalyzeTexture(filePath): 텍스처 파일 분석
        - EstimateMemoryUsage(texturePaths): 여러 텍스처 메모리 추정
        - EstimateBuildMemory(projectPath): 빌드 메모리 추정
        
        최적화 가이드라인:
        - 텍스처: 4K 는 8MB 이하, 모바일은 2MB 이하
        - 메모리: 100MB 이상이면 경고
        - 빌드: 2GB 이상이면 최적화 필요
        """,
    name: "ResourceOptimizer",
    tools: [
        AIFunctionFactory.Create(textureTool.AnalyzeTexture),
        AIFunctionFactory.Create(textureTool.EstimateMemoryUsage),
        AIFunctionFactory.Create(memoryTool.EstimateBuildMemory)
    ]
);

Console.WriteLine("✅ 리소스 최적화 도구가 초기화되었습니다.\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"텍스처 메모리 사용량을 분석해줘\"");
Console.WriteLine("  - \"빌드 메모리 추정이 필요해\"");
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
