// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _07A_DocumentSearchBot.Services;

// ==========================================
// 7 단계 A: 문서 검색 봇 (RAG 기본)
// ==========================================
// 학습 목표:
// 1. 텍스트 임베딩 생성
// 2. Vector Store 구축
// 3. 유사도 검색
// 4. 검색된 컨텍스트 기반 응답
// ==========================================

Console.WriteLine("📚 문서 검색 봇 (RAG) 에 오신 것을 환영합니다!");
Console.WriteLine("문서를 검색하고 답변합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 1. Vector Store 초기화
var vectorStore = new VectorStore();

// 2. 샘플 문서 추가 (게임 FAQ)
vectorStore.AddDocument("FAQ001", "게임의 최대 레벨은 99 입니다. 99 레벨에 도달하면 추가 경험치는 명성 포인트로 전환됩니다.", "게임 가이드");
vectorStore.AddDocument("FAQ002", "파티는 최대 4 명까지 구성할 수 있습니다. 레이드 던전의 경우 8 명 파티가 가능합니다.", "게임 가이드");
vectorStore.AddDocument("FAQ003", "PVP 는 레벨 30 에 도달하고 메인 퀘스트 3 장을 클리어하면 해금됩니다.", "게임 가이드");
vectorStore.AddDocument("FAQ004", "장비 강화는 대도시 대장간 NPC 에게서 할 수 있습니다. 강화석과 골드가 필요합니다.", "게임 가이드");
vectorStore.AddDocument("FAQ005", "골드를 얻는 가장 빠른 방법은 일일 퀘스트, 던전 클리어, 아이템 판매입니다.", "게임 가이드");
vectorStore.AddDocument("FAQ006", "유료 아이템은 의상, 탈것, 펫, 인벤토리 확장, 경험치 부스터 등이 있습니다.", "상점 가이드");
vectorStore.AddDocument("FAQ007", "아이템 거래는 경매장이나 직접 거래 (Trade) 를 사용할 수 있습니다. 일부 아이템은 거래 불가입니다.", "게임 가이드");
vectorStore.AddDocument("FAQ008", "비밀번호 재설정은 로그인 화면에서 '비밀번호 찾기'를 클릭하고 이메일을 입력하면 됩니다.", "계정 가이드");

Console.WriteLine($"✅ {vectorStore.Count}개 문서가 인덱싱되었습니다.\n");

var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 게임 정보 검색 어시스턴트입니다.
        
        당신의 역할:
        1. 사용자의 질문에 관련 문서를 검색합니다
        2. 검색된 문서를 바탕으로 정확하게 답변합니다
        3. 문서에 없는 정보는 모른다고 답변합니다
        
        RAG 워크플로우:
        1. 사용자 질문 → 임베딩 생성
        2. Vector Store 에서 유사 문서 검색
        3. 검색된 문서를 컨텍스트로 추가
        4. LLM 이 컨텍스트 기반 답변 생성
        """,
    name: "DocumentSearchBot"
);

Console.WriteLine("✅ 문서 검색 봇이 초기화되었습니다.\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"최대 레벨이 얼마야?\"");
Console.WriteLine("  - \"파티는 몇 명까지 가능해?\"");
Console.WriteLine("  - \"PVP 언제 되는데?\"");
Console.WriteLine("  - \"골드 빨리 버는 방법 알려줘\"\n");

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
        // 3. Vector Search
        var searchResults = vectorStore.Search(userInput, topK: 3);
        
        Console.WriteLine($"\n🔍 검색된 문서 ({searchResults.Count}개):");
        foreach (var (doc, score) in searchResults)
        {
            Console.WriteLine($"  - [{doc.Source}] {doc.Content[..50]}... (유사도: {score:F3})");
        }
        
        // 4. 검색된 문서를 컨텍스트로 추가
        var context = string.Join("\n\n", searchResults.Select(r => 
            $"[출처: {r.doc.Source}]\n{r.doc.Content}"
        ));
        
        var enhancedPrompt = $"""
            다음 문서를 참고하여 사용자 질문에 답변해주세요.
            문서에 없는 정보는 모른다고 답변하세요.
            
            [검색된 문서]
            {context}
            
            [사용자 질문]
            {userInput}
            """;

        var response = await agent.RunAsync(enhancedPrompt);
        
        Console.WriteLine($"\n🤖 에이전트: {response}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
