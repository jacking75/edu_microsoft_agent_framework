// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _01C_FAQBot.Agents;

// ==========================================
// 1 단계 C: 대화형 FAQ 봇
// ==========================================
// 학습 목표:
// 1. FAQ 데이터베이스 구조
// 2. 키워드 매칭 기반 검색
// 3. 대화 컨텍스트 유지
// 4. 유사도 스코어 계산
// ==========================================

Console.WriteLine("❓ FAQ 봇에 오신 것을 환영합니다!");
Console.WriteLine("게임에 대한 궁금증을 물어보세요.\n");

// 1. 환경 변수에서 API 키와 베이스 URL 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 2. FAQAgent 초기화
var faqPath = Path.Combine(AppContext.BaseDirectory, "data", "faq_database.json");

if (!File.Exists(faqPath))
{
    var projectDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.FullName 
        ?? AppContext.BaseDirectory;
    faqPath = Path.Combine(projectDir, "data", "faq_database.json");
}

var faqAgent = new FAQAgent(faqPath);
Console.WriteLine($"✅ FAQ 데이터 로드됨: {faqPath}\n");

// 3. 사용 가능한 카테고리 안내
Console.WriteLine("📋 FAQ 카테고리:");
foreach (var category in faqAgent.GetCategories())
{
    Console.WriteLine($"  - {category}");
}
Console.WriteLine("\n💡 팁: '계정', '게임플레이', '아이템', '기술' 등으로 검색할 수 있습니다.");
Console.WriteLine();

// 4. OpenAI 클라이언트 생성 - API 키와 베이스 URL 사용
var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// 5. AIAgent 생성 - Microsoft Agent Framework 사용
AIAgent agent = chatClient.AsAIAgent(
    instructions: faqAgent.GetSystemPrompt(),
    name: "FAQBot"
);

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
        // FAQ 에서 가장 유사한 질문 찾기
        var match = faqAgent.FindBestMatch(userInput);

        var enhancedPrompt = match.HasValue
            ? $"""
                다음 FAQ 를 참고하여 사용자 질문에 답변해주세요:
                
                [찾은 FAQ]
                카테고리: {match.Value.category}
                질문: {match.Value.question}
                답변: {match.Value.answer}
                
                [사용자 질문]
                {userInput}
                
                위 FAQ 를 바탕으로 친절하게 답변해주세요.
                """
            : $"""
                다음 질문에 답변해주세요:
                
                질문: {userInput}
                
                관련 FAQ 를 찾을 수 없습니다.
                "해당 정보는 FAQ 에 없습니다. 고객센터에 문의해주세요"라고 안내해주세요.
                고객센터: support@gamecompany.com
                """;

        var response = await agent.RunAsync(enhancedPrompt);
        
        // 매칭 점수 표시 (학습 목적)
        if (match.HasValue)
        {
            Console.WriteLine($"   [매칭 점수: {match.Value.score}]");
        }
        
        Console.WriteLine($"\n🤖 에이전트: {response}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
