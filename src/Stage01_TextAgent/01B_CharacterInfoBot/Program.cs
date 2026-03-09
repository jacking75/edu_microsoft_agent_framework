// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _01B_CharacterInfoBot.Agents;

// ==========================================
// 1 단계 B: 캐릭터 정보问答 봇
// ==========================================
// 학습 목표:
// 1. YAML 데이터 포맷 처리
// 2. YamlDotNet 라이브러리 사용
// 3. 한글/영문 클래스명 매핑
// 4. 복잡한 데이터 구조 조회
// ==========================================

Console.WriteLine("⚔️ 캐릭터 정보 질문 봇에 오신 것을 환영합니다!");
Console.WriteLine("캐릭터 스탯, 스킬, 상성에 대해 무엇이든 물어보세요.\n");

// 1. 환경 변수에서 API 키와 베이스 URL 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 2. CharacterAgent 초기화
var jsonPath = Path.Combine(AppContext.BaseDirectory, "data", "characters.json");

if (!File.Exists(jsonPath))
{
    var projectDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.FullName 
        ?? AppContext.BaseDirectory;
    jsonPath = Path.Combine(projectDir, "data", "characters.json");
}

var characterAgent = new CharacterAgent(jsonPath);
Console.WriteLine($"✅ 캐릭터 데이터 로드됨: {jsonPath}\n");

// 3. 사용 가능한 클래스 안내
Console.WriteLine("📋 사용 가능한 클래스:");
foreach (var className in characterAgent.GetAllClassNames())
{
    Console.WriteLine($"  - {className}");
}
Console.WriteLine();

// 4. OpenAI 클라이언트 생성 - API 키와 베이스 URL 사용
var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// 5. AIAgent 생성 - Microsoft Agent Framework 사용
// JSON 데이터를 시스템 프롬프트에 포함하여 AI 가 실제 데이터를 참조할 수 있게 함
var systemPrompt = $"""
    {characterAgent.GetSystemPrompt()}
    
    === 실제 게임 데이터 (이 정보를 바탕으로 정확하게 답변하세요) ===
    
    {characterAgent.GetCharacterDataAsText()}
    """;

AIAgent agent = chatClient.AsAIAgent(
    instructions: systemPrompt,
    name: "CharacterInfoBot"
);

Console.WriteLine("✅ 에이전트 초기화 완료\n");

// 6. 예시 질문 안내
Console.WriteLine("💡 예시 질문:");
Console.WriteLine("  - \"전사의 HP 와 공격력은 얼마인가요?\"");
Console.WriteLine("  - \"마법사의 스킬 목록을 알려주세요\"");
Console.WriteLine("  - \"암살자와 궁수 중 누가 이기나요?\"");
Console.WriteLine("  - \"초보자에게 추천하는 클래스는 무엇인가요?\"");
Console.WriteLine("  - \"Warrior 의 성장률을 알려줘\"");
Console.WriteLine();

// 7. 대화 루프 시작
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
            캐릭터 정보에 대한 다음 질문에 답변해주세요:
            
            질문: {userInput}
            
            사용 가능한 클래스: {string.Join(", ", characterAgent.GetAllClassNames())}
            
            참고: 한글 (전사, 마법사 등) 또는 영문 (warrior, mage 등) 으로 질문할 수 있습니다.
            """;

        var response = await agent.RunAsync(enhancedPrompt);
        
        Console.WriteLine($"\n🤖 에이전트: {response}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
