// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _07C_CodebaseSearcher.Services;

// ==========================================
// 7 단계 C: 코드베이스 검색기 (RAG)
// ==========================================
// 학습 목표:
// 1. 소스 코드 파싱 및 인덱싱
// 2. 메서드/클래스 단위 검색
// 3. 코드 유사도 검색
// 4. 위치 정보 (파일, 줄번호) 제공
// ==========================================

Console.WriteLine("🔍 코드베이스 검색기에 오신 것을 환영합니다!");
Console.WriteLine("소스 코드를 임베딩하고 유사 코드를 검색합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 1. 코드 Vector Store 초기화
var codeStore = new CodeVectorStore();

// 2. 샘플 코드 파일 로드
var sampleCodePath = Path.Combine(AppContext.BaseDirectory, "Data", "sample_code.txt");

if (!File.Exists(sampleCodePath))
{
    var projectDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.FullName 
        ?? AppContext.BaseDirectory;
    sampleCodePath = Path.Combine(projectDir, "Data", "sample_code.cs");
}

if (File.Exists(sampleCodePath))
{
    Console.WriteLine($"📁 코드 파일 로딩 중: {Path.GetFileName(sampleCodePath)}\n");
    
    var code = File.ReadAllText(sampleCodePath);
    codeStore.IndexCSharpFile(sampleCodePath, code);
    
    Console.WriteLine($"✅ {codeStore.Count}개 코드 스니펫이 인덱싱되었습니다.\n");
    
    // 인덱스된 파일 목록 표시
    var files = codeStore.GetFiles();
    if (files.Any())
    {
        Console.WriteLine($"📂 인덱스된 파일:");
        foreach (var file in files)
        {
            Console.WriteLine($"   - {Path.GetFileName(file)}");
        }
        Console.WriteLine();
    }
}
else
{
    Console.WriteLine($"❌ 샘플 코드 파일을 찾을 수 없습니다: {sampleCodePath}\n");
    Console.WriteLine("인라인 샘플 코드를 추가합니다...\n");
    
    // 인라인 샘플 코드 추가
    codeStore.AddSnippet(new CodeSnippet
    {
        Id = "InventoryService.AddItem",
        FilePath = "InventoryService.cs",
        ClassName = "InventoryService",
        MethodName = "AddItem",
        Content = """
            public bool AddItem(Item item)
            {
                if (_items.Count >= _maxCapacity) return false;
                var existingItem = _items.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem != null)
                {
                    existingItem.StackCount += item.StackCount;
                    return true;
                }
                _items.Add(item);
                return true;
            }
            """,
        LineNumber = 15
    });
    
    codeStore.AddSnippet(new CodeSnippet
    {
        Id = "QuestService.CompleteQuest",
        FilePath = "QuestService.cs",
        ClassName = "QuestService",
        MethodName = "CompleteQuest",
        Content = """
            public void CompleteQuest(string questId)
            {
                var quest = _activeQuests.FirstOrDefault(q => q.Id == questId);
                if (quest == null) return;
                quest.Status = QuestStatus.Completed;
                _activeQuests.Remove(quest);
                _completedQuests.Add(quest);
            }
            """,
        LineNumber = 85
    });
    
    codeStore.AddSnippet(new CodeSnippet
    {
        Id = "Player.CalculateDamage",
        FilePath = "Player.cs",
        ClassName = "Player",
        MethodName = "CalculateDamage",
        Content = """
            public int CalculateDamage(Weapon weapon, Target target)
            {
                var baseDamage = weapon.Damage + Strength * 2;
                var multiplier = target.GetElementMultiplier(weapon.Element);
                var critChance = CalculateCritChance();
                var isCrit = Random.Shared.NextDouble() < critChance;
                return isCrit ? (int)(baseDamage * multiplier * 1.5) : (int)(baseDamage * multiplier);
            }
            """,
        LineNumber = 42
    });
    
    codeStore.AddSnippet(new CodeSnippet
    {
        Id = "Database.SavePlayer",
        FilePath = "Database.cs",
        ClassName = "Database",
        MethodName = "SavePlayer",
        Content = """
            public async Task SavePlayer(Player player)
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();
                using var cmd = new SqliteCommand("INSERT OR REPLACE INTO Players VALUES (...)", connection);
                await cmd.ExecuteNonQueryAsync();
            }
            """,
        LineNumber = 28
    });
    
    Console.WriteLine($"✅ {codeStore.Count}개 샘플 코드 스니펫이 인덱싱되었습니다.\n");
}

// 3. OpenAI 클라이언트 생성
var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// 4. 코드 검색 도구 생성
var codeSearchTool = new CodeSearchTool(codeStore);

// 5. AIAgent 생성 - 도구 등록
AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 코드 검색 및 분석 전문가입니다.
        
        당신의 역할:
        1. 사용자의 자연어 질의를 코드 검색으로 변환합니다
        2. 유사한 코드 스니펫을 찾습니다
        3. 코드 위치 (파일, 클래스, 메서드, 줄번호) 를 명시합니다
        4. 코드 작동 방식을 설명합니다
        
        사용 가능한 도구:
        - SearchCode(query, topK): 코드 유사도 검색
        - SearchByClass(className): 클래스명 검색
        - GetIndexedFiles(): 인덱스된 파일 목록
        
        응답 가이드라인:
        - 파일명과 줄번호를 항상 표시하세요
        - 클래스명.메서드명 형태로 위치를 명시하세요
        - 관련 코드가 여러개 있으면 모두 보여주세요
        """,
    name: "CodebaseSearcher",
    tools: [
        AIFunctionFactory.Create(codeSearchTool.SearchCode),
        AIFunctionFactory.Create(codeSearchTool.SearchByClass),
        AIFunctionFactory.Create(codeSearchTool.GetIndexedFiles)
    ]
);

Console.WriteLine("✅ 코드 검색기가 초기화되었습니다.\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"인벤토리에 아이템 추가하는 코드가 어디 있어?\"");
Console.WriteLine("  - \"퀘스트 완료 처리 로직을 보여줘\"");
Console.WriteLine("  - \"Damage 계산하는 메서드를 찾아줘\"");
Console.WriteLine("  - \"InventoryService 클래스의 모든 메서드를 보여줘\"\n");

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
/// 코드 검색을 위한 Tool
/// </summary>
public class CodeSearchTool
{
    private readonly CodeVectorStore _codeStore;

    public CodeSearchTool(CodeVectorStore codeStore)
    {
        _codeStore = codeStore;
    }

    /// <summary>
    /// 코드 유사도 검색
    /// </summary>
    public string SearchCode(string query, int topK = 3)
    {
        var results = _codeStore.Search(query, topK);
        
        if (!results.Any())
        {
            return "관련 코드를 찾을 수 없습니다.";
        }

        var formatted = results.Select(r => 
            $"""
            📍 [{r.snippet.FilePath}:{r.snippet.LineNumber}]
            클래스: {r.snippet.ClassName}.{r.snippet.MethodName}
            
            ```csharp
            {r.snippet.Content}
            ```
            
            (유사도: {r.score:F3})
            """
        );

        return string.Join("\n\n", formatted);
    }

    /// <summary>
    /// 클래스명 검색
    /// </summary>
    public string SearchByClass(string className)
    {
        var results = _codeStore.SearchByClass(className);
        
        if (!results.Any())
        {
            return $"클래스 '{className}'를 찾을 수 없습니다.";
        }

        var formatted = results.Select(r => 
            $"- {r.MethodName} ({r.FilePath}:{r.LineNumber})"
        );

        return $"""
            클래스 '{className}'의 메서드 ({results.Count}개):
            
            {string.Join("\n", formatted)}
            """;
    }

    /// <summary>
    /// 인덱스된 파일 목록
    /// </summary>
    public string GetIndexedFiles()
    {
        var files = _codeStore.GetFiles();
        
        if (!files.Any())
        {
            return "인덱스된 파일이 없습니다.";
        }

        return $"""
            인덱스된 코드 파일 ({files.Count}개):
            
            {string.Join("\n", files.Select(f => $"- {Path.GetFileName(f)}"))}
            """;
    }
}
