namespace SkillSystem2;

public class SkillMatchResult
{
    public SkillDefinition Skill { get; set; } = null!;
    public int Score { get; set; }
    public List<string> MatchedKeywords { get; set; } = new();
}

public class SkillMatcher
{
    private readonly List<SkillDefinition> _allSkills;

    public SkillMatcher(List<SkillDefinition> allSkills)
    {
        _allSkills = allSkills;
    }

    public List<SkillDefinition> FindRelevantSkills(string userInput, int threshold = 1)
    {
        Console.WriteLine($"\n🔍 스킬 매칭 시작: \"{userInput}\"");
        Console.WriteLine(new string('-', 50));
        
        var userKeywords = ExtractKeywords(userInput);
        Console.WriteLine($"   추출된 키워드: {string.Join(", ", userKeywords)}");
        
        var results = new List<SkillMatchResult>();
        
        foreach (var skill in _allSkills)
        {
            var matchResult = MatchSkill(skill, userKeywords);
            if (matchResult.Score >= threshold)
            {
                results.Add(matchResult);
            }
        }
        
        results.Sort((a, b) => b.Score.CompareTo(a.Score));
        
        foreach (var result in results)
        {
            Console.WriteLine($"   {result.Skill.Name}: {result.Score}점 ({string.Join(", ", result.MatchedKeywords)})");
        }
        
        var matchedSkills = results.Select(r => r.Skill).ToList();
        
        if (matchedSkills.Count == 0)
        {
            Console.WriteLine($"⚠️  일치하는 스킬을 찾을 수 없습니다 (임계값: {threshold})");
        }
        else
        {
            Console.WriteLine($"✅ {matchedSkills.Count}개의 스킬을 선택했습니다.");
        }
        
        Console.WriteLine(new string('-', 50));
        
        return matchedSkills;
    }

    private SkillMatchResult MatchSkill(SkillDefinition skill, List<string> userKeywords)
    {
        var result = new SkillMatchResult
        {
            Skill = skill
        };
        
        var allKeywords = skill.Keywords
            .Concat(new[] { skill.Name.ToLower() })
            .Concat(skill.Description.ToLower().Split(' '))
            .Distinct()
            .ToList();
        
        foreach (var userKeyword in userKeywords)
        {
            foreach (var skillKeyword in allKeywords)
            {
                if (skillKeyword.Contains(userKeyword) || userKeyword.Contains(skillKeyword))
                {
                    result.Score++;
                    if (!result.MatchedKeywords.Contains(userKeyword))
                    {
                        result.MatchedKeywords.Add(userKeyword);
                    }
                    break;
                }
            }
        }
        
        foreach (var userKeyword in userKeywords)
        {
            foreach (var func in skill.Functions)
            {
                if (func.Name.Contains(userKeyword, StringComparison.OrdinalIgnoreCase))
                {
                    result.Score += 2;
                    if (!result.MatchedKeywords.Contains(userKeyword))
                    {
                        result.MatchedKeywords.Add(userKeyword);
                    }
                }
                
                if (func.Description.Contains(userKeyword, StringComparison.OrdinalIgnoreCase))
                {
                    result.Score++;
                    if (!result.MatchedKeywords.Contains(userKeyword))
                    {
                        result.MatchedKeywords.Add(userKeyword);
                    }
                }
            }
        }
        
        return result;
    }

    private List<string> ExtractKeywords(string text)
    {
        var keywords = new List<string>();
        
        var koreanWords = System.Text.RegularExpressions.Regex.Matches(text, @"\b[가 - 힣]+\b")
            .Cast<System.Text.RegularExpressions.Match>()
            .Select(m => m.Value.ToLower());
        
        var englishWords = System.Text.RegularExpressions.Regex.Matches(text, @"\b[a-zA-Z]+\b")
            .Cast<System.Text.RegularExpressions.Match>()
            .Select(m => m.Value.ToLower());
        
        var numbers = System.Text.RegularExpressions.Regex.Matches(text, @"\d+")
            .Cast<System.Text.RegularExpressions.Match>()
            .Select(m => m.Value);
        
        keywords.AddRange(koreanWords.Where(w => w.Length >= 2));
        keywords.AddRange(englishWords.Where(w => w.Length >= 2));
        keywords.AddRange(numbers);
        
        var stopWords = new HashSet<string> { "해줘", "주세요", "구해줘", "계산", "알려줘", "뭐", "어떤", "어떻게" };
        keywords = keywords.Where(k => !stopWords.Contains(k)).ToList();
        
        return keywords.Distinct().ToList();
    }
}
