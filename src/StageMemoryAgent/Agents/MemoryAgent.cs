using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using Microsoft.Agents.AI;
using StageMemoryAgent.Services;

namespace StageMemoryAgent.Agents;

/// <summary>
/// 기억 기능을 가진 에이전트
/// </summary>
public class MemoryAgent
{
    private readonly AIAgent _agent;
    private readonly MemoryStore _memoryStore;
    private readonly List<ChatMessage> _conversationHistory = new();
    private int _conversationCount = 0;
    private const int AutoSaveInterval = 5;

    /// <summary>
    /// MemoryAgent 생성자
    /// </summary>
    public MemoryAgent(MemoryStore memoryStore)
    {
        _memoryStore = memoryStore;
        _agent = CreateAIAgent();
    }

    /// <summary>
    /// AIAgent 생성
    /// </summary>
    private AIAgent CreateAIAgent()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수가 설정되지 않았습니다.");

        var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
            ?? "https://api.openai.com/v1";

        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        return chatClient.AsAIAgent(new ChatClientAgentOptions
        {
            ChatOptions = new()
            {
                Instructions = """
                    당신은 기억 기능을 가진 친절한 어시스턴트입니다.
                    
                    당신의 역할:
                    1. 이전 대화에서 언급된 내용을 기억하고 자연스럽게 참조하세요.
                    2. "이전에 말씀해주셨는데...", "제 기억에..." 등으로 기억을 언급하세요.
                    3. 기억에 없는 정보는 모른다고 정직하게 답변하세요.
                    4. 사용자의 이름, 선호도, 과거 대화 내용을 고려하여 답변하세요.
                    5. 기억을 가지고 있지만 과도하게 언급하지 마세요.
                    
                    중요한 정보는 자동으로 저장됩니다.
                    사용자가 이름을 알려주면 기억하세요.
                    선호도, 결정 사항, 새로운 사실을 기억하세요.
                    """
            }
        });
    }

    /// <summary>
    /// 에이전트 초기화 - 기억 로드
    /// </summary>
    public void Initialize()
    {
        _memoryStore.LoadAllMemories();
        _conversationHistory.Clear();
        _conversationCount = 0;

        Console.WriteLine("✅ MemoryAgent 가 초기화되었습니다.");
    }

    /// <summary>
    /// 사용자 입력 처리 및 응답 생성
    /// </summary>
    public async Task<string> RunAsync(string userInput)
    {
        _conversationCount++;

        var prompt = $"""
            [대화 내역]
            {string.Join("\n", _conversationHistory.Select(m =>
                $"{(m is UserChatMessage ? "사용자" : "어시스턴트")}: {m.Content[0].Text}"))}
            
            [새 사용자 입력]
            {userInput}
            """;

        var response = await _agent.RunAsync(prompt);
        var responseText = response.ToString() ?? "";

        _conversationHistory.Add(new UserChatMessage(userInput));
        _conversationHistory.Add(new AssistantChatMessage(responseText));

        if (_conversationCount % AutoSaveInterval == 0)
        {
            await AutoSaveImportantInfoAsync();
        }

        return responseText;
    }

    /// <summary>
    /// 중요 정보 자동 저장
    /// </summary>
    private async Task AutoSaveImportantInfoAsync()
    {
        var historyText = string.Join("\n",
            _conversationHistory.TakeLast(20).Select(m =>
                $"{(m is UserChatMessage ? "사용자" : "어시스턴트")}: {m.Content[0].Text}"));

        var extractionPrompt = $"""
            다음 대화에서 기억할 만한 중요 정보를 추출해주세요.
            
            [대화 내역]
            {historyText}
            
            [추출할 정보]
            1. 사용자의 이름이나 개인 정보
            2. 선호도 (색음식, 취향 등)
            3. 중요한 결정 사항
            4. 새로 알게 된 사실
            5. 프로젝트나 작업 관련 정보
            
            [출력 형식]
            각 정보는 다음 형식으로 작성:
            - 제목: [짧은 제목]
            - 내용: [내용]
            - 태그: [태그 1, 태그 2]
            - 중요도: [1-5]
            
            해당 정보가 없으면 "새로운 중요 정보 없음"이라고만 작성.
            """;

        try
        {
            var extractionResponse = await _agent.RunAsync(extractionPrompt);
            var extractedText = extractionResponse.ToString() ?? "";

            if (!extractedText.Contains("새로운 중요 정보 없음"))
            {
                var entries = ParseExtractedMemories(extractedText);
                foreach (var entry in entries)
                {
                    _memoryStore.SaveMemory(entry);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  자동 저장 오류: {ex.Message}");
        }
    }

    /// <summary>
    /// 추출된 텍스트에서 MemoryEntry 파싱
    /// </summary>
    private List<MemoryEntry> ParseExtractedMemories(string text)
    {
        var entries = new List<MemoryEntry>();
        var lines = text.Split('\n');

        MemoryEntry? currentEntry = null;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if (trimmed.StartsWith("- 제목:") || trimmed.StartsWith("제목:"))
            {
                if (currentEntry != null) entries.Add(currentEntry);
                currentEntry = new MemoryEntry
                {
                    Title = trimmed.Split(':')[1].Trim(),
                    Content = ""
                };
            }
            else if (trimmed.StartsWith("- 내용:") || trimmed.StartsWith("내용:"))
            {
                if (currentEntry != null)
                {
                    currentEntry.Content = trimmed.Split(':')[1].Trim();
                }
            }
            else if (trimmed.StartsWith("- 태그:") || trimmed.StartsWith("태그:"))
            {
                if (currentEntry != null)
                {
                    var tags = trimmed.Split(':')[1]
                        .Split(',')
                        .Select(t => t.Trim().TrimStart('#'))
                        .Where(t => !string.IsNullOrEmpty(t))
                        .ToList();
                    currentEntry.Tags = tags;
                }
            }
            else if (trimmed.StartsWith("- 중요도:") || trimmed.StartsWith("중요도:"))
            {
                if (currentEntry != null)
                {
                    var importanceStr = trimmed.Split(':')[1].Trim();
                    if (int.TryParse(importanceStr, out var importance))
                    {
                        currentEntry.Importance = Math.Clamp(importance, 1, 5);
                    }
                }
            }
        }

        if (currentEntry != null && !string.IsNullOrEmpty(currentEntry.Content))
        {
            entries.Add(currentEntry);
        }

        return entries;
    }

    /// <summary>
    /// 기억 검색
    /// </summary>
    public List<MemoryEntry> SearchMemories(string query)
    {
        return _memoryStore.FindMemories(query);
    }

    /// <summary>
    /// 수동으로 기억 저장
    /// </summary>
    public void SaveMemory(string title, string content, List<string>? tags = null, int importance = 3)
    {
        var entry = new MemoryEntry
        {
            Title = title,
            Content = content,
            Tags = tags ?? new List<string>(),
            Importance = importance
        };

        _memoryStore.SaveMemory(entry);
    }

    /// <summary>
    /// 대화 히스토리 초기화
    /// </summary>
    public void ClearConversationHistory()
    {
        _conversationHistory.Clear();
        Console.WriteLine("🧹 대화 히스토리를 초기화했습니다.");
    }

    /// <summary>
    /// 현재 세션을 파일로 저장
    /// </summary>
    public void SaveCurrentSession(string sessionName)
    {
        var sessionMemories = _memoryStore.GetAllMemories();
        _memoryStore.SaveToSessionFile(sessionName, sessionMemories);
    }
}
