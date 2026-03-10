// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _03C_SaveFileValidator.Tools;

// ==========================================
// 3 단계 C: 세이브 파일 검증기
// ==========================================

Console.WriteLine("🔐 세이브 파일 검증기에 오신 것을 환영합니다!");
Console.WriteLine("게임 세이브 파일의 무결성을 확인합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

var validatorTool = new FileWriteTool();

var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 게임 세이브 파일 검증 전문가입니다.
        
        당신의 역할:
        1. JSON 세이브 파일의 무결성을 검사합니다
        2. 필수 필드가 있는지 확인합니다
        3. 데이터 손상이나 변조를 탐지합니다
        4. 복구 방안을 제안합니다
        
        사용 가능한 도구:
        - ValidateJsonFile(filePath): JSON 무결성 검증
        - CheckRequiredFields(filePath, requiredFields): 필수 필드 검사
        
        검사 항목:
        - JSON 형식 유효성
        - 필수 필드 존재 여부 (playerId, playerName, level 등)
        - 데이터 타입 일관성
        """,
    name: "SaveFileValidator",
    tools: [
        AIFunctionFactory.Create(validatorTool.ValidateJsonFile),
        AIFunctionFactory.Create(validatorTool.CheckRequiredFields)
    ]
);

var defaultSavePath = Path.Combine(AppContext.BaseDirectory, "samples", "save_data.json");

if (!File.Exists(defaultSavePath))
{
    var projectDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.FullName 
        ?? AppContext.BaseDirectory;
    defaultSavePath = Path.Combine(projectDir, "samples", "save_data.json");
}

Console.WriteLine($"📁 기본 세이브 파일: {defaultSavePath}\n");

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
            다음 요청을 처리해주세요. 세이브 파일 경로는 아래와 같습니다.
            
            파일 경로: {defaultSavePath}
            
            사용자 요청: {userInput}
            
            파일을 검증하고 결과를 보고하세요.
            """;

        var response = await agent.RunAsync(enhancedPrompt);
        
        Console.WriteLine($"\n🤖 에이전트: {response}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
