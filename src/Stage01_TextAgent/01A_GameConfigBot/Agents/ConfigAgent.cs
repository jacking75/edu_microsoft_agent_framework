using System.Text.Json;

namespace _01A_GameConfigBot.Agents;

/// <summary>
/// 게임 설정 파일을 읽고 질문에 답변하는 에이전트
/// Microsoft Agent Framework 의 AgentFactory 를 사용하여 초기화
/// </summary>
public class ConfigAgent
{
    private readonly Dictionary<string, object> _configData;
    private readonly string _systemPrompt;

    /// <summary>
    /// ConfigAgent 생성자
    /// JSON 설정 파일을 로드하고 시스템 프롬프트를 초기화
    /// </summary>
    /// <param name="configPath">게임 설정 JSON 파일 경로</param>
    public ConfigAgent(string configPath)
    {
        var jsonContent = File.ReadAllText(configPath);
        _configData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent) 
            ?? new Dictionary<string, object>();

        _systemPrompt = """
            당신은 게임 설정 정보 어시스턴트입니다.
            
            당신의 역할:
            1. 게임 설정 파일에서 정보를 찾아 정확하게 답변합니다
            2. 숫자 값은 그대로 전달하고, 필요한 경우 계산합니다
            3. 모르는 정보는 "해당 정보를 찾을 수 없습니다"라고 답변합니다
            4. 항상 친절하고 명확하게 답변합니다
            
            사용 가능한 정보:
            - 게임 기본 설정 (제목, 버전, 최대 레벨 등)
            - 전투 시스템 (데미지, 크리티컬, 회피 등)
            - 경제 시스템 (골드 드롭, 아이템 드롭, 세금 등)
            - 플레이어 스탯 (HP, MP, 스태미나 등)
            - 월드 정보 (존, 던전, 보스 등)
            """;
    }

    /// <summary>
    /// 설정 파일에서 특정 키의 값을 조회
    /// 중첩된 객체도 점 표기법으로 접근 가능 (예: "combat.criticalChance")
    /// </summary>
    public object? GetValue(string key)
    {
        var keys = key.Split('.');
        object? current = _configData;

        foreach (var k in keys)
        {
            if (current is Dictionary<string, object> dict && dict.TryGetValue(k, out var value))
            {
                current = value;
            }
            else
            {
                return null;
            }
        }

        return current;
    }

    /// <summary>
    /// 시스템 프롬프트 반환
    /// Agent 생성시 사용
    /// </summary>
    public string GetSystemPrompt() => _systemPrompt;

    /// <summary>
    /// 설정 파일의 모든 최상위 키 반환
    /// </summary>
    public IEnumerable<string> GetTopLevelKeys() => _configData.Keys;

    /// <summary>
    /// 특정 섹션의 모든 키 반환
    /// </summary>
    public Dictionary<string, object>? GetSection(string section)
    {
        if (_configData.TryGetValue(section, out var value) && 
            value is Dictionary<string, object> dict)
        {
            return dict;
        }
        return null;
    }

    /// <summary>
    /// 설정 데이터 전체를 반환
    /// Context Provider 에서 LLM 에 데이터 주입시 사용
    /// </summary>
    public Dictionary<string, object> GetAllConfigData() => _configData;
}
