using System.Text.Json;

namespace _01C_FAQBot.Agents;

/// <summary>
/// FAQ 데이터베이스에서 유사한 질문을 찾아 답변하는 에이전트
/// 키워드 매칭과 간단한 유사도 계산을 사용
/// </summary>
public class FAQAgent
{
    private readonly List<FaqCategory> _categories;
    private readonly string _systemPrompt;
    private readonly Dictionary<string, string> _keywordCache;

    /// <summary>
    /// FAQAgent 생성자
    /// FAQ JSON 데이터를 로드
    /// </summary>
    public FAQAgent(string faqPath)
    {
        var jsonContent = File.ReadAllText(faqPath);
        var doc = JsonSerializer.Deserialize<JsonDocument>(jsonContent);
        
        _categories = new List<FaqCategory>();
        _keywordCache = new Dictionary<string, string>();

        if (doc?.RootElement.TryGetProperty("faqCategories", out var categoriesElement) == true)
        {
            foreach (var categoryElement in categoriesElement.EnumerateArray())
            {
                var category = new FaqCategory();
                if (categoryElement.TryGetProperty("name", out var nameProp))
                {
                    category.Name = nameProp.GetString() ?? "Unknown";
                }

                if (categoryElement.TryGetProperty("questions", out var questionsProp))
                {
                    foreach (var qElement in questionsProp.EnumerateArray())
                    {
                        var q = qElement.GetProperty("q").GetString() ?? "";
                        var a = qElement.GetProperty("a").GetString() ?? "";
                        category.Questions.Add((q, a));
                        
                        BuildKeywordCache(q, $"{category.Name}: {a}");
                    }
                }
                _categories.Add(category);
            }
        }

        _systemPrompt = """
            당신은 게임 FAQ 어시스턴트입니다.
            
            당신의 역할:
            1. 사용자의 질문에 가장 적합한 FAQ 를 찾아 답변합니다
            2. 정확한 정보를 제공하며, 추측하지 않습니다
            3. FAQ 에 없는 질문은 "해당 정보는 FAQ 에 없습니다. 고객센터에 문의해주세요"라고 답변합니다
            4. 친절하고 이해하기 쉽게 설명합니다
            
            사용 가능한 FAQ 카테고리:
            - 계정 (비밀번호, 탈퇴, 연동 등)
            - 게임플레이 (레벨, 파티, PVP, 강화 등)
            - 아이템/상점 (골드, 유료아이템, 거래 등)
            - 기술/버그 (연결, 아이템분실, 로그인 등)
            """;
    }

    private void BuildKeywordCache(string question, string answer)
    {
        var keywords = question.ToLower()
            .Replace("?", "")
            .Replace("!", "")
            .Replace(".", "")
            .Split(' ', '\t')
            .Where(k => k.Length > 2);

        foreach (var keyword in keywords)
        {
            if (!_keywordCache.ContainsKey(keyword))
            {
                _keywordCache[keyword] = answer;
            }
        }
    }

    public (int score, string category, string question, string answer)? FindBestMatch(string userQuestion)
    {
        var userKeywords = userQuestion.ToLower()
            .Replace("?", "")
            .Replace("!", "")
            .Split(' ', '\t')
            .Where(k => k.Length > 2)
            .ToList();

        var bestMatch = (score: 0, category: "", question: "", answer: "");

        foreach (var category in _categories)
        {
            foreach (var (q, a) in category.Questions)
            {
                var score = CalculateMatchScore(userKeywords, q);
                if (score > bestMatch.score)
                {
                    bestMatch = (score, category.Name, q, a);
                }
            }
        }

        return bestMatch.score > 0 ? bestMatch : null;
    }

    private int CalculateMatchScore(List<string> userKeywords, string faqQuestion)
    {
        var faqKeywords = faqQuestion.ToLower()
            .Replace("?", "")
            .Split(' ', '\t')
            .Where(k => k.Length > 2)
            .ToList();

        var matchCount = userKeywords.Count(k => faqKeywords.Contains(k));
        
        return matchCount;
    }

    public List<(string question, string answer)> GetFaqsByCategory(string categoryName)
    {
        var category = _categories.FirstOrDefault(c => 
            c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        
        return category?.Questions ?? new List<(string, string)>();
    }

    public List<string> GetCategories() => _categories.Select(c => c.Name).ToList();

    public string GetSystemPrompt() => _systemPrompt;
}

/// <summary>
/// FAQ 카테고리 데이터 모델
/// </summary>
public class FaqCategory
{
    public string Name { get; set; } = "";
    public List<(string q, string a)> Questions { get; set; } = new();
}
