// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _07B_GDD_QA.Services;

// ==========================================
// 7 단계 B: 게임 디자인 문서 QA (RAG)
// ==========================================
// 학습 목표:
// 1. 구조화된 문서 (JSON) 인덱싱
// 2. 카테고리별 검색
// 3. RAG 기반 QA
// 4. 출처 표시
// ==========================================

Console.WriteLine("📖 게임 디자인 문서 QA 에 오신 것을 환영합니다!");
Console.WriteLine("GDD(Game Design Document) 를 검색하고 질의응답합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 1. Vector Store 초기화
var vectorStore = new VectorStore();

// 2. GDD 문서 로드 (JSON 에서 파싱)
var gddPath = Path.Combine(AppContext.BaseDirectory, "Data", "gdd.json");

if (!File.Exists(gddPath))
{
    var projectDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.FullName 
        ?? AppContext.BaseDirectory;
    gddPath = Path.Combine(projectDir, "Data", "gdd.json");
}

if (File.Exists(gddPath))
{
    var json = File.ReadAllText(gddPath);
    var doc = System.Text.Json.JsonDocument.Parse(json);
    var root = doc.RootElement;
    
    // 게임 정보 표시
    var gameTitle = root.GetProperty("gameTitle").GetString() ?? "Unknown";
    var version = root.GetProperty("version").GetString() ?? "1.0";
    Console.WriteLine($"📚 로드 중인 문서: {gameTitle} v{version}\n");
    
    // 섹션 문서 인덱싱
    if (root.TryGetProperty("sections", out var sections))
    {
        foreach (var section in sections.EnumerateArray())
        {
            var id = section.GetProperty("id").GetString() ?? "";
            var category = section.GetProperty("category").GetString() ?? "";
            var title = section.GetProperty("title").GetString() ?? "";
            var content = section.GetProperty("content").GetString() ?? "";
            
            var fullContent = $"[{title}] {content}";
            vectorStore.AddDocument(id, fullContent, source: gameTitle, category: category);
        }
    }
    
    Console.WriteLine($"✅ {vectorStore.Count}개 GDD 섹션이 인덱싱되었습니다.\n");
    
    // 카테고리 정보 표시
    var categories = vectorStore.GetCategories();
    Console.WriteLine($"📑 카테고리: {string.Join(", ", categories)}\n");
}
else
{
    Console.WriteLine($"❌ GDD 파일을 찾을 수 없습니다: {gddPath}\n");
    Console.WriteLine("샘플 문서를 추가합니다...");
    
    // 샘플 문서 추가
    vectorStore.AddDocument("TEMP001", "게임의 최대 레벨은 99 입니다", source: "Sample", category: "Progression");
    vectorStore.AddDocument("TEMP002", "클래스는 전사, 마법사, 궁수, 힐러, 도적, 소환사가 있습니다", source: "Sample", category: "Classes");
    vectorStore.AddDocument("TEMP003", "파티는 최대 4 명, 레이드는 8 명입니다", source: "Sample", category: "Combat");
    
    Console.WriteLine($"✅ {vectorStore.Count}개 샘플 문서가 인덱싱되었습니다.\n");
}

// 3. OpenAI 클라이언트 생성
var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// 4. RAG 검색 도구 생성
var ragTool = new RagSearchTool(vectorStore);

// 5. AIAgent 생성 - 도구 등록
AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 게임 디자인 문서 (GDD) QA 전문가입니다.
        
        당신의 역할:
        1. 사용자의 질문에 관련 GDD 섹션을 검색합니다
        2. 검색된 문서에 기반하여 정확하게 답변합니다
        3. 문서에 없는 정보는 모른다고 답변합니다
        4. 출처 (섹션 ID, 카테고리) 를 명시합니다
        
        사용 가능한 도구:
        - SearchGdd(query, topK): GDD 검색
        - GetCategories(): 카테고리 목록 조회
        
        응답 가이드라인:
        - GDD001, GDD002 등 섹션 ID 를 인용하세요
        - [Category] 형태로 카테고리를 표시하세요
        - 여러 관련 문서가 있으면 모두 인용하세요
        """,
    name: "GDD_QA",
    tools: [
        AIFunctionFactory.Create(ragTool.SearchGdd),
        AIFunctionFactory.Create(ragTool.GetCategories)
    ]
);

Console.WriteLine("✅ GDD QA 봇이 초기화되었습니다.\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"클래스 시스템이 어떻게 돼?\"");
Console.WriteLine("  - \"최대 레벨이 얼마야?\"");
Console.WriteLine("  - \"길드 전쟁에 대해 알려줘\"");
Console.WriteLine("  - \"수익 모델이 뭐야?\"\n");

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

/// <summary>
/// RAG 검색을 위한 Tool
/// </summary>
public class RagSearchTool
{
    private readonly VectorStore _vectorStore;

    public RagSearchTool(VectorStore vectorStore)
    {
        _vectorStore = vectorStore;
    }

    /// <summary>
    /// GDD 문서 검색
    /// </summary>
    public string SearchGdd(string query, int topK = 3)
    {
        var results = _vectorStore.Search(query, topK);
        
        if (!results.Any())
        {
            return "관련 문서를 찾을 수 없습니다.";
        }

        var formatted = results.Select(r => 
            $"[ID: {r.doc.Id}] [{r.doc.Category}]\n{r.doc.Content}\n(유사도: {r.score:F3})"
        );

        return string.Join("\n\n", formatted);
    }

    /// <summary>
    /// 카테고리 목록 조회
    /// </summary>
    public string GetCategories()
    {
        var categories = _vectorStore.GetCategories();
        return $"사용 가능한 카테고리: {string.Join(", ", categories)}";
    }
}
