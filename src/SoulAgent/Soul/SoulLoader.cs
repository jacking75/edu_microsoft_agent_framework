namespace SoulAgent.Soul;

/// <summary>
/// Soul 정의 관리 클래스
/// </summary>
public class SoulLoader
{
    private readonly string _soulPath;
    private SoulDefinition? _soul;

    public SoulLoader(string soulPath)
    {
        _soulPath = soulPath;
    }

    /// <summary>
    /// Soul 정의 로드
    /// </summary>
    public void Load()
    {
        if (!File.Exists(_soulPath))
        {
            throw new FileNotFoundException($"Soul 파일을 찾을 수 없습니다: {_soulPath}");
        }

        var json = File.ReadAllText(_soulPath);
        _soul = System.Text.Json.JsonSerializer.Deserialize<SoulDefinition>(json)
            ?? throw new InvalidOperationException("Soul 정의를 파싱할 수 없습니다.");

        Console.WriteLine($"✅ Soul 로드 완료: {_soul.Name}");
    }

    /// <summary>
    /// Soul 정의 반환
    /// </summary>
    public SoulDefinition GetSoul()
    {
        return _soul ?? throw new InvalidOperationException("Soul 이 로드되지 않았습니다.");
    }

    /// <summary>
    /// 시스템 프롬프트 생성
    /// </summary>
    public string BuildSystemPrompt()
    {
        if (_soul == null)
        {
            throw new InvalidOperationException("Soul 이 로드되지 않았습니다.");
        }

        var values = _soul.CoreValues.Count > 0
            ? string.Join("\n", _soul.CoreValues.Select((v, i) => $"  {i + 1}. {v}"))
            : "  명시된 가치 없음";

        var principles = _soul.Principles.Count > 0
            ? string.Join("\n", _soul.Principles.Select((p, i) => $"  {i + 1}. {p}"))
            : "  명시된 원칙 없음";

        var taboos = _soul.Taboos.Count > 0
            ? string.Join("\n", _soul.Taboos.Select((t, i) => $"  {i + 1}. {t}"))
            : "  명시된 금지 사항 없음";

        return $"""
            당신은 {_soul.Identity} 입니다.
            
            ## 이름
            {_soul.Name}
            
            ## 미션
            {_soul.Mission}
            
            ## 핵심 가치
            {values}
            
            ## 행동 원칙
            {principles}
            
            ## 말투 및 언어 스타일
            {_soul.SpeakingStyle}
            
            ## 금지 사항 (절대 하지 않아야 할 일)
            {taboos}
            
            ## 응답 가이드라인
            1. 위의 정체성과 가치를 항상 유지하며 응답하세요.
            2. 금지 사항을 위반하는 요청은 정중하게 거절하세요.
            3. 핵심 가치에 부합하는 방향으로 답변하세요.
            4. 일관된 말투와 성격을 유지하세요.
            5. 사용자가 당신의 Soul 에 대해 물어보면 솔직하게 답변하세요.
            """;
    }

    /// <summary>
    /// Soul 정보 표시
    /// </summary>
    public void DisplaySoulInfo()
    {
        if (_soul == null)
        {
            throw new InvalidOperationException("Soul 이 로드되지 않았습니다.");
        }

        Console.WriteLine($"\n👤 에이전트: {_soul.Name}");
        Console.WriteLine($"🎭 정체성: {_soul.Identity}");
        Console.WriteLine($"🎯 미션: {_soul.Mission}");

        Console.WriteLine("\n💎 핵심 가치:");
        foreach (var value in _soul.CoreValues)
        {
            Console.WriteLine($"   • {value}");
        }

        Console.WriteLine("\n📜 행동 원칙:");
        foreach (var principle in _soul.Principles)
        {
            Console.WriteLine($"   • {principle}");
        }

        Console.WriteLine("\n🗣️ 말투: {_soul.SpeakingStyle}");

        Console.WriteLine("\n🚫 금지 사항:");
        foreach (var taboo in _soul.Taboos)
        {
            Console.WriteLine($"   • {taboo}");
        }
    }
}
