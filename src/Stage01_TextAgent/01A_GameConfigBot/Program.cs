// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _01A_GameConfigBot.Agents;
using System.Text.Json;

// ==========================================
// 1 단계 A: 게임 설정 조회 봇
// ==========================================
// 학습 목표:
// 1. Microsoft Agent Framework 를 사용한 Agent 초기화
// 2. AsAIAgent() 메서드로 에이전트 생성
// 3. RunAsync() 로 단일 응답, RunStreamingAsync() 로 스트리밍
// 4. OPENAI_API_KEY, OPENAI_BASE_URL 환경 변수 사용
// ==========================================

Console.WriteLine("🎮 게임 설정 조회 봇에 오신 것을 환영합니다!");
Console.WriteLine("게임 설정에 대해 무엇이든 물어보세요.\n");

// 1. 환경 변수에서 API 키와 베이스 URL 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 2. ConfigAgent 초기화 - 게임 설정 JSON 파일 로드
var configPath = Path.Combine(AppContext.BaseDirectory, "data", "game_config.json");

if (!File.Exists(configPath))
{
    var projectDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.FullName 
        ?? AppContext.BaseDirectory;
    configPath = Path.Combine(projectDir, "data", "game_config.json");
}

var configAgent = new ConfigAgent(configPath);
Console.WriteLine($"✅ 게임 설정 로드됨: {configPath}\n");

// 3. 사용 가능한 설정 키 안내
Console.WriteLine("📋 사용 가능한 설정 카테고리:");
foreach (var key in configAgent.GetTopLevelKeys())
{
    Console.WriteLine($"  - {key}");
}
Console.WriteLine();

// 4. OpenAI 클라이언트 생성 - API 키와 베이스 URL 사용
var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// 5. AIAgent 생성 - Microsoft Agent Framework 사용
// AIContextProviders 를 사용하여 설정 데이터를 LLM 컨텍스트에 주입
AIAgent agent = chatClient.AsAIAgent(new ChatClientAgentOptions()
{
    ChatOptions = new() { Instructions = configAgent.GetSystemPrompt() },
    AIContextProviders = [new ConfigContextProvider(configAgent.GetAllConfigData())]
});

Console.WriteLine("✅ 에이전트 초기화 완료\n");

// 6. 대화 루프 시작
Console.WriteLine("질문을 입력하세요 (종료: 'quit' 또는 'exit')\n");

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
            게임 설정 파일에서 다음 질문에 답변해주세요:
            
            질문: {userInput}
            
            현재 로드된 설정: {string.Join(", ", configAgent.GetTopLevelKeys())}
            """;

        var response = await agent.RunAsync(enhancedPrompt);
        
        Console.WriteLine($"\n🤖 에이전트: {response}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
