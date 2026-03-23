using StageMemoryAgent.Agents;
using StageMemoryAgent.Services;

namespace StageMemoryAgent;

/// <summary>
/// 기억 기능을 가진 에이전트 데모 프로그램
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🧠 기억 기능을 가진 에이전트");
        Console.WriteLine("=============================\n");

        var memoryDir = Path.Combine(AppContext.BaseDirectory, "memory");
        var memoryStore = new MemoryStore(memoryDir);
        var memoryAgent = new MemoryAgent(memoryStore);

        memoryAgent.Initialize();

        PrintHelp();

        while (true)
        {
            Console.Write("\n👤 사용자: ");
            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                continue;
            }

            var command = userInput.Trim().ToLower();

            if (command == "quit" || command == "exit" || command == "q")
            {
                Console.WriteLine("\n👋 안녕히 가세요!");
                break;
            }

            if (command == "help" || command == "h")
            {
                PrintHelp();
                continue;
            }

            if (command.StartsWith("search "))
            {
                var keyword = userInput.Substring(7).Trim();
                SearchMemories(memoryAgent, keyword);
                continue;
            }

            if (command.StartsWith("save "))
            {
                await SaveMemoryManually(memoryAgent);
                continue;
            }

            if (command == "clear")
            {
                memoryAgent.ClearConversationHistory();
                continue;
            }

            if (command == "memories")
            {
                ShowAllMemories(memoryAgent);
                continue;
            }

            try
            {
                Console.Write("🤖 에이전트: ");
                var response = await memoryAgent.RunAsync(userInput);
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ 오류: {ex.Message}");
                Console.WriteLine("OPENAI_API_KEY 환경 변수를 확인하세요.");
            }
        }
    }

    /// <summary>
    /// 도움말 출력
    /// </summary>
    private static void PrintHelp()
    {
        Console.WriteLine("\n📖 사용법:");
        Console.WriteLine("  - 자유롭게 대화하세요 (기억됩니다)");
        Console.WriteLine("  - search [키워드] - 기억 검색");
        Console.WriteLine("  - save - 수동으로 기억 저장");
        Console.WriteLine("  - memories - 모든 기억 표시");
        Console.WriteLine("  - clear - 대화 히스토리 초기화");
        Console.WriteLine("  - help - 도움말 표시");
        Console.WriteLine("  - quit, exit, q - 종료");
    }

    /// <summary>
    /// 기억 검색
    /// </summary>
    private static void SearchMemories(MemoryAgent agent, string keyword)
    {
        var results = agent.SearchMemories(keyword);

        if (results.Count == 0)
        {
            Console.WriteLine($"\n⚠️  '{keyword}'에 대한 기억이 없습니다.");
            return;
        }

        Console.WriteLine($"\n🔍 '{keyword}' 검색 결과 ({results.Count}개):");
        foreach (var memory in results)
        {
            Console.WriteLine($"\n  📌 {memory.Title}");
            Console.WriteLine($"     태그: {string.Join(", ", memory.Tags)}");
            Console.WriteLine($"     중요도: {new string('⭐', memory.Importance)}");
            Console.WriteLine($"     내용: {memory.Content}");
        }
    }

    /// <summary>
    /// 수동 기억 저장
    /// </summary>
    private static async Task SaveMemoryManually(MemoryAgent agent)
    {
        Console.WriteLine("\n💾 기억 저장 모드 (종료: 'cancel')");

        Console.Write("  제목: ");
        var title = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(title) || title.ToLower() == "cancel") return;

        Console.Write("  내용: ");
        var content = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(content) || content.ToLower() == "cancel") return;

        Console.Write("  태그 (쉼표로 구분): ");
        var tagsInput = Console.ReadLine();
        var tags = string.IsNullOrWhiteSpace(tagsInput)
            ? new List<string>()
            : tagsInput.Split(',').Select(t => t.Trim()).ToList();

        Console.Write("  중요도 (1-5, 기본 3): ");
        var importanceInput = Console.ReadLine();
        var importance = 3;
        if (!string.IsNullOrWhiteSpace(importanceInput) && int.TryParse(importanceInput, out var val))
        {
            importance = Math.Clamp(val, 1, 5);
        }

        agent.SaveMemory(title, content ?? "", tags, importance);
        Console.WriteLine("✅ 기억을 저장했습니다.");
    }

    /// <summary>
    /// 모든 기억 표시
    /// </summary>
    private static void ShowAllMemories(MemoryAgent agent)
    {
        var memories = agent.SearchMemories("");

        if (memories.Count == 0)
        {
            Console.WriteLine("\n⚠️  저장된 기억이 없습니다.");
            return;
        }

        Console.WriteLine($"\n📚 저장된 기억 ({memories.Count}개):");
        foreach (var memory in memories)
        {
            Console.WriteLine($"\n  📌 {memory.Title}");
            Console.WriteLine($"     ID: {memory.Id}");
            Console.WriteLine($"     생성: {memory.CreatedAt:yyyy-MM-dd}");
            Console.WriteLine($"     태그: {string.Join(", ", memory.Tags)}");
            Console.WriteLine($"     중요도: {new string('⭐', memory.Importance)}");
            Console.WriteLine($"     내용: {memory.Content}");
        }
    }
}
